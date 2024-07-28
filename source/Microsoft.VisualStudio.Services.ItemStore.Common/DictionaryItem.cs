// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.DictionaryItem
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  internal class DictionaryItem : Item
  {
    private readonly IDictionary<string, object> rawData;

    public DictionaryItem(IDictionary<string, object> data)
      : base((IItemData) new DictionaryItemData(data))
    {
      this.rawData = data;
    }

    public DictionaryItem(IReadOnlyDictionary<string, object> data)
      : base((IItemData) new DictionaryItemData((IDictionary<string, object>) data.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value))))
    {
      this.rawData = (IDictionary<string, object>) data.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value));
    }

    internal IDictionary<string, object> RawData => this.rawData;
  }
}
