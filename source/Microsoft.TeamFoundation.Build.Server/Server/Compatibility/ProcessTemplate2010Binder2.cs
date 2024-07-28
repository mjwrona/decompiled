// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.ProcessTemplate2010Binder2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal sealed class ProcessTemplate2010Binder2 : BuildObjectBinder<ProcessTemplate2010>
  {
    private IVssRequestContext m_requestContext;
    private SqlColumnBinder processTemplateId = new SqlColumnBinder("ProcessTemplateId");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder serverPath = new SqlColumnBinder("ServerPath");
    private SqlColumnBinder type = new SqlColumnBinder("Type");
    private SqlColumnBinder supportedReasons = new SqlColumnBinder("SupportedReasons");
    private SqlColumnBinder fileExists = new SqlColumnBinder("FileExists");
    private SqlColumnBinder description = new SqlColumnBinder("Description");
    private SqlColumnBinder parameters = new SqlColumnBinder("Parameters");

    internal ProcessTemplate2010Binder2(
      IVssRequestContext requestContext,
      BuildSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
    }

    protected override ProcessTemplate2010 Bind() => new ProcessTemplate2010()
    {
      Id = this.processTemplateId.GetInt32((IDataReader) this.Reader),
      TeamProjectObj = this.Component.GetTeamProjectFromGuid(this.m_requestContext, this.Component.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader))),
      ServerPath = this.Component.DataspaceDBPathToVersionControlPath(this.serverPath.GetString((IDataReader) this.Reader, false)),
      TemplateType = (ProcessTemplateType2010) this.type.GetInt32((IDataReader) this.Reader),
      SupportedReasons = RosarioHelper.Convert(this.supportedReasons.GetBuildReason(this.Reader)),
      FileExists = this.fileExists.GetBoolean((IDataReader) this.Reader, false),
      Description = this.description.GetString((IDataReader) this.Reader, true),
      Parameters = this.parameters.GetString((IDataReader) this.Reader, true)
    };
  }
}
