// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.IBuild2Converter
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Build.Server
{
  [InheritedExport]
  public interface IBuild2Converter
  {
    List<Tuple<QueuedBuild, BuildDetail>> GetQueuedBuilds(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildQueueSpec spec);

    Tuple<QueuedBuild, BuildDetail> GetQueuedBuildById(
      IVssRequestContext requestContext,
      int buildId,
      IList<string> informationTypes);

    BuildDetail GetBuildByUri(
      IVssRequestContext requestContext,
      string buildUri,
      IList<string> informationTypes,
      bool alwaysReturnDefaultNodes = false);

    bool IsBuild2Id(IVssRequestContext requestContext, Guid projectId, int buildId);

    BuildDetail CancelBuild(IVssRequestContext requestContext, int buildId);

    BuildDetail CancelBuild(IVssRequestContext requestContext, Guid projectId, string buildUri);

    BuildDetail DeleteBuild(
      IVssRequestContext requestContext,
      string buildUri,
      bool setBuildRecordAsDeleted);

    QueuedBuild QueueBuild(IVssRequestContext requestContext, Guid projectId, BuildRequest request);

    string CreateAzureWebsiteBuildDefinition(
      IVssRequestContext requestContext,
      string definitionName,
      string teamProject,
      string websiteName,
      string connectedServiceId,
      string repositoryId = null,
      string gitBranch = null);

    string CreateAzureCloudBuildDefinition(
      IVssRequestContext requestContext,
      string definitionName,
      string teamProject,
      string hostedServiceName,
      string connectedServiceId,
      string storageAccountName,
      string repositoryId = null,
      string gitBranch = null);

    bool DisconnectBuildDefinition(
      IVssRequestContext requestContext,
      string definitionName,
      string teamProject);
  }
}
