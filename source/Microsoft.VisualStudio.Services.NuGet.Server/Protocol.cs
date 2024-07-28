// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Protocol
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public class Protocol : ProtocolBase
  {
    public static readonly Protocol NuGet = new Protocol();
    private const string NuGetString = "NuGet";
    private const string NuGetStringLowercase = "nuget";

    private Protocol()
      : base(nameof (NuGet), nameof (NuGet), new IntegerRange(21000000, 21999999), FeatureEnabledConstants.NuGetFeatureEnabled, "Packaging.NuGet.ReadOnly", "Packaging.NuGet.DisasterRecovery.ChangeProcessingBypass", new BookmarkTokenKey("nuget", "feed", "searchIndexLastUpdatedBookmarkToken", 2), new BookmarkTokenKey("nuget", "feed", "DeletedPackageLastUpdatedBookmarkToken", 2), (BookmarkTokenKey) null, false)
    {
    }

    public override IReadOnlyList<BlockedPackageIdentity> PermanentlyBlockedPackageIdentities { get; } = (IReadOnlyList<BlockedPackageIdentity>) ImmutableList.Create<BlockedPackageIdentity>(new BlockedPackageIdentity((IPackageName) new VssNuGetPackageName("test-blocked-identity-oneversion-b97c3d6d5a224860a48368dc80de389c"), (IPackageVersion) new VssNuGetPackageVersion("1.0.0")), BlockedPackageIdentity.AllVersionsOf((IPackageName) new VssNuGetPackageName("test-blocked-identity-allversions-8739e733ab704039b92c31c8574c92cf")));
  }
}
