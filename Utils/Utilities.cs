using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Integrador.Utils;

public static class Utilities
{

    /// <summary>
    /// This method is used to convert a json string to a Root object
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Request</returns>
    public static List<Request?> GetRequests(List<string> request)
    {

        List<Request?> requests = request.Select(x => JsonConvert.DeserializeObject<Request>(x)).ToList();
        return requests;
    }

    public static string GenerateSeededGuid(string seedString)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(seedString));
        int seed = BitConverter.ToInt32(hashBytes, 0);
        Random random = new(seed);
        var guid = new byte[16];
        random.NextBytes(guid);

        return new Guid(guid).ToString("D").ToUpperInvariant();
    }
}