// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Protocol
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server
{
  [ExcludeFromCodeCoverage]
  public class Protocol : ProtocolBase
  {
    public static readonly Protocol Cargo = new Protocol();
    private const string CorrectlyCasedNameString = "Cargo";
    private const string LowercasedNameString = "cargo";
    public const string Version1 = "1";

    private Protocol()
      : base(nameof (Cargo), nameof (Cargo), new IntegerRange(91000000, 91999999), CargoSettings.EnabledFeatureFlag, "Packaging.Cargo.ReadOnly", "Packaging.Cargo.DisasterRecovery.ChangeProcessingBypass", new BookmarkTokenKey("cargo", "feed", "searchIndexLastUpdatedBookmarkToken", 1), new BookmarkTokenKey("cargo", "feed", "DeletedPackageLastUpdatedBookmarkToken", 1), new BookmarkTokenKey("cargo", "feed", "contentVerificationLastUpdatedBookmarkToken", 1), false)
    {
    }

    public override IReadOnlyList<BlockedPackageIdentity> PermanentlyBlockedPackageIdentities { get; } = (IReadOnlyList<BlockedPackageIdentity>) ImmutableList.Create<BlockedPackageIdentity>(new BlockedPackageIdentity((IPackageName) CargoPackageNameParser.Parse("test-oneversion-c376bbb5b545401387bcd953f5a80eba"), (IPackageVersion) CargoPackageVersionParser.Parse("1.0.0")), BlockedPackageIdentity.AllVersionsOf((IPackageName) CargoPackageNameParser.Parse("test-allversions-a5dfebe1147b43d1a91d7f6d410a988c")));
  }
}
