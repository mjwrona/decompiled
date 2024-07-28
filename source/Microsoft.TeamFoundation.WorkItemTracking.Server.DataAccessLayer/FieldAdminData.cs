// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FieldAdminData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class FieldAdminData
  {
    public int FldId { get; set; }

    public int Type { get; set; }

    public int ParentFldId { get; set; }

    public string Name { get; set; }

    public string ReferenceName { get; set; }

    public bool FEditable { get; set; }

    public bool FSemiEditable { get; set; }

    public int ReportingType { get; set; }

    public int ReportingFormula { get; set; }

    public string ReportingName { get; set; }

    public string ReportingReferenceName { get; set; }

    public bool FReportingEnabled { get; set; }

    public long? CacheStamp { get; set; }

    public bool FDeleted { get; set; }

    public bool FSupportsTextQuery { get; set; }
  }
}
