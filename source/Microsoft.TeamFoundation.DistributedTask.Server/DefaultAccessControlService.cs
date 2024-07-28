// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DefaultAccessControlService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class DefaultAccessControlService : IAccessControlService, IVssFrameworkService
  {
    public void DeleteServiceIdentity(IVssRequestContext requestContext, Guid serviceIdentityId) => requestContext.GetService<TeamFoundationAccessControlService>().DeleteServiceIdentity(requestContext, serviceIdentityId);

    public ServiceIdentity GetServiceIdentity(
      IVssRequestContext requestContext,
      Guid serviceIdentityId)
    {
      return ((IEnumerable<ServiceIdentity>) requestContext.GetService<TeamFoundationAccessControlService>().QueryServiceIdentities(requestContext, new Guid[1]
      {
        serviceIdentityId
      }, false)).FirstOrDefault<ServiceIdentity>();
    }

    public ServiceIdentity ProvisionServiceIdentity(
      IVssRequestContext requestContext,
      ServiceIdentityInfo info,
      IList<IdentityDescriptor> groupMemberships)
    {
      return requestContext.GetService<TeamFoundationAccessControlService>().ProvisionServiceIdentity(requestContext, info, groupMemberships.ToArray<IdentityDescriptor>());
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
