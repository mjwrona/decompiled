// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Settings.ISettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Settings
{
  [DefaultServiceImplementation(typeof (SettingsService))]
  public interface ISettingsService : IVssFrameworkService
  {
    T GetValue<T>(IVssRequestContext requestContext, SettingsUserScope userScope, string key);

    T GetValue<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      T defaultValue);

    T GetValue<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      T defaultValue,
      bool throwOnError);

    T GetValue<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key);

    T GetValue<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      T defaultValue);

    T GetValue<T>(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      T defaultValue,
      bool throwOnError);

    IDictionary<string, object> GetValues(
      IVssRequestContext requestContext,
      SettingsUserScope userScope);

    IDictionary<string, object> GetValues(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key);

    IDictionary<string, object> GetValues(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      bool throwOnError);

    IDictionary<string, object> GetValues(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key);

    IDictionary<string, object> GetValues(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      bool throwOnError);

    void SetValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      object value);

    void SetValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      object value,
      bool throwOnError);

    void SetValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      object value);

    void SetValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      object value,
      bool throwOnError);

    void RemoveValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      bool recursive);

    void RemoveValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string key,
      bool recursive,
      bool throwOnError);

    void RemoveValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      bool recursive);

    void RemoveValue(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      string key,
      bool recursive,
      bool throwOnError);

    bool HasWritePermission(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue);

    bool HasWritePermission(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      bool throwOnError);

    bool HasReadPermission(
      IVssRequestContext requestContext,
      SettingsUserScope userScope,
      string settingScope,
      string settingScopeValue,
      bool throwOnError);
  }
}
