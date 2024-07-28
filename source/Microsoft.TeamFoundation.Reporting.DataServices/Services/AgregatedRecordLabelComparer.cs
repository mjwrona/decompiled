// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.AgregatedRecordLabelComparer
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public class AgregatedRecordLabelComparer : IComparer<AggregatedRecord>
  {
    private readonly TransformOptions m_options;

    public AgregatedRecordLabelComparer(TransformOptions options) => this.m_options = options;

    public int Compare(AggregatedRecord x, AggregatedRecord y)
    {
      int num = string.IsNullOrEmpty(this.m_options.Series) ? 0 : this.CompareNullableProperty(x.Series, y.Series);
      return num == 0 ? this.CompareNullableProperty(x.Group, y.Group) : num;
    }

    public int CompareNullableProperty(object x, object y)
    {
      if (x == null && y == null)
        return 0;
      if (x == null)
        return -1;
      if (y == null)
        return 1;
      switch (x)
      {
        case string _:
          return StringComparer.OrdinalIgnoreCase.Compare(x, y);
        case IComparable comparable:
          return comparable.CompareTo(y);
        default:
          return -1;
      }
    }
  }
}
