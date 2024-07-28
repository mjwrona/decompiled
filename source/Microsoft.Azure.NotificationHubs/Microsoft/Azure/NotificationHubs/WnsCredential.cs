// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.WnsCredential
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "WnsCredential", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class WnsCredential : PnsCredential
  {
    internal const string AppPlatformName = "windows";
    internal const string ProdAccessTokenServiceUrl = "https://login.live.com/accesstoken.srf";
    internal const string MockAccessTokenServiceUrl = "http://localhost:8450/LiveID/accesstoken.srf";
    internal const string MockIntAccessTokenServiceUrl = "http://pushtestservice.cloudapp.net/LiveID/accesstoken.srf";
    internal const string MockRunnerAccessTokenServiceUrl = "http://pushtestservice2.cloudapp.net/LiveID/accesstoken.srf";
    internal const string MockIntInvalidAccessTokenServiceUrl = "http://pushtestserviceInvalid.cloudapp.net/LiveID/accesstoken.srf";
    internal const string MockPerformanceAccessTokenServiceUrl = "http://pushperfnotificationserver.cloudapp.net/LiveID/accesstoken.srf";
    internal const string MockEnduranceAccessTokenServiceUrl = "http://pushstressnotificationserver.cloudapp.net/LiveID/accesstoken.srf";
    internal const string MockEnduranceAccessTokenServiceUrl1 = "http://pushnotificationserver.cloudapp.net/LiveID/accesstoken.srf";

    public WnsCredential()
    {
    }

    public WnsCredential(string packageSid, string secretKey)
    {
      this.PackageSid = packageSid;
      this.SecretKey = secretKey;
    }

    internal override string AppPlatform => "windows";

    public string PackageSid
    {
      get => this[nameof (PackageSid)];
      set => this[nameof (PackageSid)] = value;
    }

    public string SecretKey
    {
      get => this[nameof (SecretKey)];
      set => this[nameof (SecretKey)] = value;
    }

    public string WindowsLiveEndpoint
    {
      get => this[nameof (WindowsLiveEndpoint)] ?? "https://login.live.com/accesstoken.srf";
      set => this[nameof (WindowsLiveEndpoint)] = value;
    }

    protected override void OnValidate(bool allowLocalMockPns)
    {
      if (this.Properties == null || this.Properties.Count > 3)
        throw new InvalidDataContractException(SRClient.PackageSidAndSecretKeyAreRequired);
      if (this.Properties.Count < 2 || string.IsNullOrWhiteSpace(this.PackageSid) || string.IsNullOrWhiteSpace(this.SecretKey))
        throw new InvalidDataContractException(SRClient.PackageSidOrSecretKeyInvalid);
      if (this.Properties.Count == 3 && string.IsNullOrEmpty(this["WindowsLiveEndpoint"]))
        throw new InvalidDataContractException(SRClient.PackageSidAndSecretKeyAreRequired);
      if (!Uri.TryCreate(this.WindowsLiveEndpoint, UriKind.Absolute, out Uri _))
        throw new InvalidDataContractException(SRClient.InvalidWindowsLiveEndpoint);
    }

    public override bool Equals(object other) => other is WnsCredential wnsCredential && wnsCredential.PackageSid == this.PackageSid && wnsCredential.SecretKey == this.SecretKey;

    public override int GetHashCode() => string.IsNullOrWhiteSpace(this.PackageSid) || string.IsNullOrWhiteSpace(this.SecretKey) ? base.GetHashCode() : this.PackageSid.GetHashCode() ^ this.SecretKey.GetHashCode();
  }
}
