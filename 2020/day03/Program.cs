var lines = File.ReadAllLines("input.txt");
int height = lines.Length;
int width = lines[0].Length;
var map = (int x, int y) => lines[y][x % width];
int vx = 3, vy = 1;
int x = 0, y = 0;
int trees = 0;
while (y < height)
{
    if (map(x, y) == '#')
        trees++;
    x += vx;
    y += vy;

}
Console.WriteLine(trees);