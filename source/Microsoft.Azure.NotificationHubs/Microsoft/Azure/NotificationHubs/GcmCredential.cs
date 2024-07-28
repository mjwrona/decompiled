// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.GcmCredential
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "GcmCredential", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class GcmCredential : PnsCredential
  {
    internal const string AppPlatformName = "gcm";
    internal const string ProdAccessTokenServiceUrl = "https://android.googleapis.com/gcm/send";
    internal const string MockAccessTokenServiceUrl = "http://localhost:8450/gcm/send";
    internal const string MockRunnerAccessTokenServiceUrl = "http://pushtestservice.cloudapp.net/gcm/send";
    internal const string MockIntAccessTokenServiceUrl = "http://pushtestservice2.cloudapp.net/gcm/send";
    internal const string MockPerformanceAccessTokenServiceUrl = "http://pushperfnotificationserver.cloudapp.net/gcm/send";
    internal const string MockEnduranceAccessTokenServiceUrl = "http://pushstressnotificationserver.cloudapp.net/gcm/send";
    internal const string MockEnduranceAccessTokenServiceUrl1 = "http://pushnotificationserver.cloudapp.net/gcm/send";

    public GcmCredential()
    {
    }

    public GcmCredential(string googleApiKey) => this.GoogleApiKey = googleApiKey;

    public string GcmEndpoint
    {
      get => this[nameof (GcmEndpoint)] ?? "https://android.googleapis.com/gcm/send";
      set => this[nameof (GcmEndpoint)] = value;
    }

    public string GoogleApiKey
    {
      get => this[nameof (GoogleApiKey)];
      set => this[nameof (GoogleApiKey)] = value;
    }

    internal override string AppPlatform => "gcm";

    internal static bool IsMockGcm(string endpoint) => endpoint.ToUpperInvariant().Contains("CLOUDAPP.NET");

    protected override void OnValidate(bool allowLocalMockPns)
    {
      if (this.Properties == null || this.Properties.Count > 2)
        throw new InvalidDataContractException(SRClient.GcmRequiredProperties);
      if (this.Properties.Count < 1 || string.IsNullOrWhiteSpace(this.GoogleApiKey))
        throw new InvalidDataContractException(SRClient.GoogleApiKeyNotSpecified);
      if (this.Properties.Count == 2 && string.IsNullOrEmpty(this["GcmEndpoint"]))
        throw new InvalidDataContractException(SRClient.GcmEndpointNotSpecified);
      if (!Uri.TryCreate(this.GcmEndpoint, UriKind.Absolute, out Uri _) || !string.Equals(this.GcmEndpoint, "https://android.googleapis.com/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.GcmEndpoint, "http://pushtestservice.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.GcmEndpoint, "http://pushtestservice2.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.GcmEndpoint, "http://pushperfnotificationserver.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.GcmEndpoint, "http://pushstressnotificationserver.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.GcmEndpoint, "http://pushnotificationserver.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && (!allowLocalMockPns || !string.Equals(this.GcmEndpoint, "http://localhost:8450/gcm/send", StringComparison.OrdinalIgnoreCase)))
        throw new InvalidDataContractException(SRClient.InvalidGcmEndpoint);
    }

    public override bool Equals(object other) => other is GcmCredential gcmCredential && gcmCredential.GoogleApiKey == this.GoogleApiKey;

    public override int GetHashCode() => string.IsNullOrWhiteSpace(this.GoogleApiKey) ? base.GetHashCode() : this.GoogleApiKey.GetHashCode();
  }
}
