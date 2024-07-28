// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.EntityColumns
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class EntityColumns : ObjectBinder<SqlEntity>
  {
    protected SqlColumnBinder IdxColumn = new SqlColumnBinder("Idx");
    protected SqlColumnBinder PartitionKeyColumn = new SqlColumnBinder("PartitionKey");
    protected SqlColumnBinder RowKeyColumn = new SqlColumnBinder("RowKey");
    protected SqlColumnBinder ETagColumn = new SqlColumnBinder("ETag");

    protected override SqlEntity Bind()
    {
      SqlEntity sqlEntity = new SqlEntity();
      sqlEntity.Idx = this.IdxColumn.GetInt32((IDataReader) this.Reader);
      sqlEntity.PartitionKey = this.PartitionKeyColumn.GetString((IDataReader) this.Reader, false);
      sqlEntity.RowKey = this.RowKeyColumn.GetString((IDataReader) this.Reader, false);
      sqlEntity.ETag = this.ETagColumn.GetString((IDataReader) this.Reader, true);
      return sqlEntity;
    }
  }
}
