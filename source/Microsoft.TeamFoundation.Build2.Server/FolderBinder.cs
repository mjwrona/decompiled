// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.FolderBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class FolderBinder : ObjectBinder<Folder>
  {
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_path = new SqlColumnBinder("Path");
    private SqlColumnBinder m_description = new SqlColumnBinder("Description");
    private SqlColumnBinder m_createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_changedBy = new SqlColumnBinder("ChangedBy");
    private SqlColumnBinder m_changedOn = new SqlColumnBinder("ChangedOn");
    private BuildSqlComponentBase m_resourceComponent;

    public FolderBinder(BuildSqlComponentBase component) => this.m_resourceComponent = component;

    protected override Folder Bind() => new Folder()
    {
      Path = DBHelper.DBPathToServerPath(this.m_path.GetString((IDataReader) this.Reader, false)),
      Description = this.m_description.GetString((IDataReader) this.Reader, true),
      CreatedBy = this.m_createdBy.GetGuid((IDataReader) this.Reader),
      CreatedOn = this.m_createdOn.GetDateTime((IDataReader) this.Reader),
      LastChangedBy = this.m_changedBy.GetGuid((IDataReader) this.Reader, true),
      LastChangedDate = this.m_changedOn.GetNullableDateTime((IDataReader) this.Reader),
      ProjectId = this.m_resourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader))
    };
  }
}
