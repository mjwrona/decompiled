// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildArtifactExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildArtifactExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.BuildArtifact ToWebApiBuildArtifact(
      this Microsoft.TeamFoundation.Build2.Server.BuildArtifact srvBuildArtifact,
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      Guid projectId,
      int buildId,
      ApiResourceVersion resourceVersion,
      bool expandSignedContent = false)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      ArgumentUtility.CheckForNull<ApiResourceVersion>(resourceVersion, nameof (resourceVersion));
      if (srvBuildArtifact == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildArtifact) null;
      Microsoft.TeamFoundation.Build.WebApi.BuildArtifact artifact = new Microsoft.TeamFoundation.Build.WebApi.BuildArtifact(securedObject)
      {
        Id = srvBuildArtifact.Id,
        Name = srvBuildArtifact.Name,
        Source = srvBuildArtifact.Source
      };
      if (srvBuildArtifact.Resource != null)
        artifact.Resource = new ArtifactResource(securedObject)
        {
          Data = srvBuildArtifact.Resource.Data,
          Type = srvBuildArtifact.Resource.Type,
          Properties = srvBuildArtifact.Resource.Properties,
          DownloadUrl = srvBuildArtifact.Resource.DownloadUrl
        };
      artifact.UpdateReferences(requestContext, projectId, buildId, resourceVersion, expandSignedContent);
      return artifact;
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildArtifact ToBuildServerBuildArtifact(
      this Microsoft.TeamFoundation.Build.WebApi.BuildArtifact webApiBuildArtifact)
    {
      if (webApiBuildArtifact == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildArtifact) null;
      Microsoft.TeamFoundation.Build2.Server.BuildArtifact serverBuildArtifact = new Microsoft.TeamFoundation.Build2.Server.BuildArtifact()
      {
        Id = webApiBuildArtifact.Id,
        Name = webApiBuildArtifact.Name,
        Source = webApiBuildArtifact.Source
      };
      if (webApiBuildArtifact.Resource != null)
        serverBuildArtifact.Resource = new BuildArtifactResource()
        {
          Data = webApiBuildArtifact.Resource.Data,
          Type = webApiBuildArtifact.Resource.Type,
          Properties = webApiBuildArtifact.Resource.Properties,
          DownloadUrl = webApiBuildArtifact.Resource.DownloadUrl
        };
      return serverBuildArtifact;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.BuildArtifact UpdateReferences(
      this Microsoft.TeamFoundation.Build.WebApi.BuildArtifact artifact,
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      ApiResourceVersion resourceVersion,
      bool expandSignedContent = false)
    {
      ArgumentUtility.CheckForNull<ApiResourceVersion>(resourceVersion, nameof (resourceVersion));
      if (artifact != null && artifact.Resource != null)
      {
        artifact.Resource.Url = requestContext.GetService<IBuildRouteService>().GetArtifactRestUrl(requestContext, projectId, buildId, artifact.Name, resourceVersion);
        IArtifactProvider artifactProvider;
        if (requestContext.GetService<IBuildArtifactProviderService>().TryGetArtifactProvider(requestContext, artifact.Resource.Type, out artifactProvider))
          artifactProvider.UpdateReferences(requestContext, projectId, buildId, artifact, expandSignedContent);
      }
      return artifact;
    }

    public static void TryInferType(this ArtifactResource resource)
    {
      if (resource == null || !string.IsNullOrEmpty(resource.Type) || string.IsNullOrEmpty(resource.Data))
        return;
      if (resource.Data.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
        resource.Type = "VersionControl";
      else if (resource.Data.StartsWith("#", StringComparison.OrdinalIgnoreCase))
      {
        resource.Type = "Container";
      }
      else
      {
        Uri result = (Uri) null;
        if (!Uri.TryCreate(resource.Data, UriKind.RelativeOrAbsolute, out result))
          return;
        if (result.IsFile || result.IsUnc)
          resource.Type = "FilePath";
        if (result.IsAbsoluteUri)
          resource.DownloadUrl = result.AbsoluteUri;
        else
          resource.DownloadUrl = resource.Data;
      }
    }
  }
}
