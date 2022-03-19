
class Move {
    public int x;
    public int y;
    public int z;

    public byte[,,] pre_stones;

    public ResponsePuzzle.MoveTree pre_tree;
    public Move(int x, int y, ResponsePuzzle.MoveTree tree = null, byte[,,] stones = null): this(x, y, 0, tree, stones){}
    public Move(int x, int y, int z = 0, ResponsePuzzle.MoveTree tree = null, byte[,,] stones = null)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.pre_tree = tree;
        this.pre_stones = stones;
    }
}