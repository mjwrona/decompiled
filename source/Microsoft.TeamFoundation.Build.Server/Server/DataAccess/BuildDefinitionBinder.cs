// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDefinitionBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class BuildDefinitionBinder : BuildObjectBinder<BuildDefinition>
  {
    private IVssRequestContext m_requestContext;
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder teamProject = new SqlColumnBinder("TeamProject");
    private SqlColumnBinder groupPath = new SqlColumnBinder("GroupPath");
    private SqlColumnBinder controllerId = new SqlColumnBinder("ControllerId");
    private SqlColumnBinder dropLocation = new SqlColumnBinder("DropLocation");
    private SqlColumnBinder triggerType = new SqlColumnBinder("TriggerType");
    private SqlColumnBinder quietPeriod = new SqlColumnBinder("ContinuousIntegrationQuietPeriod");
    private SqlColumnBinder batchSize = new SqlColumnBinder("BatchSize");
    private SqlColumnBinder lastBuildUri = new SqlColumnBinder("LastBuildUri");
    private SqlColumnBinder lastGoodBuildUri = new SqlColumnBinder("LastGoodBuildUri");
    private SqlColumnBinder lastGoodBuildLabel = new SqlColumnBinder("LastGoodBuildLabel");
    private SqlColumnBinder queueStatus = new SqlColumnBinder("QueueStatus");
    private SqlColumnBinder description = new SqlColumnBinder("Description");
    private SqlColumnBinder processTemplateId = new SqlColumnBinder("ProcessTemplateId");
    private SqlColumnBinder processParameters = new SqlColumnBinder("ProcessParameters");
    private SqlColumnBinder scheduleJobId = new SqlColumnBinder("ScheduleJobId");
    private SqlColumnBinder scheduleJobDelay = new SqlColumnBinder("ScheduleJobDelay");
    private SqlColumnBinder dateCreated = new SqlColumnBinder("DateCreated");

    internal BuildDefinitionBinder(
      IVssRequestContext requestContext,
      BuildSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
    }

    protected override BuildDefinition Bind()
    {
      BuildDefinition buildDefinition = new BuildDefinition()
      {
        Uri = this.definitionId.GetArtifactUriFromInt32(this.Reader, "Definition", false),
        TeamProject = this.Component.GetTeamProjectFromUri(this.m_requestContext, this.teamProject.GetString((IDataReader) this.Reader, false))
      };
      buildDefinition.FullPath = BuildPath.RootNoCanonicalize(buildDefinition.TeamProject.Name, this.groupPath.GetBuildItem(this.Reader, false));
      buildDefinition.BuildControllerUri = this.controllerId.GetArtifactUriFromInt32(this.Reader, "Controller", false);
      buildDefinition.DefaultDropLocation = this.dropLocation.GetString((IDataReader) this.Reader, true);
      buildDefinition.TriggerType = this.triggerType.GetTriggerType(this.Reader);
      buildDefinition.ContinuousIntegrationQuietPeriod = this.quietPeriod.GetInt32((IDataReader) this.Reader, 0);
      buildDefinition.BatchSize = this.batchSize.GetInt32((IDataReader) this.Reader, 1);
      buildDefinition.LastBuildUri = this.lastBuildUri.GetArtifactUriFromString(this.Reader, "Build", true);
      buildDefinition.LastGoodBuildUri = this.lastGoodBuildUri.GetArtifactUriFromString(this.Reader, "Build", true);
      buildDefinition.LastGoodBuildLabel = this.lastGoodBuildLabel.GetString((IDataReader) this.Reader, true);
      buildDefinition.QueueStatus = (DefinitionQueueStatus) this.queueStatus.GetByte((IDataReader) this.Reader);
      buildDefinition.Description = this.description.GetString((IDataReader) this.Reader, true);
      buildDefinition.ProcessTemplateId = this.processTemplateId.GetInt32((IDataReader) this.Reader, -1);
      buildDefinition.ProcessParameters = this.processParameters.GetString((IDataReader) this.Reader, true);
      buildDefinition.ScheduleJobId = this.scheduleJobId.GetGuid((IDataReader) this.Reader);
      int int32 = this.scheduleJobDelay.GetInt32((IDataReader) this.Reader, int.MinValue);
      if (int32 > int.MinValue)
        buildDefinition.ScheduleJobDelay = new TimeSpan?(TimeSpan.FromSeconds((double) Math.Max(0, int32)));
      buildDefinition.DateCreated = this.dateCreated.GetDateTime((IDataReader) this.Reader);
      return buildDefinition;
    }
  }
}
