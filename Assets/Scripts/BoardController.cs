using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardController : MonoBehaviour
{
    public int MaxWidth = 9;

    public int MaxHeight = 9;

    public static readonly float chipPositionZ = -1f;

    public int pauseGravity;

    [Header("Slot")]
    public float slotSize = 0.1f;

    public GameObject slotPrefab;

    [Header("Simple Chip")]
    public GameObject[] chipCollection;

    [Header("Horizontal Bomb")]
    public GameObject[] horiazontalBombCollection;

    [Header("Horizontal Bomb")]
    public GameObject[] verticalBombCollection;

    public int width;

    public int height;

    private Slot[,] slots;

    private bool[,] canPlaceChips;

    private Chip[,] chips;

    private Chip[,] runtimeChips;

    public Vector3 beginPosition;

    private List<Chip> pendingKillChips = new List<Chip>();

    private List<EntryGate> entryGates = new List<EntryGate>();

    private LinkedList<Slot> emptyChipSlots = new LinkedList<Slot>();

    private bool isSwapping;

    public void TriggerExplosion(int x, int y)
    {
        Chip chip = GetRuntimeChip(x, y);
        if (chip)
        {
            chip.Explode(this);

            ResetChipLocation(chip.x, chip.y);
        }         
    }

    public Chip GetRuntimeChip(int x, int y)
    {
        return runtimeChips[x, y];
    }

    public void SetRuntimeChip(int x, int y)
    {

    }

    void UpdateRuntimeChip(Chip chip)
    {
        if (IsValidLocation(chip.rtx, chip.rty))
        {
            runtimeChips[chip.rtx, chip.rty] = null;
        }

        Vector2 position = chip.transform.position - beginPosition;
        int x = (int)(position.x / slotSize);
        int y = (int)(position.y / slotSize);
        chip.rtx = x;
        chip.rty = y;

        if (IsValidLocation(x, y))
        {
            runtimeChips[x, y] = chip;
        }   
    }

    void ApplyGravity(float dt)
    {
        emptyChipSlots.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var chip = GetChip(x, y);
                if (chip)
                {
                    UpdateRuntimeChip(chip);
                    chip.MoveToTarget(dt);
                }                    

                if (IsValidLocation(x, y) && canPlaceChips[x, y] && !IsChipExist(x, y))
                {
                    emptyChipSlots.AddLast(slots[x, y]);
                }
            }
        }

        int count = 0;
        while (emptyChipSlots.Count > 0)
        {
            count++;
            if (count > 500)
            {
                Debug.LogWarning("Infinite loop");
                break;
            }

            var node = emptyChipSlots.First;
            while (node != null)
            {
                var emptySlot = node.Value;

                if (emptySlot.entryGate != null)
                {
                    Chip chip = CreateLiteChip(emptySlot.x, emptySlot.y, UnityEngine.Random.Range(0, 6));
                    chip.transform.localPosition = GetPositionForChip(emptySlot.x, emptySlot.y + 1);

                    chip.AddPathPosition(GetPositionForChip(emptySlot.x, emptySlot.y));

                    emptySlot.entryGate.Add(chip);

                    var nextNode = node.Next;
                    emptyChipSlots.Remove(node);

                    node = nextNode;
                }
                else if (IsValidLocation(emptySlot.x, emptySlot.y + 1) 
                    && (CanPlaceChip(emptySlot.x, emptySlot.y + 1) && !IsChipExist(emptySlot.x, emptySlot.y + 1)))
                {
                    node = node.Next;
                }
                else
                {
                    if (IsValidLocation(emptySlot.x, emptySlot.y + 1) && IsChipExist(emptySlot.x, emptySlot.y + 1))
                    {
                        Chip chip = GetChip(emptySlot.x, emptySlot.y + 1);

                        ResetChipLocation(emptySlot.x, emptySlot.y + 1);

                        SetChip(emptySlot.x, emptySlot.y, chip);
                        chip.AddPathPosition(GetPositionForChip(emptySlot.x, emptySlot.y));

                        emptyChipSlots.Remove(node);
                        node = emptyChipSlots.AddFirst(slots[emptySlot.x, emptySlot.y + 1]);
                    }
                    else if (IsValidLocation(emptySlot.x - 1, emptySlot.y + 1) && IsChipExist(emptySlot.x - 1, emptySlot.y + 1))
                    {
                        Chip chip = GetChip(emptySlot.x - 1, emptySlot.y + 1);

                        ResetChipLocation(emptySlot.x - 1, emptySlot.y + 1);

                        SetChip(emptySlot.x, emptySlot.y, chip);
                        chip.AddPathPosition(GetPositionForChip(emptySlot.x, emptySlot.y));

                        emptyChipSlots.Remove(node);
                        node = emptyChipSlots.AddFirst(slots[emptySlot.x - 1, emptySlot.y + 1]);
                    }
                    else if (IsValidLocation(emptySlot.x + 1, emptySlot.y + 1) && IsChipExist(emptySlot.x + 1, emptySlot.y + 1))
                    {
                        Chip chip = GetChip(emptySlot.x + 1, emptySlot.y + 1);

                        ResetChipLocation(emptySlot.x + 1, emptySlot.y + 1);

                        SetChip(emptySlot.x, emptySlot.y, chip);
                        chip.AddPathPosition(GetPositionForChip(emptySlot.x, emptySlot.y));

                        emptyChipSlots.Remove(node);
                        node = emptyChipSlots.AddFirst(slots[emptySlot.x + 1, emptySlot.y + 1]);
                    }
                    else
                    {
                        node = node.Next;
                    }
                }
            }
        }

        for (int i = 0; i < entryGates.Count; i++)
        {
            entryGates[i].Update();
        }
    }

    public void Swap(Location locationA, Location locationB)
    {
        isSwapping = true;

        Chip chipA = GetChip(locationA.x, locationA.y);
        Chip chipB = GetChip(locationB.x, locationB.y);

        SetChip(locationA.x, locationA.y, chipB);
        SetChip(locationB.x, locationB.y, chipA);

        Vector3 positionA = chipA.transform.localPosition;
        Vector3 positionB = chipB.transform.localPosition;

        chipA.transform.DOMove(positionB, 0.25f);
        chipB.transform.DOMove(positionA, 0.25f).OnComplete(() => isSwapping = false); ;
    }

    private void Awake()
    {
        width = MaxWidth;
        height = MaxHeight;
        beginPosition = transform.position - new Vector3(slotSize * width * 0.5f, slotSize * height * 0.5f, 0f);
        
        canPlaceChips = AllocationUtility.Allocate2D<bool>(width, height, true);
        chips = AllocationUtility.Allocate2D<Chip>(width, height, null);
        runtimeChips = AllocationUtility.Allocate2D<Chip>(width, height, null);
        slots = AllocationUtility.Allocate2D<Slot>(width, height, null);
    }

    private void Start()
    {
        //canPlaceChips[4, 4] = false;
        //canPlaceChips[5, 4] = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if ((x + y) % 2 == 0 && (y > 5 || y < 2)) canPlaceChips[x, y] = false;

                if (canPlaceChips[x, y] == true)
                {
                    GetPosition(x, y, out float px, out float py);

                    GameObject slot = Instantiate(slotPrefab);                   
                    slot.transform.localPosition = new Vector3(px, py, 0f);
                    slots[x, y] = slot.GetComponent<Slot>();
                    slots[x, y].x = x;
                    slots[x, y].y = y;
                    
                    Chip chip = CreateLiteChip(x, y, UnityEngine.Random.Range(0, 6));                    
                }              
            }
        }

        for (int x = 0; x < width; x++)
        {
            if (slots[x, height - 1])
            {
                EntryGate entryGate = new EntryGate();
                entryGate.position = GetPositionForChip(x, height);
                entryGate.queueDistance = slotSize;

                entryGates.Add(entryGate);

                slots[x, height - 1].entryGate = entryGate;
            }   
        }       
    }

    void Update()
    {
        if (pauseGravity == 0)
        {
            ApplyGravity(Time.deltaTime);
        }
    }

    public Chip CreateLiteChip(int x, int y, int colorType)
    {
        Chip chip = Instantiate(chipCollection[colorType]).GetComponent<Chip>();
        chip.Initialize();
        chip.colorType = colorType;
        chip.transform.localPosition = GetPositionForChip(x, y);
        SetChip(x, y, chip);

        return chip;
    }

    public Chip CreateHorizontalChip(int x, int y, int colorType)
    {
        Chip chip = Instantiate(horiazontalBombCollection[colorType]).GetComponent<Chip>();
        chip.Initialize();
        chip.colorType = colorType;
        chip.transform.localPosition = GetPositionForChip(x, y);
        SetChip(x, y, chip);

        return chip;
    }

    public Chip CreateVerticalChip(int x, int y, int colorType)
    {
        Chip chip = Instantiate(verticalBombCollection[colorType]).GetComponent<Chip>();
        chip.Initialize();
        chip.colorType = colorType;
        chip.transform.localPosition = GetPositionForChip(x, y);
        SetChip(x, y, chip);

        return chip;
    }

    public void ResetChipLocation(int x, int y)
    {
        chips[x, y] = null;
    }

    public bool IsSwapping()
    {
        return isSwapping;
    }

    public bool IsChipExist(int x, int y)
    {
        return chips[x, y] != null;
    }

    void SetChip(int x, int y, Chip chip)
    {
        chips[x, y] = chip;
        chip.x = x;
        chip.y = y;
    }

    bool CanPlaceChip(int x, int y)
    {
        return canPlaceChips[x, y];
    }

    public bool IsValidLocation(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public Chip GetChip(int x, int y)
    {
        return chips[x, y];
    }

    void GetPosition(int x, int y, out float px, out float py)
    {
        px = beginPosition.x + slotSize * (x + 0.5f);
        py = beginPosition.y + slotSize * (y + 0.5f);
    }

    Vector3 GetPositionForChip(int x, int y)
    {
        return new Vector3(beginPosition.x + slotSize * (x + 0.5f), beginPosition.y + slotSize * (y + 0.5f), chipPositionZ);
    }

    public Location GetLocationFromWorldPosition(Vector2 worldPosition)
    {
        int x = (int)((worldPosition.x - beginPosition.x) / slotSize);
        int y = (int)((worldPosition.y - beginPosition.y) / slotSize);

        return new Location { x = x, y = y };
    }
}
