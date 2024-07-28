// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TrendDataRebuildJobData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class TrendDataRebuildJobData
  {
    public DateTime EndDate { get; set; }

    public DateTime BeginDate { get; set; }

    public int Interval { get; set; }

    public string FieldReferenceName { get; set; }

    public int FieldId { get; set; }

    public string MarkerFieldReferenceName { get; set; }

    public int MarkerFieldId { get; set; }

    public TrendDataBaselineRecord[] Baseline { get; set; }
  }
}
