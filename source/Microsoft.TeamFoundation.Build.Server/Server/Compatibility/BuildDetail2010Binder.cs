// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildDetail2010Binder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal sealed class BuildDetail2010Binder : BuildObjectBinder<BuildDetail2010>
  {
    private IVssRequestContext m_requestContext;
    private SqlColumnBinder buildUri = new SqlColumnBinder("BuildUri");
    private SqlColumnBinder queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder buildNumber = new SqlColumnBinder("BuildNumber");
    private SqlColumnBinder startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder buildStatus = new SqlColumnBinder("BuildStatus");
    private SqlColumnBinder quality = new SqlColumnBinder("Quality");
    private SqlColumnBinder compilationStatus = new SqlColumnBinder("CompilationStatus");
    private SqlColumnBinder testStatus = new SqlColumnBinder("TestStatus");
    private SqlColumnBinder dropLocation = new SqlColumnBinder("DropLocation");
    private SqlColumnBinder dropLocationRoot = new SqlColumnBinder("DropLocationRoot");
    private SqlColumnBinder logLocation = new SqlColumnBinder("LogLocation");
    private SqlColumnBinder sourceGetVersion = new SqlColumnBinder("SourceGetVersion");
    private SqlColumnBinder shelvesetName = new SqlColumnBinder("ShelvesetName");
    private SqlColumnBinder controllerId = new SqlColumnBinder("ControllerId");
    private SqlColumnBinder requestedFor = new SqlColumnBinder("RequestedFor");
    private SqlColumnBinder requestedBy = new SqlColumnBinder("RequestedBy");
    private SqlColumnBinder lastChangedOn = new SqlColumnBinder("LastChangedOn");
    private SqlColumnBinder lastChangedBy = new SqlColumnBinder("LastChangedBy");
    private SqlColumnBinder labelName = new SqlColumnBinder("LabelName");
    private SqlColumnBinder reason = new SqlColumnBinder("Reason");
    private SqlColumnBinder processParameters = new SqlColumnBinder("ProcessParameters");
    private SqlColumnBinder keepForever = new SqlColumnBinder("KeepForever");
    private SqlColumnBinder isDeleted = new SqlColumnBinder("Deleted");

    public BuildDetail2010Binder(
      IVssRequestContext requestContext,
      BuildSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
    }

    protected override BuildDetail2010 Bind()
    {
      BuildDetail2010 buildDetail2010 = new BuildDetail2010()
      {
        QueueId = this.queueId.GetInt32((IDataReader) this.Reader, 0)
      };
      buildDetail2010.Uri = RosarioHelper.ConvertBuildUri(this.buildUri.GetString((IDataReader) this.Reader, false), buildDetail2010.QueueId, false);
      buildDetail2010.BuildDefinitionUri = this.definitionId.GetArtifactUriFromInt32(this.Reader, "Definition", false);
      buildDetail2010.BuildNumber = this.buildNumber.GetBuildItem(this.Reader, false);
      buildDetail2010.StartTime = this.startTime.GetDateTime((IDataReader) this.Reader);
      buildDetail2010.FinishTime = this.finishTime.GetDateTime((IDataReader) this.Reader);
      buildDetail2010.Status = RosarioHelper.Convert(this.buildStatus.GetBuildStatus(this.Reader));
      buildDetail2010.Quality = BuildQuality.TryConvertResIdToBuildQuality(this.quality.GetString((IDataReader) this.Reader, true));
      buildDetail2010.CompilationStatus = RosarioHelper.Convert(this.compilationStatus.GetBuildPhaseStatus(this.Reader));
      buildDetail2010.TestStatus = RosarioHelper.Convert(this.testStatus.GetBuildPhaseStatus(this.Reader));
      buildDetail2010.DropLocation = this.dropLocation.GetString((IDataReader) this.Reader, true);
      buildDetail2010.DropLocationRoot = this.dropLocationRoot.GetString((IDataReader) this.Reader, true);
      buildDetail2010.LogLocation = this.logLocation.GetString((IDataReader) this.Reader, true);
      buildDetail2010.SourceGetVersion = this.sourceGetVersion.GetString((IDataReader) this.Reader, true);
      buildDetail2010.ShelvesetName = this.shelvesetName.GetString((IDataReader) this.Reader, true);
      buildDetail2010.BuildControllerUri = this.controllerId.GetArtifactUriFromInt32(this.Reader, "Controller", false);
      buildDetail2010.RequestedFor = this.GetUniqueName(this.m_requestContext, this.requestedFor.GetString((IDataReader) this.Reader, true));
      buildDetail2010.RequestedBy = this.GetUniqueName(this.m_requestContext, this.requestedBy.GetString((IDataReader) this.Reader, false));
      buildDetail2010.LastChangedOn = this.lastChangedOn.GetDateTime((IDataReader) this.Reader);
      buildDetail2010.LastChangedBy = this.GetUniqueName(this.m_requestContext, this.lastChangedBy.GetString((IDataReader) this.Reader, false));
      buildDetail2010.LabelName = this.labelName.GetString((IDataReader) this.Reader, true);
      buildDetail2010.Reason = RosarioHelper.Convert(this.reason.GetBuildReason(this.Reader));
      buildDetail2010.ProcessParameters = this.processParameters.GetString((IDataReader) this.Reader, true);
      buildDetail2010.KeepForever = this.keepForever.GetBoolean((IDataReader) this.Reader);
      buildDetail2010.IsDeleted = this.isDeleted.GetBoolean((IDataReader) this.Reader);
      return buildDetail2010;
    }
  }
}
