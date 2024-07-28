// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ShallowScaledAgileViewRecordBinder
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ShallowScaledAgileViewRecordBinder : ObjectBinder<ShallowScaledAgileViewRecord>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder OwnerIdColumn = new SqlColumnBinder("OwnerId");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder TypeColumn = new SqlColumnBinder("Type");
    private SqlColumnBinder CreatedDateColumn = new SqlColumnBinder("CreatedDate");

    protected override ShallowScaledAgileViewRecord Bind() => new ShallowScaledAgileViewRecord()
    {
      Id = this.IdColumn.GetGuid((IDataReader) this.Reader),
      OwnerId = this.OwnerIdColumn.GetGuid((IDataReader) this.Reader),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
      Type = this.TypeColumn.GetInt32((IDataReader) this.Reader),
      CreatedDate = this.CreatedDateColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
