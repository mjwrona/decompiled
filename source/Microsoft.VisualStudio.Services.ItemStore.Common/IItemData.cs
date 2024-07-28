// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.IItemData
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public interface IItemData
  {
    IEnumerable<string> Keys { get; }

    string this[string key] { get; set; }

    IEnumerable<string> GetStrings(string key);

    void SetStrings(string key, IEnumerable<string> values);

    void SetMaybeStrings(string key, IEnumerable<string> values);

    bool? GetBool(string key);

    Microsoft.VisualStudio.Services.ItemStore.Common.Item GetItem(string key, int index = -1);

    IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item> GetItems(string key);

    void SetItem(string key, Microsoft.VisualStudio.Services.ItemStore.Common.Item value);

    void SetItems(string key, IEnumerable<Microsoft.VisualStudio.Services.ItemStore.Common.Item> items);

    JObject ToJson();
  }
}
