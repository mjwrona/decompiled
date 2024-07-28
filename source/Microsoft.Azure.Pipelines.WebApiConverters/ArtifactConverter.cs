// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.ArtifactConverter
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.Azure.Pipelines.Routes;
using Microsoft.Azure.Pipelines.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public class ArtifactConverter
  {
    public static readonly VersionedObjectFactory VersionedObjectFactory = new VersionedObjectFactory(new IVersionedObjectCreator[1]
    {
      (IVersionedObjectCreator) new VersionedObjectCreator<ArtifactConverter>(1)
    });

    public virtual Microsoft.Azure.Pipelines.WebApi.Artifact ToWebApiArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Server.ObjectModel.Artifact source,
      int pipelineId,
      int runId,
      bool includeSignedContent,
      Action<IVssRequestContext, int> signedContentPermissionCheck)
    {
      if (source == null)
        return (Microsoft.Azure.Pipelines.WebApi.Artifact) null;
      IPipelinesRouteService service1 = requestContext.GetService<IPipelinesRouteService>();
      Microsoft.Azure.Pipelines.WebApi.Artifact webApiArtifact = new Microsoft.Azure.Pipelines.WebApi.Artifact()
      {
        Name = source.Name,
        Url = service1.GetArtifactRestUrl(requestContext, projectId, pipelineId, runId, source.Name)
      };
      if (includeSignedContent && !string.IsNullOrEmpty(source.Resource?.ContentUrl))
      {
        string contentUrl = source.Resource.ContentUrl;
        signedContentPermissionCheck(requestContext, source.Id);
        IUrlSigningService service2 = requestContext.GetService<IUrlSigningService>();
        DateTime expires = DateTime.UtcNow.Add(SignedContentConstants.TimeToLive);
        webApiArtifact.SignedContent = new SignedUrl()
        {
          Url = service2.Sign(requestContext, new Uri(contentUrl), expires),
          SignatureExpires = expires
        };
      }
      return webApiArtifact;
    }
  }
}
