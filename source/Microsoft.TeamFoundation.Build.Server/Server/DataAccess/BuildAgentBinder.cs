// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildAgentBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class BuildAgentBinder : BuildObjectBinder<BuildAgent>
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
    private SqlColumnBinder url = new SqlColumnBinder("Url");
    private SqlColumnBinder messageQueueUrl = new SqlColumnBinder("MessageQueueUrl");

    protected override BuildAgent Bind()
    {
      BuildAgent buildAgent = new BuildAgent();
      buildAgent.Uri = this.agentId.GetArtifactUriFromInt32(this.Reader, "Agent", false);
      buildAgent.ServiceHostUri = this.serviceHostId.GetArtifactUriFromInt32(this.Reader, "ServiceHost", false);
      buildAgent.ControllerUri = this.controllerId.GetArtifactUriFromInt32(this.Reader, "Controller", false);
      buildAgent.Name = this.displayName.GetBuildItem(this.Reader, false);
      buildAgent.Description = this.description.GetString((IDataReader) this.Reader, true);
      buildAgent.BuildDirectory = this.buildDirectory.GetString((IDataReader) this.Reader, false);
      buildAgent.Status = (AgentStatus) this.status.GetByte((IDataReader) this.Reader, (byte) 1);
      buildAgent.Enabled = this.enabled.GetBoolean((IDataReader) this.Reader);
      buildAgent.StatusMessage = this.statusMessage.GetString((IDataReader) this.Reader, true);
      buildAgent.DateCreated = this.dateCreated.GetDateTime((IDataReader) this.Reader);
      buildAgent.DateUpdated = this.dateUpdated.GetDateTime((IDataReader) this.Reader);
      buildAgent.ReservedForBuild = this.buildId.GetArtifactUriFromInt32(this.Reader, "Build", true);
      buildAgent.Tags.AddRange((IEnumerable<string>) this.tags.XmlToListOfString(this.Reader));
      buildAgent.Url = DBHelper.DBUrlToServerUrl(this.url.GetString((IDataReader) this.Reader, false));
      buildAgent.MessageQueueUrl = DBHelper.DBUrlToServerUrl(this.messageQueueUrl.GetString((IDataReader) this.Reader, false));
      return buildAgent;
    }
  }
}
