// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Protocol
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.PyPi.Server.Constants;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.PyPi.Server
{
  public class Protocol : ProtocolBase
  {
    public const string Version1 = "1";
    public static readonly Protocol PyPi = new Protocol();
    private const string PyPiString = "PyPI";
    private const string PyPiStringLowercase = "pypi";

    private Protocol()
      : base("PyPI", nameof (PyPi), new IntegerRange(35000000, 35999999), (IOrgLevelPackagingSettingDefinition<bool>) PyPiFeatureFlags.PyPiEnabled, "Packaging.PyPi.ReadOnly", "Packaging.PyPi.DisasterRecovery.ChangeProcessingBypass", new BookmarkTokenKey("pypi", "feed", "searchIndexLastUpdatedBookmarkToken", 1), new BookmarkTokenKey("pypi", "feed", "DeletedPackageLastUpdatedBookmarkToken", 1), new BookmarkTokenKey("pypi", "feed", "contentVerificationLastUpdatedBookmarkToken", 1), true)
    {
    }

    public override IReadOnlyList<BlockedPackageIdentity> PermanentlyBlockedPackageIdentities { get; } = (IReadOnlyList<BlockedPackageIdentity>) ImmutableList.Create<BlockedPackageIdentity>(new BlockedPackageIdentity((IPackageName) new PyPiPackageName("test-disallowed-package-oneversion-0e2923072846424aa2d931df4bd0baab"), (IPackageVersion) PyPiPackageVersionParser.Parse("1.0.0")), BlockedPackageIdentity.AllVersionsOf((IPackageName) new PyPiPackageName("test-disallowed-package-allversions-f163ee0a826b40a58d7fc397428f729f")), BlockedPackageIdentity.AllVersionsOf((IPackageName) new PyPiPackageName("jeilyfish")), BlockedPackageIdentity.AllVersionsOf((IPackageName) new PyPiPackageName("python3-dateutil")), BlockedPackageIdentity.AllVersionsOf((IPackageName) new PyPiPackageName("request")));
  }
}
