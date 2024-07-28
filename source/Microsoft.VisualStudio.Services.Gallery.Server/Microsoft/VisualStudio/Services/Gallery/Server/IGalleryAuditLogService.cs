// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IGalleryAuditLogService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (GalleryAuditLogService))]
  public interface IGalleryAuditLogService : IVssFrameworkService
  {
    void LogAuditEntry(
      IVssRequestContext requestContext,
      string auditAction,
      string resourceId,
      string resourceType,
      string data = null,
      bool batchRequests = false);
  }
}
