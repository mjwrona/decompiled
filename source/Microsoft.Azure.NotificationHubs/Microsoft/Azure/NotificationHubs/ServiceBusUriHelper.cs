// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ServiceBusUriHelper
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.Azure.NotificationHubs
{
  internal static class ServiceBusUriHelper
  {
    public static readonly Regex SafeBasicLatinUriSegmentExpression = new Regex("^[\\u0020\\u0021\\u0024-\\u002E\\u0030-\\u003B\\u003D\\u0040-\\u005B\\u005D-\\u007D\\u007E]*/?$", RegexOptions.Compiled);
    public static readonly Regex SafeMessagingEntityNameExpression = new Regex("^[\\w-\\.\\$]*/?$", RegexOptions.Compiled | RegexOptions.ECMAScript);
    private static readonly Regex BasicLatinNonControlStringExpression = new Regex("^[\\u0020-\\u007E]*$", RegexOptions.Compiled);

    internal static Uri CreateServiceUri(string scheme, string authority, string servicePath) => ServiceBusUriHelper.CreateServiceUri(scheme, authority, servicePath, false);

    internal static Uri CreateServiceUri(
      string scheme,
      string authority,
      string servicePath,
      bool suppressRelayPathPrefix)
    {
      UriBuilder uriBuilder = new UriBuilder("tempscheme://" + authority);
      if (uriBuilder.Port == -1)
        uriBuilder.Port = ServiceBusUriHelper.GetSchemePort(scheme);
      uriBuilder.Scheme = scheme;
      uriBuilder.Path = ServiceBusUriHelper.RefinePath(servicePath, suppressRelayPathPrefix);
      return uriBuilder.Uri;
    }

    internal static Uri CreateServiceUri(
      string scheme,
      string serviceNamespace,
      string hostName,
      string servicePath)
    {
      return ServiceBusUriHelper.CreateServiceUri(scheme, serviceNamespace, hostName, servicePath, false);
    }

    internal static Uri CreateServiceUri(
      string scheme,
      string serviceNamespace,
      string hostName,
      string servicePath,
      bool suppressRelayPathPrefix)
    {
      string str;
      if (!string.IsNullOrEmpty(serviceNamespace))
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", new object[2]
        {
          (object) serviceNamespace,
          (object) hostName
        });
      else
        str = hostName;
      string authority = str;
      return ServiceBusUriHelper.CreateServiceUri(scheme, authority, servicePath, suppressRelayPathPrefix);
    }

    private static int GetSchemePort(string scheme)
    {
      switch (scheme)
      {
        case "http":
          return RelayEnvironment.RelayHttpPort;
        case "https":
          return RelayEnvironment.RelayHttpsPort;
        default:
          return -1;
      }
    }

    internal static bool IsRequestAuthorityIpAddress(Uri requestUri)
    {
      if ((Uri) null == requestUri)
        return false;
      return requestUri.HostNameType == UriHostNameType.IPv4 || requestUri.HostNameType == UriHostNameType.IPv6;
    }

    internal static string RemoveServicePathPrefix(string path)
    {
      string refinedPath;
      ServiceBusUriHelper.TryRemoveServicePathPrefix(path, out refinedPath, true);
      return refinedPath;
    }

    internal static bool TryRemoveServicePathPrefix(string path, out string refinedPath) => ServiceBusUriHelper.TryRemoveServicePathPrefix(path, out refinedPath, false);

    private static bool TryRemoveServicePathPrefix(
      string path,
      out string refinedPath,
      bool throwOnFailure)
    {
      refinedPath = path;
      if (!ServiceBusUriHelper.IsOneboxEnvironment)
        return true;
      if (!path.StartsWith(RelayEnvironment.RelayPathPrefix, StringComparison.OrdinalIgnoreCase))
      {
        if (throwOnFailure)
          throw new ArgumentException(SRClient.InputURIPath((object) path));
        return false;
      }
      refinedPath = path.Substring(RelayEnvironment.RelayPathPrefix.Length);
      return true;
    }

    internal static string NormalizeUri(Uri uri, bool ensureTrailingSlash = false) => ServiceBusUriHelper.NormalizeUri(uri.AbsoluteUri, uri.Scheme, ensureTrailingSlash: ensureTrailingSlash);

    internal static string NormalizeUri(
      string uri,
      string scheme,
      bool stripQueryParameters = true,
      bool stripPath = false,
      bool ensureTrailingSlash = false)
    {
      UriBuilder uriBuilder = new UriBuilder(uri)
      {
        Scheme = scheme,
        Port = -1,
        Fragment = string.Empty,
        Password = string.Empty,
        UserName = string.Empty
      };
      if (stripPath)
        uriBuilder.Path = string.Empty;
      if (stripQueryParameters)
        uriBuilder.Query = string.Empty;
      if (ensureTrailingSlash && !uriBuilder.Path.EndsWith("/", StringComparison.Ordinal))
        uriBuilder.Path += "/";
      return uriBuilder.Uri.AbsoluteUri;
    }

    internal static bool IsSafeUri(Uri uri)
    {
      string str = !(uri == (Uri) null) ? HttpUtility.UrlDecode(uri.AbsoluteUri) : throw new ArgumentNullException(nameof (uri));
      int length = str.Length;
      bool flag = false;
      for (int index = 0; index < length; ++index)
      {
        char ch = str[index];
        if (flag || (' ' > ch || ch > '~') && (' ' > ch || ch > '\uD7FF') && ('\uE000' > ch || ch > '�'))
        {
          if (!flag && '\uD800' <= ch && ch <= '\uDBFF')
          {
            flag = true;
          }
          else
          {
            if (!flag || '\uDC00' > ch || ch > '\uDFFF')
              return false;
            flag = false;
          }
        }
      }
      return !flag;
    }

    internal static bool IsSafeBasicLatinUriPath(Uri uri)
    {
      string[] strArray = !(uri == (Uri) null) ? uri.Segments : throw new ArgumentNullException(nameof (uri));
      for (int index = 1; index < strArray.Length; ++index)
      {
        string input = HttpUtility.UrlDecode(strArray[index]);
        if (!ServiceBusUriHelper.SafeBasicLatinUriSegmentExpression.IsMatch(input))
          return false;
      }
      return true;
    }

    internal static bool IsSafeMessagingEntityUriPath(Uri uri)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      if (!string.IsNullOrWhiteSpace(uri.Fragment) || !string.IsNullOrWhiteSpace(uri.UserInfo))
        return false;
      string[] segments = uri.Segments;
      for (int index = 1; index < segments.Length; ++index)
      {
        string input = HttpUtility.UrlDecode(segments[index]);
        if (!ServiceBusUriHelper.SafeMessagingEntityNameExpression.IsMatch(input))
          return false;
      }
      return true;
    }

    internal static bool IsBasicLatinNonControlString(string str) => ServiceBusUriHelper.BasicLatinNonControlStringExpression.IsMatch(str);

    internal static string ParseServiceNamespace(
      this Uri uri,
      string expectedHostnameSuffix,
      bool isReservedSuffixAllowed)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      if (expectedHostnameSuffix == null)
        throw new ArgumentNullException(nameof (expectedHostnameSuffix));
      string host = uri.Host;
      string serviceNamespaceCandidate = (string) null;
      if (host.EndsWith(expectedHostnameSuffix, StringComparison.OrdinalIgnoreCase))
        serviceNamespaceCandidate = host.Replace(expectedHostnameSuffix, string.Empty);
      if (serviceNamespaceCandidate == null)
        throw new FormatException(SRClient.UnexpedtedURIHostName((object) uri));
      return ServiceBusUriHelper.ServiceBusStringExtension.IsValidServiceNamespace(serviceNamespaceCandidate, isReservedSuffixAllowed) ? serviceNamespaceCandidate : throw new FormatException(SRClient.URIServiceNameSpace((object) uri));
    }

    private static bool IsOneboxEnvironment => !string.IsNullOrEmpty(RelayEnvironment.RelayPathPrefix);

    private static string RefinePath(string path, bool suppressRelayPathPrefix)
    {
      string str = path;
      return !ServiceBusUriHelper.IsOneboxEnvironment | suppressRelayPathPrefix ? str : (!string.IsNullOrEmpty(path) ? (path.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? RelayEnvironment.RelayPathPrefix + path : RelayEnvironment.RelayPathPrefix + "/" + path) : RelayEnvironment.RelayPathPrefix);
    }

    private static class ServiceBusStringExtension
    {
      private const int MinServiceNamespaceLength = 6;
      private const int MaxServiceNamespaceLength = 50;
      private static readonly string ServiceNamespacePattern = "^[a-zA-Z][a-zA-Z0-9-]{" + (object) 4 + "," + (object) 48 + "}[a-zA-Z0-9]$";
      private static readonly Regex ServiceNamespaceRegex = new Regex(ServiceBusUriHelper.ServiceBusStringExtension.ServiceNamespacePattern, RegexOptions.Compiled);
      private static IEnumerable<string> reservedHostnameSuffixes = (IEnumerable<string>) new string[3]
      {
        "-sb",
        "-mgmt",
        "-sb-mgmt"
      };

      public static bool IsValidServiceNamespace(
        string serviceNamespaceCandidate,
        bool isReservedSuffixAllowed)
      {
        if (serviceNamespaceCandidate == null)
          return false;
        string suffix;
        bool flag = ServiceBusUriHelper.ServiceBusStringExtension.HasReservedSuffix(serviceNamespaceCandidate, out suffix);
        if (!isReservedSuffixAllowed & flag)
          return false;
        serviceNamespaceCandidate = flag ? ServiceBusUriHelper.ServiceBusStringExtension.StripReservedSuffix(serviceNamespaceCandidate, suffix) : serviceNamespaceCandidate;
        return ServiceBusUriHelper.ServiceBusStringExtension.ServiceNamespaceRegex.IsMatch(serviceNamespaceCandidate) && !serviceNamespaceCandidate.StartsWith("xn--", StringComparison.OrdinalIgnoreCase);
      }

      private static string StripReservedSuffix(string serviceNamespaceCandidate, string suffix) => serviceNamespaceCandidate.Substring(0, serviceNamespaceCandidate.Length - suffix.Length);

      private static bool HasReservedSuffix(string serviceNamespaceCandidate, out string suffix)
      {
        suffix = ServiceBusUriHelper.ServiceBusStringExtension.reservedHostnameSuffixes.FirstOrDefault<string>((Func<string, bool>) (s => serviceNamespaceCandidate.EndsWith(s, StringComparison.Ordinal)));
        return suffix != null;
      }
    }
  }
}
