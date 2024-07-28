// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.ItemComparer
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class ItemComparer : IComparer
  {
    public static readonly ItemComparer Instance = new ItemComparer();
    public static readonly ItemComparer.MinValueItem MinValue = new ItemComparer.MinValueItem();
    public static readonly ItemComparer.MaxValueItem MaxValue = new ItemComparer.MaxValueItem();

    public int Compare(object obj1, object obj2)
    {
      if (obj1 == obj2)
        return 0;
      if (obj1 is ItemComparer.MinValueItem)
        return -1;
      if (obj2 is ItemComparer.MinValueItem || obj1 is ItemComparer.MaxValueItem)
        return 1;
      if (obj2 is ItemComparer.MaxValueItem)
        return -1;
      ItemType itemType1 = ItemTypeHelper.GetItemType(obj1);
      ItemType itemType2 = ItemTypeHelper.GetItemType(obj2);
      int num = itemType1.CompareTo((object) itemType2);
      if (num != 0)
        return num;
      switch (itemType1)
      {
        case ItemType.NoValue:
        case ItemType.Null:
          return 0;
        case ItemType.Bool:
          return Comparer<bool>.Default.Compare((bool) obj1, (bool) obj2);
        case ItemType.Number:
          return Comparer<double>.Default.Compare(Convert.ToDouble(obj1, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToDouble(obj2, (IFormatProvider) CultureInfo.InvariantCulture));
        case ItemType.String:
          return string.CompareOrdinal((string) obj1, (string) obj2);
        case ItemType.Array:
        case ItemType.Object:
          return DistinctHash.GetHashToken((JToken) obj1).CompareTo(DistinctHash.GetHashToken((JToken) obj2));
        default:
          throw new InvalidCastException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected type: {0}", (object) itemType1));
      }
    }

    public static bool IsMinOrMax(object obj) => obj == ItemComparer.MinValue || obj == ItemComparer.MaxValue;

    public sealed class MinValueItem
    {
    }

    public sealed class MaxValueItem
    {
    }
  }
}
