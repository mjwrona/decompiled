// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerCleanupSegmentedBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerCleanupSegmentedBinder : ObjectBinder<FileCleanupSegmentedStats>
  {
    private SqlColumnBinder DeletedItemColumn = new SqlColumnBinder("TotalDeleted");

    protected override FileCleanupSegmentedStats Bind()
    {
      FileCleanupSegmentedStats cleanupSegmentedStats = new FileCleanupSegmentedStats();
      if (this.Reader.FieldCount == 1)
        cleanupSegmentedStats.TotalDeleted = this.DeletedItemColumn.GetInt32((IDataReader) this.Reader);
      return cleanupSegmentedStats;
    }
  }
}
