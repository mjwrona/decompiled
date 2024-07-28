// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildControllerBinder2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class BuildControllerBinder2 : BuildObjectBinder<BuildController>
  {
    private SqlColumnBinder controllerId = new SqlColumnBinder("ControllerId");
    private SqlColumnBinder serviceHostId = new SqlColumnBinder("ServiceHostId");
    private SqlColumnBinder displayName = new SqlColumnBinder("DisplayName");
    private SqlColumnBinder description = new SqlColumnBinder("Description");
    private SqlColumnBinder customAssemblyLocation = new SqlColumnBinder("CustomAssemblyLocation");
    private SqlColumnBinder maxConcurrentBuilds = new SqlColumnBinder("MaxConcurrentBuilds");
    private SqlColumnBinder queueCount = new SqlColumnBinder("QueueCount");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder enabled = new SqlColumnBinder("Enabled");
    private SqlColumnBinder statusMessage = new SqlColumnBinder("StatusMessage");
    private SqlColumnBinder dateCreated = new SqlColumnBinder("DateCreated");
    private SqlColumnBinder dateUpdated = new SqlColumnBinder("DateUpdated");
    private SqlColumnBinder tags = new SqlColumnBinder("Tags");
    private SqlColumnBinder url = new SqlColumnBinder("Url");
    private SqlColumnBinder messageQueueUrl = new SqlColumnBinder("MessageQueueUrl");

    public BuildControllerBinder2(BuildSqlResourceComponent component)
      : base(component)
    {
    }

    protected override BuildController Bind()
    {
      BuildController buildController = new BuildController();
      buildController.Uri = this.controllerId.GetArtifactUriFromInt32(this.Reader, "Controller", false);
      buildController.ServiceHostUri = this.serviceHostId.GetArtifactUriFromInt32(this.Reader, "ServiceHost", false);
      buildController.Name = this.displayName.GetBuildItem(this.Reader, false);
      buildController.Description = this.description.GetString((IDataReader) this.Reader, true);
      buildController.CustomAssemblyPath = this.Component.DataspaceDBPathToVersionControlPath(this.customAssemblyLocation.GetString((IDataReader) this.Reader, true));
      buildController.MaxConcurrentBuilds = this.maxConcurrentBuilds.GetInt32((IDataReader) this.Reader);
      buildController.QueueCount = this.queueCount.GetInt32((IDataReader) this.Reader);
      buildController.Status = (ControllerStatus) this.status.GetByte((IDataReader) this.Reader, (byte) 1);
      buildController.Enabled = this.enabled.GetBoolean((IDataReader) this.Reader);
      buildController.StatusMessage = this.statusMessage.GetString((IDataReader) this.Reader, true);
      buildController.DateCreated = this.dateCreated.GetDateTime((IDataReader) this.Reader);
      buildController.DateUpdated = this.dateUpdated.GetDateTime((IDataReader) this.Reader);
      buildController.Tags.AddRange((IEnumerable<string>) this.tags.XmlToListOfString(this.Reader));
      buildController.Url = DBHelper.DBUrlToServerUrl(this.url.GetString((IDataReader) this.Reader, false));
      buildController.MessageQueueUrl = DBHelper.DBUrlToServerUrl(this.messageQueueUrl.GetString((IDataReader) this.Reader, false));
      return buildController;
    }
  }
}
