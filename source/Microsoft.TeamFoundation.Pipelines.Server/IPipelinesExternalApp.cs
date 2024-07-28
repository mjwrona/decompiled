// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.IPipelinesExternalApp
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public interface IPipelinesExternalApp
  {
    string AppId { get; }

    string UserLoginPropertyName { get; }

    string GetInstallationDetails(IVssRequestContext requestContext, string installationId);

    string GetInstallationIdForRepository(IVssRequestContext requestContext, string repositoryId);

    string FindMatchingInstallationId(
      IVssRequestContext requestContext,
      string repositoryId,
      out bool isInstalledForRepo);

    bool UserHasAccess(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string user,
      ServiceEndpoint userConnection);

    bool UserCanInstall(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string user,
      ServiceEndpoint userConnection,
      bool checkRepo);

    bool RequireValidation(IVssRequestContext requestContext, string installationId);

    bool ManageConnections(IVssRequestContext requestContext);

    string GetValidationUrl(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      ServiceEndpoint connectionToCreate,
      string installationId,
      string redirectUrl);
  }
}
