// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantAuditEntry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  public class ConstantAuditEntry
  {
    public Guid TeamFoundationId { get; set; }

    public int ConstId { get; set; }

    public string DomainPart { get; set; }

    public string NamePart { get; set; }

    public string DisplayPart { get; set; }

    public int ChangerId { get; set; }

    public DateTime AddedDate { get; set; }

    public DateTime RemovedDate { get; set; }

    public string SID { get; set; }
  }
}
