// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Linking.TeamFoundationLinkingService
// Assembly: Microsoft.Azure.Boards.Linking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2FA874A3-91E6-4EEC-B5F5-3126D83824FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Linking.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.Linking
{
  public class TeamFoundationLinkingService : ITeamFoundationLinkingService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string GetArtifactUrlExternal(IVssRequestContext requestContext, string uri)
    {
      string locationServiceUrl = requestContext.Elevate().GetService<ILocationService>().GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.PublicAccessMappingMoniker);
      return this.GetArtifactUrl(requestContext, LinkingUtilities.DecodeUri(uri), locationServiceUrl);
    }

    public string GetArtifactUrlExternal(IVssRequestContext requestContext, ArtifactId artId)
    {
      string locationServiceUrl = requestContext.Elevate().GetService<ILocationService>().GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.PublicAccessMappingMoniker);
      return this.GetArtifactUrl(requestContext, artId, locationServiceUrl);
    }

    private string GetArtifactUrl(
      IVssRequestContext requestContext,
      ArtifactId artId,
      string serverUrl)
    {
      ArgumentUtility.CheckForNull<ArtifactId>(artId, nameof (artId));
      if (!LinkingUtilities.IsToolTypeWellFormed(artId.Tool) || !LinkingUtilities.IsArtifactTypeWellFormed(artId.ArtifactType))
        throw new ArgumentException(CommonResources.MalformedArtifactId((object) artId.ToString()));
      IReadOnlyCollection<RegistrationArtifactType> artifactLinkTypes = requestContext.GetService<IArtifactLinkTypesService>().GetArtifactLinkTypes(requestContext, artId.Tool);
      bool flag = false;
      foreach (RegistrationArtifactType registrationArtifactType in (IEnumerable<RegistrationArtifactType>) artifactLinkTypes)
      {
        if (VssStringComparer.ArtifactType.Compare(registrationArtifactType.Name, artId.ArtifactType) == 0)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        throw new ArgumentException(CommonResources.MalformedArtifactId((object) artId.ToString()));
      return LinkingUtilities.GetArtifactUrl(string.Empty, artId, serverUrl);
    }

    private bool IsDeploymentLevelRegistrationEntriesEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Framework.EnableDeploymentRegistrationEntries") && requestContext.ServiceHost.DeploymentServiceHost.IsHosted;
  }
}
