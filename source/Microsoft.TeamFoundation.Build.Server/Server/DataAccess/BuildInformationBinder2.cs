// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildInformationBinder2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class BuildInformationBinder2 : BuildObjectBinder<BuildInformationNode>
  {
    private IVssRequestContext m_requestContext;
    private SqlColumnBinder groupId = new SqlColumnBinder("GroupId");
    private SqlColumnBinder buildUri = new SqlColumnBinder("BuildUri");
    private SqlColumnBinder nodeId = new SqlColumnBinder("NodeId");
    private SqlColumnBinder parentId = new SqlColumnBinder("ParentId");
    private SqlColumnBinder nodeType = new SqlColumnBinder("NodeType");
    private SqlColumnBinder lastModifiedDate = new SqlColumnBinder("LastModifiedDate");
    private SqlColumnBinder lastModifiedBy = new SqlColumnBinder("LastModifiedBy");
    private SqlColumnBinder fields = new SqlColumnBinder("Fields");

    public BuildInformationBinder2(
      IVssRequestContext requestContext,
      BuildSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
    }

    protected override BuildInformationNode Bind()
    {
      BuildInformationNode buildInformationNode = new BuildInformationNode();
      buildInformationNode.GroupId = this.groupId.GetInt32((IDataReader) this.Reader);
      buildInformationNode.BuildUri = this.buildUri.GetArtifactUriFromString(this.Reader, "Build", false);
      buildInformationNode.NodeId = this.nodeId.GetInt32((IDataReader) this.Reader);
      buildInformationNode.ParentId = this.parentId.GetInt32((IDataReader) this.Reader, 0);
      buildInformationNode.Type = this.nodeType.GetString((IDataReader) this.Reader, false);
      buildInformationNode.LastModifiedDate = this.lastModifiedDate.GetDateTime((IDataReader) this.Reader);
      buildInformationNode.LastModifiedBy = this.GetUniqueName(this.m_requestContext, this.lastModifiedBy.GetString((IDataReader) this.Reader, false));
      buildInformationNode.Fields.AddRange((IEnumerable<InformationField>) this.fields.XmlToListOfInformationField(this.Reader));
      return buildInformationNode;
    }
  }
}
