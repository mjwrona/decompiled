// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.TokenItem
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.ItemStore.Common;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing
{
  public class TokenItem : StoredItem
  {
    public const string SearchIndexTokenItemType = "TokenItem";
    private const string SearchIndexTokenKey = "Token";

    public TokenItem()
      : base(nameof (TokenItem))
    {
    }

    public TokenItem(IItemData data)
      : base(data, nameof (TokenItem))
    {
    }

    public string Token
    {
      get => this.Data[nameof (Token)];
      set => this.Data[nameof (Token)] = value;
    }
  }
}
