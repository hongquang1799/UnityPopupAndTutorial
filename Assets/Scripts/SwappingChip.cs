using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwappingChip : MonoBehaviour
{
    public float minDragDistanceToSwap = 0.4f;

    private BoardController board;

    private Camera camera;

    private Location sourceLocation;

    private Vector2 beginTouchPosition;

    private bool swapped;

    private List<Chip> pendingKillChips = new List<Chip>();

    void Awake()
    {
        board = GetComponent<BoardController>();

        camera = Camera.main;
    }

    void Update()
    {
        //if (TouchUtility.TouchCount == 1)
        //{
        //    Touch touch = TouchUtility.GetTouch(0);
        //    Vector2 mousePosition = touch.position;
        //    Vector2 touchPositionWorld = camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));

        //    if (touch.phase == TouchPhase.Began)
        //    {
        //        swapped = false;
        //        beginTouchPosition = touchPositionWorld;
        //        sourceLocation = board.GetLocationFromWorldPosition(beginTouchPosition);
        //    }
        //    else if (swapped == false && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended))
        //    {
        //        if (board.IsValidLocation(sourceLocation.x, sourceLocation.y)
        //            && board.IsChipExist(sourceLocation.x, sourceLocation.y))
        //        {
        //            Vector2 delta = touchPositionWorld - beginTouchPosition;
        //            if (delta.magnitude > minDragDistanceToSwap)
        //            {
        //                float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg + 45f;
        //                if (angle < 0f) angle += 360f;
        //                Location dstLocation = sourceLocation;

        //                if (angle > 0f && angle < 90f) // right
        //                {
        //                    dstLocation.x += 1;
        //                }
        //                else if (angle > 90f && angle < 180f) // top
        //                {
        //                    dstLocation.y += 1;
        //                }
        //                else if (angle > 180f && angle < 270f) // left
        //                {
        //                    dstLocation.x += -1;
        //                }
        //                else // bottom
        //                {
        //                    dstLocation.y += -1;
        //                }

        //                if (board.IsValidLocation(dstLocation.x, dstLocation.y)
        //                    && board.IsChipExist(dstLocation.x, dstLocation.y))
        //                {
        //                    swapped = true;
        //                    board.Swap(sourceLocation, dstLocation);
        //                }
        //            }
        //        }
                
        //    }
        //}

        if (TouchUtility.TouchCount == 1)
        {
            Touch touch = TouchUtility.GetTouch(0);
            Vector2 mousePosition = touch.position;
            Vector3 touchPositionWorld = camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));
            Vector2 touchPositionOnBoard = touchPositionWorld - board.beginPosition;
            int x = (int)(touchPositionOnBoard.x / board.slotSize);
            int y = (int)(touchPositionOnBoard.y / board.slotSize);
            if (board.IsValidLocation(x, y) && board.GetChip(x, y) != null)
            {
                Chip chip = board.GetChip(x, y);
                if (!pendingKillChips.Contains(chip))
                    pendingKillChips.Add(chip);
            }
            if (touch.phase == TouchPhase.Ended)
            {
                for (int i = 0; i < pendingKillChips.Count; i++)
                {
                    Chip chip = pendingKillChips[i];
                    chip.Explode(board);
                    board.ResetChipLocation(chip.x, chip.y);
                }
                pendingKillChips.Clear();
            }
        }
    }
}
