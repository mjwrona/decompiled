// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackagingTracesExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public static class PackagingTracesExtensions
  {
    public static void AddPackagingTracesProperty(
      this IVssRequestContext requestContext,
      string key,
      object value)
    {
      if (!(requestContext.Items is ConcurrentDictionary<string, object> items))
        return;
      ((ConcurrentDictionary<string, object>) items.GetOrAdd("Packaging.Properties", (Func<string, object>) (_ => (object) new ConcurrentDictionary<string, object>())))[key] = value;
    }

    public static object GetPackagingTracesProperty(
      this IVssRequestContext requestContext,
      string key)
    {
      if (requestContext.Items is ConcurrentDictionary<string, object> items)
      {
        object packagingTracesProperty;
        if (((ConcurrentDictionary<string, object>) items.GetOrAdd("Packaging.Properties", (Func<string, object>) (_ => (object) new ConcurrentDictionary<string, object>()))).TryGetValue(key, out packagingTracesProperty))
          return packagingTracesProperty;
      }
      return (object) null;
    }

    public static void SetPackageNameForPackagingTraces(
      this IVssRequestContext requestContext,
      IPackageName packageName)
    {
      requestContext.Items["Packaging.PackageName"] = (object) packageName;
    }

    public static void SetPackageIdentityForPackagingTraces(
      this IVssRequestContext requestContext,
      IPackageIdentity packageIdentity)
    {
      requestContext.Items["Packaging.PackageIdentity"] = (object) packageIdentity;
    }

    public static void SetProtocolForPackagingTraces(
      this IVssRequestContext requestContext,
      IProtocol protocol)
    {
      requestContext.Items["Packaging.Protocol"] = (object) protocol;
    }

    public static void SetFeedForPackagingTraces(
      this IVssRequestContext requestContext,
      FeedCore feed)
    {
      requestContext.Items["Packaging.Feed"] = (object) feed;
    }

    public static void SetPackagingTracesInfoFromFeedRequest(
      this IVssRequestContext requestContext,
      IProtocolAgnosticFeedRequest protocolAgnosticFeedRequest)
    {
      requestContext.SetFeedForPackagingTraces(protocolAgnosticFeedRequest.Feed);
      if (protocolAgnosticFeedRequest is IFeedRequest feedRequest)
        requestContext.SetProtocolForPackagingTraces(feedRequest.Protocol);
      if (protocolAgnosticFeedRequest is IPackageNameRequest packageNameRequest)
        requestContext.SetPackageNameForPackagingTraces(packageNameRequest.PackageName);
      if (!(protocolAgnosticFeedRequest is IPackageRequest packageRequest))
        return;
      requestContext.SetPackageIdentityForPackagingTraces(packageRequest.PackageId);
    }

    public static int Increment(this IPackagingTraces packagingTraces, string key)
    {
      int num = ((int?) packagingTraces.GetProperty(key)).GetValueOrDefault() + 1;
      packagingTraces.AddProperty(key, (object) num);
      return num;
    }
  }
}
