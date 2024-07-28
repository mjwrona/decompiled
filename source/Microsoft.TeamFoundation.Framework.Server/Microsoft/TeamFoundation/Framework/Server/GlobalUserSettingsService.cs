// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GlobalUserSettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class GlobalUserSettingsService : IGlobalUserSettingsService, IVssFrameworkService
  {
    private const string c_settingsRequestKey = "centralized-user-settings";
    private const string c_area = "GlobalUserSetting";
    private const string c_layer = "UserSettingsService";
    private static readonly JsonSerializerSettings s_serializerSettings = new VssJsonMediaTypeFormatter().SerializerSettings;

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public T GetValue<T>(IVssRequestContext requestContext, string key) => this.GetValue<T>(requestContext, key, default (T));

    public T GetValue<T>(IVssRequestContext requestContext, string key, T defaultValue)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (!this.IsUserRequest(requestContext, userIdentity))
        return defaultValue;
      string serializedValue = this.GetSerializedValue(requestContext, key);
      return !string.IsNullOrEmpty(serializedValue) ? this.DeserializeEntryValue<T>(requestContext, serializedValue, key, defaultValue) : defaultValue;
    }

    public IDictionary<string, object> GetValues(IVssRequestContext requestContext) => this.GetValues(requestContext, (string) null);

    public IDictionary<string, object> GetValues(IVssRequestContext requestContext, string key)
    {
      IDictionary<string, object> values = (IDictionary<string, object>) new Dictionary<string, object>();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (this.IsUserRequest(requestContext, userIdentity))
      {
        GlobalUserSettings userAttributes = this.GetUserAttributes(requestContext);
        if (userAttributes != null)
        {
          string str = string.Empty;
          if (!string.IsNullOrEmpty(key))
            str = key + "/";
          object obj1;
          GlobalUserSettings globalUserSettings1;
          if (requestContext.Items.TryGetValue("centralized-user-settings", out obj1) && obj1 is GlobalUserSettings)
          {
            globalUserSettings1 = (GlobalUserSettings) obj1;
          }
          else
          {
            GlobalUserSettings globalUserSettings2 = new GlobalUserSettings()
            {
              IsInError = false,
              UserAttributes = (IReadOnlyDictionary<string, UserAttribute>) new ConcurrentDictionary<string, UserAttribute>()
            };
            requestContext.Items["centralized-user-settings"] = (object) globalUserSettings2;
            globalUserSettings1 = globalUserSettings2;
          }
          foreach (KeyValuePair<string, UserAttribute> userAttribute in (IEnumerable<KeyValuePair<string, UserAttribute>>) globalUserSettings1.UserAttributes)
          {
            if (userAttribute.Key.StartsWith(str, StringComparison.OrdinalIgnoreCase))
            {
              object obj2 = this.DeserializeEntryValue<object>(requestContext, userAttribute.Value.Value, userAttribute.Key, (object) null);
              if (obj2 != null)
                values[userAttribute.Key.Substring(str.Length)] = obj2;
            }
          }
          foreach (KeyValuePair<string, UserAttribute> userAttribute in (IEnumerable<KeyValuePair<string, UserAttribute>>) userAttributes.UserAttributes)
          {
            if (!globalUserSettings1.UserAttributes.ContainsKey(userAttribute.Key) && userAttribute.Key.StartsWith(str, StringComparison.OrdinalIgnoreCase))
            {
              object obj3 = this.DeserializeEntryValue<object>(requestContext, userAttribute.Value.Value, userAttribute.Key, (object) null);
              if (obj3 != null)
                values[userAttribute.Key.Substring(str.Length)] = obj3;
            }
          }
        }
      }
      return values;
    }

    public void SetValue(IVssRequestContext requestContext, string key, object value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = vssRequestContext.GetUserIdentity();
      if (!this.IsUserRequest(requestContext, userIdentity))
        return;
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "GlobalSettingsService.SetValue"))
      {
        performanceTimer.AddProperty(nameof (key), (object) key);
        string b = JsonConvert.SerializeObject(value, Formatting.None, GlobalUserSettingsService.s_serializerSettings);
        if (string.Equals(this.GetSerializedValue(requestContext, key), b) || IdentityHelper.IsServiceIdentity(vssRequestContext, (IReadOnlyVssIdentity) userIdentity))
          return;
        vssRequestContext.GetService<IUserService>().SetAttributes(vssRequestContext, userIdentity.Id, (IList<SetUserAttributeParameters>) new List<SetUserAttributeParameters>()
        {
          new SetUserAttributeParameters()
          {
            Name = "UserSettings." + key,
            Value = b
          }
        });
        this.RemoveFromCache(vssRequestContext, userIdentity.Id);
        requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.UserSettingsChanged, userIdentity.Id.ToString("D"));
      }
    }

    public void RemoveValue(IVssRequestContext requestContext, string key, bool recursive)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "GlobalUserService.RemoveValue"))
      {
        performanceTimer.AddProperty(nameof (key), (object) key);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = vssRequestContext.GetUserIdentity();
        IUserService service = vssRequestContext.GetService<IUserService>();
        if (recursive)
        {
          foreach (string key1 in (IEnumerable<string>) this.GetValues(requestContext, key).Keys)
            service.DeleteAttribute(vssRequestContext, userIdentity.Id, "UserSettings." + key + "/" + key1);
        }
        service.DeleteAttribute(vssRequestContext, userIdentity.Id, "UserSettings." + key);
        this.RemoveFromCache(vssRequestContext, userIdentity.Id);
        requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.UserSettingsChanged, userIdentity.Id.ToString("D"));
      }
    }

    private GlobalUserSettings GetUserAttributes(IVssRequestContext requestContext)
    {
      object userAttributes1;
      if (requestContext.Items.TryGetValue("centralized-user-settings", out userAttributes1) && userAttributes1 is GlobalUserSettings)
        return (GlobalUserSettings) userAttributes1;
      using (PerformanceTimer.StartMeasure(requestContext, "UserSettingsService.GetUserAttributes"))
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        if (userIdentity == null)
        {
          requestContext.Trace(10013544, TraceLevel.Info, "GlobalUserSetting", "UserSettingsService", "User identity null trying to read user attributes");
          return (GlobalUserSettings) null;
        }
        IGlobalUserSettingsCacheService service = requestContext.GetService<IGlobalUserSettingsCacheService>();
        (bool flag, GlobalUserSettings userAttributes2) = service.TryGetValue(requestContext, userIdentity.Id);
        if (!flag)
        {
          requestContext.Trace(10013565, TraceLevel.Info, "GlobalUserSetting", "UserSettingsService", "GlobalUserSettingsService.GetUserAttributes: Cache miss looking up settings for key: {0}", (object) userIdentity.Id);
          try
          {
            IList<UserAttribute> userAttributeList = requestContext.GetService<IUserService>().QueryAttributes(requestContext, userIdentity.Id, WellKnownUserAttributeNames.UserSettingsContainerWildcardQuery);
            IDictionary<string, UserAttribute> dictionary = (IDictionary<string, UserAttribute>) new Dictionary<string, UserAttribute>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            foreach (UserAttribute userAttribute in (IEnumerable<UserAttribute>) userAttributeList)
            {
              string key = userAttribute.Name;
              if (key.StartsWith(WellKnownUserAttributeNames.UserSettingsContainerNamePrefix))
                key = key.Substring(WellKnownUserAttributeNames.UserSettingsContainerNamePrefix.Length);
              dictionary.Add(key, userAttribute);
            }
            userAttributes2 = new GlobalUserSettings()
            {
              IsInError = false,
              UserAttributes = (IReadOnlyDictionary<string, UserAttribute>) new Dictionary<string, UserAttribute>(dictionary)
            };
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10013540, "GlobalUserSetting", "UserSettingsService", ex);
            userAttributes2 = new GlobalUserSettings()
            {
              IsInError = true,
              UserAttributes = (IReadOnlyDictionary<string, UserAttribute>) new Dictionary<string, UserAttribute>()
            };
          }
          service.Set(requestContext, userIdentity.Id, userAttributes2);
        }
        if (userAttributes2 == null)
          requestContext.Trace(10013560, TraceLevel.Info, "GlobalUserSetting", "UserSettingsService", "GlobalUserSettingsService.GetUserAttributes: Setting null GlobalUserSettings in requestContext cache: {0}", (object) userIdentity.Id);
        requestContext.Items["centralized-user-settings"] = (object) userAttributes2;
        return userAttributes2;
      }
    }

    private string GetSerializedValue(IVssRequestContext requestContext, string key)
    {
      string serializedValue = (string) null;
      GlobalUserSettings userAttributes = this.GetUserAttributes(requestContext);
      if (userAttributes == null)
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        requestContext.Trace(10013541, TraceLevel.Info, "GlobalUserSetting", "UserSettingsService", "GlobalUserSettings null for user: {0}", (object) (userIdentity != null ? userIdentity.Id : Guid.Empty));
      }
      else if (userAttributes.UserAttributes == null)
      {
        requestContext.Trace(10013542, TraceLevel.Info, "GlobalUserSetting", "UserSettingsService", "GlobalUserSettings.UserAttributes not found for key: {0}", (object) key);
      }
      else
      {
        UserAttribute userAttribute;
        if (userAttributes.UserAttributes.TryGetValue(key, out userAttribute))
        {
          if (userAttribute == null)
            requestContext.Trace(10013543, TraceLevel.Info, "GlobalUserSetting", "UserSettingsService", "Attribute not found for key: {0}", (object) key);
          else
            serializedValue = userAttribute.Value;
        }
      }
      return serializedValue;
    }

    private T DeserializeEntryValue<T>(
      IVssRequestContext requestContext,
      string serializedValue,
      string key,
      T defaultValue)
    {
      T obj = defaultValue;
      if (!string.IsNullOrEmpty(serializedValue))
      {
        try
        {
          obj = JsonConvert.DeserializeObject<T>(serializedValue, GlobalUserSettingsService.s_serializerSettings);
        }
        catch (Exception ex)
        {
          requestContext.Trace(10025102, TraceLevel.Warning, "GlobalUserSetting", "UserSettingsService", "Failed to deserialize setting with key '{0}': {1}", (object) key, (object) ex);
        }
      }
      return obj;
    }

    private void RemoveFromCache(IVssRequestContext requestContext, Guid id)
    {
      requestContext.GetService<IGlobalUserSettingsCacheService>().Remove(requestContext, id);
      requestContext.Items.Remove("centralized-user-settings");
    }

    private bool IsUserRequest(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity) => identity != null && !(identity.Id == Guid.Empty) && !IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) identity) && !(identity.Id == AnonymousAccessConstants.AnonymousSubjectId) && !IdentityHelper.IsAcsServiceIdentity((IReadOnlyVssIdentity) identity) && !ServicePrincipals.IsServicePrincipal(requestContext, identity.Descriptor);

    internal static IDictionary<string, UserAttribute> DefaultUserAttributes { get; } = (IDictionary<string, UserAttribute>) new Dictionary<string, UserAttribute>();

    [Flags]
    private enum ExceptionThrowBehavior
    {
      Never = 0,
      ThrowIfScopeNotFound = 1,
      ThrowIfAccessDenied = 2,
      Always = ThrowIfAccessDenied | ThrowIfScopeNotFound, // 0x00000003
    }
  }
}
