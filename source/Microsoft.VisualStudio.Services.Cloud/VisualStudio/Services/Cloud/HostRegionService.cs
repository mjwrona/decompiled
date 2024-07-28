// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostRegionService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostRegionService : IHostRegionService, IVssFrameworkService
  {
    protected static readonly string s_area = "Framework";
    protected static readonly string s_layer = "RegionManagement";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string GetHostRegion(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      try
      {
        requestContext.TraceEnter(1049151, HostRegionService.s_area, HostRegionService.s_layer, nameof (GetHostRegion));
        switch (requestContext.ServiceHost.HostType)
        {
          case TeamFoundationHostType.Application:
            Microsoft.VisualStudio.Services.Organization.Organization organization = requestContext.GetService<IOrganizationService>().GetOrganization(requestContext, (IEnumerable<string>) null);
            if (organization != null && !string.IsNullOrEmpty(organization.PreferredRegion))
              return organization.PreferredRegion;
            break;
          case TeamFoundationHostType.ProjectCollection:
            Collection collection = requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) null);
            if (collection != null && !string.IsNullOrEmpty(collection.PreferredRegion))
              return collection.PreferredRegion;
            break;
          default:
            throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
        }
        return this.GetRegionFallback(requestContext);
      }
      finally
      {
        requestContext.TraceLeave(1049150, HostRegionService.s_area, HostRegionService.s_layer, nameof (GetHostRegion));
      }
    }

    protected virtual string GetRegionFallback(IVssRequestContext requestContext) => throw new RegionNotFoundException(requestContext.ServiceHost.InstanceId);
  }
}
