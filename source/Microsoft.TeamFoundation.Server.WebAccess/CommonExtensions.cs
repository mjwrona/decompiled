// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.CommonExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class CommonExtensions
  {
    internal static void LogErrorInternal(
      this ITfsController tfsController,
      string message,
      int eventId)
    {
      TeamFoundationEventLog.Default.Log(tfsController.TfsRequestContext, message, eventId, EventLogEntryType.Error);
    }

    public static void Trace(
      this ITfsController tfsController,
      int tracepoint,
      TraceLevel traceLevel,
      string message,
      params object[] args)
    {
      CommonExtensions.Implementation.Instance.Trace(tfsController, tracepoint, traceLevel, message, args);
    }

    public static void TraceException(
      this ITfsController tfsController,
      int tracepoint,
      Exception e,
      TraceLevel traceLevel = TraceLevel.Error)
    {
      if (tfsController.TfsWebContext.RequestContext == null)
        return;
      string layer = tfsController.TfsWebContext.RequestContext.RouteData == null ? TfsTraceLayers.Controller : tfsController.TfsWebContext.RequestContext.RouteData.GetRouteValue<string>("controller");
      tfsController.TfsRequestContext.TraceException(tracepoint, traceLevel, tfsController.TraceArea, layer, e);
    }

    public static Uri ToUri(this string uriString) => uriString.ToUri(UriKind.RelativeOrAbsolute);

    public static Uri ToUri(this string uriString, UriKind uriKind)
    {
      Uri result = (Uri) null;
      if (!string.IsNullOrEmpty(uriString))
        Uri.TryCreate(uriString, uriKind, out result);
      return result;
    }

    public static string ToBrowserSafeString(this Uri uri)
    {
      if (uri == (Uri) null)
        return string.Empty;
      if (uri.IsUnc)
        return "file://///" + uri.Authority + uri.PathAndQuery;
      return uri.IsFile ? "file:///" + uri.Authority + uri.PathAndQuery : uri.ToString();
    }

    public static DateTime ConvertToLocal(this TimeZoneInfo timeZoneInfo, DateTime dateTime) => TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc, timeZoneInfo);

    public static DateTime ConvertToUtc(this TimeZoneInfo timeZoneInfo, DateTime dateTime) => TimeZoneInfo.ConvertTime(dateTime, timeZoneInfo, TimeZoneInfo.Utc);

    public static RouteValueDictionary Merge(
      this RouteValueDictionary dictionary,
      object routeValues)
    {
      return dictionary.Merge(routeValues, true);
    }

    public static RouteValueDictionary Merge(
      this RouteValueDictionary dictionary,
      object routeValues,
      bool replaceExisting)
    {
      switch (routeValues)
      {
        case null:
        case RouteValueDictionary _:
          return CommonExtensions.Merge(dictionary, (RouteValueDictionary) routeValues, replaceExisting);
        default:
          return CommonExtensions.Merge(dictionary, new RouteValueDictionary(routeValues), replaceExisting);
      }
    }

    public static RouteValueDictionary Merge(
      this RouteValueDictionary dictionary,
      RouteValueDictionary routeValues)
    {
      return CommonExtensions.Merge(dictionary, routeValues, true);
    }

    public static RouteValueDictionary Merge(
      this RouteValueDictionary dictionary,
      RouteValueDictionary routeValues,
      bool replaceExisting)
    {
      if (routeValues != null)
      {
        foreach (KeyValuePair<string, object> routeValue in routeValues)
        {
          if (replaceExisting || !dictionary.ContainsKey(routeValue.Key))
            dictionary[routeValue.Key] = routeValue.Value;
        }
      }
      return dictionary;
    }

    public class Implementation
    {
      private static CommonExtensions.Implementation s_instance;

      protected Implementation()
      {
      }

      public static CommonExtensions.Implementation Instance
      {
        get
        {
          if (CommonExtensions.Implementation.s_instance == null)
            CommonExtensions.Implementation.s_instance = new CommonExtensions.Implementation();
          return CommonExtensions.Implementation.s_instance;
        }
        internal set => CommonExtensions.Implementation.s_instance = value;
      }

      public virtual void Trace(
        ITfsController tfsController,
        int tracepoint,
        TraceLevel traceLevel,
        string message,
        params object[] args)
      {
        if (tfsController.TfsWebContext.RequestContext == null)
          return;
        string layer = tfsController.TfsWebContext.RequestContext.RouteData == null ? TfsTraceLayers.Controller : tfsController.TfsWebContext.RequestContext.RouteData.GetRouteValue<string>("controller");
        VssRequestContextExtensions.Trace(tfsController.TfsRequestContext, tracepoint, traceLevel, tfsController.TraceArea, layer, message, args);
      }
    }
  }
}
