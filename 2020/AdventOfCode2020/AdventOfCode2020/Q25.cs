using System;
using System.Linq;

namespace AdventOfCode2020;

public static class Q25
{
    public static void Solve()
    {
        var input = Tools.GetInput(25, false);
        var cardPublicKey = int.Parse(input[0]);
        var doorPublicKey = int.Parse(input[1]);

        var cardLoopSize = DetermineLoopSize(cardPublicKey, 7);
        var doorLoopSize = DetermineLoopSize(doorPublicKey, 7);
        
        var encryptionKey1 = EncryptionKey(doorPublicKey, cardLoopSize);
        var encryptionKey2 = EncryptionKey(cardPublicKey, doorLoopSize);

        if (encryptionKey1 != encryptionKey2)
        {
            throw new Exception($"Encryption keys don't match: {encryptionKey1},{encryptionKey2}");
        }
        Console.WriteLine($"Part 1: {encryptionKey1}");
    }

    private static int DetermineLoopSize(long publicKey, long subjectNumber)
    {
        var current = 1L;
        var loopSize = 0;
        while (current != publicKey)
        {
            current = Transform(current, subjectNumber);
            loopSize += 1;
        }

        return loopSize;
    }

    private static long Transform(long current, long subjectNumber)
    {
        return current * subjectNumber % 20201227;
    }

    private static long EncryptionKey(long subjectNumber, int loopSize)
    {
        var current = 1L;
        foreach (var _ in Enumerable.Range(0, loopSize))
        {
            current = Transform(current, subjectNumber);
        }
        return current;
    }
}