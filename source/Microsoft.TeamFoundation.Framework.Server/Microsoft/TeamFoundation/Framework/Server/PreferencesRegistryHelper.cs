// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PreferencesRegistryHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class PreferencesRegistryHelper
  {
    private const string RegistryPrefix = "/UserPreferences/";
    private const string RegistryPattern = "/UserPreferences/*";

    internal static T LoadPreferences<T>(IVssRequestContext requestContext, bool userHive) where T : BasePreferences
    {
      SecuredRegistryManager service = requestContext.GetService<SecuredRegistryManager>();
      return PreferencesHelper.Load<T>((userHive ? (!service.UserHiveOperationsAllowed(requestContext) ? (IEnumerable<RegistryEntry>) new List<RegistryEntry>() : (IEnumerable<RegistryEntry>) service.QueryUserEntries(requestContext, "/UserPreferences/*", false)) : (IEnumerable<RegistryEntry>) service.QueryRegistryEntries(requestContext, "/UserPreferences/*", false)).ToDictionary<RegistryEntry, string, string>((Func<RegistryEntry, string>) (entry => entry.Path.Substring("/UserPreferences/".Length)), (Func<RegistryEntry, string>) (entry => entry.Value)));
    }

    internal static void SavePreferences(
      IVssRequestContext requestContext,
      Dictionary<string, string> delta,
      string[] entriesToDelete,
      bool userHive)
    {
      RegistryEntry[] array1 = delta.Select<KeyValuePair<string, string>, RegistryEntry>((Func<KeyValuePair<string, string>, RegistryEntry>) (x => new RegistryEntry("/UserPreferences/" + x.Key, x.Value))).ToArray<RegistryEntry>();
      string[] array2 = ((IEnumerable<string>) entriesToDelete).Select<string, string>((Func<string, string>) (x => "/UserPreferences/" + x)).ToArray<string>();
      SecuredRegistryManager service = requestContext.GetService<SecuredRegistryManager>();
      if (!userHive)
      {
        service.UpdateRegistryEntries(requestContext, array1);
        service.RemoveRegistryEntries(requestContext, array2);
      }
      else
      {
        service.UpdateUserEntries(requestContext, array1);
        service.RemoveUserEntries(requestContext, array2);
      }
    }
  }
}
