// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerCleanupStatsBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerCleanupStatsBinder : ObjectBinder<FileContainerCleanupStats>
  {
    private SqlColumnBinder ContainerItemsDeletedColumn = new SqlColumnBinder("ContainerItemsDeleted");
    private SqlColumnBinder FileReferencesDeletedColumn = new SqlColumnBinder("FileReferencesDeleted");

    protected override FileContainerCleanupStats Bind()
    {
      FileContainerCleanupStats containerCleanupStats = new FileContainerCleanupStats();
      if (this.Reader.FieldCount >= 2)
      {
        containerCleanupStats.ContainerItemsDeleted = this.ContainerItemsDeletedColumn.GetInt32((IDataReader) this.Reader);
        containerCleanupStats.FileReferencesDeleted = this.FileReferencesDeletedColumn.GetInt32((IDataReader) this.Reader);
      }
      return containerCleanupStats;
    }
  }
}
