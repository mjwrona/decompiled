// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities.PrecomputedBlockedPackageIdentityProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities
{
  public class PrecomputedBlockedPackageIdentityProvider : IBlockedPackageIdentityProvider
  {
    private readonly IImmutableList<BlockedPackageIdentity> blockedIdentities;

    public PrecomputedBlockedPackageIdentityProvider(
      IEnumerable<BlockedPackageIdentity> blockedIdentities)
    {
      this.blockedIdentities = (IImmutableList<BlockedPackageIdentity>) blockedIdentities.ToImmutableList<BlockedPackageIdentity>();
    }

    public bool IsIdentityBlocked(IPackageIdentity identity, out bool allVersionsBlocked)
    {
      BlockedPackageIdentity blockedPackageIdentity = this.blockedIdentities.FirstOrDefault<BlockedPackageIdentity>((Func<BlockedPackageIdentity, bool>) (x => x.Matches(identity)));
      allVersionsBlocked = blockedPackageIdentity != null && blockedPackageIdentity.AllVersionsBlocked;
      return blockedPackageIdentity != null;
    }
  }
}
