// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.FullScaledAgileViewRecordBinder2
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class FullScaledAgileViewRecordBinder2 : ObjectBinder<FullScaledAgileViewRecord2>
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
    private SqlColumnBinder MarkersColumn = new SqlColumnBinder("Markers");

    protected override FullScaledAgileViewRecord2 Bind()
    {
      FullScaledAgileViewRecord2 agileViewRecord2 = new FullScaledAgileViewRecord2();
      agileViewRecord2.Id = this.IdColumn.GetGuid((IDataReader) this.Reader);
      agileViewRecord2.OwnerId = this.OwnerIdColumn.GetGuid((IDataReader) this.Reader);
      agileViewRecord2.Name = this.NameColumn.GetString((IDataReader) this.Reader, false);
      agileViewRecord2.Type = this.TypeColumn.GetInt32((IDataReader) this.Reader);
      agileViewRecord2.CreatedDate = this.CreatedDateColumn.GetDateTime((IDataReader) this.Reader);
      agileViewRecord2.CreatedBy = this.CreatedByColumn.GetGuid((IDataReader) this.Reader);
      agileViewRecord2.ModifiedDate = this.ModifiedDateColumn.GetDateTime((IDataReader) this.Reader);
      agileViewRecord2.ModifiedBy = this.ModifiedByColumn.GetGuid((IDataReader) this.Reader);
      agileViewRecord2.Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true);
      agileViewRecord2.Revision = this.RevisionColumn.GetInt32((IDataReader) this.Reader);
      agileViewRecord2.Criteria = this.CriteriaColumn.GetString((IDataReader) this.Reader, true);
      agileViewRecord2.Markers = this.MarkersColumn.GetString((IDataReader) this.Reader, true);
      return agileViewRecord2;
    }
  }
}
