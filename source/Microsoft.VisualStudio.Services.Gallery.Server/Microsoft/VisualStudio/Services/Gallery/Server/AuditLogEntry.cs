// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AuditLogEntry
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class AuditLogEntry
  {
    public Guid ChangedByIdentity { get; set; }

    public string AuditAction { get; set; }

    public DateTime ActionDate { get; set; }

    public string ResourceId { get; set; }

    public string ResourceType { get; set; }

    public string Data { get; set; }
  }
}
