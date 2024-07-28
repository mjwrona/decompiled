// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IBuildServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface IBuildServiceHelper
  {
    Microsoft.TeamFoundation.Build.WebApi.Build QueryBuildByUri(
      IVssRequestContext context,
      Guid projectId,
      string uri,
      bool includeBuildDefinitionDetails);

    Microsoft.TeamFoundation.Build.WebApi.Build QueryBuildById(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      bool includeBuildDefinitionDetails);

    BuildConfiguration QueryBuildConfigurationById(
      IVssRequestContext context,
      Guid projectId,
      int buildId);

    BuildConfiguration QueryBuildConfigurationByBuildUri(
      IVssRequestContext context,
      Guid projectId,
      string uri);

    BuildConfiguration QueryBuildConfigurationByBuildNumber(
      IVssRequestContext context,
      Guid projectId,
      string buildNumber);

    BuildConfiguration QueryLastSuccessfulBuild(
      IVssRequestContext context,
      Guid projectId,
      BuildConfiguration currentBuild,
      DateTime maxFinishTimeForBuild);

    BuildConfiguration QueryLastCompleteSuccessfulBuild(
      IVssRequestContext context,
      Guid projectId,
      BuildConfiguration currentBuild,
      DateTime maxFinishTimeForBuild);

    IList<BuildConfiguration> QueryBuildsByUris(
      IVssRequestContext context,
      Guid projectId,
      List<string> buildUris);

    BuildDefinition GetBuildDefinition(
      IVssRequestContext context,
      Guid projectId,
      int definitionId);

    int GetBuildDefinitionIdFromName(
      IVssRequestContext context,
      Guid projectId,
      string definitionName);

    string GetBuildDefinitionNameFromId(
      IVssRequestContext context,
      Guid projectId,
      int definitionId);

    List<BuildDefinitionReference> QueryBuildDefinitionsByIds(
      IVssRequestContext context,
      Guid projectId,
      int[] definitionIds);

    Uri GetBuildDefinitionUriFromId(IVssRequestContext context, Guid projectId, int definitionId);

    int GetBuildArtifactId(string buildUri);

    BuildSettings GetBuildSettings(IVssRequestContext context);

    Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference GetBuildRepresentation(
      IVssRequestContext requestContext,
      BuildConfiguration buildRef);

    bool IsPullRequestBuild(IVssRequestContext requestContext, Guid projectId, string buildUri);
  }
}
