// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.PlatformIdentityIdentifierConversionService
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class PlatformIdentityIdentifierConversionService : 
    IdentityIdentifierConversionServiceBase
  {
    protected override IEnumerable<IIdentityIdentifierRepository> GetRepositories(
      IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return (IEnumerable<IIdentityIdentifierRepository>) new IIdentityIdentifierRepository[2]
        {
          (IIdentityIdentifierRepository) new MemoryCacheIdentityIdentifierRepository(requestContext, TeamFoundationHostType.Deployment),
          (IIdentityIdentifierRepository) new SqlComponentIdentityIdentifierRepository(TeamFoundationHostType.Deployment)
        };
      if (!requestContext.ServiceHost.IsOnly(TeamFoundationHostType.Application))
        return (IEnumerable<IIdentityIdentifierRepository>) Array.Empty<IIdentityIdentifierRepository>();
      IIdentityIdentifierRepository memoryCacheRepository = requestContext.To(TeamFoundationHostType.Deployment).GetService<PlatformIdentityIdentifierConversionService>().GetDeploymentMemoryCacheRepository();
      if (memoryCacheRepository == null)
        requestContext.Trace(6307324, TraceLevel.Error, "IdentityIdentifierConversion", "Service", "Cannot get the Deployment cache repository");
      return (IEnumerable<IIdentityIdentifierRepository>) new IIdentityIdentifierRepository[4]
      {
        (IIdentityIdentifierRepository) new MemoryCacheIdentityIdentifierRepository(requestContext, TeamFoundationHostType.Application, memoryCacheRepository as MemoryCacheIdentityIdentifierRepository),
        memoryCacheRepository,
        (IIdentityIdentifierRepository) new SqlComponentIdentityIdentifierRepository(TeamFoundationHostType.Application),
        (IIdentityIdentifierRepository) new SqlComponentIdentityIdentifierRepository(TeamFoundationHostType.Deployment)
      };
    }

    public override IdentityDescriptor GetDescriptorByLocalId(
      IVssRequestContext requestContext,
      Guid localId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(localId, nameof (localId));
      this.ValidateServiceHost(requestContext);
      try
      {
        requestContext.TraceEnter(6307302, "IdentityIdentifierConversion", "Service", nameof (GetDescriptorByLocalId));
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          this.CheckPermission(requestContext);
        Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          localId
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        if (readIdentity != null && readIdentity.Id != localId)
          throw new InvalidGetDescriptorRequestException(localId);
        return readIdentity?.Descriptor;
      }
      finally
      {
        requestContext.TraceLeave(6307303, "IdentityIdentifierConversion", "Service", nameof (GetDescriptorByLocalId));
      }
    }

    protected override void CheckPermission(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false);
    }

    internal IIdentityIdentifierRepository GetDeploymentMemoryCacheRepository() => this.Repositories.Where<IIdentityIdentifierRepository>((Func<IIdentityIdentifierRepository, bool>) (x => x is MemoryCacheIdentityIdentifierRepository && x.HostType == TeamFoundationHostType.Deployment)).SingleOrDefault<IIdentityIdentifierRepository>();
  }
}
