// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.IItemDataExtensions
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public static class IItemDataExtensions
  {
    public static T TryGetWithNullCheck<T>(
      this IItemData itemData,
      string key,
      T defaultValue,
      Func<string, T> deserializeFunc)
    {
      string str = itemData[key];
      return str == null ? defaultValue : deserializeFunc(str);
    }
  }
}
