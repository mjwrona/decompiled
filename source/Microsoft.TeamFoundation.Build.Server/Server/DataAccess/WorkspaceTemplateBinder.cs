// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.WorkspaceTemplateBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class WorkspaceTemplateBinder : BuildObjectBinder<WorkspaceTemplate>
  {
    private IVssRequestContext m_requestContext;
    private SqlColumnBinder workspaceId = new SqlColumnBinder("WorkspaceId");
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder lastModifiedDate = new SqlColumnBinder("LastModifiedDate");
    private SqlColumnBinder lastModifiedBy = new SqlColumnBinder("LastModifiedBy");

    public WorkspaceTemplateBinder(
      IVssRequestContext requestContext,
      BuildSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
    }

    protected override WorkspaceTemplate Bind() => new WorkspaceTemplate()
    {
      WorkspaceId = this.workspaceId.GetInt32((IDataReader) this.Reader),
      DefinitionUri = this.definitionId.GetArtifactUriFromInt32(this.Reader, "Definition", false),
      LastModifiedDate = this.lastModifiedDate.GetDateTime((IDataReader) this.Reader),
      LastModifiedBy = this.GetUniqueName(this.m_requestContext, this.lastModifiedBy.GetString((IDataReader) this.Reader, false))
    };
  }
}
