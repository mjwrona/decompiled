// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.ItemDetails
// Assembly: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D4A0500-806F-44D4-BA97-D409A2311716
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache
{
  public struct ItemDetails : IComparable<ItemDetails>, IEquatable<ItemDetails>
  {
    public int AreaId { get; set; }

    public DateTime ActivityDate { get; set; }

    public ItemDetails(int areaId, DateTime activityDate)
    {
      this.AreaId = areaId;
      this.ActivityDate = activityDate;
    }

    public bool Equals(ItemDetails other) => this.AreaId == other.AreaId && this.ActivityDate == other.ActivityDate;

    public override bool Equals(object obj) => obj is ItemDetails other && this.Equals(other);

    public override int GetHashCode() => this.AreaId * 397 ^ this.ActivityDate.GetHashCode();

    public static bool operator ==(ItemDetails left, ItemDetails right) => left.Equals(right);

    public static bool operator !=(ItemDetails left, ItemDetails right) => !left.Equals(right);

    public static bool operator <(ItemDetails left, ItemDetails right) => left.CompareTo(right) < 0;

    public static bool operator >(ItemDetails left, ItemDetails right) => left.CompareTo(right) > 0;

    public static bool operator <=(ItemDetails left, ItemDetails right) => left.CompareTo(right) <= 0;

    public static bool operator >=(ItemDetails left, ItemDetails right) => left.CompareTo(right) >= 0;

    public override string ToString() => "AreaId: " + this.AreaId.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ", Activity Date: " + this.ActivityDate.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    public int CompareTo(ItemDetails other)
    {
      int num = this.ActivityDate.CompareTo(other.ActivityDate);
      if (num == 0)
        num = this.AreaId.CompareTo(other.AreaId);
      return num;
    }
  }
}
