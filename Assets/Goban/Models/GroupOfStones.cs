using System.Collections.Generic;

public class GroupOfStones
{
    public GroupOfStones(int val)
    {
        stones = new List<Stone>();
        liberties = new List<Stone>();
        this.val = val;
    }
    public List<Stone> stones { get; private set; }
    public List<Stone> liberties { get; private set; }
    public int val { get; private set; }
    public bool AddStone(Stone stone)
    {
        if (stone.val != val) return false;
        if (HasStone(stone)) return false;
        stones.Add(stone);
        stone.inGroup = true;
        return true;
    }
    public bool HasStone(Stone loc)
    {
        foreach(Stone temp in stones)
        {
            if (temp.x == loc.x && temp.y == loc.y && temp.z == loc.z) return true;
        }
        return false;
    }
    public bool AddLiberty(Stone stone)
    {
        if (HasLiberty(stone)) return false;
        liberties.Add(stone);
        return true;
    }
    public bool HasLiberty(Stone loc)
    {
        foreach (Stone temp in liberties)
        {
            if (temp.x == loc.x && temp.y == loc.y && temp.z == loc.z) return true;
        }
        return false;
    }
}