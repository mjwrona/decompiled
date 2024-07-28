// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Mru.MruService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.Mru
{
  internal class MruService : IMruService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ClearMruEntries(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string mruKey)
    {
      this.ClearMruEntries(requestContext, scope, (string) null, (string) null, mruKey);
    }

    public void ClearMruEntries(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string settingScope,
      string settingScopeValue,
      string mruKey)
    {
      requestContext.GetService<ISettingsService>().RemoveValue(requestContext, scope, settingScope, settingScopeValue, mruKey, false);
    }

    public IList<T> GetMruEntries<T>(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string mruKey)
    {
      return this.GetMruEntries<T>(requestContext, scope, (string) null, (string) null, mruKey);
    }

    public IList<T> GetMruEntries<T>(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string settingScope,
      string settingScopeValue,
      string mruKey)
    {
      return requestContext.GetService<ISettingsService>().GetValue<IList<T>>(requestContext, scope, mruKey, (IList<T>) null, false) ?? (IList<T>) new List<T>();
    }

    public void AddMruEntry<T>(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string mruKey,
      T entry,
      int maxLimit)
    {
      this.AddMruEntry<T>(requestContext, scope, (string) null, (string) null, mruKey, entry, maxLimit);
    }

    public void AddMruEntry<T>(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string settingScope,
      string settingScopeValue,
      string mruKey,
      T entry,
      int maxLimit)
    {
      this.AddMruEntry<T>(requestContext, scope, settingScope, settingScopeValue, mruKey, entry, maxLimit, (Func<T, T, bool>) ((a, b) => (object) a != null && a.Equals((object) b)));
    }

    public void AddMruEntry<T>(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string settingScope,
      string settingScopeValue,
      string mruKey,
      T entry,
      int maxLimit,
      Func<T, T, bool> equals)
    {
      ISettingsService service = requestContext.GetService<ISettingsService>();
      IList<T> list = (IList<T>) this.GetMruEntries<T>(requestContext, scope, mruKey).Where<T>((Func<T, bool>) (v => !equals(entry, v))).Take<T>(maxLimit - 1).ToList<T>();
      list.Insert(0, entry);
      IVssRequestContext requestContext1 = requestContext;
      SettingsUserScope userScope = scope;
      string settingScope1 = settingScope;
      string settingScopeValue1 = settingScopeValue;
      string key = mruKey;
      IList<T> objList = list;
      service.SetValue(requestContext1, userScope, settingScope1, settingScopeValue1, key, (object) objList, false);
    }

    public void RemoveMruEntry<T>(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string mruKey,
      T entry)
    {
      this.RemoveMruEntry<T>(requestContext, scope, (string) null, (string) null, mruKey, entry);
    }

    public void RemoveMruEntry<T>(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string settingScope,
      string settingScopeValue,
      string mruKey,
      T entry)
    {
      this.RemoveMruEntry<T>(requestContext, scope, settingScope, settingScopeValue, mruKey, entry, (Func<T, T, bool>) ((a, b) => (object) a != null && a.Equals((object) b)));
    }

    public void RemoveMruEntry<T>(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string settingScope,
      string settingScopeValue,
      string mruKey,
      T entry,
      Func<T, T, bool> equals)
    {
      ISettingsService service = requestContext.GetService<ISettingsService>();
      IList<T> list = (IList<T>) this.GetMruEntries<T>(requestContext, scope, mruKey).Where<T>((Func<T, bool>) (v => !equals(entry, v))).ToList<T>();
      IVssRequestContext requestContext1 = requestContext;
      SettingsUserScope userScope = scope;
      string key = mruKey;
      IList<T> objList = list;
      service.SetValue(requestContext1, userScope, key, (object) objList);
    }

    public void RemoveMruEntries<T>(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string mruKey,
      IList<T> entriesToRemove)
    {
      this.RemoveMruEntries<T>(requestContext, scope, (string) null, (string) null, mruKey, entriesToRemove);
    }

    public void RemoveMruEntries<T>(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string settingScope,
      string settingScopeValue,
      string mruKey,
      IList<T> entriesToRemove)
    {
      this.RemoveMruEntries<T>(requestContext, scope, settingScope, settingScopeValue, mruKey, entriesToRemove, (Func<T, T, bool>) ((a, b) => (object) a != null && a.Equals((object) b)));
    }

    public void RemoveMruEntries<T>(
      IVssRequestContext requestContext,
      SettingsUserScope scope,
      string settingScope,
      string settingScopeValue,
      string mruKey,
      IList<T> entriesToRemove,
      Func<T, T, bool> equals)
    {
      ISettingsService service = requestContext.GetService<ISettingsService>();
      IList<T> list = (IList<T>) this.GetMruEntries<T>(requestContext, scope, mruKey).Where<T>((Func<T, bool>) (v => !((IEnumerable<T>) entriesToRemove).Any<T>((Func<T, bool>) (toRemove => equals(v, toRemove))))).ToList<T>();
      IVssRequestContext requestContext1 = requestContext;
      SettingsUserScope userScope = scope;
      string key = mruKey;
      IList<T> objList = list;
      service.SetValue(requestContext1, userScope, key, (object) objList);
    }
  }
}
