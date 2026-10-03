using System;
using UnityEngine;
using _Bludoku.Scripts.Blocks;
using _Bludoku.Scripts.Boards;
using System.Collections.Generic;
using _Bludoku.Scripts.Analytics;

namespace _Bludoku.Scripts.Core
{
    public class FiguresController : MonoBehaviour
    {
        public event Action OnGameOver;

        [SerializeField] private Board board;
        [SerializeField] private List<Transform> figurePositions;
        [SerializeField] private int minTotalCells = 20;
        
        private readonly FiguresSaveLoad _saveLoad = new();
        private readonly List<Figure> _currentFigures = new();

        private IAnalyticsProvider _analytics;

        public void Initialize(IAnalyticsProvider analytics) =>
            _analytics = analytics;

        public void LoadFigures()
        {
            int[] savedFigureIds = FiguresSaveLoad.LoadFigures(figurePositions.Count);
            if (savedFigureIds.Length == 0)
            {
                UpdateFigures();
            }
            else
            {
                for (int i = 0; i < savedFigureIds.Length; i++)
                {
                    if (savedFigureIds[i] < 0)
                        continue;

                    Figure newFigure = FigureFactory.GetByIndex(savedFigureIds[i]);

                    Debug.Log($"Loaded figure ID: {savedFigureIds[i]} at position {i}");
                    RegisterFigure(newFigure, i);
                }
            }

            CheckPlaceability();
        }

        public void UpdateFigures(int totalCells = -1)
        {
            List<int> indices;
            if (totalCells < 0)
            {
                indices = FigureSuggestion.SelectShapeIndices(board.GetGrid(), FigureFactory.Shapes,
                    figurePositions.Count, minTotalCells);
            }
            else
            {
                indices = FigureSuggestion.SelectShapeIndices(board.GetGrid(), FigureFactory.Shapes,
                    figurePositions.Count, totalCells);
            }

            for (int i = 0; i < figurePositions.Count; i++)
            {
                Figure newFigure = i < indices.Count
                    ? FigureFactory.GetByIndex(indices[i])
                    : FigureFactory.GetRandom();

                RegisterFigure(newFigure, i);
            }
            
            FiguresSaveLoad.SaveFigures(_currentFigures, figurePositions.Count);
        }

        public void ResetFigures()
        {
            foreach (var figure in _currentFigures)
            {
                Destroy(figure.gameObject);
            }
            _currentFigures.Clear();

            UpdateFigures();
        }
        
        public void UpdateToEasyFigures()
        {
            foreach (var figure in _currentFigures)
            {
                Destroy(figure.gameObject);
            }
            _currentFigures.Clear();

            UpdateFigures(1);
        }

        private void FigurePicked(Figure figure)
        {
        }

        private void FigureDragged(Figure figure)
        {
            board.UpdateHighlight(figure);
        }

        private void FigureReleased(Figure figure)
        {
            board.ClearHighlight();

            if (board.CanPlaceFigure(figure))
            {
                PlaceFigure(figure);
            }
            else
            {
                figure.SnapBack();
                TrackPieceMoved(figure, false);
            }
        }

        private void RegisterFigure(Figure figure,  int index)
        {
            figure.SetInitialPosition(figurePositions[index], index);
            figure.transform.position = figurePositions[index].position;

            figure.OnPicked += FigurePicked;
            figure.OnDragged += FigureDragged;
            figure.OnReleased += FigureReleased;

            _currentFigures.Add(figure);
        }

        private void PlaceFigure(Figure figure)
        {
            figure.OnPicked -= FigurePicked;
            figure.OnDragged -= FigureDragged;
            figure.OnReleased -= FigureReleased;

            board.SetFigure(figure);
            TrackPieceMoved(figure, true);

            _currentFigures.Remove(figure);
            Destroy(figure.gameObject);

            if (_currentFigures.Count == 0)
                UpdateFigures();

            CheckPlaceability();
            
            FiguresSaveLoad.SaveFigures(_currentFigures, figurePositions.Count);
        }

        private void CheckPlaceability()
        {
            bool anyCanBePlaced = false;

            foreach (Figure figure in _currentFigures)
            {
                bool canPlace = board.CanPlaceAnywhere(figure.Grid);
                figure.SetPlaceable(canPlace);
                if (canPlace) anyCanBePlaced = true;
            }

            if (!anyCanBePlaced && _currentFigures.Count > 0)
                OnGameOver?.Invoke();
        }

        private void TrackPieceMoved(Figure figure, bool placed) =>
            _analytics.Track(new AnalyticsEvent(AnalyticsEvents.PieceMoved)
                .Add("figure_id", figure.ID)
                .Add("outcome", placed ? "placed" : "returned"));
    }
}
