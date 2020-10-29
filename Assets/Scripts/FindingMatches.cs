using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Match
{
    public Chip[] chips;

    public abstract void Process(BoardController board);
}

public class Match3 : Match
{
    public Match3()
    {
        chips = new Chip[3];
    }

    public override void Process(BoardController board)
    {
        for (int i = 0; i < chips.Length; i++)
        {
            board.TriggerExplosion(chips[i].x, chips[i].y);
        }     
    }
}

public class Match4 : Match
{
    public Match4()
    {
        chips = new Chip[4];
    }

    public override void Process(BoardController board)
    {
        for (int i = 0; i < chips.Length; i++)
        {
            board.TriggerExplosion(chips[i].x, chips[i].y);
        }

        board.CreateVerticalChip(chips[1].x, chips[1].y, chips[1].colorType);
    }
}

public class Match5 : Match
{
    public Match5()
    {
        chips = new Chip[5];
    }

    public override void Process(BoardController board)
    {
        for (int i = 0; i < chips.Length; i++)
        {
            board.TriggerExplosion(chips[i].x, chips[i].y);
        }
    }
}

public class FindingMatches : MonoBehaviour
{
    private BoardController boardController;

    private bool[,] chipJoinedMatch;

    private int width = 9;

    private int height = 9;
    void Awake()
    {
        boardController = GetComponent<BoardController>();
        chipJoinedMatch = AllocationUtility.Allocate2D<bool>(width, height, false);
    }

    void Start()
    {
        StartCoroutine(FindMatchesCoroutine());
    }

    IEnumerator FindMatchesCoroutine()
    {
        WaitForSeconds executeInterval = new WaitForSeconds(0.25f);

        while (true)
        {
            yield return executeInterval;

            FindMatches(); 
        } 
    }

    void ResetChipJoinedMatchBuffer()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                chipJoinedMatch[x, y] = false;
            }
        }
    }

    bool IsChipJoinedMatch(int x, int y)
    {
        return chipJoinedMatch[x, y];
    }

    void SetChipJoinedMatch(int x, int y)
    {
        chipJoinedMatch[x, y] = true;
    }

    void FindMatches()
    {
        List<Match> matches = new List<Match>();

        // reset data
        ResetChipJoinedMatchBuffer();

        int totalSearch = 0;
        // find horizontal
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                Chip chip = boardController.GetChip(x, y);

                if (chip != null && chip.CanMatch() && !IsChipJoinedMatch(x, y))
                {
                    totalSearch++;
                    int matchCount = 1;
                    SetChipJoinedMatch(x, y);

                    for (int xr = x + 1; xr < width; xr++)
                    {
                        Chip rightChip = boardController.GetChip(xr, y);

                        if (rightChip != null && rightChip.CanMatch() 
                            && !IsChipJoinedMatch(xr, y) 
                            && chip.colorType == rightChip.colorType)
                        {
                            matchCount++;
                            SetChipJoinedMatch(xr, y);
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    if (matchCount > 2)
                    {
                        if (matchCount == 3)
                        {
                            Match3 match3 = new Match3();
                            match3.chips[0] = chip;
                            match3.chips[1] = boardController.GetChip(x + 1, y);
                            match3.chips[2] = boardController.GetChip(x + 2, y);

                            matches.Add(match3);
                        }
                        else if (matchCount == 4)
                        {
                            Match4 match4 = new Match4();
                            match4.chips[0] = chip;
                            match4.chips[1] = boardController.GetChip(x + 1, y);
                            match4.chips[2] = boardController.GetChip(x + 2, y);
                            match4.chips[3] = boardController.GetChip(x + 3, y);

                            matches.Add(match4);
                        }
                        else if (matchCount == 5)
                        {
                            Match5 match5 = new Match5();
                            match5.chips[0] = chip;
                            match5.chips[1] = boardController.GetChip(x + 1, y);
                            match5.chips[2] = boardController.GetChip(x + 2, y);
                            match5.chips[3] = boardController.GetChip(x + 3, y);
                            match5.chips[4] = boardController.GetChip(x + 4, y);

                            matches.Add(match5);
                        }
                    }
                }
            }
        }

        //Debug.Log(totalSearch);

        //for (int x = 0; x < 9; x++)
        //{
        //    for (int y = 0; y < 9; y++)
        //    {
        //        Chip chip = boardController.GetChip(x, y);

        //        if (chip != null)
        //        {
        //            chip.transform.localScale = Vector3.one;
        //        }
        //    }
        //}

        if (!boardController.IsSwapping())
        {
            for (int i = 0; i < matches.Count; i++)
            {
                bool allChipNotFalling = true;

                for (int ci = 0; ci < matches[i].chips.Length; ci++)
                {
                    Chip chip = matches[i].chips[ci];
                    if (chip.IsFalling())
                    {
                        allChipNotFalling = false;
                        break;
                    }
                }

                if (allChipNotFalling)
                {
                    matches[i].Process(boardController);
                }
            }
        }      
    }
}
