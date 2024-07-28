// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence.DictionaryExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence
{
  internal static class DictionaryExtensions
  {
    internal static TEnum GetValueOrDefault<TEnum>(
      this IDictionary<string, object> dictionary,
      string key,
      TEnum defaultValue)
      where TEnum : struct
    {
      TEnum result = defaultValue;
      string str;
      dictionary.TryGetValue<string>(key, out str);
      if (typeof (TEnum).IsEnum)
        Enum.TryParse<TEnum>(str, out result);
      return result;
    }
  }
}
