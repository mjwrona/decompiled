// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureDnsUtils
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class AzureDnsUtils
  {
    public static bool UseSdkDns(this IVssRequestContext requestContext) => !(requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS) && !requestContext.IsFeatureEnabled(AzureDnsFeatureFlags.DisableSdkDns);

    public static string NormalizeRecordName(string recordName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(recordName, nameof (recordName));
      return !recordName.EndsWith(".") ? recordName + "." : recordName;
    }

    public static string GetDnsRecord(string collectionName, string rootUri)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(collectionName, nameof (collectionName));
      ArgumentUtility.CheckStringForNullOrEmpty(rootUri, nameof (rootUri));
      return collectionName + "." + AzureDnsUtils.GetDnsRecord(rootUri);
    }

    public static string GetDnsRecord(string uri)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(uri, nameof (uri));
      return AzureDnsUtils.GetDnsRecord(new Uri(uri));
    }

    public static string GetDnsRecord(Uri uri)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      return uri.Host + ".";
    }

    public static IPAddress GetAzureVip(string url)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(url, nameof (url));
      return ((IEnumerable<IPAddress>) Dns.GetHostAddresses(url)).FirstOrDefault<IPAddress>((Func<IPAddress, bool>) (ip => ip.AddressFamily == AddressFamily.InterNetwork));
    }

    public static bool IsValidDnsEntry(string hostname, out IPHostEntry entry)
    {
      try
      {
        entry = Dns.GetHostEntry(hostname);
        return true;
      }
      catch (SocketException ex)
      {
        if (ex.SocketErrorCode == SocketError.HostNotFound || ex.SocketErrorCode == SocketError.NoData)
        {
          entry = (IPHostEntry) null;
          return false;
        }
        throw;
      }
    }

    public static bool IsValidDnsEntry(string hostname) => AzureDnsUtils.IsValidDnsEntry(hostname, out IPHostEntry _);

    public static string RemoveDotFromName(string name) => name.TrimEnd('.');
  }
}
