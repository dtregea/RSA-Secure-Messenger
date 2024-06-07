// Daniel Tregea

using System.Numerics;
using System.Text;
namespace SecureMessenger.Services.Util;

/// <summary>
/// RSA Encoding and Decoding tools
/// </summary>
public class RSA
{
    /// <summary>
    /// Encode a message with RSA
    /// </summary>
    /// <param name="message">Message to encode</param>
    /// <param name="E">E value</param>
    /// <param name="N">N value</param>
    /// <returns>Base 64 string of encoded message</returns>
    public static string Encode(string message, BigInteger E, BigInteger N)
    {
        var plainTextBytes = Encoding.ASCII.GetBytes(message);
        var cipher = BigInteger.ModPow(new BigInteger(plainTextBytes, true), E, N);
        var cipherBytes = cipher.ToByteArray();
        return Convert.ToBase64String(cipherBytes);
    }

    /// <summary>
    /// Decode an encoded message with RSA
    /// </summary>
    /// <param name="msg">Base64 encoded message to decode</param>
    /// <param name="D">D value</param>
    /// <param name="N">N value</param>
    /// <returns>String of decoded message</returns>
    public static string Decode(string msg, BigInteger D, BigInteger N)
    {
        var cipherBytes = Convert.FromBase64String(msg);
        var cipher = new BigInteger(cipherBytes, true);
        var plainTextInteger = BigInteger.ModPow(cipher, D, N);
        return Encoding.Default.GetString(plainTextInteger.ToByteArray());
    }
    
    /// <summary>
    /// Compute the Mod inverse of a number
    /// </summary>
    /// <param name="a">A value</param>
    /// <param name="n">N value</param>
    /// <returns>Mod inverse of parameter a</returns>
    public static BigInteger ModInverse(BigInteger a, BigInteger n)
    {
        BigInteger i = n, v = 0, d = 1;
        while (a > 0)
        {
            BigInteger t = i / a, x = a;
            a = i % x;
            i = x;
            x = d;
            d = v - t * x;
            v = x;
        }

        v %= n;
        if (v < 0) v = (v + n) % n;
        return v;
    }
    
    /// <summary>
    /// Decode a public or private key
    /// </summary>
    /// <param name="key">Key to decode</param>
    /// <returns>Array containing E/D value and N value</returns>
    public static BigInteger[] DecodeKey(string key)
    {
        var cipherBytes = Convert.FromBase64String(key);

        var numEBytes = cipherBytes.Take(4).ToArray();
        Array.Reverse(numEBytes);
        var numE = BitConverter.ToInt32(numEBytes, 0);

        var eBytes = cipherBytes.Skip(4).Take(numE).ToArray();
        var E = new BigInteger(eBytes);

        var numNBytes = cipherBytes.Skip(4 + numE).Take(4).ToArray();
        Array.Reverse(numNBytes);
        var numN = BitConverter.ToInt32(numNBytes, 0);
        var N = new BigInteger(cipherBytes.Skip(8 + numE).Take(numN).ToArray());
        return new[] { E, N };
    }
}