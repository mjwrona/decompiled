// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.HistoryRange
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public abstract class HistoryRange
  {
    public static T FindNamedRange<T>(IEnumerable<T> ranges, string rangeName) where T : HistoryRange => ranges.First<T>((Func<T, bool>) (c => c.Name == rangeName));

    public NameLabelPair GetInfo() => new NameLabelPair()
    {
      Name = this.Name,
      LabelText = this.LabelText
    };

    public string Name { get; set; }

    public string LabelText { get; set; }
  }
}
