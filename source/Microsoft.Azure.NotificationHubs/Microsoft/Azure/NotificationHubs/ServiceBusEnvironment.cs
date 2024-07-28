// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ServiceBusEnvironment
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Globalization;
using System.Net;

namespace Microsoft.Azure.NotificationHubs
{
  public static class ServiceBusEnvironment
  {
    private static readonly ConnectivitySettings HttpListenerSettings = new ConnectivitySettings();

    public static string DefaultIdentityHostName => RelayEnvironment.StsHostName;

    public static IWebProxy Proxy { get; set; }

    public static ConnectivitySettings SystemConnectivity => ServiceBusEnvironment.HttpListenerSettings;

    internal static bool? UseNoRendezvous { get; set; }

    public static Uri CreateAccessControlUri(string serviceNamespace)
    {
      if (string.IsNullOrEmpty(serviceNamespace))
        throw new ArgumentException(SRClient.NullServiceNameSpace, nameof (serviceNamespace));
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://{0}-sb.{1}:{2}/WRAPv0.9/", new object[3]
      {
        (object) serviceNamespace,
        (object) RelayEnvironment.StsHostName,
        (object) RelayEnvironment.StsHttpsPort
      }));
    }

    public static Uri CreateServiceUri(string scheme, string serviceNamespace, string servicePath) => ServiceBusEnvironment.CreateServiceUri(scheme, serviceNamespace, servicePath, false, RelayEnvironment.RelayHostRootName);

    public static Uri CreateServiceUri(
      string scheme,
      string serviceNamespace,
      string servicePath,
      bool suppressRelayPathPrefix)
    {
      return ServiceBusEnvironment.CreateServiceUri(scheme, serviceNamespace, servicePath, suppressRelayPathPrefix, RelayEnvironment.RelayHostRootName);
    }

    internal static string CreateAccessControlIssuer(string serviceNamespace)
    {
      if (string.IsNullOrEmpty(serviceNamespace))
        throw new ArgumentException(SRClient.NullServiceNameSpace, nameof (serviceNamespace));
      return RelayEnvironment.StsHttpsPort != 443 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://{0}-sb.{1}:{2}/", new object[3]
      {
        (object) serviceNamespace,
        (object) RelayEnvironment.StsHostName,
        (object) RelayEnvironment.StsHttpsPort
      }) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://{0}-sb.{1}/", new object[2]
      {
        (object) serviceNamespace,
        (object) RelayEnvironment.StsHostName
      });
    }

    private static Uri CreateServiceUri(
      string scheme,
      string serviceNamespace,
      string servicePath,
      bool suppressRelayPathPrefix,
      string hostName)
    {
      string serviceNamespace1 = serviceNamespace.Trim();
      ServiceBusEnvironment.ValidateSchemeAndNamespace(scheme, serviceNamespace1);
      if (!servicePath.EndsWith("/", StringComparison.Ordinal))
        servicePath += "/";
      Uri serviceUri = ServiceBusUriHelper.CreateServiceUri(scheme, serviceNamespace1, hostName, servicePath, suppressRelayPathPrefix);
      return ServiceBusUriHelper.IsSafeBasicLatinUriPath(serviceUri) ? serviceUri : throw new ArgumentException(SRClient.PathSegmentASCIICharacters, servicePath);
    }

    private static void ValidateSchemeAndNamespace(string scheme, string serviceNamespace)
    {
      if (!ServiceBusEnvironment.ValidateScheme(scheme))
        throw new ArgumentException(SRClient.InvalidSchemeValue((object) "sb"), nameof (scheme));
      if (!ServiceBusEnvironment.ValidateServiceNamespace(serviceNamespace))
        throw new ArgumentException(SRClient.InvalidServiceNameSpace((object) serviceNamespace), nameof (serviceNamespace));
    }

    private static bool ValidateScheme(string scheme)
    {
      if (string.IsNullOrEmpty(scheme))
        return false;
      return scheme.Equals("http") || scheme.Equals("https") || scheme.Equals("sb");
    }

    private static bool ValidateServiceNamespace(string serviceNamespace) => !string.IsNullOrEmpty(serviceNamespace) && ServiceBusUriHelper.IsBasicLatinNonControlString(serviceNamespace);
  }
}
