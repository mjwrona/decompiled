// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDeploymentComponent3
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildDeploymentComponent3 : BuildDeploymentComponent2
  {
    public BuildDeploymentComponent3()
    {
      this.ServiceVersion = ServiceVersion.V4;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override bool IsADeployment(BuildDetail build)
    {
      this.PrepareStoredProcedure("prc_IsADeployment");
      this.BindItemUriToInt32("@deploymentBuildId", build.Uri);
      return Convert.ToBoolean(this.ExecuteScalar(), (IFormatProvider) CultureInfo.InvariantCulture);
    }

    internal override ResultCollection AddBuildDeployment(
      Guid projectId,
      string deploymentUri,
      string sourceUri,
      string environmentName)
    {
      this.PrepareStoredProcedure("prc_AddBuildDeployment");
      this.BindItemUriToInt32("@deploymentBuildId", deploymentUri);
      this.BindItemUriToInt32("@sourceBuildId", sourceUri);
      this.BindString("@environmentName", environmentName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDeployment>((ObjectBinder<BuildDeployment>) new BuildDeploymentBinder2());
      resultCollection.AddBinder<Tuple<string, Guid>>((ObjectBinder<Tuple<string, Guid>>) new BuildDeploymentRequestForBinder2());
      resultCollection.AddBinder<Tuple<string, ChangesetDisplayInformation>>((ObjectBinder<Tuple<string, ChangesetDisplayInformation>>) new BuildDeploymentChangesetBinder2());
      return resultCollection;
    }

    internal override ResultCollection QueryBuildDeployments(
      IVssRequestContext requestContext,
      BuildDeploymentSpec spec)
    {
      this.TraceEnter(0, nameof (QueryBuildDeployments));
      this.PrepareStoredProcedure("prc_QueryBuildDeployments");
      if (!string.IsNullOrWhiteSpace(spec.TeamProject))
        this.BindTeamProject(requestContext, "@teamProject", BuildCommonUtil.IsStar(spec.TeamProject) ? (string) null : spec.TeamProject, true);
      if (!string.IsNullOrWhiteSpace(spec.EnvironmentName))
        this.BindString("@environmentName", spec.EnvironmentName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      if (!string.IsNullOrWhiteSpace(spec.DefinitionPath))
        this.BindItemPath("@definitionPath", spec.Path, false);
      else
        this.BindItemPath("@definitionPath", BuildPath.Root(BuildPath.RootFolder, BuildConstants.Star), false);
      this.BindUtcDateTime("@minFinishTime", spec.MinFinishTime);
      this.BindUtcDateTime("@maxFinishTime", spec.MaxFinishTime);
      this.BindInt("@deploymentStatusFilter", (int) spec.DeploymentStatus);
      this.BindInt("@queryOrder", (int) spec.QueryOrder);
      this.BindIdentityFilter("@requestedFor", spec.RequestedFor, spec.RequestedForIdentity);
      this.BindInt("@maxDeployments", spec.MaxDeployments);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDeployment>((ObjectBinder<BuildDeployment>) new BuildDeploymentBinder2());
      resultCollection.AddBinder<Tuple<string, Guid>>((ObjectBinder<Tuple<string, Guid>>) new BuildDeploymentRequestForBinder2());
      resultCollection.AddBinder<Tuple<string, ChangesetDisplayInformation>>((ObjectBinder<Tuple<string, ChangesetDisplayInformation>>) new BuildDeploymentChangesetBinder2());
      return resultCollection;
    }

    internal override ResultCollection QueryBuildDeploymentsByUri(
      IVssRequestContext requestContext,
      IList<string> deploymentUris)
    {
      this.TraceEnter(0, "QueryBuildDeploymentByUri");
      this.PrepareStoredProcedure("prc_QueryBuildDeploymentsByUri");
      this.BindTable<KeyValuePair<int, string>>("@deploymentUriTable", (TeamFoundationTableValueParameter<KeyValuePair<int, string>>) BuildSqlResourceComponent.UrisToOrderedStringTable((IEnumerable<string>) deploymentUris));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDeployment>((ObjectBinder<BuildDeployment>) new BuildDeploymentBinder2());
      resultCollection.AddBinder<Tuple<string, Guid>>((ObjectBinder<Tuple<string, Guid>>) new BuildDeploymentRequestForBinder2());
      resultCollection.AddBinder<Tuple<string, ChangesetDisplayInformation>>((ObjectBinder<Tuple<string, ChangesetDisplayInformation>>) new BuildDeploymentChangesetBinder2());
      return resultCollection;
    }
  }
}
