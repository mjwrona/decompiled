// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Protocol
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Constants;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class Protocol : ProtocolBase
  {
    public static readonly Protocol Maven = new Protocol();
    private const string MavenString = "Maven";
    private const string MavenStringLowercase = "maven";

    public string V1 => "v1";

    private Protocol()
      : base(nameof (Maven), "maven", new IntegerRange(23000000, 23999999), MavenFeatureFlagConstants.MavenFeatureEnabled, "Packaging.Maven.ReadOnly", "Packaging.Maven.DisasterRecovery.ChangeProcessingBypass", new BookmarkTokenKey("maven", "feed", "searchIndexLastUpdatedBookmarkToken", 2), new BookmarkTokenKey("maven", "feed", "DeletedPackageLastUpdatedBookmarkToken", 2), new BookmarkTokenKey("maven", "feed", "contentVerificationLastUpdatedBookmarkToken", 1), true)
    {
    }

    public override IReadOnlyList<BlockedPackageIdentity> PermanentlyBlockedPackageIdentities { get; } = (IReadOnlyList<BlockedPackageIdentity>) ImmutableList.Create<BlockedPackageIdentity>(new BlockedPackageIdentity((IPackageName) new MavenPackageName("test-blocked-package", "oneversion-6d235f0c7dba4ba8a6794836079ffe31"), (IPackageVersion) new MavenPackageVersion("1.0.1")), BlockedPackageIdentity.AllVersionsOf((IPackageName) new MavenPackageName("test-blocked-package", "allversions-a79f7bd3a4994aa5983f3c18fe1c5d0e")));
  }
}
