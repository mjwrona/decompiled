// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Teams.Service.WebTeamContextExtension
// Assembly: Microsoft.Azure.Devops.Teams.Service, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 643BB522-27FA-4CE7-8A1E-31D95F8AC409
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Teams.Service.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Settings;
using System;

namespace Microsoft.Azure.Devops.Teams.Service
{
  public static class WebTeamContextExtension
  {
    private const string c_mruTeamIdSettingKey = "mruTeamId";
    private const string c_requestTeamItemsKey = "requestTeam";
    private const string c_requestContextKey = "WebTeamContext";

    public static IWebTeamContext EnsureWebTeamContext(
      this IVssRequestContext requestContext,
      Guid? fallbackTeamId = null)
    {
      WebTeamContextResolveMode mode = WebTeamContextResolveMode.Default;
      IWebTeamContext webTeamContext;
      if (!WebTeamContextExtension.TryGetCachedWebTeamContext(requestContext, mode, out webTeamContext))
      {
        webTeamContext = (IWebTeamContext) new WebTeamContext(requestContext, fallbackTeamId);
        WebTeamContextExtension.CacheWebTeamContext(requestContext, webTeamContext, mode);
      }
      return webTeamContext;
    }

    public static IWebTeamContext GetWebTeamContext(this IVssRequestContext requestContext)
    {
      WebTeamContextResolveMode mode = WebTeamContextResolveMode.Default;
      IWebTeamContext webTeamContext;
      if (!WebTeamContextExtension.TryGetCachedWebTeamContext(requestContext, mode, out webTeamContext))
      {
        webTeamContext = (IWebTeamContext) new WebTeamContext(requestContext);
        WebTeamContextExtension.CacheWebTeamContext(requestContext, webTeamContext, mode);
        WebTeamContextExtension.UpdateMostRecentTeam(requestContext, webTeamContext);
      }
      return webTeamContext;
    }

    public static bool TryGetWebTeamContext(
      this IVssRequestContext requestContext,
      out IWebTeamContext context)
    {
      return WebTeamContextExtension.TryGetWebTeamContextInternal(requestContext, WebTeamContextResolveMode.Default, out context);
    }

    public static bool TryGetWebTeamContextWithoutGlobalContext(
      this IVssRequestContext requestContext,
      out IWebTeamContext context)
    {
      return WebTeamContextExtension.TryGetWebTeamContextInternal(requestContext, WebTeamContextResolveMode.IgnoreGlobalTeamContext, out context);
    }

    public static WebApiTeam GetRequestTeamOrDefault(this IVssRequestContext requestContext) => requestContext.GetRequestTeamOrDefault(out bool _);

    public static WebApiTeam GetRequestTeamOrDefault(
      this IVssRequestContext requestContext,
      out bool explicitlyProvided)
    {
      ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
      if (project == null)
      {
        explicitlyProvided = false;
        return (WebApiTeam) null;
      }
      IWebTeamContext context;
      if (requestContext.TryGetWebTeamContextWithoutGlobalContext(out context))
      {
        explicitlyProvided = true;
        return context.Team;
      }
      explicitlyProvided = false;
      WebApiTeam requestTeamOrDefault = (WebApiTeam) null;
      if (requestContext.Items.TryGetValue<WebApiTeam>("requestTeam", out requestTeamOrDefault))
        return requestTeamOrDefault;
      Guid guid = requestContext.GetService<ISettingsService>().GetValue<Guid>(requestContext, SettingsUserScope.User, "project", project.Id.ToString(), "mruTeamId", Guid.Empty, false);
      if (guid != Guid.Empty)
        requestTeamOrDefault = requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, project.Id, guid.ToString());
      if (requestTeamOrDefault == null)
        requestTeamOrDefault = requestContext.GetService<ITeamService>().GetDefaultTeam(requestContext, project.Id);
      requestContext.Items["requestTeam"] = (object) requestTeamOrDefault;
      return requestTeamOrDefault;
    }

    private static void UpdateMostRecentTeam(
      IVssRequestContext requestContext,
      IWebTeamContext webTeamContext)
    {
      if (webTeamContext.Team == null)
        return;
      requestContext.GetService<ISettingsService>().SetValue(requestContext, SettingsUserScope.User, "project", webTeamContext.Project.Id.ToString(), "mruTeamId", (object) webTeamContext.Team.Id, false);
    }

    private static bool TryGetWebTeamContextInternal(
      IVssRequestContext requestContext,
      WebTeamContextResolveMode mode,
      out IWebTeamContext context)
    {
      try
      {
        string cacheKey = WebTeamContextExtension.GetCacheKey(requestContext, mode);
        if (requestContext.Items.TryGetValue<IWebTeamContext>(cacheKey, out context))
          return true;
        context = (IWebTeamContext) new WebTeamContext(requestContext, mode | WebTeamContextResolveMode.AllowEmptyTeam);
        if (context.Team == null || context.Project == null)
          return false;
        requestContext.Items[cacheKey] = (object) context;
        WebTeamContextExtension.UpdateMostRecentTeam(requestContext, context);
        return true;
      }
      catch (Exception ex) when (ex is TeamNotFoundException || ex is TeamNotFoundInUrlException)
      {
        context = (IWebTeamContext) null;
      }
      return false;
    }

    private static bool TryGetCachedWebTeamContext(
      IVssRequestContext requestContext,
      WebTeamContextResolveMode mode,
      out IWebTeamContext webTeamContext)
    {
      string cacheKey = WebTeamContextExtension.GetCacheKey(requestContext, mode);
      return requestContext.Items.TryGetValue<IWebTeamContext>(cacheKey, out webTeamContext);
    }

    private static void CacheWebTeamContext(
      IVssRequestContext requestContext,
      IWebTeamContext instance,
      WebTeamContextResolveMode mode)
    {
      if (instance == null || instance.Team == null)
        return;
      string cacheKey = WebTeamContextExtension.GetCacheKey(requestContext, mode);
      requestContext.Items[cacheKey] = (object) instance;
    }

    private static string GetCacheKey(
      IVssRequestContext requestContext,
      WebTeamContextResolveMode mode)
    {
      return mode != WebTeamContextResolveMode.Default ? "WebTeamContext" + mode.ToString() : "WebTeamContext";
    }
  }
}
