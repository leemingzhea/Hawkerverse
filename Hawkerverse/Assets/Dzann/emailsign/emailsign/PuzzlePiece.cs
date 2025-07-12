using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _pickUpClip, _dropClip;

    private bool _dragging;
    private Vector2 _originalPosition;
    private Transform _originalParent;
    private Canvas _canvas;

    public string itemType; // e.g., "email", "profile", "phone", etc.

    [HideInInspector] public PuzzleSlot currentSlot;

    void Awake()
    {
        _originalPosition = transform.localPosition;
        _originalParent = transform.parent;
        _canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (_dragging)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                Input.mousePosition,
                null,
                out Vector2 localPoint
            );

            transform.localPosition = localPoint;

            // On mouse up, stop dragging
            if (Input.GetMouseButtonUp(0))
            {
                _dragging = false;

                // Try snapping to nearest DropSlot under pointer
                PuzzleSlot targetSlot = GetSlotUnderPointer();

                if (targetSlot != null)
                {
                    // If slot already taken, reset old piece
                    if (targetSlot.placedPiece != null && targetSlot.placedPiece != this)
                    {
                        targetSlot.placedPiece.ResetToStart();
                    }

                    // Unlink from old slot
                    if (currentSlot != null)
                        currentSlot.placedPiece = null;

                    // Link to new slot
                    currentSlot = targetSlot;
                    currentSlot.placedPiece = this;

                    // Snap to slot
                    transform.SetParent(currentSlot.transform);
                    transform.localPosition = Vector3.zero;
                }
                else
                {
                    ResetToStart();
                }

                _source.PlayOneShot(_dropClip);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _dragging = true;
        _source.PlayOneShot(_pickUpClip);

        // Detach from slot temporarily
        transform.SetParent(_canvas.transform);

        if (currentSlot != null)
        {
            currentSlot.placedPiece = null;
            currentSlot = null;
        }
    }

    public void ResetToStart()
    {
        if (currentSlot != null)
        {
            currentSlot.placedPiece = null;
            currentSlot = null;
        }

        transform.SetParent(_originalParent);
        transform.localPosition = _originalPosition;
    }

    private PuzzleSlot GetSlotUnderPointer()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        foreach (var result in raycastResults)
        {
            PuzzleSlot slot = result.gameObject.GetComponent<PuzzleSlot>();
            if (slot != null)
            {
                return slot;
            }
        }

        return null;
    }
}
