/// Daniel Tregea
using System.Numerics;
using static System.Security.Cryptography.RandomNumberGenerator;

/// <summary>
/// Utility methods for large prime number generation
/// </summary>
public static class PrimeUtils
{
    /// <summary>
    /// Determine if a large integer is probably prime
    /// </summary>
    /// <param name="n">Large integer to determine if prime</param>
    /// <param name="k">Rounds to test primality</param>
    /// <returns>Whether n is probably prime</returns>
    public static bool IsProbablyPrime(this BigInteger n, int k = 10)
    {
        if (n < 2 || n % 2 == 0)
        {
            return false;
        }

        var d = n - 1;
        var r = 0;
        while (d % 2 != 1)
        {
            d /= 2;
            r++;
        }

        var byteCount = n.GetByteCount();
        var testPass = false;
        for (var i = 0; i < k; i++)
        {
            var a = GetRandomNumber(byteCount) % (n - 2) + 2;
            var x = BigInteger.ModPow(a, d, n);
            if (!(x == 1 || x == n - 1))
            {
                for (var j = 0; j < r - 1; j++)
                {
                    x = BigInteger.ModPow(x, 2, n); 
                    
                    // Continue witness loop
                    if (x == n - 1)
                    {
                        testPass = true; 
                        break;
                    }
                    testPass = false;
                }
                if (!testPass)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Generate a random number
    /// </summary>
    /// <param name="size">Size in bytes of random number</param>
    /// <returns>Random number of 'size' bytes</returns>
    public static BigInteger GetRandomNumber(int size)
    {
        return BigInteger.Abs(new BigInteger(GetBytes(size))) ;
    }
}