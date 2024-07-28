// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FieldDefinitionRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class FieldDefinitionRecord
  {
    internal FieldDefinitionRecord() => this.IsHistoryEnabled = true;

    public int Id { get; set; }

    public string Name { get; set; }

    public string ReferenceName { get; set; }

    public int DBType { get; set; }

    public bool IsReportable { get; set; }

    public bool Editable { get; set; }

    public bool SemiEditable { get; set; }

    public string ReportingName { get; set; }

    public string ReportingReferenceName { get; set; }

    public bool SupportsTextQuery { get; set; }

    public bool IsIdentity { get; set; }

    public bool IsHistoryEnabled { get; set; }
  }
}
