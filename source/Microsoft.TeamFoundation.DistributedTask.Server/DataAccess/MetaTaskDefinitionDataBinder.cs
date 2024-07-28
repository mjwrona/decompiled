// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.MetaTaskDefinitionDataBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class MetaTaskDefinitionDataBinder : ObjectBinder<MetaTaskDefinitionData>
  {
    private SqlColumnBinder DefinitionIdColumn = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder RevisionColumn = new SqlColumnBinder("Revision");
    private SqlColumnBinder CreatedByColumn = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder CreatedOnColumn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder ModifiedByColumn = new SqlColumnBinder("ModifiedBy");
    private SqlColumnBinder ModifiedOnColumn = new SqlColumnBinder("ModifiedOn");
    private SqlColumnBinder MetaTaskDocumentColumn = new SqlColumnBinder("MetaTaskDocument");
    private SqlColumnBinder MajorVersionColumn = new SqlColumnBinder("MajorVersion");
    private SqlColumnBinder PreviewColumn = new SqlColumnBinder("Preview");
    private SqlColumnBinder DisabledColumn = new SqlColumnBinder("Disabled");
    private SqlColumnBinder ParentDefinitionIdColumn = new SqlColumnBinder("ParentDefinitionId");
    private SqlColumnBinder DeletedColumn = new SqlColumnBinder("Deleted");

    protected override MetaTaskDefinitionData Bind()
    {
      Guid guid = this.ParentDefinitionIdColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty);
      return new MetaTaskDefinitionData()
      {
        Id = this.DefinitionIdColumn.GetGuid((IDataReader) this.Reader),
        Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
        Revision = this.RevisionColumn.GetInt32((IDataReader) this.Reader, 1, 1),
        CreatedBy = this.CreatedByColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty),
        CreatedOn = this.CreatedOnColumn.GetDateTime((IDataReader) this.Reader, DateTime.Now),
        ModifiedBy = this.ModifiedByColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty),
        ModifiedOn = this.ModifiedOnColumn.GetDateTime((IDataReader) this.Reader, DateTime.Now),
        MetaTaskDocument = this.MetaTaskDocumentColumn.GetString((IDataReader) this.Reader, string.Empty),
        MajorVersion = this.MajorVersionColumn.GetInt32((IDataReader) this.Reader, 1, 1),
        Preview = this.PreviewColumn.GetBoolean((IDataReader) this.Reader, false, false),
        Disabled = this.DisabledColumn.GetBoolean((IDataReader) this.Reader, false, false),
        ParentDefinitionId = guid != Guid.Empty ? new Guid?(guid) : new Guid?(),
        Deleted = this.DeletedColumn.GetBoolean((IDataReader) this.Reader, false, false)
      };
    }
  }
}
