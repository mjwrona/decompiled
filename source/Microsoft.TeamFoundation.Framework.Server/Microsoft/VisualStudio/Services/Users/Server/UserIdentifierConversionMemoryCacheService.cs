// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.UserIdentifierConversionMemoryCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  internal class UserIdentifierConversionMemoryCacheService : 
    VssMemoryCacheService<SubjectDescriptor, Guid>
  {
    private IVssMemoryCacheGrouping<SubjectDescriptor, Guid, Guid> m_StorageKeyGrouping;

    public UserIdentifierConversionMemoryCacheService()
      : base((IEqualityComparer<SubjectDescriptor>) SubjectDescriptorComparer.Instance, TimeSpan.FromHours(1.0))
    {
      this.MaxCacheLength.Value = 150000;
      this.ExpiryInterval.Value = TimeSpan.FromDays(1.0);
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.m_StorageKeyGrouping = VssMemoryCacheGroupingFactory.Create<SubjectDescriptor, Guid, Guid>(systemRequestContext, this.MemoryCache, (Func<SubjectDescriptor, Guid, IEnumerable<Guid>>) ((k, v) => (IEnumerable<Guid>) new Guid[1]
      {
        v
      }), (IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, VssMemoryCacheGroupingBehavior.Replace);
    }

    public Guid GetStorageKey(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      Guid guid;
      return this.TryGetValue(requestContext, subjectDescriptor, out guid) ? guid : new Guid();
    }

    public SubjectDescriptor GetSubjectDescriptor(
      IVssRequestContext requestContext,
      Guid storageKey)
    {
      IEnumerable<SubjectDescriptor> keys;
      return this.m_StorageKeyGrouping.TryGetKeys(storageKey, out keys) ? keys.Single<SubjectDescriptor>() : new SubjectDescriptor();
    }

    public override bool Remove(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      return base.Remove(requestContext, subjectDescriptor);
    }

    public bool Remove(IVssRequestContext requestContext, Guid storageKey)
    {
      SubjectDescriptor subjectDescriptor = this.GetSubjectDescriptor(requestContext, storageKey);
      return subjectDescriptor != new SubjectDescriptor() && this.Remove(requestContext, subjectDescriptor);
    }
  }
}
