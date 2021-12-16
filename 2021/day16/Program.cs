class Program
{
    static void Main()
    {
        // BitReader reader = new("D2FE28");
        // BitReader reader = new("38006F45291200");
        // BitReader reader = new("A0016C880162017C3686B18A3D4780");
        BitReader reader = new(File.ReadAllText("input.txt"));
        var packet = Packet.ReadPacket(reader);
        // var packet = Packet.ReadPacket(new BitReader("9C005AC2F8F0"));
        Console.WriteLine(new { sum = packet.SumOfVersions(), value = packet.Evaluate() });
    }
}

class Packet
{
    public int version;
    public int type;
    public List<Packet> _subpackets = new();

    public Packet(int version, int type)
    {
        this.version = version;
        this.type = type;
    }

    public static Packet ReadPacket(BitReader reader)
    {
        int version = reader.ReadBits(3);
        int type = reader.ReadBits(3);
        return type switch
        {
            4 => new LiteralValuePacket(version, type, reader),
            _ => new OperatorPacket(version, type, reader),
        };
    }

    public int SumOfVersions()
    {
        return version + _subpackets.Sum(p => p.SumOfVersions());
    }

    public virtual long Evaluate()
    {
        return 0;
    }
}

class LiteralValuePacket : Packet
{
    public long value;

    public LiteralValuePacket(int version, int type, BitReader reader)
        : base(version, type)
    {
        while (true)
        {
            long bits = reader.ReadBits(5);
            value <<= 4;
            value |= bits & 0b1111;
            if ((bits & 0b10000) == 0)
                break;
        }
    }

    public override long Evaluate() => value;
}

class OperatorPacket : Packet
{
    public OperatorPacket(int version, int type, BitReader reader)
        : base(version, type)
    {
        int lengthTypeIdentifier = reader.ReadBits(1);
        if (lengthTypeIdentifier == 0)
        {
            int subpacketBits = reader.ReadBits(15);
            int startBit = reader._bitPosition;
            while (reader._bitPosition < startBit + subpacketBits)
            {
                _subpackets.Add(Packet.ReadPacket(reader));
            }
        }
        else
        {
            int numSubpackets = reader.ReadBits(11);
            _subpackets.AddRange(Enumerable.Range(0, numSubpackets)
                                           .Select(_ => Packet.ReadPacket(reader)));
        }
    }

    public override long Evaluate()
    {
        List<long> subvalues = _subpackets.Select(p => p.Evaluate()).ToList();
        switch (type)
        {
            case 0:
                return subvalues.Sum();
            case 1:
                return subvalues.Aggregate(1L, (a, b) => a * b);
            case 2:
                return subvalues.Min();
            case 3:
                return subvalues.Max();
            case 5:
                return subvalues[0] > subvalues[1] ? 1 : 0;
            case 6:
                return subvalues[0] < subvalues[1] ? 1 : 0;
            case 7:
                return subvalues[0] == subvalues[1] ? 1 : 0;
            default:
                throw new InvalidOperationException($"Unknown operation {type}");
        }
    }
}

class BitReader
{
    byte[] _bytes;
    public int _bitPosition = 0;
    int BytePosition() => _bitPosition / 8;
    int BitPositionInCurrentByte() => _bitPosition % 8;
    int BitsAvailableInCurrentByte() => 8 - BitPositionInCurrentByte();

    public BitReader(string hexBytes)
        : this(Convert.FromHexString(hexBytes))
    {
    }

    public BitReader(byte[] bytes)
    {
        _bytes = bytes;
    }

    public int ReadBits(int numBits)
    {
        const int MaxSize = sizeof(int) * 8;
        if (numBits > MaxSize)
        {
            throw new ArgumentOutOfRangeException(nameof(numBits), numBits,
                $"{nameof(numBits)} is {numBits} which is greater than {MaxSize}");
        }

        int value = 0;
        while (numBits > 0)
        {
            if (BytePosition() >= _bytes.Length)
                throw new InvalidOperationException("Ran out of bytes");

            int bitsToTake = Math.Min(numBits, BitsAvailableInCurrentByte());
            byte bits = _bytes[BytePosition()];
            bits >>= (8 - BitPositionInCurrentByte()) - bitsToTake;
            bits &= (byte)((1 << bitsToTake) - 1);
            value <<= bitsToTake;
            value |= bits;
            numBits -= bitsToTake;
            _bitPosition += bitsToTake;
        }

        return value;
    }
}
