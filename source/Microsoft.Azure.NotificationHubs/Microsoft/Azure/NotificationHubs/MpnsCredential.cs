// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.MpnsCredential
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "MpnsCredential", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class MpnsCredential : PnsCredential
  {
    internal const string AppPlatformName = "windowsphone";

    public MpnsCredential()
    {
    }

    public MpnsCredential(X509Certificate mpnsCertificate, string certificateKey)
      : this(MpnsCredential.ExportCertificateBytes(mpnsCertificate, certificateKey), certificateKey)
    {
    }

    public MpnsCredential(byte[] certificateBuffer, string certificateKey)
      : this()
    {
      try
      {
        this.MpnsCertificate = Convert.ToBase64String(certificateBuffer);
      }
      catch (Exception ex)
      {
        if (!Fx.IsFatal(ex))
          throw new ArgumentException(nameof (certificateBuffer), ex);
        throw;
      }
      this.CertificateKey = certificateKey;
    }

    public MpnsCredential(string certificatePath, string certificateKey)
      : this()
    {
      try
      {
        this.MpnsCertificate = this.GetMpnsClientCertificate(certificatePath);
      }
      catch (Exception ex)
      {
        throw new ArgumentException(nameof (certificatePath), ex);
      }
      this.CertificateKey = certificateKey;
    }

    internal override string AppPlatform => "windowsphone";

    public string MpnsCertificate
    {
      get => this[nameof (MpnsCertificate)];
      set => this[nameof (MpnsCertificate)] = value;
    }

    public string CertificateKey
    {
      get => this[nameof (CertificateKey)];
      set => this[nameof (CertificateKey)] = value;
    }

    internal X509Certificate2 NativeCertificate { get; set; }

    private string GetMpnsClientCertificate(string certPath)
    {
      using (FileStream fileStream = File.OpenRead(certPath))
      {
        byte[] array;
        using (MemoryStream destination = new MemoryStream())
        {
          fileStream.CopyTo((Stream) destination);
          array = destination.ToArray();
        }
        return Convert.ToBase64String(array);
      }
    }

    protected override void OnValidate(bool allowLocalMockPns)
    {
      if (this.Properties == null || this.Properties.Count <= 0)
        return;
      if (this.Properties.Count > 2)
        throw new InvalidDataContractException(SRClient.MpnsRequiredPropertiesError);
      if (this.Properties.Count >= 2)
      {
        if (!string.IsNullOrWhiteSpace(this.MpnsCertificate))
        {
          try
          {
            X509Certificate2 x509Certificate2 = this.CertificateKey == null ? new X509Certificate2(this.MpnsCertificate) : new X509Certificate2(Convert.FromBase64String(this.MpnsCertificate), this.CertificateKey);
            if (!x509Certificate2.HasPrivateKey)
              throw new InvalidDataContractException(SRClient.MpnsCertificatePrivatekeyMissing);
            if (DateTime.Now > x509Certificate2.NotAfter)
              throw new InvalidDataContractException(SRClient.MpnsCertificateExpired);
            if (!(DateTime.Now < x509Certificate2.NotBefore))
              return;
            throw new InvalidDataContractException(SRClient.InvalidMpnsCertificate);
          }
          catch (CryptographicException ex)
          {
            throw new InvalidDataContractException(SRClient.MpnsCertificateError((object) ex.Message));
          }
          catch (FormatException ex)
          {
            throw new InvalidDataContractException(SRClient.MpnsCertificateError((object) ex.Message));
          }
        }
      }
      throw new InvalidDataContractException(SRClient.MpnsInvalidPropeties);
    }

    public override bool Equals(object other) => other is MpnsCredential mpnsCredential && mpnsCredential.CertificateKey == this.CertificateKey && mpnsCredential.MpnsCertificate == this.MpnsCertificate;

    public override int GetHashCode()
    {
      if (string.IsNullOrWhiteSpace(this.CertificateKey) || string.IsNullOrWhiteSpace(this.MpnsCertificate))
        return base.GetHashCode();
      return string.IsNullOrWhiteSpace(this.CertificateKey) ? this.MpnsCertificate.GetHashCode() : this.CertificateKey.GetHashCode() ^ this.MpnsCertificate.GetHashCode();
    }

    private static byte[] ExportCertificateBytes(
      X509Certificate mpnsCertificate,
      string certificateKey)
    {
      if (mpnsCertificate == null)
        throw new ArgumentNullException(nameof (mpnsCertificate));
      return !string.IsNullOrEmpty(certificateKey) ? mpnsCertificate.Export(X509ContentType.Pfx, certificateKey) : throw new ArgumentNullException(nameof (certificateKey));
    }
  }
}
