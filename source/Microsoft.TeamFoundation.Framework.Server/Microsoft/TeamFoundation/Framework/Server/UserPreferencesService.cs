// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UserPreferencesService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class UserPreferencesService : IUserPreferencesService, IVssFrameworkService
  {
    internal static readonly string UserPreferencesFeature = "VisualStudio.ProfileService.UserPreferences";
    private static readonly string userPreferencesInProfileFeature = "VisualStudio.ProfileService.ProfileUserPreferences";
    private static readonly string profileMessageBusNotifications = "VisualStudio.ProfileService.MessageBusNotifications";
    private static readonly string Area = nameof (UserPreferencesService);
    private static readonly string Layer = "BusinessLogic";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public UserPreferences GetUserPreferences(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      requestContext.TraceEnter(21003010, UserPreferencesService.Area, UserPreferencesService.Layer, nameof (GetUserPreferences));
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      try
      {
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.IsFeatureEnabled(UserPreferencesService.UserPreferencesFeature) || identity == null || identity.Id == Guid.Empty || this.IsServiceIdentity(identity.Descriptor) || identity.Id == AnonymousAccessConstants.AnonymousSubjectId)
          return UserPreferencesService.DefaultUserPreferences;
        UserPreferences preferences1;
        if (this.TryGetFromCache(requestContext, identity.Id, out preferences1))
          return preferences1.Clone();
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled(UserPreferencesService.userPreferencesInProfileFeature) && !IdentityHelper.IsAcsServiceIdentity((IReadOnlyVssIdentity) identity) && !ServicePrincipals.IsServicePrincipal(requestContext, identity.Descriptor))
          return this.LoadUserPreferencesFromUserService(requestContext, identity.Id);
        UserPreferences preferences2 = PreferencesRegistryHelper.LoadPreferences<UserPreferences>(requestContext, true);
        this.UpdateCache(requestContext, identity.Id, preferences2);
        return preferences2;
      }
      finally
      {
        requestContext.TraceLeave(21003011, UserPreferencesService.Area, UserPreferencesService.Layer, nameof (GetUserPreferences));
      }
    }

    public UserPreferences GetUserPreferences(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(21003010, UserPreferencesService.Area, UserPreferencesService.Layer, nameof (GetUserPreferences));
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      try
      {
        if (!(requestContext.UserContext != (IdentityDescriptor) null))
          return UserPreferencesService.DefaultUserPreferences;
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        return this.GetUserPreferences(requestContext, userIdentity);
      }
      finally
      {
        requestContext.TraceLeave(21003011, UserPreferencesService.Area, UserPreferencesService.Layer, nameof (GetUserPreferences));
      }
    }

    public void SetUserPreferences(
      IVssRequestContext requestContext,
      UserPreferences userPreferences,
      bool merge)
    {
      try
      {
        requestContext.TraceEnter(21003007, UserPreferencesService.Area, UserPreferencesService.Layer, nameof (SetUserPreferences));
        requestContext = requestContext.To(TeamFoundationHostType.Deployment);
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.IsFeatureEnabled(UserPreferencesService.UserPreferencesFeature) || this.IsServiceIdentity(requestContext.UserContext))
          return;
        this.Validate(requestContext, userPreferences, true);
        if (requestContext.IsAnonymous())
        {
          requestContext.Trace(72723068, TraceLevel.Error, nameof (UserPreferencesService), nameof (UserPreferencesService), "Cannot call set user preferences when there is no user context");
        }
        else
        {
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          string[] entriesToDelete;
          Dictionary<string, string> delta = PreferencesHelper.ComputeDelta((BasePreferences) userPreferences.Clone(), (BasePreferences) this.GetUserPreferences(requestContext), merge, out entriesToDelete);
          if (delta.Count == 0 && entriesToDelete.Length == 0)
            return;
          List<SetUserAttributeParameters> list = delta.Select<KeyValuePair<string, string>, SetUserAttributeParameters>((Func<KeyValuePair<string, string>, SetUserAttributeParameters>) (x => new SetUserAttributeParameters()
          {
            Name = "TFS." + x.Key,
            Value = x.Value
          })).ToList<SetUserAttributeParameters>();
          if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled(UserPreferencesService.userPreferencesInProfileFeature))
            this.UpdatePreferencesInUserService(requestContext, userIdentity.Id, (IList<SetUserAttributeParameters>) list, entriesToDelete);
          else
            PreferencesRegistryHelper.SavePreferences(requestContext, delta, entriesToDelete, true);
          this.RemoveFromCache(requestContext, userIdentity.Id);
          if (requestContext.IsFeatureEnabled(UserPreferencesService.profileMessageBusNotifications))
            return;
          requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.UserPreferenceChanged, userIdentity.Id.ToString("D"));
        }
      }
      finally
      {
        requestContext.TraceLeave(21003008, UserPreferencesService.Area, UserPreferencesService.Layer, nameof (SetUserPreferences));
      }
    }

    private void Validate(
      IVssRequestContext requestContext,
      UserPreferences userPreferences,
      bool allowNullValues = false)
    {
      string argumentName = nameof (userPreferences);
      PreferencesHelper.Validate((BasePreferences) userPreferences, allowNullValues, argumentName);
      if (userPreferences.Language == null)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IInternalInstalledLanguageService>().CheckAnyLanguageInstalled(vssRequestContext, userPreferences.Language.LCID, argumentName);
    }

    private void UpdatePreferencesInUserService(
      IVssRequestContext requestContext,
      Guid identityId,
      IList<SetUserAttributeParameters> attributes,
      string[] entriesToDelete)
    {
      try
      {
        IUserService service = requestContext.GetService<IUserService>();
        service.SetAttributes(requestContext, identityId, attributes);
        foreach (string str in entriesToDelete)
        {
          try
          {
            service.DeleteAttribute(requestContext, identityId, "TFS." + str);
          }
          catch (UserAttributeDoesNotExistException ex)
          {
            requestContext.Trace(86413629, TraceLevel.Warning, UserPreferencesService.Area, UserPreferencesService.Layer, "Unexpected delete operation on an attribute that has already been delted. Attribute: {0}", (object) str);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(21003009, UserPreferencesService.Area, UserPreferencesService.Layer, ex);
      }
    }

    private UserPreferences LoadUserPreferencesFromUserService(
      IVssRequestContext requestContext,
      Guid identityId)
    {
      UserPreferences preferences = UserPreferencesService.DefaultUserPreferences;
      IUserService service = requestContext.GetService<IUserService>();
      try
      {
        IList<UserAttribute> source = service.QueryAttributes(requestContext, identityId, WellKnownUserAttributeNames.TFSContainerWildcardQuery);
        if (source.Count == 0)
          preferences = PreferencesRegistryHelper.LoadPreferences<UserPreferences>(requestContext, true);
        preferences = PreferencesHelper.Load<UserPreferences>(source.ToDictionary<UserAttribute, string, string>((Func<UserAttribute, string>) (attribute => ((IEnumerable<string>) attribute.Name.Split('.')).Last<string>()), (Func<UserAttribute, string>) (attribute => attribute.Value)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(21003006, UserPreferencesService.Area, UserPreferencesService.Layer, ex);
      }
      this.UpdateCache(requestContext, identityId, preferences);
      return preferences;
    }

    private void UpdateCache(
      IVssRequestContext requestContext,
      Guid id,
      UserPreferences preferences)
    {
      requestContext.GetService<IUserPreferencesCacheService>().Set(requestContext, id, preferences);
    }

    private void RemoveFromCache(IVssRequestContext requestContext, Guid id) => requestContext.GetService<IUserPreferencesCacheService>().Remove(requestContext, id);

    private bool TryGetFromCache(
      IVssRequestContext requestContext,
      Guid id,
      out UserPreferences preferences)
    {
      return requestContext.GetService<IUserPreferencesCacheService>().TryGetValue(requestContext, id, out preferences);
    }

    private bool IsServiceIdentity(IdentityDescriptor descriptor) => descriptor != (IdentityDescriptor) null && descriptor.IdentityType == "Microsoft.TeamFoundation.ServiceIdentity";

    internal static UserPreferences DefaultUserPreferences { get; } = new UserPreferences();
  }
}
