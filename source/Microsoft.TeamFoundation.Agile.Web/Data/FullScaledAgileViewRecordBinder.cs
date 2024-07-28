// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.FullScaledAgileViewRecordBinder
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class FullScaledAgileViewRecordBinder : ObjectBinder<FullScaledAgileViewRecord>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder OwnerIdColumn = new SqlColumnBinder("OwnerId");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder TypeColumn = new SqlColumnBinder("Type");
    private SqlColumnBinder CreatedDateColumn = new SqlColumnBinder("CreatedDate");
    private SqlColumnBinder CreatedByColumn = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder ModifiedDateColumn = new SqlColumnBinder("ModifiedDate");
    private SqlColumnBinder ModifiedByColumn = new SqlColumnBinder("ModifiedBy");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder RevisionColumn = new SqlColumnBinder("Revision");
    private SqlColumnBinder CriteriaColumn = new SqlColumnBinder("Criteria");

    protected override FullScaledAgileViewRecord Bind()
    {
      FullScaledAgileViewRecord scaledAgileViewRecord = new FullScaledAgileViewRecord();
      scaledAgileViewRecord.Id = this.IdColumn.GetGuid((IDataReader) this.Reader);
      scaledAgileViewRecord.OwnerId = this.OwnerIdColumn.GetGuid((IDataReader) this.Reader);
      scaledAgileViewRecord.Name = this.NameColumn.GetString((IDataReader) this.Reader, false);
      scaledAgileViewRecord.Type = this.TypeColumn.GetInt32((IDataReader) this.Reader);
      scaledAgileViewRecord.CreatedDate = this.CreatedDateColumn.GetDateTime((IDataReader) this.Reader);
      scaledAgileViewRecord.CreatedBy = this.CreatedByColumn.GetGuid((IDataReader) this.Reader);
      scaledAgileViewRecord.ModifiedDate = this.ModifiedDateColumn.GetDateTime((IDataReader) this.Reader);
      scaledAgileViewRecord.ModifiedBy = this.ModifiedByColumn.GetGuid((IDataReader) this.Reader);
      scaledAgileViewRecord.Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true);
      scaledAgileViewRecord.Revision = this.RevisionColumn.GetInt32((IDataReader) this.Reader);
      scaledAgileViewRecord.Criteria = this.CriteriaColumn.GetString((IDataReader) this.Reader, true);
      return scaledAgileViewRecord;
    }
  }
}
