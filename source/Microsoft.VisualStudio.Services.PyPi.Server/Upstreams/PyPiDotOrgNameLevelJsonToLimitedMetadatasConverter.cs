// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.PyPiDotOrgNameLevelJsonToLimitedMetadatasConverter
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using BuildXL.Cache.ContentStore.UtilitiesCore.Internal;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class PyPiDotOrgNameLevelJsonToLimitedMetadatasConverter : 
    IConverter<string, PyPiPackageRegistrationState>,
    IHaveInputType<string>,
    IHaveOutputType<PyPiPackageRegistrationState>
  {
    public PyPiPackageRegistrationState Convert(string request)
    {
      JObject metadataDoc = JObject.Parse(request);
      ImmutableDictionary<PyPiPackageVersion, PyPiPackageVersionRegistrationState> Versions = PyPiDotOrgNameLevelJsonToLimitedMetadatasConverter.BuildVersionsDictionary(this.ReadReleases(PyPiIdentityResolver.Instance.ResolvePackageName(PyPiMetadataUtils.GetRequiredProperty<string>("name", PyPiMetadataUtils.GetRequiredProperty<JObject>("info", metadataDoc))), PyPiMetadataUtils.GetRequiredProperty<JObject>("releases", metadataDoc)));
      JToken jtoken = metadataDoc["last_serial"];
      ulong? nullable = jtoken != null ? new ulong?(jtoken.Value<ulong>()) : new ulong?();
      PyPiChangelogCursor LastSerial = nullable.HasValue ? new PyPiChangelogCursor(nullable.Value) : (PyPiChangelogCursor) null;
      return new PyPiPackageRegistrationState(Versions, LastSerial);
    }

    private IEnumerable<PyPiPackageVersionRegistrationState> ReadReleases(
      PyPiPackageName parsedName,
      JObject releasesObject)
    {
      List<PyPiPackageVersionRegistrationState> registrationStateList = new List<PyPiPackageVersionRegistrationState>();
      foreach (KeyValuePair<string, JToken> source in releasesObject)
      {
        string key;
        JToken jtoken;
        source.Deconstruct<string, JToken>(out key, out jtoken);
        string packageVersion = key;
        PyPiPackageVersion parsedVersion;
        if (jtoken is JArray rawFiles && PyPiPackageIngestionValidationUtils.TryValidateAndParseVersion(packageVersion, out parsedVersion) == null)
        {
          ImmutableArray<PyPiPackageVersionFileRegistrationState> Files = PyPiDotOrgNameLevelJsonToLimitedMetadatasConverter.ReadPackageFiles(rawFiles);
          registrationStateList.Add(new PyPiPackageVersionRegistrationState(PyPiIdentityResolver.Instance.FusePackageIdentity(parsedName, parsedVersion), Files));
        }
      }
      return (IEnumerable<PyPiPackageVersionRegistrationState>) registrationStateList;
    }

    private static ImmutableDictionary<PyPiPackageVersion, PyPiPackageVersionRegistrationState> BuildVersionsDictionary(
      IEnumerable<PyPiPackageVersionRegistrationState> versions)
    {
      IEnumerable<IGrouping<IPackageVersion, PyPiPackageVersionRegistrationState>> groupings = versions.GroupBy<PyPiPackageVersionRegistrationState, IPackageVersion>((Func<PyPiPackageVersionRegistrationState, IPackageVersion>) (x => (IPackageVersion) x.CanonicalIdentity.Version), (IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion);
      ImmutableDictionary<PyPiPackageVersion, PyPiPackageVersionRegistrationState>.Builder builder = ImmutableDictionary.CreateBuilder<PyPiPackageVersion, PyPiPackageVersionRegistrationState>((IEqualityComparer<PyPiPackageVersion>) PackageVersionComparer.NormalizedVersion);
      foreach (IGrouping<IPackageVersion, PyPiPackageVersionRegistrationState> grouping in groupings)
      {
        IGrouping<IPackageVersion, PyPiPackageVersionRegistrationState> group = grouping;
        PyPiPackageVersionRegistrationState registrationState = group.Count<PyPiPackageVersionRegistrationState>() == 1 ? group.Single<PyPiPackageVersionRegistrationState>() : group.FirstOrDefault<PyPiPackageVersionRegistrationState>((Func<PyPiPackageVersionRegistrationState, bool>) (x => StringComparer.Ordinal.Equals(x.CanonicalIdentity.Version.DisplayVersion, group.Key.NormalizedVersion)));
        if ((object) registrationState != null && !registrationState.Files.IsEmpty)
          builder.Add(registrationState.CanonicalIdentity.Version, registrationState);
      }
      return builder.ToImmutable();
    }

    private static ImmutableArray<PyPiPackageVersionFileRegistrationState> ReadPackageFiles(
      JArray rawFiles)
    {
      ImmutableArray<PyPiPackageVersionFileRegistrationState>.Builder builder = ImmutableArray.CreateBuilder<PyPiPackageVersionFileRegistrationState>(rawFiles.Count);
      foreach (JToken rawFile in rawFiles)
      {
        Dictionary<string, string[]> dictionary = PyPiUpstreamJsonMetadataUtility.ConvertJsonMetadataToDictionary(rawFile);
        string requiredMetadataField1 = PyPiMetadataUtils.GetRequiredMetadataField("filename", (IReadOnlyDictionary<string, string[]>) dictionary);
        PyPiDistType pyPiDistType = (PyPiDistType) Enum.Parse(typeof (PyPiDistType), PyPiMetadataUtils.GetRequiredMetadataField("packagetype", (IReadOnlyDictionary<string, string[]>) dictionary));
        long Size = long.Parse(PyPiMetadataUtils.GetRequiredMetadataField("size", (IReadOnlyDictionary<string, string[]>) dictionary));
        DateTime universalTime = DateTime.Parse(PyPiMetadataUtils.GetRequiredMetadataField("upload_time", (IReadOnlyDictionary<string, string[]>) dictionary)).ToUniversalTime();
        string requiredMetadataField2 = PyPiMetadataUtils.GetRequiredMetadataField("url", (IReadOnlyDictionary<string, string[]>) dictionary);
        ImmutableArray<HashAndType> digests = PyPiDotOrgNameLevelJsonToLimitedMetadatasConverter.ParseDigests(rawFile);
        string optionalMetadataField = PyPiMetadataUtils.GetOptionalMetadataField("requires_python", (IReadOnlyDictionary<string, string[]>) dictionary);
        bool? HasSig = rawFile.Value<bool?>((object) "has_sig");
        bool? IsYanked = rawFile.Value<bool?>((object) "yanked");
        string YankedReason = rawFile.Value<string>((object) "yanked_reason");
        builder.Add(new PyPiPackageVersionFileRegistrationState(requiredMetadataField1, digests, Size, universalTime, new PyPiDistType?(pyPiDistType), requiredMetadataField2, HasSig, optionalMetadataField, IsYanked, YankedReason));
      }
      return builder.MoveToImmutable();
    }

    private static ImmutableArray<HashAndType> ParseDigests(JToken fileMetadataNode)
    {
      string str = fileMetadataNode[(object) "digests"]?[(object) "sha256"]?.ToString();
      if (string.IsNullOrWhiteSpace(str))
        return ImmutableArray<HashAndType>.Empty;
      return ImmutableArray.Create<HashAndType>(new HashAndType()
      {
        HashType = HashType.SHA256,
        Value = str
      });
    }
  }
}
