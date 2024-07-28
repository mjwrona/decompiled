// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.HistoryConfiguration
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [DataContract]
  public class HistoryConfiguration
  {
    [DataMember]
    public TrendGranularity TrendGranularity { get; set; }

    [DataMember]
    public HistoryType HistoryType { get; set; }

    [DataMember]
    public int? RollingDays { get; set; }

    [DataMember]
    public DateRange DateRange { get; set; }

    [DataMember(IsRequired = false)]
    public bool? ExcludeOldCompletedWorkItems { get; set; }

    [DataMember(IsRequired = false)]
    public DateTimeOffset? OldCompletedItemsCutoffDate { get; set; }
  }
}
