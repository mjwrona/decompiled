// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.JObjectItem
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  internal class JObjectItem : Item
  {
    private JObject rawData;

    public JObjectItem(JObject data)
      : base((IItemData) new JObjectItemData(data))
    {
      this.rawData = data;
    }

    internal JObject RawData => this.rawData;
  }
}
