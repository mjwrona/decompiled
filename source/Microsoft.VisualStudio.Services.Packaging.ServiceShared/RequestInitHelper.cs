// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RequestInitHelper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.Controllers;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public static class RequestInitHelper
  {
    public static string? GetUserSuppliedProjectNameOrId(IDictionary<string, object>? routeValues)
    {
      if (routeValues == null)
        return (string) null;
      string str;
      return !routeValues.TryGetValue<string>("project", out str) || string.IsNullOrWhiteSpace(str) ? (string) null : str;
    }

    public static ProtocolAgnosticFeedRequest GetProtocolAgnosticFeedRequest(
      IVssRequestContext vssRequestContext,
      string? userSuppliedProjectNameOrId,
      Guid projectId,
      string feedNameOrId,
      IValidator<FeedCore>? validator = null)
    {
      FeedCore feed = vssRequestContext.GetService<IFeedCacheService>().GetFeed(vssRequestContext, projectId, feedNameOrId);
      ProtocolAgnosticFeedRequest protocolAgnosticFeedRequest = new ProtocolAgnosticFeedRequest(userSuppliedProjectNameOrId, feedNameOrId, feed);
      vssRequestContext.SetPackagingTracesInfoFromFeedRequest((IProtocolAgnosticFeedRequest) protocolAgnosticFeedRequest);
      validator?.Validate(feed);
      return protocolAgnosticFeedRequest;
    }

    public static IFeedRequest GetFeedRequest(
      IVssRequestContext vssRequestContext,
      string? userSuppliedProjectNameOrId,
      Guid projectId,
      IProtocol protocol,
      string feedNameOrId,
      IValidator<FeedCore>? validator = null)
    {
      ArgumentUtility.CheckForNull<string>(feedNameOrId, nameof (feedNameOrId));
      feedNameOrId = ProvenanceUtils.GetFeedId(vssRequestContext, feedNameOrId, protocol.CorrectlyCasedName);
      FeedRequest protocolAgnosticFeedRequest = new FeedRequest((IProtocolAgnosticFeedRequest) RequestInitHelper.GetProtocolAgnosticFeedRequest(vssRequestContext, userSuppliedProjectNameOrId, projectId, feedNameOrId, validator), protocol);
      vssRequestContext.SetPackagingTracesInfoFromFeedRequest((IProtocolAgnosticFeedRequest) protocolAgnosticFeedRequest);
      return (IFeedRequest) protocolAgnosticFeedRequest;
    }

    public static string? GetUserSuppliedProjectNameOrId(HttpRequestContext? requestContext) => RequestInitHelper.GetUserSuppliedProjectNameOrId(requestContext?.RouteData?.Values);

    public static string? GetUserSuppliedProjectNameOrId(HttpContextBase? httpContext) => RequestInitHelper.GetUserSuppliedProjectNameOrId((IDictionary<string, object>) httpContext?.Request.RequestContext.RouteData.Values);
  }
}
