// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildController2010Binder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal sealed class BuildController2010Binder : BuildObjectBinder<BuildController2010>
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

    protected override BuildController2010 Bind()
    {
      BuildController2010 buildController2010 = new BuildController2010();
      buildController2010.Uri = this.controllerId.GetArtifactUriFromInt32(this.Reader, "Controller", false);
      buildController2010.ServiceHostUri = this.serviceHostId.GetArtifactUriFromInt32(this.Reader, "ServiceHost", false);
      buildController2010.Name = this.displayName.GetBuildItem(this.Reader, false);
      buildController2010.Description = this.description.GetString((IDataReader) this.Reader, true);
      buildController2010.CustomAssemblyPath = this.customAssemblyLocation.GetString((IDataReader) this.Reader, true);
      buildController2010.MaxConcurrentBuilds = this.maxConcurrentBuilds.GetInt32((IDataReader) this.Reader);
      buildController2010.QueueCount = this.queueCount.GetInt32((IDataReader) this.Reader);
      buildController2010.Status = (ControllerStatus2010) this.status.GetByte((IDataReader) this.Reader, (byte) 1);
      buildController2010.Enabled = this.enabled.GetBoolean((IDataReader) this.Reader);
      buildController2010.StatusMessage = this.statusMessage.GetString((IDataReader) this.Reader, true);
      buildController2010.DateCreated = this.dateCreated.GetDateTime((IDataReader) this.Reader);
      buildController2010.DateUpdated = this.dateUpdated.GetDateTime((IDataReader) this.Reader);
      buildController2010.Tags.AddRange((IEnumerable<string>) this.tags.XmlToListOfString(this.Reader));
      return buildController2010;
    }
  }
}
