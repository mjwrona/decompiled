// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ExpandedChangeComparer
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ExpandedChangeComparer : IComparer<ExpandedChange>
  {
    private ExpandedChangeComparer.ComparisonOrder m_comparisonOrder;
    private ExpandedChangeComparer.ComparisonType m_comparisonType;

    public ExpandedChangeComparer()
      : this(ExpandedChangeComparer.ComparisonType.ServerItem, ExpandedChangeComparer.ComparisonOrder.Ascending)
    {
    }

    public ExpandedChangeComparer(
      ExpandedChangeComparer.ComparisonType comparisonType)
      : this(comparisonType, ExpandedChangeComparer.ComparisonOrder.Ascending)
    {
    }

    public ExpandedChangeComparer(
      ExpandedChangeComparer.ComparisonType comparisonType,
      ExpandedChangeComparer.ComparisonOrder comparisonOrder)
    {
      this.m_comparisonOrder = comparisonOrder;
      this.m_comparisonType = comparisonType;
    }

    public int Compare(ExpandedChange x, ExpandedChange y)
    {
      int num = 0;
      if (this.m_comparisonType == ExpandedChangeComparer.ComparisonType.ServerItem)
        num = VersionControlPath.CompareTopDown(x.serverItem, y.serverItem);
      if (this.m_comparisonOrder == ExpandedChangeComparer.ComparisonOrder.Descending)
        num *= -1;
      return num;
    }

    public enum ComparisonOrder
    {
      Ascending,
      Descending,
    }

    public enum ComparisonType
    {
      ServerItem,
    }
  }
}
