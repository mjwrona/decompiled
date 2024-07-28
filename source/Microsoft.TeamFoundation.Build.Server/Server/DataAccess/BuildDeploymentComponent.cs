// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDeploymentComponent
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
  internal class BuildDeploymentComponent : BuildSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<BuildDeploymentComponent>(2),
      (IComponentCreator) new ComponentCreator<BuildDeploymentComponent2>(3),
      (IComponentCreator) new ComponentCreator<BuildDeploymentComponent3>(4),
      (IComponentCreator) new ComponentCreator<BuildDeploymentComponent4>(5)
    }, "BuildDeployment", "Build");

    public BuildDeploymentComponent() => this.ServiceVersion = ServiceVersion.V2;

    internal virtual ResultCollection AddBuildDeployment(
      Guid projectId,
      string deploymentUri,
      string sourceUri,
      string environmentName)
    {
      this.PrepareStoredProcedure("prc_AddBuildDeployment");
      this.BindItemUriToInt64("@deploymentBuildId", deploymentUri);
      this.BindItemUriToInt64("@sourceBuildId", sourceUri);
      this.BindString("@environmentName", environmentName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDeployment>((ObjectBinder<BuildDeployment>) new BuildDeploymentBinder());
      resultCollection.AddBinder<Tuple<string, Guid>>((ObjectBinder<Tuple<string, Guid>>) new BuildDeploymentRequestForBinder());
      resultCollection.AddBinder<Tuple<string, ChangesetDisplayInformation>>((ObjectBinder<Tuple<string, ChangesetDisplayInformation>>) new BuildDeploymentChangesetBinder());
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuildDeployments(
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
      resultCollection.AddBinder<BuildDeployment>((ObjectBinder<BuildDeployment>) new BuildDeploymentBinder());
      resultCollection.AddBinder<Tuple<string, Guid>>((ObjectBinder<Tuple<string, Guid>>) new BuildDeploymentRequestForBinder());
      resultCollection.AddBinder<Tuple<string, ChangesetDisplayInformation>>((ObjectBinder<Tuple<string, ChangesetDisplayInformation>>) new BuildDeploymentChangesetBinder());
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuildDeploymentsByUri(
      IVssRequestContext requestContext,
      IList<string> deploymentUris)
    {
      this.TraceEnter(0, "QueryBuildDeploymentByUri");
      this.PrepareStoredProcedure("prc_QueryBuildDeploymentsByUri");
      this.BindTable<KeyValuePair<int, string>>("@deploymentUriTable", (TeamFoundationTableValueParameter<KeyValuePair<int, string>>) BuildSqlResourceComponent.UrisToOrderedStringTable((IEnumerable<string>) deploymentUris));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDeployment>((ObjectBinder<BuildDeployment>) new BuildDeploymentBinder());
      resultCollection.AddBinder<Tuple<string, Guid>>((ObjectBinder<Tuple<string, Guid>>) new BuildDeploymentRequestForBinder());
      resultCollection.AddBinder<Tuple<string, ChangesetDisplayInformation>>((ObjectBinder<Tuple<string, ChangesetDisplayInformation>>) new BuildDeploymentChangesetBinder());
      return resultCollection;
    }

    internal virtual bool IsADeployment(BuildDetail build)
    {
      this.PrepareStoredProcedure("prc_IsADeployment");
      this.BindItemUriToInt64("@deploymentBuildId", build.Uri);
      return Convert.ToBoolean(this.ExecuteScalar(), (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }
}
