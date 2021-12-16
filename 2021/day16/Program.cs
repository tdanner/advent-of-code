class Program
{
    static void Main()
    {
        // BitReader reader = new("D2FE28");
        // BitReader reader = new("38006F45291200");
        // BitReader reader = new("A0016C880162017C3686B18A3D4780");
        BitReader reader = new(File.ReadAllText("input.txt"));
        Console.WriteLine(new { sum = Packet.ReadPacket(reader).SumOfVersions() });
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
}

class LiteralValuePacket : Packet
{
    public int value;

    public LiteralValuePacket(int version, int type, BitReader reader)
        : base(version, type)
    {
        while (true)
        {
            int bits = reader.ReadBits(5);
            value <<= 4;
            value |= bits & 0b1111;
            if ((bits & 0b10000) == 0)
                break;
        }
    }
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
