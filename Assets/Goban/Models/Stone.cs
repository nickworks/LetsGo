
public class Stone {
    public Stone(byte x, byte y, byte z, byte val = 0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.val = 0;
    }
    public byte x;
    public byte y;
    public byte z;
    public byte val;
    public bool inGroup; // flag used during group formation
}