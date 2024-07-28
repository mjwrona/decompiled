// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildAgent2010Binder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal sealed class BuildAgent2010Binder : BuildObjectBinder<BuildAgent2010>
  {
    private SqlColumnBinder agentId = new SqlColumnBinder("AgentId");
    private SqlColumnBinder serviceHostId = new SqlColumnBinder("ServiceHostId");
    private SqlColumnBinder controllerId = new SqlColumnBinder("ControllerId");
    private SqlColumnBinder displayName = new SqlColumnBinder("DisplayName");
    private SqlColumnBinder description = new SqlColumnBinder("Description");
    private SqlColumnBinder buildDirectory = new SqlColumnBinder("BuildDirectory");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder enabled = new SqlColumnBinder("Enabled");
    private SqlColumnBinder statusMessage = new SqlColumnBinder("StatusMessage");
    private SqlColumnBinder dateCreated = new SqlColumnBinder("DateCreated");
    private SqlColumnBinder dateUpdated = new SqlColumnBinder("DateUpdated");
    private SqlColumnBinder buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder tags = new SqlColumnBinder("Tags");

    protected override BuildAgent2010 Bind()
    {
      BuildAgent2010 buildAgent2010 = new BuildAgent2010();
      buildAgent2010.Uri = this.agentId.GetArtifactUriFromInt32(this.Reader, "Agent", false);
      buildAgent2010.ServiceHostUri = this.serviceHostId.GetArtifactUriFromInt32(this.Reader, "ServiceHost", false);
      buildAgent2010.ControllerUri = this.controllerId.GetArtifactUriFromInt32(this.Reader, "Controller", false);
      buildAgent2010.Name = this.displayName.GetBuildItem(this.Reader, false);
      buildAgent2010.Description = this.description.GetString((IDataReader) this.Reader, true);
      buildAgent2010.BuildDirectory = this.buildDirectory.GetString((IDataReader) this.Reader, false);
      buildAgent2010.Status = (AgentStatus2010) this.status.GetByte((IDataReader) this.Reader, (byte) 1);
      buildAgent2010.Enabled = this.enabled.GetBoolean((IDataReader) this.Reader);
      buildAgent2010.StatusMessage = this.statusMessage.GetString((IDataReader) this.Reader, true);
      buildAgent2010.DateCreated = this.dateCreated.GetDateTime((IDataReader) this.Reader);
      buildAgent2010.DateUpdated = this.dateUpdated.GetDateTime((IDataReader) this.Reader);
      buildAgent2010.ReservedForBuild = this.buildId.GetArtifactUriFromInt32(this.Reader, "Build", true);
      buildAgent2010.Tags.AddRange((IEnumerable<string>) this.tags.XmlToListOfString(this.Reader));
      return buildAgent2010;
    }
  }
}
