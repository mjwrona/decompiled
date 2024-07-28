// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.FieldRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  internal class FieldRecord
  {
    public FieldRecord() => this.IsHistoryEnabled = true;

    public int Id { get; set; }

    public string ReferenceName { get; set; }

    public string Name { get; set; }

    public int FieldDataType { get; set; }

    public bool IsCore { get; set; }

    public InternalFieldUsages Usages { get; set; }

    public bool? OftenQueriedAsText { get; set; }

    public bool? SupportsTextQuery { get; set; }

    public bool IsReportable { get; set; }

    public int ReportingType { get; set; }

    public int ReportingFormula { get; set; }

    public string ReportingReferenceName { get; set; }

    public string ReportingName { get; set; }

    public bool IsIdentity { get; set; }

    public int ParentFieldId { get; set; }

    public Guid ProcessId { get; set; }

    public string Descriptor { get; set; }

    public Guid? PickListId { get; set; }

    public bool IsHistoryEnabled { get; set; }

    internal bool IsDeleted { get; set; }

    public bool IsLocked { get; set; }
  }
}
