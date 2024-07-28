// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.History.WorkItemFieldHistoryRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.History
{
  public class WorkItemFieldHistoryRecord
  {
    public int FieldId { get; set; }

    public string Name { get; set; }

    public string ReferenceName { get; set; }

    public int FieldDataType { get; set; }

    public string ReportingReferenceName { get; set; }

    public string ReportingName { get; set; }

    public int ReportingFormula { get; set; }

    public long Timestamp { get; set; }

    public int ReportingType { get; set; }

    public bool IsDeleted { get; set; }
  }
}
