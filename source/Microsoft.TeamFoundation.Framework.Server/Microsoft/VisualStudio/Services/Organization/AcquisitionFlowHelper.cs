// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.AcquisitionFlowHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.HostAcquisition;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class AcquisitionFlowHelper
  {
    public void CreateCollection(
      IVssRequestContext deploymentContext,
      string collectionName,
      string preferredRegion,
      Microsoft.VisualStudio.Services.Identity.Identity creatorIdentity,
      IDictionary<string, object> creationProperties = null)
    {
      this.CreateCollectionUnderNewOrganization(deploymentContext, collectionName, preferredRegion, creatorIdentity, creationProperties);
    }

    private void CreateCollectionUnderNewOrganization(
      IVssRequestContext deploymentContext,
      string collectionName,
      string preferredRegion,
      Microsoft.VisualStudio.Services.Identity.Identity creatorIdentity,
      IDictionary<string, object> creationProperties)
    {
      if (!(deploymentContext.ServiceInstanceType() == ServiceInstanceTypes.SPS))
      {
        CollectionCreationContext creationContext = new CollectionCreationContext(collectionName, creationProperties)
        {
          OwnerId = creatorIdentity.Id,
          PreferredRegion = preferredRegion
        };
        deploymentContext.GetService<IHostAcquisitionService>().CreateCollection(deploymentContext, creationContext);
      }
      else
      {
        OrganizationCreationContext creationContext = new OrganizationCreationContext((string) null, true)
        {
          CreatorId = creatorIdentity.Id,
          PreferredRegion = preferredRegion,
          PrimaryCollection = new CollectionCreationContext(collectionName, creationProperties)
          {
            OwnerId = creatorIdentity.Id,
            PreferredRegion = preferredRegion
          }
        };
        deploymentContext.GetService<IOrganizationCatalogService>().CreateOrganization(deploymentContext.Elevate(), creationContext);
      }
    }
  }
}
