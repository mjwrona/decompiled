// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.AdmCredential
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "AdmCredential", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class AdmCredential : PnsCredential
  {
    internal const string AppPlatformName = "adm";
    internal const string ProdAuthTokenUrl = "https://api.amazon.com/auth/O2/token";
    internal const string MockAuthTokenUrl = "http://localhost:8450/adm/token";
    internal const string MockRunnerAuthTokenUrl = "http://pushtestservice.cloudapp.net/adm/token";
    internal const string MockIntAuthTokenUrl = "http://pushtestservice2.cloudapp.net/adm/token";
    internal const string MockPerformanceAuthTokenUrl = "http://pushperfnotificationserver.cloudapp.net/adm/token";
    internal const string MockEndurancAuthTokenUrl = "http://pushstressnotificationserver.cloudapp.net/adm/token";
    internal const string MockEndurancAuthTokenUrl1 = "http://pushnotificationserver.cloudapp.net/adm/token";
    internal const string ProdSendUrlTemplate = "https://api.amazon.com/messaging/registrations/{0}/messages";
    internal const string MockSendUrlTemplate = "http://localhost:8450/adm/send/{0}/messages";
    internal const string MockRunnerSendUrlTemplate = "http://pushtestservice.cloudapp.net/adm/send/{0}/messages";
    internal const string MockIntSendUrlTemplate = "http://pushtestservice2.cloudapp.net/adm/send/{0}/messages";
    internal const string MockPerformanceSendUrlTemplate = "http://pushperfnotificationserver.cloudapp.net/adm/send/{0}/messages";
    internal const string MockEndurancSendUrlTemplate = "http://pushstressnotificationserver.cloudapp.net/adm/send/{0}/messages";
    internal const string MockEndurancSendUrlTemplate1 = "http://pushnotificationserver.cloudapp.net/adm/send/{0}/messages";
    private const string ClientIdName = "ClientId";
    private const string ClientSecretName = "ClientSecret";
    private const string AuthTokenUrlName = "AuthTokenUrl";
    private const string SendUrlTemplateName = "SendUrlTemplate";
    private const string RequiredPropertiesList = "ClientId, ClientSecret";

    public AdmCredential()
    {
    }

    public AdmCredential(string clientId, string clientSecret)
    {
      this.ClientId = clientId;
      this.ClientSecret = clientSecret;
    }

    public string ClientId
    {
      get => this[nameof (ClientId)];
      set => this[nameof (ClientId)] = value;
    }

    public string ClientSecret
    {
      get => this[nameof (ClientSecret)];
      set => this[nameof (ClientSecret)] = value;
    }

    public string AuthTokenUrl
    {
      get => this[nameof (AuthTokenUrl)] ?? "https://api.amazon.com/auth/O2/token";
      set => this[nameof (AuthTokenUrl)] = value;
    }

    public string SendUrlTemplate
    {
      get => this[nameof (SendUrlTemplate)] ?? "https://api.amazon.com/messaging/registrations/{0}/messages";
      set => this[nameof (SendUrlTemplate)] = value;
    }

    internal override string AppPlatform => "adm";

    public override bool Equals(object other) => other is AdmCredential admCredential && admCredential.ClientId == this.ClientId && admCredential.ClientSecret == this.ClientSecret;

    public override int GetHashCode() => string.IsNullOrWhiteSpace(this.ClientId) || string.IsNullOrWhiteSpace(this.ClientSecret) ? base.GetHashCode() : this.ClientId.GetHashCode() ^ this.ClientSecret.GetHashCode();

    internal static bool IsMockAdm(string endpoint) => !endpoint.ToUpperInvariant().Contains("//API.AMAZON.COM");

    protected override void OnValidate(bool allowLocalMockPns)
    {
      if (this.Properties == null || string.IsNullOrEmpty(this.Properties["ClientId"]) && string.IsNullOrEmpty(this.Properties["ClientSecret"]))
        throw new InvalidDataContractException(SRClient.RequiredPropertiesNotSpecified((object) "ClientId, ClientSecret"));
      if (string.IsNullOrEmpty(this.Properties["ClientId"]))
        throw new InvalidDataContractException(SRClient.RequiredPropertyNotSpecified((object) "ClientId"));
      if (string.IsNullOrEmpty(this.Properties["ClientSecret"]))
        throw new InvalidDataContractException(SRClient.RequiredPropertyNotSpecified((object) "ClientSecret"));
      if (this.Properties.Count > 2)
      {
        if (this.Properties.Count > this.Properties.Keys.Intersect<string>((IEnumerable<string>) new string[4]
        {
          "ClientId",
          "ClientSecret",
          "SendUrlTemplate",
          "AuthTokenUrl"
        }).Count<string>())
          throw new InvalidDataContractException(SRClient.OnlyNPropertiesRequired((object) 2, (object) "ClientId, ClientSecret"));
      }
      Uri result;
      if (!Uri.TryCreate(this.AuthTokenUrl, UriKind.Absolute, out result) || !string.Equals(this.AuthTokenUrl, "https://api.amazon.com/auth/O2/token", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.AuthTokenUrl, "http://pushtestservice.cloudapp.net/adm/token", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.AuthTokenUrl, "http://pushtestservice2.cloudapp.net/adm/token", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.AuthTokenUrl, "http://pushperfnotificationserver.cloudapp.net/adm/token", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.AuthTokenUrl, "http://pushstressnotificationserver.cloudapp.net/adm/token", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.AuthTokenUrl, "http://pushnotificationserver.cloudapp.net/adm/token", StringComparison.OrdinalIgnoreCase) && (!allowLocalMockPns || !string.Equals(this.AuthTokenUrl, "http://localhost:8450/adm/token", StringComparison.OrdinalIgnoreCase)))
        throw new InvalidDataContractException(SRClient.InvalidAdmAuthTokenUrl);
      try
      {
        if (!string.IsNullOrWhiteSpace(this.SendUrlTemplate))
        {
          if (Uri.TryCreate(string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.SendUrlTemplate, new object[1]
          {
            (object) "AdmRegistrationId"
          }), UriKind.Absolute, out result) && (string.Equals(this.SendUrlTemplate, "https://api.amazon.com/messaging/registrations/{0}/messages", StringComparison.OrdinalIgnoreCase) || string.Equals(this.SendUrlTemplate, "http://pushtestservice.cloudapp.net/adm/send/{0}/messages", StringComparison.OrdinalIgnoreCase) || string.Equals(this.SendUrlTemplate, "http://pushtestservice2.cloudapp.net/adm/send/{0}/messages", StringComparison.OrdinalIgnoreCase) || string.Equals(this.SendUrlTemplate, "http://pushperfnotificationserver.cloudapp.net/adm/send/{0}/messages", StringComparison.OrdinalIgnoreCase) || string.Equals(this.SendUrlTemplate, "http://pushstressnotificationserver.cloudapp.net/adm/send/{0}/messages", StringComparison.OrdinalIgnoreCase) || string.Equals(this.SendUrlTemplate, "http://pushnotificationserver.cloudapp.net/adm/send/{0}/messages", StringComparison.OrdinalIgnoreCase) || allowLocalMockPns && string.Equals(this.SendUrlTemplate, "http://localhost:8450/adm/send/{0}/messages", StringComparison.OrdinalIgnoreCase)))
            return;
        }
        throw new InvalidDataContractException(SRClient.InvalidAdmSendUrlTemplate);
      }
      catch (FormatException ex)
      {
        throw new InvalidDataContractException(SRClient.InvalidAdmSendUrlTemplate);
      }
    }
  }
}
