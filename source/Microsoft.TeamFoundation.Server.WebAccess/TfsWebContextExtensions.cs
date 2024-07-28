// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsWebContextExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class TfsWebContextExtensions
  {
    public static T GetProjectSetting<T>(
      this TfsWebContext tfsWebContext,
      SettingsUserScope userScope,
      string key,
      T defaultValue = null,
      bool throwOnError = true)
    {
      string settingScopeValue = tfsWebContext.Project?.Id.ToString();
      return tfsWebContext.TfsRequestContext.GetService<ISettingsService>().GetValue<T>(tfsWebContext.TfsRequestContext, userScope, "Project", settingScopeValue, key, defaultValue, throwOnError);
    }

    public static void SetProjectSetting(
      this TfsWebContext tfsWebContext,
      SettingsUserScope userScope,
      string key,
      object value,
      bool throwOnError = true)
    {
      string settingScopeValue = tfsWebContext.Project?.Id.ToString();
      tfsWebContext.TfsRequestContext.GetService<ISettingsService>().SetValue(tfsWebContext.TfsRequestContext, userScope, "Project", settingScopeValue, key, value, throwOnError);
    }

    public static T GetTeamSetting<T>(
      this TfsWebContext tfsWebContext,
      SettingsUserScope userScope,
      string key,
      T defaultValue = null,
      bool throwOnError = true)
    {
      string settingScopeValue = tfsWebContext.Team?.Id.ToString();
      return tfsWebContext.TfsRequestContext.GetService<ISettingsService>().GetValue<T>(tfsWebContext.TfsRequestContext, userScope, "Team", settingScopeValue, key, defaultValue, throwOnError);
    }

    public static void SetTeamSetting(
      this TfsWebContext tfsWebContext,
      SettingsUserScope userScope,
      string key,
      object value,
      bool throwOnError = true)
    {
      string settingScopeValue = tfsWebContext.Team?.Id.ToString();
      tfsWebContext.TfsRequestContext.GetService<ISettingsService>().SetValue(tfsWebContext.TfsRequestContext, userScope, "Team", settingScopeValue, key, value, throwOnError);
    }

    public static T GetAccountSetting<T>(
      this TfsWebContext tfsWebContext,
      SettingsUserScope userScope,
      string key,
      T defaultValue = null,
      bool throwOnError = true)
    {
      return TfsWebContextExtensions.GetAccountSetting<T>(tfsWebContext.TfsRequestContext, userScope, key, defaultValue, throwOnError);
    }

    public static T GetAccountSetting<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      T defaultValue = null,
      bool throwOnError = true)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        return requestContext.GetService<ISettingsService>().GetValue<T>(requestContext, userScope, key, defaultValue, throwOnError);
      bool elevate;
      SettingsUserScope applicationUserScope = TfsWebContextExtensions.GetApplicationUserScope(requestContext, userScope, out elevate);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      if (elevate)
        vssRequestContext = vssRequestContext.Elevate();
      return vssRequestContext.GetService<ISettingsService>().GetValue<T>(vssRequestContext, applicationUserScope, key, defaultValue, throwOnError);
    }

    public static void SetAccountSetting(
      this TfsWebContext tfsWebContext,
      SettingsUserScope userScope,
      string key,
      object value,
      bool throwOnError = true)
    {
      TfsWebContextExtensions.SetAccountSetting(tfsWebContext.TfsRequestContext, userScope, key, value, throwOnError);
    }

    public static void SetAccountSetting(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      object value,
      bool throwOnError = true)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        requestContext.GetService<ISettingsService>().SetValue(requestContext, userScope, key, value, throwOnError);
      }
      else
      {
        bool elevate;
        SettingsUserScope applicationUserScope = TfsWebContextExtensions.GetApplicationUserScope(requestContext, userScope, out elevate);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        if (elevate)
          vssRequestContext = vssRequestContext.Elevate();
        vssRequestContext.GetService<ISettingsService>().SetValue(vssRequestContext, applicationUserScope, key, value, throwOnError);
      }
    }

    private static SettingsUserScope GetApplicationUserScope(
      IVssRequestContext requestContext,
      SettingsUserScope collectionUserScope,
      out bool elevate)
    {
      elevate = false;
      SettingsUserScope applicationUserScope = collectionUserScope;
      if (collectionUserScope.IsUserScoped)
      {
        Guid userId = collectionUserScope.UserId;
        if (userId == Guid.Empty)
        {
          userId = requestContext.GetUserId();
          elevate = true;
        }
        Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          userId
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        if (readIdentity != null)
          userId = readIdentity.MasterId;
        applicationUserScope = SettingsUserScope.SpecificUser(userId);
      }
      return applicationUserScope;
    }

    public static bool IsInTeamContext(this TfsWebContext tfsWebContext)
    {
      ArgumentUtility.CheckForNull<TfsWebContext>(tfsWebContext, nameof (tfsWebContext));
      if (tfsWebContext.NavigationContext.TopMostLevel == NavigationContextLevels.Team)
        return true;
      return tfsWebContext.NavigationContext.TopMostLevel == NavigationContextLevels.Project && tfsWebContext.InDefaultTeamContext;
    }
  }
}
