// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.BaiduCredential
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "BaiduCredential", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class BaiduCredential : PnsCredential
  {
    internal const string AppPlatformName = "baidu";
    internal const string ProdAccessTokenServiceUrl = "https://channel.api.duapp.com/rest/2.0/channel/channel";
    internal const string NokiaProdAccessTokenServiceUrl = "https://nnapi.ovi.com/nnapi/2.0/send";
    internal const string MockAccessTokenServiceUrl = "http://localhost:8450/gcm/send";
    internal const string MockRunnerAccessTokenServiceUrl = "http://pushtestservice.cloudapp.net/gcm/send";
    internal const string MockIntAccessTokenServiceUrl = "http://pushtestservice2.cloudapp.net/gcm/send";
    internal const string MockPerformanceAccessTokenServiceUrl = "http://pushperfnotificationserver.cloudapp.net/gcm/send";
    internal const string MockEnduranceAccessTokenServiceUrl = "http://pushstressnotificationserver.cloudapp.net/gcm/send";
    internal const string MockEnduranceAccessTokenServiceUrl1 = "http://pushnotificationserver.cloudapp.net/gcm/send";

    public BaiduCredential()
    {
    }

    public BaiduCredential(string baiduApiKey) => this.BaiduApiKey = baiduApiKey;

    public string BaiduApiKey
    {
      get => this[nameof (BaiduApiKey)];
      set => this[nameof (BaiduApiKey)] = value;
    }

    public string BaiduEndPoint
    {
      get => this[nameof (BaiduEndPoint)] ?? "https://channel.api.duapp.com/rest/2.0/channel/channel";
      set => this[nameof (BaiduEndPoint)] = value;
    }

    public string BaiduSecretKey
    {
      get => this[nameof (BaiduSecretKey)];
      set => this[nameof (BaiduSecretKey)] = value;
    }

    internal override string AppPlatform => "baidu";

    public override bool Equals(object obj) => obj is BaiduCredential baiduCredential && baiduCredential.BaiduApiKey == this.BaiduApiKey;

    public override int GetHashCode() => string.IsNullOrWhiteSpace(this.BaiduApiKey) ? base.GetHashCode() : this.BaiduApiKey.GetHashCode();

    internal static bool IsMockBaidu(string endPoint) => endPoint.ToUpperInvariant().Contains("CLOUDAPP.NET");

    protected override void OnValidate(bool allowLocalMockPns)
    {
      if (this.Properties == null || this.Properties.Count > 2)
        throw new InvalidDataContractException(SRClient.BaiduRequiredProperties);
      if (this.Properties.Count < 1 || string.IsNullOrWhiteSpace(this.BaiduApiKey))
        throw new InvalidDataContractException(SRClient.BaiduApiKeyNotSpecified);
      bool flag = !string.Equals(this.BaiduEndPoint, "https://channel.api.duapp.com/rest/2.0/channel/channel", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.BaiduEndPoint, "https://nnapi.ovi.com/nnapi/2.0/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.BaiduEndPoint, "http://pushtestservice.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.BaiduEndPoint, "http://pushtestservice2.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.BaiduEndPoint, "http://pushperfnotificationserver.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.BaiduEndPoint, "http://pushstressnotificationserver.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.BaiduEndPoint, "http://pushnotificationserver.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && (!allowLocalMockPns || !string.Equals(this.BaiduEndPoint, "http://localhost:8450/gcm/send", StringComparison.OrdinalIgnoreCase));
      if (!Uri.TryCreate(this.BaiduEndPoint, UriKind.Absolute, out Uri _) | flag)
        throw new InvalidDataContractException(SRClient.InvalidBaiduEndpoint);
    }
  }
}
