namespace SecureMessenger.Services.Util;
public class KeyGenClass
{
    private class PublicKey
    {
        public string Key { get; set; }
        public PublicKey(string key)
        {
            Key = key;
        }
    }

    private class PrivateKey
    {
        public string Key { get; set; }
        public PrivateKey(string key)
        {
            Key = key;
        }
    }

    /// <summary>
    /// Generate a public and private key
    /// </summary>
    /// <param name="keySize">Bit size of keys</param>
    /// <returns>Tuple containing public and private keys as base64 strings</returns>
    public static (string publicKey, string privateKey) GenerateKeys(int keySize)
    {
        // Vary P and Q bytes by 20%
        var random = new Random();
        var modifier = 1 - random.Next(0, 20) / (double)100;
        var pBytes = Convert.ToInt32((keySize / 2 / 8) * modifier);
        var qBytes = (keySize / 8) - pBytes;

        var primeGen = new PrimeGen(200);
        var P = primeGen.GeneratePrime(pBytes);
        var Q = primeGen.GeneratePrime(qBytes);
        var R = (P - 1) * (Q - 1);
        var N = P * Q;
        var E = 65537;
        var D = RSA.ModInverse(E, R);

        var eBytes = BitConverter.GetBytes(E);
        var numE = eBytes.Length;
        var numEBytes = BitConverter.GetBytes(numE);
        Array.Reverse(numEBytes);
        var nBytes = N.ToByteArray();
        var numN = nBytes.Length;
        var numNBytes = BitConverter.GetBytes(numN);
        Array.Reverse(numNBytes);

        var publicKey = numEBytes.Concat(eBytes).Concat(numNBytes).Concat(nBytes).ToArray();
        var encodedPublicKey = Convert.ToBase64String(publicKey);

        var dBytes = D.ToByteArray();
        var numD = dBytes.Length;
        var numDBytes = BitConverter.GetBytes(numD);
        Array.Reverse(numDBytes);

        var privateKey = numDBytes.Concat(dBytes).Concat(numNBytes).Concat(nBytes).ToArray();
        var encodedPrivateKey = Convert.ToBase64String(privateKey);

        return (encodedPublicKey, encodedPrivateKey);
    }
}