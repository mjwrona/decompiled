// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.WebRegistryUtils
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class WebRegistryUtils
  {
    public static string ToWebRegistryPath(string path) => WebAccessRegistryConstants.Prefix + path;

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
