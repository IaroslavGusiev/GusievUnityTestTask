using UnityEngine;
using _Bludoku.Scripts.UI;
using _Bludoku.Scripts.Core;
using _Bludoku.Scripts.Score;
using _Bludoku.Scripts.Combo;
using _Bludoku.Scripts.Boards;
using _Bludoku.Scripts.Effects;
using _Bludoku.Scripts.Analytics;

namespace _Bludoku.Scripts
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }
        public IAnalyticsProvider Analytics { get; private set; }
        
        [Header("--- Core ---")]
        [SerializeField] private UIMediator uiMediator;
        [SerializeField] private ScoreMediator scoreMediator;
        [SerializeField] private ComboMediator comboMediator;
        [SerializeField] private ComboPresenter comboPresenter;
        [SerializeField] private EffectsManager effectsManager;
        
        [Header("--- Board ---")]
        [SerializeField] private Board board;
        [SerializeField] private FiguresController figuresController;

        [Header("--- Combo Parameters ---")]
        [SerializeField] private ComboRuleType comboRule = ComboRuleType.GraceMoves;
        [SerializeField, Min(0)] private int allowedNonClearingMoves = 2;
        [SerializeField] private ComboVisualSettings comboVisualSettings;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            InitializeGame();
        }

        private void Start()
        {
            RestoreScoreAndCombo();
            figuresController.OnGameOver += HandleGameOver;
            TrackInitialGameStart();
            LoadGame();
        }

        private void OnDestroy()
        {
            ShutdownGame();

            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void NewGame()
        {
            comboMediator.ResetCombo();
            BoardSaveLoad.Delete();
            board.ResetBoard();
            figuresController.ResetFigures();
            uiMediator.HideGameOver();
            scoreMediator.ResetScore();
            comboPresenter.ResumePresentation();
            TrackGameStarted("new");
        }

        public void SecondChance()
        {
            uiMediator.HideGameOver();
            figuresController.UpdateToEasyFigures();
            scoreMediator.RefreshViews();
            comboPresenter.ResumePresentation();
            TrackSecondChance();
        }

        private void InitializeGame()
        {
            Analytics = new ConsoleAnalyticsProvider();

            int highComboThreshold = ComboVisualSettings.ResolveHighComboThreshold(comboVisualSettings);
            
            figuresController.Initialize(Analytics);
            comboMediator.Initialize(comboRule, allowedNonClearingMoves);
            scoreMediator.Initialize(highComboThreshold, Analytics);
            comboPresenter.Initialize(comboRule, allowedNonClearingMoves, highComboThreshold);
            effectsManager.Initialize(comboVisualSettings);

            comboMediator.StateChanged += TrackComboBroken;
        }

        private void ShutdownGame()
        {
            if (figuresController != null)
            {
                figuresController.OnGameOver -= HandleGameOver;
            }

            if (effectsManager != null)
            {
                effectsManager.Dispose();
            }

            if (comboPresenter != null)
            {
                comboPresenter.Dispose();
            }

            if (scoreMediator != null)
            {
                scoreMediator.Dispose();
            }

            if (comboMediator != null)
            {
                comboMediator.StateChanged -= TrackComboBroken;
                comboMediator.Dispose();
            }
        }

        private void RestoreScoreAndCombo()
        {
            ScoreSystem.LoadScore();

            int threshold = ScoreSystem.IsBoosterEnabled
                ? ComboState.ActivationThreshold
                : 0;

            comboMediator.RestoreCombo(threshold);
            
            scoreMediator.RefreshViews();
        }

        private void LoadGame()
        {
            board.LoadGrid();
            figuresController.LoadFigures();
        }

        private void HandleGameOver()
        {
            comboPresenter.SuspendPresentation();
            uiMediator.ShowGameOver();
            TrackGameOver();
        }

        private void TrackInitialGameStart()
        {
            bool restored = BoardSaveLoad.TryLoad(out _);
            
            TrackGameStarted(restored 
                ? "restored" 
                : "new");
        }

        private void TrackGameStarted(string source)
        {
            Analytics.Track(new AnalyticsEvent(AnalyticsEvents.GameStarted)
                .Add("source", source)
                .Add("score", ScoreSystem.Score)
                .Add("combo_count", comboMediator.State.Count));
        }

        private void TrackSecondChance()
        {
            Analytics.Track(new AnalyticsEvent(AnalyticsEvents.PowerUpUsed)
                .Add("power_up_type", "second_chance")
                .Add("score", ScoreSystem.Score)
                .Add("combo_count", comboMediator.State.Count));
        }

        private void TrackGameOver()
        {
            Analytics.Track(new AnalyticsEvent(AnalyticsEvents.GameOver)
                .Add("score", ScoreSystem.Score)
                .Add("high_score", ScoreSystem.HighScore)
                .Add("combo_count", comboMediator.State.Count));
        }

        private void TrackComboBroken(ComboChange change)
        {
            if (change.Reason != ComboChangeReason.Broken || change.Previous.IsActive == false)
            {
                return;
            }

            Analytics.Track(new AnalyticsEvent(AnalyticsEvents.ComboBroken)
                .Add("combo_count", change.Previous.Count)
                .Add("grace_moves_used", change.Previous.ConsecutiveMisses));
        }
    }
}
