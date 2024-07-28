// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Settings.SettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Settings;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Settings
{
  internal class SettingsService : ISettingsService, IVssFrameworkService
  {
    private const string c_settingsRequestKeyFormat = "settings-{0}";
    private const string c_settingsUpdatesRequestKey = "settings-updates";
    private const string c_settingsUserScopesReadKey = "settings-user-scopes-read";
    private const string c_registrySettingsRoot = "/Configuration/Settings/";
    private const string c_registryUserSettingsRootFormat = "/Users/{0}/Settings/";
    private const string c_registryPathDefaultScope = "Host/";
    private const string c_area = "Settings";
    private const string c_layer = "SettingsService";
    private static readonly JsonSerializerSettings s_serializerSettings = new VssJsonMediaTypeFormatter().SerializerSettings;
    private IDictionary<string, ISettingsScopePlugin> m_registeredPlugins;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_registeredPlugins = (IDictionary<string, ISettingsScopePlugin>) systemRequestContext.GetExtensions<ISettingsScopePlugin>(ExtensionLifetime.Service, throwOnError: true).ToDictionary<ISettingsScopePlugin, string>((Func<ISettingsScopePlugin, string>) (plugin => plugin.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public T GetValue<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key)
    {
      return this.GetValue<T>(requestContext, userScope, (string) null, (string) null, key, default (T), true, true);
    }

    public T GetValue<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      T defaultValue)
    {
      return this.GetValue<T>(requestContext, userScope, (string) null, (string) null, key, defaultValue, false, false);
    }

    public T GetValue<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      T defaultValue,
      bool throwOnError)
    {
      return this.GetValue<T>(requestContext, userScope, (string) null, (string) null, key, defaultValue, false, throwOnError);
    }

    public T GetValue<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key)
    {
      return this.GetValue<T>(requestContext, userScope, settingScope, settingScopeValue, key, default (T), true, true);
    }

    public T GetValue<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      T defaultValue)
    {
      return this.GetValue<T>(requestContext, userScope, settingScope, settingScopeValue, key, defaultValue, false, false);
    }

    public T GetValue<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      T defaultValue,
      bool throwOnError)
    {
      return this.GetValue<T>(requestContext, userScope, settingScope, settingScopeValue, key, defaultValue, false, throwOnError);
    }

    private T GetValue<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      T defaultValue,
      bool throwIfScopeNotValid,
      bool throwOnError)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      if (this.ShouldCallGlobalUserService(requestContext, userScope))
      {
        if (!string.IsNullOrEmpty(settingScope))
        {
          this.HandleInvalidScope(requestContext, FrameworkResources.SettingsScopeValueInvalidForGlobalScope(), throwOnError);
          return defaultValue;
        }
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<IGlobalUserSettingsService>().GetValue<T>(vssRequestContext, key, defaultValue);
      }
      SettingsService.ExceptionThrowBehavior throwBehavior = SettingsService.ExceptionThrowBehavior.Never;
      if (throwOnError)
        throwBehavior = throwIfScopeNotValid ? SettingsService.ExceptionThrowBehavior.Always : SettingsService.ExceptionThrowBehavior.ThrowIfAccessDenied;
      string fullPath;
      string serializedValue = this.GetSerializedValue(requestContext, userScope, settingScope, settingScopeValue, key, true, throwBehavior, out fullPath);
      return this.DeserializeEntryValue<T>(requestContext, serializedValue, fullPath, defaultValue);
    }

    public IDictionary<string, object> GetValues(
      IVssRequestContext requestContext,
      SettingsUserScope userScope)
    {
      return this.GetValues(requestContext, userScope, (string) null, (string) null, (string) null, true);
    }

    public IDictionary<string, object> GetValues(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key)
    {
      return this.GetValues(requestContext, userScope, (string) null, (string) null, key, true);
    }

    public IDictionary<string, object> GetValues(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      bool throwOnError)
    {
      return this.GetValues(requestContext, userScope, (string) null, (string) null, key, throwOnError);
    }

    public IDictionary<string, object> GetValues(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key)
    {
      return this.GetValues(requestContext, userScope, settingScope, settingScopeValue, key, true);
    }

    public IDictionary<string, object> GetValues(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      bool throwOnError)
    {
      IDictionary<string, object> values = (IDictionary<string, object>) new Dictionary<string, object>();
      if (this.ShouldCallGlobalUserService(requestContext, userScope))
      {
        if (!string.IsNullOrEmpty(settingScope))
        {
          this.HandleInvalidScope(requestContext, FrameworkResources.SettingsScopeValueInvalidForGlobalScope(), throwOnError);
        }
        else
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          values = vssRequestContext.GetService<IGlobalUserSettingsService>().GetValues(vssRequestContext, key);
        }
      }
      else
      {
        string scopePath;
        RegistryEntryCollection registryEntries = this.GetRegistryEntries(requestContext, userScope, settingScope, settingScopeValue, true, throwOnError ? SettingsService.ExceptionThrowBehavior.Always : SettingsService.ExceptionThrowBehavior.Never, out scopePath);
        if (registryEntries != null)
        {
          string str = string.IsNullOrEmpty(key) ? scopePath : scopePath + key + "/";
          IDictionary<string, string> requestUpdates = this.GetRequestUpdates(requestContext);
          foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) requestUpdates)
          {
            if (keyValuePair.Key.StartsWith(str, StringComparison.OrdinalIgnoreCase))
            {
              object obj = this.DeserializeEntryValue<object>(requestContext, keyValuePair.Value, keyValuePair.Key, (object) null);
              if (obj != null)
                values[keyValuePair.Key.Substring(str.Length)] = obj;
            }
          }
          foreach (RegistryEntry registryEntry in registryEntries)
          {
            if (!requestUpdates.ContainsKey(registryEntry.Path) && registryEntry.Path.StartsWith(str, StringComparison.OrdinalIgnoreCase))
            {
              object obj = this.DeserializeEntryValue<object>(requestContext, registryEntry.Value, registryEntry.Path, (object) null);
              if (obj != null)
                values[registryEntry.Path.Substring(str.Length)] = obj;
            }
          }
        }
      }
      return values;
    }

    public void SetValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      object value)
    {
      this.SetValue(requestContext, userScope, (string) null, (string) null, key, value, true);
    }

    public void SetValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      object value,
      bool throwOnError)
    {
      this.SetValue(requestContext, userScope, (string) null, (string) null, key, value, throwOnError);
    }

    public void SetValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      object value)
    {
      this.SetValue(requestContext, userScope, settingScope, settingScopeValue, key, value, true);
    }

    public void SetValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      object value,
      bool throwOnError)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      if (this.ShouldCallGlobalUserService(requestContext, userScope))
      {
        if (!string.IsNullOrEmpty(settingScope))
        {
          this.HandleInvalidScope(requestContext, FrameworkResources.SettingsScopeValueInvalidForGlobalScope(), throwOnError);
        }
        else
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          vssRequestContext.GetService<IGlobalUserSettingsService>().SetValue(vssRequestContext, key, value);
        }
      }
      else
      {
        string registryPathForScope = this.GetRegistryPathForScope(requestContext, userScope, settingScope, settingScopeValue, SettingsService.PermissionCheck.Write, (IDictionary<string, RegistryEntryCollection>) null, throwOnError ? SettingsService.ExceptionThrowBehavior.Always : SettingsService.ExceptionThrowBehavior.Never, out ISettingsScopePlugin _, out string _);
        if (registryPathForScope == null)
          return;
        using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "SettingsService.SetValue"))
        {
          performanceTimer.AddProperty("scopePath", (object) registryPathForScope);
          performanceTimer.AddProperty(nameof (key), (object) key);
          string b = JsonConvert.SerializeObject(value, Formatting.None, SettingsService.s_serializerSettings);
          string serializedValue = this.GetSerializedValue(requestContext, userScope, settingScope, settingScopeValue, key, true, SettingsService.ExceptionThrowBehavior.Never, out string _);
          if (serializedValue != null && string.Equals(serializedValue, b))
            return;
          requestContext.GetService<IVssRegistryService>().SetValue<string>(requestContext, registryPathForScope + key, b);
          this.UpdateCachedValue(requestContext, userScope, settingScope, settingScopeValue, key, b, false);
        }
      }
    }

    public void RemoveValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      bool recursive)
    {
      this.RemoveValue(requestContext, userScope, (string) null, (string) null, key, recursive, true);
    }

    public void RemoveValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      bool recursive,
      bool throwOnError)
    {
      this.RemoveValue(requestContext, userScope, (string) null, (string) null, key, recursive, throwOnError);
    }

    public void RemoveValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      bool recursive)
    {
      this.RemoveValue(requestContext, userScope, settingScope, settingScopeValue, key, recursive, true);
    }

    public void RemoveValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      bool recursive,
      bool throwOnError)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      if (this.ShouldCallGlobalUserService(requestContext, userScope))
      {
        if (!string.IsNullOrEmpty(settingScope))
        {
          this.HandleInvalidScope(requestContext, FrameworkResources.SettingsScopeValueInvalidForGlobalScope(), throwOnError);
        }
        else
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          vssRequestContext.GetService<IGlobalUserSettingsService>().RemoveValue(vssRequestContext, key, recursive);
        }
      }
      else
      {
        string registryPathForScope = this.GetRegistryPathForScope(requestContext, userScope, settingScope, settingScopeValue, SettingsService.PermissionCheck.Write, (IDictionary<string, RegistryEntryCollection>) null, throwOnError ? SettingsService.ExceptionThrowBehavior.Always : SettingsService.ExceptionThrowBehavior.Never, out ISettingsScopePlugin _, out string _);
        if (registryPathForScope == null)
          return;
        using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "SettingsService.RemoveValue"))
        {
          performanceTimer.AddProperty("scopePath", (object) registryPathForScope);
          performanceTimer.AddProperty(nameof (key), (object) key);
          IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
          service.DeleteEntries(requestContext, registryPathForScope + key);
          if (recursive)
            service.DeleteEntries(requestContext, registryPathForScope + key + "/**");
          this.UpdateCachedValue(requestContext, userScope, settingScope, settingScopeValue, key, (string) null, recursive);
        }
      }
    }

    public bool HasWritePermission(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue)
    {
      return this.HasWritePermission(requestContext, userScope, settingScope, settingScopeValue, false);
    }

    public bool HasWritePermission(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      bool throwOnError)
    {
      try
      {
        return !string.IsNullOrEmpty(this.GetRegistryPathForScope(requestContext, userScope, settingScope, settingScopeValue, SettingsService.PermissionCheck.Write, (IDictionary<string, RegistryEntryCollection>) null, throwOnError ? SettingsService.ExceptionThrowBehavior.Always : SettingsService.ExceptionThrowBehavior.Never, out ISettingsScopePlugin _, out string _));
      }
      catch (Exception ex)
      {
        if (throwOnError)
        {
          throw;
        }
        else
        {
          requestContext.Trace(10025103, TraceLevel.Info, "Settings", nameof (SettingsService), "HasWritePermission check failed: {0}.", (object) ex);
          return false;
        }
      }
    }

    public bool HasReadPermission(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      bool throwOnError)
    {
      try
      {
        return !string.IsNullOrEmpty(this.GetRegistryPathForScope(requestContext, userScope, settingScope, settingScopeValue, SettingsService.PermissionCheck.Read, (IDictionary<string, RegistryEntryCollection>) null, throwOnError ? SettingsService.ExceptionThrowBehavior.Always : SettingsService.ExceptionThrowBehavior.Never, out ISettingsScopePlugin _, out string _));
      }
      catch (Exception ex)
      {
        requestContext.Trace(10025103, TraceLevel.Info, "Settings", nameof (SettingsService), "HasWritePermission check failed: {0}.", (object) ex);
        return false;
      }
    }

    private RegistryEntryCollection GetRegistryEntries(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      bool readIfNotCached,
      SettingsService.ExceptionThrowBehavior throwBehavior,
      out string scopePath)
    {
      IDictionary<string, RegistryEntryCollection> requestValuesLookup = this.GetRequestValuesLookup(requestContext, userScope);
      ISettingsScopePlugin scopePlugin;
      string resolvedScopeValue;
      scopePath = this.GetRegistryPathForScope(requestContext, userScope, settingScope, settingScopeValue, SettingsService.PermissionCheck.Read, requestValuesLookup, throwBehavior, out scopePlugin, out resolvedScopeValue);
      if (scopePath == null)
        return (RegistryEntryCollection) null;
      RegistryEntryCollection registryEntries1;
      if (requestValuesLookup.TryGetValue(scopePath, out registryEntries1))
        return registryEntries1;
      if (!readIfNotCached)
        return (RegistryEntryCollection) null;
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "SettingsService.GetValues", scopePath))
      {
        performanceTimer.AddProperty("userScoped", (object) userScope.IsUserScoped);
        if (userScope.IsUserScoped && userScope.UserId == Guid.Empty && !requestContext.Items.TryGetValue("settings-user-scopes-read", out object _))
        {
          List<string> source = new List<string>();
          foreach (string defaultScopePath in this.GetRequestDefaultScopePaths(requestContext, userScope, requestValuesLookup))
          {
            if (!string.Equals(defaultScopePath, scopePath, StringComparison.OrdinalIgnoreCase))
              source.Add(defaultScopePath);
          }
          source.Add(scopePath);
          performanceTimer.AddProperty("queryPaths", (object) source);
          IEnumerable<IEnumerable<RegistryItem>> registryItems = requestContext.GetService<ISqlRegistryService>().Read(requestContext, source.Select<string, RegistryQuery>((Func<string, RegistryQuery>) (p => new RegistryQuery(p + "**"))));
          int num = 0;
          RegistryEntryCollection registryEntries2 = (RegistryEntryCollection) null;
          foreach (IEnumerable<RegistryItem> items in registryItems)
          {
            string str = source[num++];
            RegistryEntryCollection registryEntryCollection = new RegistryEntryCollection(str.TrimEnd('/'), items);
            requestValuesLookup[str] = registryEntryCollection;
            if (string.Equals(str, scopePath, StringComparison.OrdinalIgnoreCase))
              registryEntries2 = registryEntryCollection;
          }
          requestContext.Items["settings-user-scopes-read"] = (object) true;
          return registryEntries2;
        }
        RegistryEntryCollection registryEntries3;
        if (scopePlugin != null && scopePlugin.IsScopeDirty(requestContext, resolvedScopeValue))
        {
          IEnumerable<RegistryItem> items = requestContext.GetService<ISqlRegistryService>().Read(requestContext, new RegistryQuery(scopePath + "**"));
          registryEntries3 = new RegistryEntryCollection(scopePath.TrimEnd('/'), items);
        }
        else
          registryEntries3 = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, new RegistryQuery(scopePath + "**"));
        requestValuesLookup[scopePath] = registryEntries3;
        return registryEntries3;
      }
    }

    private IEnumerable<string> GetRequestDefaultScopePaths(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      IDictionary<string, RegistryEntryCollection> entriesLookup)
    {
      List<string> defaultScopePaths = new List<string>();
      ISettingsScopePlugin scopePlugin;
      string resolvedScopeValue;
      string registryPathForScope1 = this.GetRegistryPathForScope(requestContext, userScope, (string) null, (string) null, SettingsService.PermissionCheck.Read, entriesLookup, SettingsService.ExceptionThrowBehavior.Never, out scopePlugin, out resolvedScopeValue);
      if (!string.IsNullOrEmpty(registryPathForScope1))
        defaultScopePaths.Add(registryPathForScope1);
      foreach (ISettingsScopePlugin settingsScopePlugin in (IEnumerable<ISettingsScopePlugin>) this.m_registeredPlugins.Values)
      {
        string requestScopeValue = settingsScopePlugin.GetRequestScopeValue(requestContext);
        if (!string.IsNullOrEmpty(requestScopeValue))
        {
          string registryPathForScope2 = this.GetRegistryPathForScope(requestContext, userScope, settingsScopePlugin.Name, requestScopeValue, SettingsService.PermissionCheck.Read, entriesLookup, SettingsService.ExceptionThrowBehavior.Never, out scopePlugin, out resolvedScopeValue);
          if (!string.IsNullOrEmpty(registryPathForScope2))
            defaultScopePaths.Add(registryPathForScope2);
        }
      }
      return (IEnumerable<string>) defaultScopePaths;
    }

    private IDictionary<string, RegistryEntryCollection> GetRequestValuesLookup(
      IVssRequestContext requestContext,
      SettingsUserScope userScope)
    {
      string key = string.Format("settings-{0}", (object) userScope);
      object requestValuesLookup1;
      if (requestContext.Items.TryGetValue(key, out requestValuesLookup1) && requestValuesLookup1 is IDictionary<string, RegistryEntryCollection>)
        return (IDictionary<string, RegistryEntryCollection>) requestValuesLookup1;
      IDictionary<string, RegistryEntryCollection> requestValuesLookup2 = (IDictionary<string, RegistryEntryCollection>) new Dictionary<string, RegistryEntryCollection>((IEqualityComparer<string>) StringComparer.Ordinal);
      requestContext.Items[key] = (object) requestValuesLookup2;
      return requestValuesLookup2;
    }

    private string GetSerializedValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      bool readIfNotCached,
      SettingsService.ExceptionThrowBehavior throwBehavior,
      out string fullPath)
    {
      string serializedValue = (string) null;
      string scopePath;
      RegistryEntryCollection registryEntries = this.GetRegistryEntries(requestContext, userScope, settingScope, settingScopeValue, readIfNotCached, throwBehavior, out scopePath);
      fullPath = scopePath + key;
      if (registryEntries != null && !this.GetRequestUpdates(requestContext).TryGetValue(fullPath, out serializedValue))
      {
        RegistryEntry registryEntry = registryEntries[key];
        if (registryEntry != null)
          serializedValue = registryEntry.Value;
      }
      return serializedValue;
    }

    private IDictionary<string, string> GetRequestUpdates(IVssRequestContext requestContext)
    {
      object requestUpdates1;
      if (requestContext.RootContext.Items.TryGetValue("settings-updates", out requestUpdates1) && requestUpdates1 is IDictionary<string, string>)
        return (IDictionary<string, string>) requestUpdates1;
      IDictionary<string, string> requestUpdates2 = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
      requestContext.Items["settings-updates"] = (object) requestUpdates2;
      return requestUpdates2;
    }

    private string GetRegistryPathForScope(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      SettingsService.PermissionCheck operation,
      IDictionary<string, RegistryEntryCollection> entriesLookup,
      SettingsService.ExceptionThrowBehavior throwBehavior,
      out ISettingsScopePlugin scopePlugin,
      out string resolvedScopeValue)
    {
      scopePlugin = (ISettingsScopePlugin) null;
      resolvedScopeValue = settingScopeValue;
      bool flag = true;
      bool throwOnError1 = throwBehavior.HasFlag((Enum) SettingsService.ExceptionThrowBehavior.ThrowIfScopeNotFound);
      string str1;
      if (userScope.IsUserScoped)
      {
        Guid userId = userScope.UserId;
        if (userId == Guid.Empty)
          userId = requestContext.GetUserId();
        if (userId == Guid.Empty)
          return this.HandleInvalidScope(requestContext, FrameworkResources.SettingsScopeInvalidUserContext(), throwOnError1);
        if (requestContext.IsAnonymousPrincipal() || requestContext.IsPublicUser())
          return this.HandleInvalidScope(requestContext, FrameworkResources.SettingsScopeAnonymousNotAllowed(), throwOnError1);
        flag = false;
        str1 = string.Format("/Users/{0}/Settings/", (object) userId);
      }
      else
        str1 = "/Configuration/Settings/";
      string str2;
      if (!string.IsNullOrEmpty(settingScope))
      {
        scopePlugin = this.GetScopePlugin(requestContext, settingScope);
        if (scopePlugin == null)
          return this.HandleInvalidScope(requestContext, FrameworkResources.SettingsScopePluginNotFound((object) settingScope), throwOnError1);
        if (string.IsNullOrEmpty(resolvedScopeValue))
        {
          resolvedScopeValue = scopePlugin.GetRequestScopeValue(requestContext);
          if (string.IsNullOrEmpty(resolvedScopeValue))
            return this.HandleInvalidScope(requestContext, FrameworkResources.SettingsScopeValueNotDetermined((object) settingScope), throwOnError1);
        }
        else if (!scopePlugin.IsValidScopeValue(requestContext, resolvedScopeValue))
          return this.HandleInvalidScope(requestContext, FrameworkResources.SettingsScopeValueInvalid((object) settingScope, (object) resolvedScopeValue), throwOnError1);
        str2 = settingScope + "/" + resolvedScopeValue + "/";
      }
      else
        str2 = "Host/";
      string key = str1 + str2;
      if (flag && (entriesLookup == null || !entriesLookup.ContainsKey(key)))
      {
        bool throwOnError2 = throwBehavior.HasFlag((Enum) SettingsService.ExceptionThrowBehavior.ThrowIfAccessDenied);
        if (!SettingsService.SecurityHelper.DetermineSettingsPermission(requestContext, scopePlugin, resolvedScopeValue, operation, throwOnError2))
        {
          requestContext.Trace(10025105, TraceLevel.Info, "Settings", nameof (SettingsService), "No $" + (operation == SettingsService.PermissionCheck.Read ? "read" : "write") + "  permission for user scope: $" + (userScope.IsUserScoped ? "user" : "all users") + ", scope: $" + settingScope + ", scope value: $" + resolvedScopeValue);
          return (string) null;
        }
      }
      return key;
    }

    private string HandleInvalidScope(
      IVssRequestContext requestContext,
      string exceptionMessage,
      bool throwOnError)
    {
      if (throwOnError)
        throw new InvalidSettingsScopeException(exceptionMessage);
      requestContext.Trace(10025106, TraceLevel.Info, "Settings", nameof (SettingsService), exceptionMessage);
      return (string) null;
    }

    private T DeserializeEntryValue<T>(
      IVssRequestContext requestContext,
      string serializedValue,
      string registryPath,
      T defaultValue)
    {
      if (!string.IsNullOrEmpty(serializedValue))
      {
        try
        {
          return JsonConvert.DeserializeObject<T>(serializedValue, SettingsService.s_serializerSettings);
        }
        catch (Exception ex)
        {
          requestContext.Trace(10025102, TraceLevel.Warning, "Settings", nameof (SettingsService), "Failed to deserialize setting with key '{0}': {1}", (object) registryPath, (object) ex);
        }
      }
      return defaultValue;
    }

    private void UpdateCachedValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      string value,
      bool recursive)
    {
      string scopePath;
      RegistryEntryCollection registryEntries = this.GetRegistryEntries(requestContext, userScope, settingScope, settingScopeValue, false, SettingsService.ExceptionThrowBehavior.Never, out scopePath);
      if (registryEntries == null)
        return;
      IDictionary<string, string> requestUpdates = this.GetRequestUpdates(requestContext);
      requestUpdates[scopePath + key] = value;
      if (!recursive)
        return;
      string str = (string) null;
      if (!string.IsNullOrEmpty(key))
        str = scopePath + key + "/";
      foreach (RegistryEntry registryEntry in registryEntries)
      {
        if (registryEntry.Path.StartsWith(str, StringComparison.OrdinalIgnoreCase))
          requestUpdates[registryEntry.Path] = (string) null;
      }
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) requestUpdates)
      {
        if (keyValuePair.Key.StartsWith(str, StringComparison.OrdinalIgnoreCase))
          stringList.Add(keyValuePair.Key);
      }
      foreach (string key1 in stringList)
        requestUpdates[key1] = (string) null;
    }

    private ISettingsScopePlugin GetScopePlugin(IVssRequestContext requestContext, string scopeName)
    {
      ISettingsScopePlugin scopePlugin;
      if (!this.m_registeredPlugins.TryGetValue(scopeName, out scopePlugin))
        requestContext.Trace(10025101, TraceLevel.Info, "Settings", nameof (SettingsService), "Could not find ISettingsScopePlugin with name \"{0}\".", (object) scopeName);
      return scopePlugin;
    }

    private bool ShouldCallGlobalUserService(
      IVssRequestContext requestContext,
      SettingsUserScope userScope)
    {
      return userScope.IsGlobalScoped && userScope.IsUserScoped && requestContext.IsFeatureEnabled("VisualStudio.UserService.CentralizedAttributes");
    }

    private enum PermissionCheck
    {
      Read,
      Write,
    }

    [Flags]
    private enum ExceptionThrowBehavior
    {
      Never = 0,
      ThrowIfScopeNotFound = 1,
      ThrowIfAccessDenied = 2,
      Always = ThrowIfAccessDenied | ThrowIfScopeNotFound, // 0x00000003
    }

    private static class SecurityHelper
    {
      public static bool DetermineSettingsPermission(
        IVssRequestContext requestContext,
        ISettingsScopePlugin plugin,
        string settingScopeValue,
        SettingsService.PermissionCheck permissionCheck,
        bool throwOnError)
      {
        bool settingsPermission = true;
        bool flag = permissionCheck == SettingsService.PermissionCheck.Read;
        if (plugin == null)
        {
          IVssSecurityNamespace securityNamespace = SettingsService.SecurityHelper.GetSecurityNamespace(requestContext);
          string token = SettingsService.SecurityHelper.GetToken(securityNamespace);
          int requestedPermissions = flag ? 1 : 2;
          if (throwOnError)
            securityNamespace.CheckPermission(requestContext, token, requestedPermissions);
          else
            settingsPermission = securityNamespace.HasPermission(requestContext, token, requestedPermissions);
        }
        else if (throwOnError)
        {
          if (flag)
            plugin.CheckReadPermission(requestContext, settingScopeValue);
          else
            plugin.CheckWritePermission(requestContext, settingScopeValue);
        }
        else
          settingsPermission = !flag ? plugin.HasWritePermission(requestContext, settingScopeValue) : plugin.HasReadPermission(requestContext, settingScopeValue);
        return settingsPermission;
      }

      private static IVssSecurityNamespace GetSecurityNamespace(IVssRequestContext requestContext)
      {
        ITeamFoundationSecurityService service = requestContext.GetService<ITeamFoundationSecurityService>();
        return service.GetSecurityNamespace(requestContext, SettingsSecurityConstants.NamespaceGuid) ?? service.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId);
      }

      private static string GetToken(IVssSecurityNamespace securityNamespace) => !securityNamespace.Description.NamespaceId.Equals(SettingsSecurityConstants.NamespaceGuid) ? FrameworkSecurity.FrameworkNamespaceToken : "Global";
    }
  }
}
