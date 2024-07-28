// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreMigrationRequest
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class LogStoreMigrationRequest
  {
    public LogStoreMigrationRequest()
    {
    }

    public LogStoreMigrationRequest(
      int migrationId,
      Guid hostId,
      LogStoreMigrationStatus status,
      string statusReason = null)
    {
      this.MigrationId = migrationId;
      this.HostId = hostId;
      this.Status = status;
      this.StatusReason = statusReason;
      this.IsSameRegion = false;
    }

    public int MigrationId { get; set; }

    public Guid HostId { get; set; }

    public LogStoreMigrationStatus Status { get; set; }

    public string StatusReason { get; set; }

    public bool IsSameRegion { get; set; }
  }
}
