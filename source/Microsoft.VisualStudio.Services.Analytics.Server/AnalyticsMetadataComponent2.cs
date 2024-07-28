// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsMetadataComponent2
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class AnalyticsMetadataComponent2 : AnalyticsMetadataComponent
  {
    public override Guid GetProjectProcessId(Guid projectId)
    {
      this.PrepareSqlBatch("SELECT ProcessId FROM AnalyticsModel.tbl_Project WHERE PartitionId = @partitionId AND ProjectSK = @projectId OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))".Length);
      this.AddStatement("SELECT ProcessId FROM AnalyticsModel.tbl_Project WHERE PartitionId = @partitionId AND ProjectSK = @projectId OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))");
      this.BindGuid("@projectId", projectId);
      Guid? nullable = (Guid?) this.ExecuteScalar();
      if (!nullable.HasValue)
        nullable = new Guid?(Guid.Empty);
      return nullable.Value;
    }
  }
}
