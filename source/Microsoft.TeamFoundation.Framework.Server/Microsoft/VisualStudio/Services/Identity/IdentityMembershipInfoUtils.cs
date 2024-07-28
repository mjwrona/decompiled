// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMembershipInfoUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityMembershipInfoUtils
  {
    internal static long GetCacheStamp(this IdentityMembershipInfo membershipInfo) => membershipInfo == null ? 0L : membershipInfo.Stamp;

    internal static bool IsInvalid(this IdentityMembershipInfo membershipInfo) => membershipInfo.IdentityId == Guid.Empty && membershipInfo.Descriptor == (IdentityDescriptor) null && membershipInfo.Parents == null && membershipInfo.ParentIds == null;

    internal static IdentityMembershipInfo CreateParentMembershipInfo(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      IdentityMembershipInfo parentMembershipInfo = new IdentityMembershipInfo();
      parentMembershipInfo.IdentityId = identity.Id;
      parentMembershipInfo.Descriptor = identity.Descriptor;
      parentMembershipInfo.Parents = identity.MemberOf != null ? new HashSet<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) identity.MemberOf, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance) : (HashSet<IdentityDescriptor>) null;
      parentMembershipInfo.ParentIds = identity.MemberOfIds != null ? new HashSet<Guid>((IEnumerable<Guid>) identity.MemberOfIds) : (HashSet<Guid>) null;
      PropertiesCollection properties = identity.Properties;
      parentMembershipInfo.IsPartialResult = properties != null && properties.ContainsKey("CacheMaxAge");
      return parentMembershipInfo;
    }

    internal static IdentityMembershipInfo CreateInvalidatedMembershipInfo(
      bool isStronglyInvalidated = false)
    {
      return new IdentityMembershipInfo()
      {
        IdentityId = Guid.Empty,
        Descriptor = (IdentityDescriptor) null,
        Parents = (HashSet<IdentityDescriptor>) null,
        ParentIds = (HashSet<Guid>) null,
        IsStronglyInvalidated = isStronglyInvalidated,
        IsPartialResult = false
      };
    }
  }
}
