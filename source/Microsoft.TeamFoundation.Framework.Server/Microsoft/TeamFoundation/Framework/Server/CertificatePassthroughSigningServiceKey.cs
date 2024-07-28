// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CertificatePassthroughSigningServiceKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CertificatePassthroughSigningServiceKey : 
    SigningServiceKey,
    ICertificatePassthroughSigningKey
  {
    internal CertificatePassthroughSigningServiceKey(Guid identifier)
      : base(SigningKeyType.CertificatePassthrough, identifier)
    {
    }

    protected override byte[] GetKeyData(IVssRequestContext requestContext)
    {
      this.CheckKeyDataIsNotNull();
      return this.LoadCertificate().Export(X509ContentType.Cert);
    }

    protected override void StoreKeyData(
      IVssRequestContext requestContext,
      byte[] rawKeyData,
      bool overwriteExisting)
    {
      this.CheckKeyDataIsNull();
      this.KeyData = this.StoreKeyDataToDatabase(requestContext, rawKeyData, overwriteExisting).KeyData;
    }

    protected override void DeleteKeyData(IVssRequestContext requestContext)
    {
    }

    protected override ISigner GetKeySigner(
      IVssRequestContext requestContext,
      SigningAlgorithm paddingAlgorithm)
    {
      return SigningManager.GetSigner(this.LoadCertificate(), paddingAlgorithm);
    }

    public string Thumbprint
    {
      get
      {
        this.CheckKeyDataIsNotNull();
        return Encoding.ASCII.GetString(this.KeyData);
      }
    }

    public X509Certificate2 LoadCertificate()
    {
      for (int index = 0; index < 5; ++index)
      {
        X509Certificate2 x509Certificate2_1 = this.LoadCertificateInner(StoreLocation.LocalMachine);
        if (x509Certificate2_1 != null)
          return x509Certificate2_1;
        X509Certificate2 x509Certificate2_2 = this.LoadCertificateInner(StoreLocation.CurrentUser);
        if (x509Certificate2_2 != null)
          return x509Certificate2_2;
        Thread.Sleep(TimeSpan.FromSeconds(2.0));
      }
      throw new CertificateNotFoundException("Signing certificate with thumbprint " + this.Thumbprint + " not found in local store", this.Thumbprint);
    }

    private X509Certificate2 LoadCertificateInner(StoreLocation storeLocation)
    {
      this.CheckKeyDataIsNotNull();
      string thumbprint = this.Thumbprint;
      X509Store x509Store = new X509Store(StoreName.My, storeLocation);
      x509Store.Open(OpenFlags.ReadOnly);
      try
      {
        X509Certificate2Collection certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, (object) thumbprint, false);
        return certificate2Collection.Count > 0 ? certificate2Collection[0] : (X509Certificate2) null;
      }
      finally
      {
        x509Store.Close();
      }
    }
  }
}
