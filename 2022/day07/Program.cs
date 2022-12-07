using System.Text;

var lines = File.ReadAllLines("input.txt");

D root = new D();
Stack<D> working = new Stack<D>();
working.Push(root);
foreach (var line in lines)
{
    if (line == "$ cd /")
    {
        Console.WriteLine($"change to root: {line}");
        working.Clear();
        working.Push(root);
    }
    else if (line == "$ cd ..")
    {
        Console.WriteLine($"change to parent: {line}");
        working.Pop();
    }
    else if (line.StartsWith("$ cd "))
    {
        Console.WriteLine($"change to dir '{line[5..]}': {line}");
        working.Push(working.Peek().Directories[line[5..]]);
    }
    else if (line == "$ ls")
    {
        Console.WriteLine($"start listing: {line}");
        // do nothing
    }
    else if (line.StartsWith("dir "))
    {
        Console.WriteLine($"notice dir '{line[4..]}': {line}");
        working.Peek().Directories[line[4..]] = new D();
    }
    else
    {
        // must be file
        var parts = line.Split(' ', 2);
        Console.WriteLine($"notice file '{parts[1]}' of size {long.Parse(parts[0])}: {line}");
        working.Peek().Files[parts[1]] = new F(long.Parse(parts[0]));
    }
}

Console.WriteLine(root);
long part1 = root.Walk().Where(dir => dir.Size() <= 100000).Sum(dir => dir.Size());
Console.WriteLine($"Part 1: {part1}");

var dirs = root.Walk().ToList();
var sorted = dirs.OrderBy(dir => dir.Size());
long diskSize = 70000000;
long freeRequired = 30000000;
long mustDelete = freeRequired - (diskSize - root.Size());
long sizeOfDirToDelete = sorted.First(dir => dir.Size() >= mustDelete).Size();
Console.WriteLine($"Part 2: {sizeOfDirToDelete}");

record class F(long Size);
record class D(Dictionary<string, F> Files, Dictionary<string, D> Directories)
{
    public D() : this(new Dictionary<string, F>(), new Dictionary<string, D>())
    {
    }

    public long Size()
    {
        return Files.Sum(f => f.Value.Size) + Directories.Sum(d => d.Value.Size());
    }

    public IEnumerable<D> Walk()
    {
        foreach (var dir in Directories)
        {
            foreach (var walked in dir.Value.Walk())
            {
                yield return walked;
            }
        }
        yield return this;
    }

    public override string ToString()
    {
        return ToString("");
    }

    public string ToString(string prefix)
    {
        var buffer = new StringBuilder();
        foreach (var (name, dir) in Directories)
        {
            buffer.Append(dir.ToString(prefix + "/" + name));
        }
        foreach (var (name, f) in Files)
        {
            buffer.AppendLine($"{prefix}/{name}\t{f.Size}");
        }
        return buffer.ToString();
    }
}
