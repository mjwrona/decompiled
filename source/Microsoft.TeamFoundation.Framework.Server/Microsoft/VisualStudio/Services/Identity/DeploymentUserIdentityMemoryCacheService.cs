// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.DeploymentUserIdentityMemoryCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class DeploymentUserIdentityMemoryCacheService : 
    VssMemoryCacheService<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>
  {
    private IVssMemoryCacheGrouping<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor> m_identityDescriptorGrouping;
    private IVssMemoryCacheGrouping<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, Guid> m_vsidGrouping;

    public DeploymentUserIdentityMemoryCacheService()
      : base((IEqualityComparer<SubjectDescriptor>) SubjectDescriptorComparer.Instance, TimeSpan.FromHours(1.0))
    {
      this.MaxCacheLength.Value = 150000;
      this.ExpiryInterval.Value = TimeSpan.FromDays(1.0);
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.m_identityDescriptorGrouping = VssMemoryCacheGroupingFactory.Create<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>(systemRequestContext, this.MemoryCache, (Func<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, IEnumerable<IdentityDescriptor>>) ((k, v) => (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        v.Descriptor
      }), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance, VssMemoryCacheGroupingBehavior.Replace);
      this.m_vsidGrouping = VssMemoryCacheGroupingFactory.Create<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, Guid>(systemRequestContext, this.MemoryCache, (Func<SubjectDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, IEnumerable<Guid>>) ((k, v) => (IEnumerable<Guid>) new Guid[1]
      {
        v.Id
      }), (IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, VssMemoryCacheGroupingBehavior.Replace);
    }

    internal bool TryGetValue(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor,
      out Microsoft.VisualStudio.Services.Identity.Identity result)
    {
      result = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      IEnumerable<SubjectDescriptor> keys;
      if (!this.m_identityDescriptorGrouping.TryGetKeys(identityDescriptor, out keys))
        return false;
      SubjectDescriptor key = keys.SingleOrDefault<SubjectDescriptor>();
      return this.TryGetValue(requestContext, key, out result);
    }

    internal bool TryGetValue(
      IVssRequestContext requestContext,
      Guid identityId,
      out Microsoft.VisualStudio.Services.Identity.Identity result)
    {
      result = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      IEnumerable<SubjectDescriptor> keys;
      if (!this.m_vsidGrouping.TryGetKeys(identityId, out keys))
        return false;
      SubjectDescriptor key = keys.SingleOrDefault<SubjectDescriptor>();
      return this.TryGetValue(requestContext, key, out result);
    }

    public bool Remove(IVssRequestContext requestContext, Guid identityId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity result;
      return this.TryGetValue(requestContext, identityId, out result) && this.Remove(requestContext, result.SubjectDescriptor);
    }
  }
}
