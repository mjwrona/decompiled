// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.ProcessTemplateBinder2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class ProcessTemplateBinder2 : BuildObjectBinder<ProcessTemplate>
  {
    private IVssRequestContext m_requestContext;
    private SqlColumnBinder processTemplateId = new SqlColumnBinder("ProcessTemplateId");
    private SqlColumnBinder teamProject = new SqlColumnBinder("TeamProject");
    private SqlColumnBinder serverPath = new SqlColumnBinder("ServerPath");
    private SqlColumnBinder type = new SqlColumnBinder("Type");
    private SqlColumnBinder supportedReasons = new SqlColumnBinder("SupportedReasons");
    private SqlColumnBinder fileExists = new SqlColumnBinder("FileExists");
    private SqlColumnBinder description = new SqlColumnBinder("Description");
    private SqlColumnBinder parameters = new SqlColumnBinder("Parameters");
    private SqlColumnBinder version = new SqlColumnBinder("Version");

    internal ProcessTemplateBinder2(
      IVssRequestContext requestContext,
      BuildSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
    }

    protected override ProcessTemplate Bind() => new ProcessTemplate()
    {
      Id = this.processTemplateId.GetInt32((IDataReader) this.Reader),
      TeamProjectObj = this.Component.GetTeamProjectFromUri(this.m_requestContext, this.teamProject.GetString((IDataReader) this.Reader, false)),
      ServerPath = DBHelper.DBPathToVersionControlPath(this.serverPath.GetString((IDataReader) this.Reader, false)),
      TemplateType = (ProcessTemplateType) this.type.GetInt32((IDataReader) this.Reader),
      SupportedReasons = this.supportedReasons.GetBuildReason(this.Reader),
      FileExists = this.fileExists.GetBoolean((IDataReader) this.Reader, false),
      Description = this.description.GetString((IDataReader) this.Reader, true),
      Parameters = this.parameters.GetString((IDataReader) this.Reader, true),
      Version = this.version.GetString((IDataReader) this.Reader, true)
    };
  }
}
