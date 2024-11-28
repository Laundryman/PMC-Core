using System;
using System.Security.Cryptography.X509Certificates;

namespace diam_planogram.Helpers
{
  public class RSACryptoHelper
  {
    public static X509Certificate2 LoadCert(string thumbprint)
    {
      X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
      store.Open(OpenFlags.ReadOnly);
      var certs = store.Certificates.Find(
                        X509FindType.FindByThumbprint,
                        thumbprint,
                        validOnly: false);

      if (certs.Count == 0) throw new Exception("Could not find cert");
      var cert = certs[0];
      return cert;
    }

  }
}