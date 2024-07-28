// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildInformationBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class BuildInformationBinder : BuildObjectBinder<BuildInformationRow>
  {
    private IVssRequestContext m_requestContext;
    private SqlColumnBinder groupId = new SqlColumnBinder("GroupId");
    private SqlColumnBinder buildUri = new SqlColumnBinder("BuildUri");
    private SqlColumnBinder nodeId = new SqlColumnBinder("NodeId");
    private SqlColumnBinder parentId = new SqlColumnBinder("ParentId");
    private SqlColumnBinder nodeType = new SqlColumnBinder("NodeType");
    private SqlColumnBinder lastModifiedDate = new SqlColumnBinder("LastModifiedDate");
    private SqlColumnBinder lastModifiedBy = new SqlColumnBinder("LastModifiedBy");
    private SqlColumnBinder fieldName = new SqlColumnBinder("FieldName");
    private SqlColumnBinder fieldValue = new SqlColumnBinder("FieldValue");

    public BuildInformationBinder(
      IVssRequestContext requestContext,
      BuildSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
    }

    protected override BuildInformationRow Bind() => new BuildInformationRow()
    {
      GroupId = this.groupId.GetInt32((IDataReader) this.Reader),
      BuildUri = this.buildUri.GetArtifactUriFromString(this.Reader, "Build", false),
      NodeId = this.nodeId.GetInt32((IDataReader) this.Reader),
      ParentId = this.parentId.GetInt32((IDataReader) this.Reader, 0),
      Type = this.nodeType.GetString((IDataReader) this.Reader, false),
      LastModifiedDate = this.lastModifiedDate.GetDateTime((IDataReader) this.Reader),
      LastModifiedBy = this.GetUniqueName(this.m_requestContext, this.lastModifiedBy.GetString((IDataReader) this.Reader, false)),
      FieldName = this.fieldName.GetString((IDataReader) this.Reader, true),
      FieldValue = this.fieldValue.GetString((IDataReader) this.Reader, true)
    };
  }
}
