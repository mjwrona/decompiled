// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildDefinition2010Binder2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal sealed class BuildDefinition2010Binder2 : BuildObjectBinder<BuildDefinition2010>
  {
    private IVssRequestContext m_requestContext;
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder groupPath = new SqlColumnBinder("GroupPath");
    private SqlColumnBinder controllerId = new SqlColumnBinder("ControllerId");
    private SqlColumnBinder dropLocation = new SqlColumnBinder("DropLocation");
    private SqlColumnBinder continuousIntegrationType = new SqlColumnBinder("ContinuousIntegrationType");
    private SqlColumnBinder quietPeriod = new SqlColumnBinder("ContinuousIntegrationQuietPeriod");
    private SqlColumnBinder lastBuildUri = new SqlColumnBinder("LastBuildUri");
    private SqlColumnBinder lastGoodBuildUri = new SqlColumnBinder("LastGoodBuildUri");
    private SqlColumnBinder lastGoodBuildLabel = new SqlColumnBinder("LastGoodBuildLabel");
    private SqlColumnBinder enabled = new SqlColumnBinder("Enabled");
    private SqlColumnBinder description = new SqlColumnBinder("Description");
    private SqlColumnBinder processTemplateId = new SqlColumnBinder("ProcessTemplateId");
    private SqlColumnBinder processParameters = new SqlColumnBinder("ProcessParameters");
    private SqlColumnBinder scheduleJobId = new SqlColumnBinder("ScheduleJobId");

    internal BuildDefinition2010Binder2(
      IVssRequestContext requestContext,
      BuildSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
    }

    protected override BuildDefinition2010 Bind()
    {
      BuildDefinition2010 buildDefinition2010 = new BuildDefinition2010();
      buildDefinition2010.Uri = this.definitionId.GetArtifactUriFromInt32(this.Reader, "Definition", false);
      buildDefinition2010.TeamProject = this.Component.GetTeamProjectFromGuid(this.m_requestContext, this.Component.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)));
      buildDefinition2010.FullPath = BuildPath.RootNoCanonicalize(buildDefinition2010.TeamProject.Name, this.groupPath.GetBuildItem(this.Reader, false));
      buildDefinition2010.BuildControllerUri = this.controllerId.GetArtifactUriFromInt32(this.Reader, "Controller", false);
      buildDefinition2010.DefaultDropLocation = this.dropLocation.GetString((IDataReader) this.Reader, true);
      buildDefinition2010.ContinuousIntegrationType = RosarioHelper.Convert(this.continuousIntegrationType.GetTriggerType(this.Reader));
      buildDefinition2010.ContinuousIntegrationQuietPeriod = this.quietPeriod.GetInt32((IDataReader) this.Reader, 0);
      buildDefinition2010.LastBuildUri = this.lastBuildUri.GetArtifactUriFromString(this.Reader, "Build", true);
      buildDefinition2010.LastGoodBuildUri = this.lastGoodBuildUri.GetArtifactUriFromString(this.Reader, "Build", true);
      buildDefinition2010.LastGoodBuildLabel = this.lastGoodBuildLabel.GetString((IDataReader) this.Reader, true);
      buildDefinition2010.Enabled = this.enabled.GetBoolean((IDataReader) this.Reader);
      buildDefinition2010.Description = this.description.GetString((IDataReader) this.Reader, true);
      buildDefinition2010.ProcessTemplateId = this.processTemplateId.GetInt32((IDataReader) this.Reader, -1);
      buildDefinition2010.ProcessParameters = this.processParameters.GetString((IDataReader) this.Reader, true);
      buildDefinition2010.ScheduleJobId = this.scheduleJobId.GetGuid((IDataReader) this.Reader);
      return buildDefinition2010;
    }
  }
}
