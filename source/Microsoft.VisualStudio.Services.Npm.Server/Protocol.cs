// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Protocol
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.Npm.Server
{
  public class Protocol : ProtocolBase
  {
    public static readonly Protocol npm = new Protocol();
    private const string NpmString = "npm";

    private Protocol()
      : base(nameof (npm), "NpmCommitLog", new IntegerRange(22000000, 22999999), FeatureFlagConstants.NpmFeatureEnabled, "Packaging.Npm.ReadOnly", "Packaging.Npm.DisasterRecovery.ChangeProcessingBypass", new BookmarkTokenKey(nameof (npm), "feed", "searchIndexLastUpdatedBookmarkToken", 3), new BookmarkTokenKey(nameof (npm), "feed", "DeletedPackageLastUpdatedBookmarkToken", 2), new BookmarkTokenKey(nameof (npm), "feed", "contentVerificationLastUpdatedBookmarkToken", 1), false)
    {
    }

    public override IReadOnlyList<BlockedPackageIdentity> PermanentlyBlockedPackageIdentities { get; } = (IReadOnlyList<BlockedPackageIdentity>) ImmutableList.Create<BlockedPackageIdentity>(new BlockedPackageIdentity((IPackageName) new NpmPackageName("scoped", "test-blocked-package-oneversion-5eb34ca9fca848eba9fe23cd10933306"), (IPackageVersion) new SemanticVersion(1, 0, 0)), BlockedPackageIdentity.AllVersionsOf((IPackageName) new NpmPackageName("test-blocked-package-allversions-65efa791f8bf4eab8b6d8f18f3173d8f")), BlockedPackageIdentity.AllVersionsOf((IPackageName) new NpmPackageName("scoped", "test-blocked-package-allversions-bfddd980bc624d8b94994affb38844a1")), new BlockedPackageIdentity((IPackageName) new NpmPackageName("@augloop/types-core"), (IPackageVersion) new SemanticVersion(9, 9, 0)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("@augloop/types-core"), (IPackageVersion) new SemanticVersion(2100, 4, 2)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("@augloop/interfaces"), (IPackageVersion) new SemanticVersion(9, 9, 0)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("@augloop/interfaces"), (IPackageVersion) new SemanticVersion(2100, 4, 2)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("@augloop/logging"), (IPackageVersion) new SemanticVersion(9, 9, 0)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("@augloop/logging"), (IPackageVersion) new SemanticVersion(2100, 4, 2)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("coa"), (IPackageVersion) new SemanticVersion(2, 0, 3)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("coa"), (IPackageVersion) new SemanticVersion(2, 0, 4)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("coa"), (IPackageVersion) new SemanticVersion(2, 1, 1)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("coa"), (IPackageVersion) new SemanticVersion(2, 1, 3)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("coa"), (IPackageVersion) new SemanticVersion(3, 0, 1)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("coa"), (IPackageVersion) new SemanticVersion(3, 1, 3)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("colors"), (IPackageVersion) new SemanticVersion(1, 4, 1)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("colors"), (IPackageVersion) new SemanticVersion(1, 4, 2)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("colors"), (IPackageVersion) new SemanticVersion(1, 4, 4, "liberty2")), new BlockedPackageIdentity((IPackageName) new NpmPackageName("eslint-scope"), (IPackageVersion) new SemanticVersion(3, 7, 2)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("eslint-config-eslint"), (IPackageVersion) new SemanticVersion(5, 0, 2)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("event-stream"), (IPackageVersion) new SemanticVersion(3, 3, 6)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("flatmap-stream"), (IPackageVersion) new SemanticVersion(0, 1, 0)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("flatmap-stream"), (IPackageVersion) new SemanticVersion(0, 1, 1)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("flatmap-stream"), (IPackageVersion) new SemanticVersion(0, 1, 2)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("node-ipc"), (IPackageVersion) new SemanticVersion(9, 2, 2)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("node-ipc"), (IPackageVersion) new SemanticVersion(10, 1, 1)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("node-ipc"), (IPackageVersion) new SemanticVersion(10, 1, 2)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("rc"), (IPackageVersion) new SemanticVersion(1, 2, 9)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("rc"), (IPackageVersion) new SemanticVersion(1, 3, 9)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("rc"), (IPackageVersion) new SemanticVersion(2, 3, 9)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("ua-parser-js"), (IPackageVersion) new SemanticVersion(0, 7, 29)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("ua-parser-js"), (IPackageVersion) new SemanticVersion(0, 8, 0)), new BlockedPackageIdentity((IPackageName) new NpmPackageName("ua-parser-js"), (IPackageVersion) new SemanticVersion(1, 0, 0)));
  }
}
