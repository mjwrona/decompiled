// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDeploymentBinder2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildDeploymentBinder2 : ObjectBinder<BuildDeployment>
  {
    private SqlColumnBinder DeploymentUriColumn = new SqlColumnBinder("DeploymentUri");
    private SqlColumnBinder SourceUriColumn = new SqlColumnBinder("SourceUri");
    private SqlColumnBinder DeploymentBuildNumberColumn = new SqlColumnBinder("DeploymentBuildNumber");
    private SqlColumnBinder SourceBuildNumberColumn = new SqlColumnBinder("SourceBuildNumber");
    private SqlColumnBinder DeploymentReasonColumn = new SqlColumnBinder("DeploymentReason");
    private SqlColumnBinder SourceReasonColumn = new SqlColumnBinder("SourceReason");
    private SqlColumnBinder DeploymentStatusColumn = new SqlColumnBinder("DeploymentStatus");
    private SqlColumnBinder SourceStatusColumn = new SqlColumnBinder("SourceStatus");
    private SqlColumnBinder DeploymentQualityColumn = new SqlColumnBinder("DeploymentQuality");
    private SqlColumnBinder SourceQualityColumn = new SqlColumnBinder("SourceQuality");
    private SqlColumnBinder DeploymentStartTimeColumn = new SqlColumnBinder("DeploymentStartTime");
    private SqlColumnBinder SourceStartTimeColumn = new SqlColumnBinder("SourceStartTime");
    private SqlColumnBinder DeploymentFinishTimeColumn = new SqlColumnBinder("DeploymentFinishTime");
    private SqlColumnBinder SourceFinishTimeColumn = new SqlColumnBinder("SourceFinishTime");
    private SqlColumnBinder DeploymentKeepForeverColumn = new SqlColumnBinder("DeploymentKeepForever");
    private SqlColumnBinder SourceKeepForeverColumn = new SqlColumnBinder("SourceKeepForever");
    private SqlColumnBinder DeploymentDefinitionNameColumn = new SqlColumnBinder("DeploymentDefinitionName");
    private SqlColumnBinder SourceGetVersionColumn = new SqlColumnBinder("SourceGetVersion");

    internal BuildDeploymentBinder2()
    {
    }

    protected override BuildDeployment Bind()
    {
      BuildSummary deployment = new BuildSummary();
      BuildSummary source = new BuildSummary();
      deployment.Uri = this.DeploymentUriColumn.GetArtifactUriFromInt32(this.Reader, "Build", false);
      source.Uri = this.SourceUriColumn.GetArtifactUriFromInt32(this.Reader, "Build", false);
      deployment.Number = this.DeploymentBuildNumberColumn.GetBuildItem(this.Reader, false);
      source.Number = this.SourceBuildNumberColumn.GetBuildItem(this.Reader, false);
      deployment.Reason = this.DeploymentReasonColumn.GetBuildReason(this.Reader);
      source.Reason = this.SourceReasonColumn.GetBuildReason(this.Reader);
      deployment.Status = this.DeploymentStatusColumn.GetBuildStatus(this.Reader);
      source.Status = this.SourceStatusColumn.GetBuildStatus(this.Reader);
      deployment.Quality = BuildQuality.TryConvertResIdToBuildQuality(this.DeploymentQualityColumn.GetString((IDataReader) this.Reader, true));
      source.Quality = BuildQuality.TryConvertResIdToBuildQuality(this.SourceQualityColumn.GetString((IDataReader) this.Reader, true));
      deployment.StartTime = this.DeploymentStartTimeColumn.GetDateTime((IDataReader) this.Reader);
      source.StartTime = this.SourceStartTimeColumn.GetDateTime((IDataReader) this.Reader);
      DateTime dateTime1 = this.DeploymentFinishTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      if (!dateTime1.Equals(DateTime.MinValue))
        deployment.FinishTime = dateTime1;
      DateTime dateTime2 = this.SourceFinishTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      if (!dateTime2.Equals(DateTime.MinValue))
        source.FinishTime = dateTime2;
      deployment.KeepForever = this.DeploymentKeepForeverColumn.GetBoolean((IDataReader) this.Reader, false);
      source.KeepForever = this.SourceKeepForeverColumn.GetBoolean((IDataReader) this.Reader, false);
      string serverPath = DBHelper.DBPathToServerPath(this.DeploymentDefinitionNameColumn.GetString((IDataReader) this.Reader, true));
      string sourceGetVersion = this.SourceGetVersionColumn.ColumnExists((IDataReader) this.Reader) ? this.SourceGetVersionColumn.GetString((IDataReader) this.Reader, true) : string.Empty;
      return new BuildDeployment(Guid.Empty, deployment, source, serverPath, sourceGetVersion);
    }
  }
}
