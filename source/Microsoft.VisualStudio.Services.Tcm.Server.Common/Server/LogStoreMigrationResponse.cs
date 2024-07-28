// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreMigrationResponse
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class LogStoreMigrationResponse
  {
    public int MigrationId { get; set; }

    public LogStoreMigrationStatus Status { get; set; }

    public long TotalTimeElapsed { get; set; }

    public LogStoreContainerCopyResponse ContainerMigratedDetails { get; set; }
  }
}
