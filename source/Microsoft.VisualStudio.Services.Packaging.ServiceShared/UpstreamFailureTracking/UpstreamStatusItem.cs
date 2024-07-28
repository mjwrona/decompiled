// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.UpstreamStatusItem
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking
{
  internal class UpstreamStatusItem : StoredItem
  {
    private const string TimestampDateFormat = "O";

    public UpstreamStatusItem()
    {
    }

    public UpstreamStatusItem(IItemData data)
      : base(data)
    {
    }

    public IEnumerable<UpstreamStatusCategory> Categories
    {
      get => this.GetItems(nameof (Categories)).Select<Item, UpstreamStatusCategory>((Func<Item, UpstreamStatusCategory>) (x => x.Convert<UpstreamStatusItem.CategoryItem>().Category));
      set => this.SetItems(nameof (Categories), (IEnumerable<Item>) value.Select<UpstreamStatusCategory, UpstreamStatusItem.CategoryItem>((Func<UpstreamStatusCategory, UpstreamStatusItem.CategoryItem>) (x => new UpstreamStatusItem.CategoryItem()
      {
        Category = x
      })));
    }

    public DateTime Timestamp
    {
      get => DateTime.ParseExact(this.Data[nameof (Timestamp)], "O", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
      set => this.Data[nameof (Timestamp)] = value.ToString("O", (IFormatProvider) CultureInfo.InvariantCulture);
    }

    private class CategoryItem : Item
    {
      public CategoryItem()
      {
      }

      public CategoryItem(IItemData data)
        : base(data)
      {
      }

      public UpstreamStatusCategory Category
      {
        get => (UpstreamStatusCategory) Enum.Parse(typeof (UpstreamStatusCategory), this.Data[nameof (Category)]);
        set => this.Data[nameof (Category)] = value.ToString("G");
      }
    }
  }
}
