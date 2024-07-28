// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.GalleryAuditLogComponent3
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class GalleryAuditLogComponent3 : GalleryAuditLogComponent2
  {
    public virtual void LogAuditEntriesBatch(List<AuditLogEntry> auditLogEntries)
    {
      foreach (AuditLogEntry auditLogEntry in auditLogEntries)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(auditLogEntry.ResourceType, "resourceName");
        ArgumentUtility.CheckStringForNullOrEmpty(auditLogEntry.ResourceId, "resourceId");
        ArgumentUtility.CheckStringForNullOrEmpty(auditLogEntry.AuditAction, "AuditAction");
      }
      this.PrepareStoredProcedure("Gallery.prc_AddGalleryAuditLogBatch");
      this.BindAuditLogEntriesTable("auditLogEntriesBatch", (IEnumerable<AuditLogEntry>) auditLogEntries);
      this.ExecuteNonQuery();
    }
  }
}
