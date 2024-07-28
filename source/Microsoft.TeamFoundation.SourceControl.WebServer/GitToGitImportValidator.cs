// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitToGitImportValidator
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  internal static class GitToGitImportValidator
  {
    internal static bool ValidateGitToGitImportParams(
      IVssRequestContext requestContext,
      ImportRepositoryValidation remoteRepository,
      string traceArea,
      ClientTraceData ctData)
    {
      bool gitImportParams = false;
      try
      {
        if (requestContext.IsFeatureEnabled("SourceControl.EnableRepoImportDNSPinning"))
          GitToGitImportValidator.ApplyIPAddressAllowedRange(requestContext, remoteRepository, ctData);
        if (GitToGitImportValidator.IsUrlPointingLocalHostAndProbingOtherPort(remoteRepository.GitSource.Url, requestContext.RequestUri().AbsoluteUri))
        {
          ctData.Add("RemoteValidationTargetsLocalHost", (object) true);
          return false;
        }
        ctData.Add("RemoteValidationTargetsLocalHost", (object) false);
        if (!string.IsNullOrWhiteSpace(remoteRepository.Username) || !string.IsNullOrWhiteSpace(remoteRepository.Password))
          ctData.Add("RemoteValidationWithCredentials", (object) true);
        else
          ctData.Add("RemoteValidationWithCredentials", (object) false);
        gitImportParams = RemoteRefUtil.GetRefs(requestContext.RequestTracer, new Uri(remoteRepository.GitSource.Url), remoteRepository.Username, remoteRepository.Password, ctData).Any<NonLibGit2Reference>();
      }
      catch (Exception ex)
      {
        ctData.Add("GitValidationWithFailedWithException", (object) true);
        requestContext.TraceException(700220, traceArea, WebApiTraceLayers.BusinessLogic, ex);
      }
      return gitImportParams;
    }

    internal static bool IsUrlPointingLocalHostAndProbingOtherPort(
      string gitRemoteUrl,
      string importRequestUrl)
    {
      return GitToGitImportValidator.IsLocalhost(gitRemoteUrl) && new Uri(gitRemoteUrl).Port != new Uri(importRequestUrl).Port;
    }

    internal static bool IsLocalhost(string hostNameOrAddress)
    {
      if (string.IsNullOrEmpty(hostNameOrAddress))
        return false;
      try
      {
        IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
        return ((IEnumerable<IPAddress>) Dns.GetHostAddresses(new Uri(hostNameOrAddress).Host)).Any<IPAddress>((Func<IPAddress, bool>) (hostIP => IPAddress.IsLoopback(hostIP) || ((IEnumerable<IPAddress>) localIPs).Contains<IPAddress>(hostIP)));
      }
      catch
      {
        return false;
      }
    }

    internal static IPAddress[] VerifyUrlIsAllowedIPAddress(
      string url,
      out string failMessage,
      bool throwIfInvalidHost = false)
    {
      failMessage = string.Empty;
      IPAddress[] hostAddresses = IPAddressRange.GetHostAddresses(url, throwIfInvalidHost);
      if (IPAddressRange.IsLoopbackIPAddress(hostAddresses))
        failMessage = failMessage + url + " is loopback address;";
      string matchingHostIPValue;
      if (IPAddressRange.IsLocalhostIPAddress(hostAddresses, out matchingHostIPValue))
        failMessage = failMessage + matchingHostIPValue + " matches localhost address;";
      string matchedHostIPValue;
      if (!IPAddressRange.IsSpecialPurposeIPAddress(hostAddresses, out matchedHostIPValue, out string _))
        return hostAddresses;
      failMessage = failMessage + matchedHostIPValue + " matches special purpose IP range;";
      return hostAddresses;
    }

    internal static void ApplyIPAddressAllowedRange(
      IVssRequestContext requestContext,
      ImportRepositoryValidation remoteRepository,
      ClientTraceData ctData)
    {
      Uri result;
      if (Uri.TryCreate(remoteRepository.GitSource.Url, UriKind.Absolute, out result))
      {
        ctData.Add("KeyRemoteValidationDnsResolvedHost", (object) result.Host);
        string failMessage;
        IPAddress[] source = GitToGitImportValidator.VerifyUrlIsAllowedIPAddress(result.ToString(), out failMessage);
        ctData.Add("KeyRemoteValidationFailed", (object) failMessage);
        if (source == null || source.Length == 0)
          return;
        IPAddress ipAddress = ((IEnumerable<IPAddress>) source).FirstOrDefault<IPAddress>((Func<IPAddress, bool>) (ip => ip.AddressFamily == AddressFamily.InterNetwork)) ?? source[0];
        ctData.Add("KeyRemoteValidationDnsResolvedIpAddress", (object) ipAddress.ToString());
      }
      else
        ctData.Add("KeyRemoteValidationFailed", (object) ("Url is invalid - " + remoteRepository.GitSource.Url));
    }
  }
}
