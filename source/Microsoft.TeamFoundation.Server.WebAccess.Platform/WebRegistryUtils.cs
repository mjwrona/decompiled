// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebRegistryUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class WebRegistryUtils
  {
    public static string ToWebRegistryPath(string path) => WebAccessRegistryConstants.Prefix + path;

    public static string FromWebRegistryPath(string path) => path.Substring(WebAccessRegistryConstants.Prefix.Length);

    internal static T ReadSetting<T>(
      this IVssRegistryService tfsRegistry,
      IVssRequestContext requestContext,
      string path,
      T defaultValue)
    {
      return WebRegistryUtils.ConvertRegistryEntryValue<T>((IEnumerable<RegistryEntry>) tfsRegistry.ReadEntries(requestContext, (RegistryQuery) path), defaultValue);
    }

    internal static T ReadWebSetting<T>(
      this IVssRegistryService tfsRegistry,
      IVssRequestContext requestContext,
      string path,
      T defaultValue)
    {
      return tfsRegistry.ReadSetting<T>(requestContext, WebRegistryUtils.ToWebRegistryPath(path), defaultValue);
    }

    internal static T ReadUserSetting<T>(
      this IVssRegistryService tfsRegistry,
      IVssRequestContext requestContext,
      string path,
      T defaultValue)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return WebRegistryUtils.ConvertRegistryEntryValue<T>((IEnumerable<RegistryEntry>) tfsRegistry.ReadEntries(requestContext, userIdentity, path, false), defaultValue);
    }

    internal static T ReadUserWebSetting<T>(
      this IVssRegistryService tfsRegistry,
      IVssRequestContext requestContext,
      string path,
      T defaultValue)
    {
      return tfsRegistry.ReadUserSetting<T>(requestContext, WebRegistryUtils.ToWebRegistryPath(path), defaultValue);
    }

    internal static void WriteSetting<T>(
      this IVssRegistryService tfsRegistry,
      IVssRequestContext requestContext,
      string path,
      T value)
    {
      tfsRegistry.WriteEntries(requestContext, (IEnumerable<RegistryEntry>) new RegistryEntry[1]
      {
        new RegistryEntry(path, ConvertUtility.ToString<T>(value))
      });
    }

    internal static void WriteWebSetting<T>(
      this IVssRegistryService tfsRegistry,
      IVssRequestContext requestContext,
      string path,
      T value)
    {
      tfsRegistry.WriteSetting<T>(requestContext, WebRegistryUtils.ToWebRegistryPath(path), value);
    }

    internal static RegistryEntry ToWebRegistryEntry<T>(string path, T value) => new RegistryEntry(WebRegistryUtils.ToWebRegistryPath(path), ConvertUtility.ToString<T>(value));

    private static T ConvertRegistryEntryValue<T>(
      IEnumerable<RegistryEntry> entries,
      T defaultValue)
    {
      string str = (string) null;
      RegistryEntry registryEntry = entries.FirstOrDefault<RegistryEntry>();
      if (registryEntry != null)
        str = registryEntry.Value;
      return ConvertUtility.FromString<T>(str, defaultValue);
    }
  }
}
