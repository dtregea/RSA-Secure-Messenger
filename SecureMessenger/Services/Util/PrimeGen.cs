// Daniel Tregea
using System.Numerics;

/// <summary>
/// Generator class for large prime numbers
/// </summary>
public class PrimeGen
{
    private readonly List<BigInteger> generatedPrimes;
    
    /// <summary>
    /// Construct a Prime number generator
    /// </summary>
    /// <param name="numPrimes">Number of primes to check for pre-test</param>
    public PrimeGen(int numPrimes)
    {
        generatedPrimes = GenerateNPrimes(numPrimes);
    }
    
    /// <summary>
    /// Generate a large prime number
    /// </summary>
    /// <param name="size">Size in bytes of prime number to generate</param>
    /// <returns>Prime number of 'size' bytes</returns>
    public BigInteger GeneratePrime(int size)
    {
        BigInteger result = -1;
        Parallel.For(0, Int32.MaxValue, (i, state) =>
        {
            var bigInteger = PrimeUtils.GetRandomNumber(size) ;

            if (bigInteger < 2 || bigInteger % 2 == 0)
            {
                return;
            }
            
            if (generatedPrimes.Any(prime => bigInteger % prime == 0))
            {
                return;
            }

            var isPrime = bigInteger.IsProbablyPrime();
            if (!isPrime) return;
            result = bigInteger;
            state.Stop();
        });
        
        return result;
    }
    private List<BigInteger> GenerateNPrimes(int toGenerate)
    {
        var primes = new List<BigInteger>{2,3};

        for (var i = 0; i < toGenerate; i++)
        {
            var primeFound = false;
            var num = primes[primes.Count - 1] + 2;
            while (!primeFound)
            {
                var isPrime = true;
                foreach (var prime in primes)
                {
                    if (num % prime == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    primes.Add(num);
                    primeFound = true;
                }
                else
                {
                    num += 2;
                }
            }
        }
        return primes;
    }
}