// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NokiaXCredential
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "NokiaXCredential", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  internal class NokiaXCredential : PnsCredential
  {
    internal const string AppPlatformName = "nokiax";
    internal const string ProdAccessTokenServiceUrl = "https://nnapi.ovi.com/nnapi/2.0/send";
    internal const string MockAccessTokenServiceUrl = "http://localhost:8450/gcm/send";
    internal const string MockRunnerAccessTokenServiceUrl = "http://pushtestservice.cloudapp.net/gcm/send";
    internal const string MockIntAccessTokenServiceUrl = "http://pushtestservice2.cloudapp.net/gcm/send";
    internal const string MockPerformanceAccessTokenServiceUrl = "http://pushperfnotificationserver.cloudapp.net/gcm/send";
    internal const string MockEnduranceAccessTokenServiceUrl = "http://pushstressnotificationserver.cloudapp.net/gcm/send";
    internal const string MockEnduranceAccessTokenServiceUrl1 = "http://pushnotificationserver.cloudapp.net/gcm/send";

    public NokiaXCredential()
    {
    }

    public NokiaXCredential(string nokiaXAuthorizationKey) => this.AuthorizationKey = nokiaXAuthorizationKey;

    public string AuthorizationKey
    {
      get => this[nameof (AuthorizationKey)];
      set => this[nameof (AuthorizationKey)] = value;
    }

    public string NokiaXEndPoint
    {
      get => this[nameof (NokiaXEndPoint)] ?? "https://nnapi.ovi.com/nnapi/2.0/send";
      set => this[nameof (NokiaXEndPoint)] = value;
    }

    internal override string AppPlatform => "nokiax";

    public override bool Equals(object obj) => obj is NokiaXCredential nokiaXcredential && nokiaXcredential.AuthorizationKey == this.AuthorizationKey;

    public override int GetHashCode() => string.IsNullOrWhiteSpace(this.AuthorizationKey) ? base.GetHashCode() : this.AuthorizationKey.GetHashCode();

    internal static bool IsMockNokiaX(string endpoint) => endpoint.ToUpperInvariant().Contains("CLOUDAPP.NET");

    protected override void OnValidate(bool allowLocalMockPns)
    {
      if (this.Properties == null || this.Properties.Count > 2)
        throw new InvalidDataContractException(SRClient.NokiaXRequiredProperties);
      if (this.Properties.Count < 1 || string.IsNullOrWhiteSpace(this.AuthorizationKey))
        throw new InvalidDataContractException(SRClient.NokiaXAuthorizationKeyNotSpecified);
      bool flag = !string.Equals(this.NokiaXEndPoint, "https://nnapi.ovi.com/nnapi/2.0/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.NokiaXEndPoint, "http://pushtestservice.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.NokiaXEndPoint, "http://pushtestservice2.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.NokiaXEndPoint, "http://pushperfnotificationserver.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.NokiaXEndPoint, "http://pushstressnotificationserver.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.NokiaXEndPoint, "http://pushnotificationserver.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && (!allowLocalMockPns || !string.Equals(this.NokiaXEndPoint, "http://localhost:8450/gcm/send", StringComparison.OrdinalIgnoreCase));
      if (!Uri.TryCreate(this.NokiaXEndPoint, UriKind.Absolute, out Uri _) | flag)
        throw new InvalidDataContractException(SRClient.InvalidNokiaXEndpoint);
    }
  }
}
