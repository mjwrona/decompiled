// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.IPipelineConnectionCreator
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public interface IPipelineConnectionCreator
  {
    string RepositoryType { get; }

    string IdentityName { get; }

    string IdentityRole { get; }

    string GetInstallationId(
      IVssRequestContext requestContext,
      IDictionary<string, string> providerData);

    string GetInstallationId(IVssRequestContext requestContext, ServiceEndpoint serviceEndpoint);

    BuildRepository CreateBuildRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string repositoryName,
      string serviceEndpointId);

    ServiceEndpoint CreateServiceEndpoint(
      IVssRequestContext requestContext,
      Guid projectId,
      string defaultName,
      IDictionary<string, string> providerData);

    ServiceEndpoint CreateUserEndpoint(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceName,
      string avatarUrl,
      string accessToken,
      string accessTokenType);

    void PreCreateConnection(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      CreatePipelineConnectionInputs inputs);

    string GetRedirectUrl(
      IVssRequestContext requestContext,
      CreatePipelineConnectionInputs inputs,
      ServiceEndpoint endpoint);

    bool IsProviderDefinition(IVssRequestContext requestContext, BuildDefinition definition);

    bool IsProviderEndpoint(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint,
      string installationId = null);

    bool IsProviderRepository(
      IVssRequestContext requestContext,
      BuildRepository repository,
      Guid projectId);
  }
}
