// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.AuditLogContextOverride
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public sealed class AuditLogContextOverride
  {
    public Guid TargetHostId { get; }

    public Guid ProjectId { get; }

    public string IPAddress { get; }

    public string UserAgent { get; }

    public AuditLogContextOverride(
      Guid targetHostId,
      Guid projectId,
      string ipAddress,
      string userAgent)
    {
      this.TargetHostId = targetHostId;
      this.ProjectId = projectId;
      this.IPAddress = ipAddress;
      this.UserAgent = userAgent;
    }

    public AuditLogContextOverride(Guid targetHostId, Guid projectId)
      : this(targetHostId, projectId, (string) null, (string) null)
    {
    }

    public AuditLogContextOverride(Guid targetHostId, string ipAddress, string userAgent)
      : this(targetHostId, Guid.Empty, ipAddress, userAgent)
    {
    }

    public AuditLogContextOverride(string ipAddress, string userAgent)
      : this(Guid.Empty, Guid.Empty, ipAddress, userAgent)
    {
    }
  }
}
