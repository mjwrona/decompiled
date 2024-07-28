// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceHostManagementService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceHostManagementService : IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual IEnumerable<HostProperties> QueryChildrenServiceHostPropertiesCached(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.EnsureHostUpdated(vssRequestContext, hostId);
      return vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryChildrenServiceHostPropertiesCached(vssRequestContext, hostId);
    }

    public virtual HostProperties QueryServiceHostPropertiesCached(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.EnsureHostUpdated(vssRequestContext, hostId);
      return vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(vssRequestContext, hostId);
    }

    public virtual TeamFoundationServiceHostProperties QueryServiceHostProperties(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.EnsureHostUpdated(vssRequestContext, hostId);
      return vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(vssRequestContext, hostId);
    }

    public virtual TeamFoundationServiceHostProperties QueryServiceHostProperties(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceHostFilterFlags filterFlags)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.EnsureHostUpdated(vssRequestContext, hostId);
      return vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(vssRequestContext, hostId, filterFlags);
    }

    public virtual void EnsureHostUpdated(IVssRequestContext requestContext, Guid hostId)
    {
      if (!requestContext.IsCommerceService())
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IHostSyncService service = vssRequestContext.GetService<IHostSyncService>();
      if (service.LocalHostExistsAndWellFormed(vssRequestContext, hostId))
        return;
      service.EnsureHostUpdated(vssRequestContext, hostId);
    }

    public virtual void ExecuteInCollectionContext(
      IVssRequestContext requestContext,
      Guid hostId,
      Action<IVssRequestContext> action,
      RequestContextType? requestContextType = null,
      [CallerMemberName] string method = null)
    {
      CollectionHelper.WithCollectionContext(requestContext, hostId, action, requestContextType, method);
    }

    public virtual void ExecuteInCollectionContext(
      IVssRequestContext requestContext,
      Guid hostId,
      Action<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity> action,
      bool checkHostType,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      [CallerMemberName] string method = null)
    {
      CollectionHelper.WithCollectionContext(requestContext, hostId, action, checkHostType, identity, method);
    }
  }
}
