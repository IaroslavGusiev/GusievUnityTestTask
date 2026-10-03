using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Bludoku.Scripts.Blocks
{
    [RequireComponent(typeof(DragAndDrop))]
    public class Figure : MonoBehaviour
    {
        public event Action<Figure> OnPicked;
        public  event Action<Figure> OnDragged;
        public event Action<Figure> OnReleased;
        
        [SerializeField] private SortingGroup sortingGroup;
        [SerializeField] private Transform tilesParent;
        [SerializeField] private Tile tilePrefab;
        [SerializeField] private float tileSize = 1f;

        private int _id;
        private Transform _initialPosition;
        private GridView _gridView;
        private DragAndDrop _dragAndDrop;
        private int[,] _grid;

        public int[,] Grid => _grid;
        public int ID => _id;
        public int SlotIndex { get; private set; }

        private void Awake()
        {
            _dragAndDrop = GetComponent<DragAndDrop>();
            _dragAndDrop.OnDragStarted += DragStarted;
            _dragAndDrop.OnDragging += () => OnDragged?.Invoke(this);
            _dragAndDrop.OnDropped += DragFinished;
        }
        
        private void Start()
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        public void Initialize(int[,] grid, int id)
        {
            _grid = grid;
            _gridView = new GridView(tilesParent);
            _id = id;
            _gridView.Build(grid);
        }

        public void SetInitialPosition(Transform initialPosition, int slotIndex)
        {
            _initialPosition = initialPosition;
            SlotIndex = slotIndex;
        }
        
        public void SetPlaceable(bool placeable)
        {
            float alpha = placeable ? 1f : 0.4f;
            _gridView.SetAlpha(alpha);
        }

        public void SnapBack()
        {
            _dragAndDrop.MoveTo(_initialPosition.position);
            transform.DOScale(Vector3.one * 0.5f, 0.2f);
        }

        private void DragFinished()
        {
            sortingGroup.sortingOrder = 1;
            OnReleased?.Invoke(this);
        }
        
        private void DragStarted()
        {
            transform.DOScale(Vector3.one, 0.1f);

            sortingGroup.sortingOrder = 100;
            OnPicked?.Invoke(this);
        }
    }
}
