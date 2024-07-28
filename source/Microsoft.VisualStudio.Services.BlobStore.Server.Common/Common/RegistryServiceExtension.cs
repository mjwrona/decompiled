// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.RegistryServiceExtension
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public static class RegistryServiceExtension
  {
    public static DateTimeOffset? GetISO8601UTC(
      this IVssRegistryService registry,
      IVssRequestContext requestContext,
      string key)
    {
      return DedupTimeRangeUtil.FromISO8601UTC(registry.GetValue(requestContext, (RegistryQuery) key, false, (string) null));
    }

    public static bool GetBoolean(
      this IVssRegistryService registry,
      IVssRequestContext requestContext,
      string key)
    {
      bool result;
      return bool.TryParse(registry.GetValue(requestContext, (RegistryQuery) key, false, (string) null), out result) && result;
    }

    public static int GetInt32(
      this IVssRegistryService registry,
      IVssRequestContext requestContext,
      string path,
      int defaultValue)
    {
      int result;
      return !int.TryParse(registry.GetValue(requestContext, (RegistryQuery) path, false, (string) null), out result) ? defaultValue : result;
    }

    public static T GetEnum<T>(
      this IVssRegistryService registry,
      IVssRequestContext requestContext,
      string path,
      T defaultValue)
      where T : struct
    {
      return registry.GetEnum<T>(requestContext, path) ?? defaultValue;
    }

    public static T? GetEnum<T>(
      this IVssRegistryService registry,
      IVssRequestContext requestContext,
      string path)
      where T : struct
    {
      string str = registry.GetValue(requestContext, (RegistryQuery) path, false, (string) null);
      return string.IsNullOrEmpty(str) ? new T?() : new T?((T) Enum.Parse(typeof (T), str));
    }

    public static IEnumerable<PathEntry> GetSubPaths(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      string path,
      Func<string, bool> valueFilter = null)
    {
      int offset = 0;
      if (path.EndsWith("/**"))
        offset = path.Length - 2;
      else if (path.EndsWith("/") || path.EndsWith("\\"))
      {
        offset = path.Length;
        path += "**";
      }
      else
      {
        offset = path.Length + 1;
        path += "/**";
      }
      foreach (RegistryEntry readEntry in registryService.ReadEntries(requestContext, new RegistryQuery(path)))
      {
        Func<string, bool> func = valueFilter;
        if ((func != null ? new bool?(func(readEntry.Value)) : new bool?()).GetValueOrDefault(true))
          yield return new PathEntry(readEntry.Path, readEntry.Path.Substring(offset), readEntry.Value);
      }
    }

    public static IEnumerable<string> GetPrefixesFromParallelism(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      string path,
      int defaultValue = 1)
    {
      return PrefixParallelismConvertor.GetPrefixesFromParallelism(Math.Min(256, registryService.GetValue<int>(requestContext, (RegistryQuery) path, false, defaultValue)));
    }
  }
}
