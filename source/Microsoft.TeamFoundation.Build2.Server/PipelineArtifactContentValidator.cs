// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PipelineArtifactContentValidator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class PipelineArtifactContentValidator : 
    BaseRouteService,
    IPipelineArtifactContentValidator
  {
    private readonly TimeSpan signedUrlLifetime = TimeSpan.FromDays(14.0);
    private readonly IClock clock;

    public PipelineArtifactContentValidator(IClock clock) => this.clock = clock;

    public async Task ValidateContentAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      BuildArtifact buildArtifact,
      Manifest manifest,
      Microsoft.VisualStudio.Services.Identity.Identity publisher,
      string publisherIpAddress)
    {
      using (requestContext.TraceScope("PipelineArtifactProvider", nameof (ValidateContentAsync)))
      {
        if (requestContext.GetService<IProjectService>().GetProjectVisibility(requestContext, projectId) == ProjectVisibility.Public)
        {
          IEnumerable<ContentValidationKey> validationKeysForBlobs = this.GetValidationKeysForBlobs(requestContext, projectId, buildId, buildArtifact, manifest);
          await requestContext.GetService<IContentValidationService>().SubmitAsync(requestContext, projectId, validationKeysForBlobs, publisher, publisherIpAddress).ConfigureAwait(true);
        }
      }
    }

    private IEnumerable<ContentValidationKey> GetValidationKeysForBlobs(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      BuildArtifact buildArtifact,
      Manifest manifest)
    {
      if (!requestContext.GetService<IBuildArtifactProviderService>().TryGetArtifactProvider(requestContext, buildArtifact.Resource.Type, out IArtifactProvider _))
        throw new ArtifactTypeNotSupportedException(BuildServerResources.ArtifactTypeNotSupported((object) buildArtifact.Resource.Type));
      try
      {
        if (requestContext.GetService<IBuildService>().GetBuildById(requestContext, projectId, buildId) == null)
          throw new BuildNotFoundException(BuildServerResources.BuildNotFound((object) buildId));
        IEnumerable<ManifestItem> manifestItems = manifest.Items.Where<ManifestItem>((Func<ManifestItem, bool>) (mi => mi.Type == ManifestItemType.File));
        List<ContentValidationKey> validationKeysForBlobs = new List<ContentValidationKey>();
        CancellationToken cancellationToken = requestContext.CancellationToken;
        Uri baseUri1 = new Uri(this.GetResourceUrl(requestContext, BuildResourceIds.Artifacts, projectId, (object) new
        {
          buildId = buildId
        }));
        IUrlSigningService service = requestContext.GetService<IUrlSigningService>();
        foreach (ManifestItem manifestItem in manifestItems)
        {
          DedupIdentifier dedupId = DedupIdentifier.Create(manifestItem.Blob.Id);
          string agnosticUnixFileName = DedupManifestArtifactHelper.GetOSAgnosticUnixFileName(manifestItem.Path);
          Uri baseUri2 = this.GetBaseUri(baseUri1, projectId, buildArtifact.Name, dedupId, agnosticUnixFileName);
          TimeSpan timeToLive = this.signedUrlLifetime > service.MaxTimeToLive ? service.MaxTimeToLive : this.signedUrlLifetime;
          ContentValidationKey contentValidationKey = new ContentValidationKey(new Uri(service.Sign(requestContext, baseUri2, timeToLive)), agnosticUnixFileName);
          validationKeysForBlobs.Add(contentValidationKey);
        }
        return (IEnumerable<ContentValidationKey>) validationKeysForBlobs;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12030160, "Service", ex);
        throw;
      }
    }

    private Uri GetBaseUri(
      Uri baseUri,
      Guid projectId,
      string artifactName,
      DedupIdentifier dedupId,
      string fileName)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>()
      {
        {
          nameof (artifactName),
          (object) artifactName
        },
        {
          "fileId",
          (object) dedupId.ValueString
        },
        {
          nameof (fileName),
          (object) fileName
        },
        {
          "api-version",
          (object) "5.0-preview"
        }
      };
      string relativeUri = VssHttpUriUtility.ReplaceRouteValues(baseUri.PathAndQuery, routeValues, true, true);
      return new Uri(baseUri, relativeUri);
    }
  }
}
