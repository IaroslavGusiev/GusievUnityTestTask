using UnityEngine;
using _Bludoku.Scripts.UI;
using _Bludoku.Scripts.Core;
using _Bludoku.Scripts.Score;
using _Bludoku.Scripts.Combo;
using _Bludoku.Scripts.Boards;
using _Bludoku.Scripts.Effects;

namespace _Bludoku.Scripts
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }
        
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
        }

        public void SecondChance()
        {
            uiMediator.HideGameOver();
            figuresController.UpdateToEasyFigures();
            scoreMediator.RefreshViews();
            comboPresenter.ResumePresentation();
        }

        private void InitializeGame()
        {
            int highComboThreshold = ComboVisualSettings.ResolveHighComboThreshold(comboVisualSettings);
            comboMediator.Initialize(comboRule, allowedNonClearingMoves);
            scoreMediator.Initialize(highComboThreshold);
            comboPresenter.Initialize(comboRule, allowedNonClearingMoves, highComboThreshold);
            effectsManager.Initialize(comboVisualSettings);
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
                comboMediator.Dispose();
            }
        }

        private void RestoreScoreAndCombo()
        {
            ScoreSystem.LoadScore();
            // Legacy saves contain the booster flag, but not the original streak length.
            comboMediator.RestoreCombo(ScoreSystem.IsBoosterEnabled ? ComboState.ActivationThreshold : 0);
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
        }
    }
}
