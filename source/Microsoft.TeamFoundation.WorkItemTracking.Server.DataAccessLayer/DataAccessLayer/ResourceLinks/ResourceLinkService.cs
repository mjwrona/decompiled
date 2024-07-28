// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.ResourceLinks.ResourceLinkService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.ResourceLinks
{
  public class ResourceLinkService : IVssFrameworkService
  {
    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<ResourceLink> GetResourceLinks(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds)
    {
      IEnumerable<ResourceLinkData> resourceLinks;
      using (ResourceLinkComponent component = requestContext.CreateComponent<ResourceLinkComponent>())
        resourceLinks = component.GetResourceLinks(workItemIds);
      IEnumerable<IGrouping<int, ResourceLinkData>> source = resourceLinks.GroupBy<ResourceLinkData, int>((Func<ResourceLinkData, int>) (rld => rld.AreaId));
      PermissionCheckHelper permissionCheckHelper = new PermissionCheckHelper(requestContext);
      Func<IGrouping<int, ResourceLinkData>, bool> predicate = (Func<IGrouping<int, ResourceLinkData>, bool>) (aidg => permissionCheckHelper.HasWorkItemPermission(aidg.Key, 16));
      return source.Where<IGrouping<int, ResourceLinkData>>(predicate).SelectMany<IGrouping<int, ResourceLinkData>, ResourceLink>((Func<IGrouping<int, ResourceLinkData>, IEnumerable<ResourceLink>>) (aidg => aidg.Select<ResourceLinkData, ResourceLink>((Func<ResourceLinkData, ResourceLink>) (rld => ResourceLink.Create(rld)))));
    }
  }
}
