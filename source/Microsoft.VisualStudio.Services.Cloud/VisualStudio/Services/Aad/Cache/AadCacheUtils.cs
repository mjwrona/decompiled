// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Cache.AadCacheUtils
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Aad.Cache
{
  internal class AadCacheUtils
  {
    internal static void ValidateTypes(IEnumerable<Type> types)
    {
      foreach (Type type in types)
      {
        if (!typeof (AadCacheObject).IsAssignableFrom(type))
          throw new ArgumentException("Type must be assignable to AadCacheObject: " + type?.ToString());
      }
    }

    internal static T GetRegistryValue<T>(
      IVssRequestContext context,
      Type type,
      T defaultValue,
      IDictionary<Type, T> typeSpecificDefaultValues,
      string defaultRegistryKey,
      IDictionary<Type, string> typeSpecificRegistryKeys)
    {
      T defaultValue1 = defaultValue;
      T defaultValue2 = !typeSpecificDefaultValues.ContainsKey(type) ? AadCacheUtils.GetRegistryValue<T>(context, defaultRegistryKey, defaultValue1) : typeSpecificDefaultValues[type];
      if (typeSpecificRegistryKeys.ContainsKey(type))
        defaultValue2 = AadCacheUtils.GetRegistryValue<T>(context, typeSpecificRegistryKeys[type], defaultValue2);
      return defaultValue2;
    }

    private static T GetRegistryValue<T>(IVssRequestContext context, string path, T defaultValue)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IVssRegistryService>().GetValue<T>(vssRequestContext, (RegistryQuery) path, defaultValue);
    }
  }
}
