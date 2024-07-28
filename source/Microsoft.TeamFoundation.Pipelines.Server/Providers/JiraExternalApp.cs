// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.JiraExternalApp
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  public class JiraExternalApp : IPipelinesExternalApp
  {
    private const string c_layer = "JiraExternalApp";

    public string AppId => "com.azure.devops.integration.jira";

    public string UserLoginPropertyName => (string) null;

    public string GetInstallationDetails(IVssRequestContext requestContext, string installationId) => throw new NotImplementedException();

    public string FindMatchingInstallationId(
      IVssRequestContext requestContext,
      string repositoryId,
      out bool isInstalledForRepo)
    {
      throw new NotImplementedException();
    }

    public bool UserHasAccess(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string user,
      ServiceEndpoint userConnection)
    {
      throw new NotImplementedException();
    }

    public bool UserCanInstall(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string user,
      ServiceEndpoint userConnection,
      bool checkRepo)
    {
      throw new NotImplementedException();
    }

    public string GetInstallationIdForRepository(
      IVssRequestContext requestContext,
      string repositoryId)
    {
      throw new NotImplementedException();
    }

    public bool RequireValidation(IVssRequestContext requestContext, string installationId) => false;

    public bool ManageConnections(IVssRequestContext requestContext) => true;

    public string GetValidationUrl(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      ServiceEndpoint connectionToCreate,
      string installationId,
      string redirectUrl)
    {
      throw new NotImplementedException();
    }
  }
}
