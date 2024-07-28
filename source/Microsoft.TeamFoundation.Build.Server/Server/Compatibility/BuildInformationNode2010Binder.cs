// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildInformationNode2010Binder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal sealed class BuildInformationNode2010Binder : BuildObjectBinder<BuildInformationNode2010>
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

    public BuildInformationNode2010Binder(
      IVssRequestContext requestContext,
      BuildSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
    }

    protected override BuildInformationNode2010 Bind()
    {
      BuildInformationNode2010 informationNode2010 = new BuildInformationNode2010();
      informationNode2010.GroupId = this.groupId.GetInt32((IDataReader) this.Reader);
      informationNode2010.BuildUri = this.buildUri.GetArtifactUriFromString(this.Reader, "Build", false);
      informationNode2010.NodeId = this.nodeId.GetInt32((IDataReader) this.Reader);
      informationNode2010.ParentId = this.parentId.GetInt32((IDataReader) this.Reader, 0);
      informationNode2010.Type = this.nodeType.GetString((IDataReader) this.Reader, false);
      informationNode2010.LastModifiedDate = this.lastModifiedDate.GetDateTime((IDataReader) this.Reader);
      informationNode2010.LastModifiedBy = this.GetUniqueName(this.m_requestContext, this.lastModifiedBy.GetString((IDataReader) this.Reader, false));
      informationNode2010.Fields.AddRange((IEnumerable<InformationField2010>) this.fields.XmlToListOfInformationField2010(this.Reader));
      return informationNode2010;
    }
  }
}
