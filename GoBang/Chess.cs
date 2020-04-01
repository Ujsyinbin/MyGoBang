
public class Chess
{
    private int x;
    private int y;
    private int color;

    public Chess()
    {
    }

    public Chess(int x, int y, int color)
    {
        this.x = x;
        this.y = y;
        this.color = color;
    }

    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
    public int Color { get => color; set => color = value; }
}
