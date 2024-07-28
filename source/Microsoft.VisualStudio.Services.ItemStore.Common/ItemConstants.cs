// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.ItemConstants
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class ItemConstants
  {
    public const string ItemStoreBaseIdentity = "http://vs.ms.com/item";
    public const string Prefix = "cr:";
    public const string BaseIdentity = "http://vs.ms.com/schema#cr";
    public const string TypeBase = "http://vs.ms.com/schema#cr/itemtype/";
    public const string PreviousETagQuery = "previousETag";
    public const string PathOptionsQuery = "pathOptions";
    public const int MaxConcurrentReferences = 2000;
  }
}
