// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository.PyPiDirectPublicRepoDataFetcher
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using BuildXL.Cache.ContentStore.UtilitiesCore.Internal;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository
{
  internal class PyPiDirectPublicRepoDataFetcher : 
    IDirectPublicRepoDataFetcher<PyPiPackageName, PyPiPubCachePackageNameFile, PyPiChangelogCursor>
  {
    private readonly IPublicUpstreamPyPiClient directClient;
    private readonly ITimeProvider timeProvider;

    public PyPiDirectPublicRepoDataFetcher(
      IPublicUpstreamPyPiClient directClient,
      ITimeProvider timeProvider)
    {
      this.directClient = directClient;
      this.timeProvider = timeProvider;
    }

    public async Task<(PyPiPubCachePackageNameFile NewDoc, bool NeedSave)> FetchFromUpstreamAndApplyToDocAsync(
      PyPiPackageName packageName,
      PyPiPubCachePackageNameFile existingDoc,
      PyPiChangelogCursor? minValidCursor)
    {
      Timestamp nowTimestamp = Timestamp.FromDateTime(this.timeProvider.Now);
      PyPiPackageRegistrationState registrationState1 = await this.directClient.GetRegistrationState(packageName);
      if (!registrationState1.Versions.Any<KeyValuePair<PyPiPackageVersion, PyPiPackageVersionRegistrationState>>())
        return (BuildNewDoc((string) null, (IEnumerable<PyPiPubCacheVersionLevelInfo>) ImmutableArray<PyPiPubCacheVersionLevelInfo>.Empty, registrationState1.LastSerial), true);
      List<PyPiPubCacheVersionLevelInfo> versionLevelInfoList = new List<PyPiPubCacheVersionLevelInfo>();
      foreach (KeyValuePair<PyPiPackageVersion, PyPiPackageVersionRegistrationState> version in registrationState1.Versions)
      {
        PyPiPackageVersionRegistrationState registrationState2;
        version.Deconstruct<PyPiPackageVersion, PyPiPackageVersionRegistrationState>(out PyPiPackageVersion _, out registrationState2);
        PyPiPackageVersionRegistrationState registrationState3 = registrationState2;
        versionLevelInfoList.Add(new PyPiPubCacheVersionLevelInfo()
        {
          Identity = registrationState3.CanonicalIdentity,
          Files = {
            registrationState3.Files.Select<PyPiPackageVersionFileRegistrationState, PyPiPubCachePackageVersionFileLevelInfo>(new Func<PyPiPackageVersionFileRegistrationState, PyPiPubCachePackageVersionFileLevelInfo>(ConvertFile))
          }
        });
      }
      return (BuildNewDoc(versionLevelInfoList.MaxByOrDefault<PyPiPubCacheVersionLevelInfo, PyPiPackageVersion>((Func<PyPiPubCacheVersionLevelInfo, PyPiPackageVersion>) (x => x.Identity.Version))?.Identity.Name.DisplayName, (IEnumerable<PyPiPubCacheVersionLevelInfo>) versionLevelInfoList, registrationState1.LastSerial), true);

      PyPiPubCachePackageNameFile BuildNewDoc(
        string? latestVersionDisplayName,
        IEnumerable<PyPiPubCacheVersionLevelInfo> versionEntries,
        PyPiChangelogCursor? cursorPosition)
      {
        return new PyPiPubCachePackageNameFile()
        {
          DisplayName = latestVersionDisplayName ?? packageName.DisplayName,
          ModifiedDate = nowTimestamp,
          Versions = {
            versionEntries
          },
          DocumentVersion = existingDoc.DocumentVersion + 1L,
          GenerationCursorPosition = cursorPosition
        };
      }

      static PyPiPubCachePackageVersionFileLevelInfo ConvertFile(
        PyPiPackageVersionFileRegistrationState fileRegState)
      {
        PyPiPubCachePackageVersionFileLevelInfo versionFileLevelInfo = new PyPiPubCachePackageVersionFileLevelInfo();
        versionFileLevelInfo.Filename = fileRegState.Filename;
        versionFileLevelInfo.RequiresPython = fileRegState.RequiresPython;
        versionFileLevelInfo.GpgSig = fileRegState.HasSig;
        versionFileLevelInfo.YankState = PyPiPubCachePackageVersionFileLevelInfo.BuildYankState(fileRegState.IsYanked, fileRegState.YankedReason);
        versionFileLevelInfo.CoreMetadataState = (PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState) null;
        versionFileLevelInfo.Size = checked ((ulong) fileRegState.Size);
        versionFileLevelInfo.UploadTime = Timestamp.FromDateTime(fileRegState.UploadTime);
        versionFileLevelInfo.Url = fileRegState.Url;
        PyPiDistType? packagetype = fileRegState.Packagetype;
        PyPiDistType pyPiDistType = PyPiDistType.Unknown;
        versionFileLevelInfo.DistType = !(packagetype.GetValueOrDefault() == pyPiDistType & packagetype.HasValue) ? fileRegState.Packagetype.ToString() : (string) null;
        versionFileLevelInfo.Hashes.Add((IDictionary<string, ByteString>) fileRegState.Digests.ToDictionary<string, ByteString, HashAndType>((Func<HashAndType, string>) (x => x.HashType.ToString().ToLower()), (Func<HashAndType, ByteString>) (x => ByteString.CopyFrom(HexConverter.ToByteArray(x.Value)))));
        return versionFileLevelInfo;
      }
    }
  }
}
