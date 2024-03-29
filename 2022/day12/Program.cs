﻿var lines = File.ReadAllLines("input.txt");
int h = lines.Length;
int w = lines[0].Length;
var z = new int[h, w];
Pt start = new Pt(-1, -1), end = new Pt(-1, -1);
Dictionary<Pt, int> dist = new();
Dictionary<Pt, Pt> prev = new();
HashSet<Pt> q = new();
for (int y = 0; y < h; y++)
{
    for (int x = 0; x < w; x++)
    {
        Pt p = new Pt(x, y);
        char c = lines[y][x];
        if (c == 'S')
        {
            c = 'a';
            start = p;
        }
        if (c == 'E')
        {
            c = 'z';
            end = p;
        }
        z[y, x] = c - 'a';
        dist[p] = int.MaxValue;
        q.Add(p);
    }
}

dist[end] = 0;
List<Pt> candidates = new();
while (q.Count > 0)
{
    Pt u = q.MinBy(p => dist[p]);
    if (z[u.y, u.x] == 0)
    {
        candidates.Add(u);
    }
    q.Remove(u);
    foreach (var v in neighbors(u).Where(q.Contains))
    {
        if (dist[u] == int.MaxValue)
            continue;
        int alt = dist[u] + 1;
        if (alt < dist[v])
        {
            dist[v] = alt;
            prev[v] = u;
        }
    }
}

Console.WriteLine($"Part 1: {dist[end]}");
Console.WriteLine($"Part 2: {candidates.Select(c => dist[c]).Min()}");

IEnumerable<Pt> neighbors(Pt a)
{
    var minz = z[a.y, a.x] - 1;
    // up
    if (a.y > 0 && z[a.y - 1, a.x] >= minz)
        yield return new Pt(a.x, a.y - 1);
    // down
    if (a.y < h - 1 && z[a.y + 1, a.x] >= minz)
        yield return new Pt(a.x, a.y + 1);
    // left
    if (a.x > 0 && z[a.y, a.x - 1] >= minz)
        yield return new Pt(a.x - 1, a.y);
    // right
    if (a.x < w - 1 && z[a.y, a.x + 1] >= minz)
        yield return new Pt(a.x + 1, a.y);
}


record struct Pt(int x, int y)
{
    public override string ToString() => $"{x},{y}";
}
