// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ApnsCredential
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "ApnsCredential", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class ApnsCredential : PnsCredential
  {
    internal const string AppPlatformName = "apple";
    internal const string ApnsGatewayEndpoint = "gateway.push.apple.com";
    internal const string ApnsGatewaySandboxEndpoint = "gateway.sandbox.push.apple.com";
    internal const string ApnsFeedbackEndpoint = "feedback.push.apple.com";
    internal const string ApnsFeedbackSandboxEndpoint = "feedback.sandbox.push.apple.com";
    private static HashSet<string> validApnsEndpoints = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "gateway.push.apple.com",
      "gateway.sandbox.push.apple.com",
      "pushtestservice.cloudapp.net",
      "pushtestservice2.cloudapp.net",
      "pushperfnotificationserver.cloudapp.net",
      "pushstressnotificationserver.cloudapp.net",
      "pushnotificationserver.cloudapp.net"
    };
    private static HashSet<string> validLocalApnsEndpoints = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "localhost",
      "127.0.0.1"
    };

    public ApnsCredential() => this.Endpoint = "gateway.push.apple.com";

    public ApnsCredential(byte[] certificateBuffer, string certificateKey)
      : this()
    {
      try
      {
        this.ApnsCertificate = Convert.ToBase64String(certificateBuffer);
      }
      catch (Exception ex)
      {
        if (!Fx.IsFatal(ex))
          throw new ArgumentException(nameof (certificateBuffer), ex);
        throw;
      }
      this.CertificateKey = certificateKey;
    }

    public ApnsCredential(string certificatePath, string certificateKey)
      : this()
    {
      try
      {
        this.ApnsCertificate = this.GetApnsClientCertificate(certificatePath);
      }
      catch (Exception ex)
      {
        throw new ArgumentException(nameof (certificatePath), ex);
      }
      this.CertificateKey = certificateKey;
    }

    internal static bool IsMockApns(string endpoint) => endpoint.ToUpperInvariant().Contains("CLOUDAPP.NET");

    internal override string AppPlatform => "apple";

    public string ApnsCertificate
    {
      get => this[nameof (ApnsCertificate)];
      set => this[nameof (ApnsCertificate)] = value;
    }

    public string CertificateKey
    {
      get => this[nameof (CertificateKey)];
      set => this[nameof (CertificateKey)] = value;
    }

    public string Endpoint
    {
      get => this[nameof (Endpoint)];
      set => this[nameof (Endpoint)] = value;
    }

    internal X509Certificate2 NativeCertificate { get; set; }

    internal static bool IsValidApnsEndpoint(string endpoint) => ApnsCredential.validApnsEndpoints.Contains(endpoint);

    private string GetApnsClientCertificate(string certPath)
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
      if (this.Properties == null || this.Properties.Count < 3)
        throw new InvalidDataContractException(SRClient.ApnsRequiredPropertiesError);
      if (string.IsNullOrWhiteSpace(this.ApnsCertificate) || string.IsNullOrWhiteSpace(this.Endpoint))
        throw new InvalidDataContractException(SRClient.ApnsPropertiesNotSpecified);
      if (!ApnsCredential.validApnsEndpoints.Contains(this.Endpoint) && (!allowLocalMockPns || !ApnsCredential.validLocalApnsEndpoints.Contains(this.Endpoint)))
        throw new InvalidDataContractException(SRClient.ApnsEndpointNotAllowed);
      try
      {
        this.NativeCertificate = this.CertificateKey == null ? new X509Certificate2(this.ApnsCertificate) : new X509Certificate2(Convert.FromBase64String(this.ApnsCertificate), this.CertificateKey);
        if (!this.NativeCertificate.HasPrivateKey)
          throw new InvalidDataContractException(SRClient.ApnsCertificatePrivatekeyMissing);
        if (DateTime.UtcNow > this.NativeCertificate.NotAfter)
          throw new InvalidDataContractException(SRClient.ApnsCertificateExpired);
        if (DateTime.UtcNow < this.NativeCertificate.NotBefore)
          throw new InvalidDataContractException(SRClient.ApnsCertificateNotValid);
      }
      catch (CryptographicException ex)
      {
        throw new InvalidDataContractException(SRClient.ApnsCertificateNotUsable((object) ex.Message));
      }
      catch (FormatException ex)
      {
        throw new InvalidDataContractException(SRClient.ApnsCertificateNotUsable((object) ex.Message));
      }
    }

    public override bool Equals(object other) => other is ApnsCredential apnsCredential && apnsCredential.CertificateKey == this.CertificateKey && apnsCredential.ApnsCertificate == this.ApnsCertificate;

    public override int GetHashCode()
    {
      if (string.IsNullOrWhiteSpace(this.CertificateKey) || string.IsNullOrWhiteSpace(this.ApnsCertificate))
        return base.GetHashCode();
      return string.IsNullOrWhiteSpace(this.CertificateKey) ? this.ApnsCertificate.GetHashCode() : this.CertificateKey.GetHashCode() ^ this.ApnsCertificate.GetHashCode();
    }
  }
}
