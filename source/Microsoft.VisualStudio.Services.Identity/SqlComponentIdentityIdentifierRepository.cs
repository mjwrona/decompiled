// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SqlComponentIdentityIdentifierRepository
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class SqlComponentIdentityIdentifierRepository : IIdentityIdentifierRepository
  {
    public SqlComponentIdentityIdentifierRepository(TeamFoundationHostType hostType) => this.HostType = hostType;

    public IdentityDescriptor GetDescriptorByLocalId(
      IVssRequestContext requestContext,
      Guid localId)
    {
      throw new NotImplementedException();
    }

    public IdentityDescriptor GetDescriptorByMasterId(
      IVssRequestContext requestContext,
      Guid masterId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(this.HostType);
      if (this.HostType == TeamFoundationHostType.Deployment)
        return vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<Guid>) new Guid[1]
        {
          masterId
        }, QueryMembership.None, (IEnumerable<string>) null)[0]?.Descriptor;
      using (IdentityManagementComponent component = vssRequestContext.CreateComponent<IdentityManagementComponent>())
        return component.GetIdentityDescriptorById(masterId);
    }

    public void OnDescriptorRetrievedByLocalId(
      IVssRequestContext requestContext,
      Guid localId,
      IdentityDescriptor identityDescriptor)
    {
    }

    public void OnDescriptorRetrievedByMasterId(
      IVssRequestContext requestContext,
      Guid masterId,
      IdentityDescriptor identityDescriptor)
    {
    }

    public void Unload(IVssRequestContext requestContext)
    {
    }

    public TeamFoundationHostType HostType { get; }
  }
}
