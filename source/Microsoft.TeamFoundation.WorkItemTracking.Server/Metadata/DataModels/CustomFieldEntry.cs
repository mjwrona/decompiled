// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.CustomFieldEntry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  public class CustomFieldEntry
  {
    public CustomFieldEntry() => this.Usage = -100;

    public string Name { get; set; }

    public string ReferenceName { get; set; }

    public int Type { get; set; }

    public int ReportingType { get; set; }

    public int ReportingFormula { get; set; }

    public bool ReportingEnabled { get; set; }

    public string ReportingName { get; set; }

    public string ReportingReferenceName { get; set; }

    public int Usage { get; set; }

    public int ParentFieldId { get; set; }

    public Guid ProcessId { get; set; }

    public string Description { get; set; }

    public Guid PicklistId { get; set; }

    public bool IsIdentityFromProcess { get; set; }

    public bool IsLocked { get; set; }
  }
}
