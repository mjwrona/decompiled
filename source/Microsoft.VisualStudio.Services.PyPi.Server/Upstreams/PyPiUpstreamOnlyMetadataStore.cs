// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.PyPiUpstreamOnlyMetadataStore
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class PyPiUpstreamOnlyMetadataStore : 
    IUpstreamMetadataService<
    #nullable disable
    PyPiPackageName, PyPiPackageVersion, PyPiPackageIdentity, IPyPiMetadataEntry>
  {
    private readonly UpstreamSource source;
    private readonly IUpstreamPyPiClient upstreamPyPiClient;
    private readonly ITimeProvider timeProvider;
    private readonly ITracerService tracerService;

    public PyPiUpstreamOnlyMetadataStore(
      UpstreamSource source,
      IUpstreamPyPiClient upstreamPyPiClient,
      ITimeProvider timeProvider,
      ITracerService tracerService)
    {
      this.source = source;
      this.upstreamPyPiClient = upstreamPyPiClient;
      this.timeProvider = timeProvider;
      this.tracerService = tracerService;
    }

    public Task<IEnumerable<IPyPiMetadataEntry>> UpdateEntriesWithTransientStateAsync(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageName packageName,
      IEnumerable<IPyPiMetadataEntry> entries,
      ICommitLogEntryHeader fakeCommitHeader)
    {
      return Task.FromResult<IEnumerable<IPyPiMetadataEntry>>(Enumerable.Empty<IPyPiMetadataEntry>());
    }

    public Task<object> GetPackageNameMetadata(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageName name)
    {
      return (Task<object>) null;
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>>> GetPackageVersionsAsync(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageName packageName)
    {
      return (IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>>) (await this.upstreamPyPiClient.GetPackageVersions(downstreamFeedRequest, packageName)).Select<VersionWithSourceChain<PyPiPackageVersion>, VersionWithSourceChain<PyPiPackageVersion>>((Func<VersionWithSourceChain<PyPiPackageVersion>, VersionWithSourceChain<PyPiPackageVersion>>) (versionWithSourceChain =>
      {
        PyPiPackageVersion parsedVersion;
        PyPiPackageIngestionValidationUtils.TryValidateAndParseVersion(versionWithSourceChain.Version.NormalizedVersion, out parsedVersion);
        return parsedVersion == null ? (VersionWithSourceChain<PyPiPackageVersion>) null : versionWithSourceChain;
      })).Where<VersionWithSourceChain<PyPiPackageVersion>>((Func<VersionWithSourceChain<PyPiPackageVersion>, bool>) (v => v != null)).ToList<VersionWithSourceChain<PyPiPackageVersion>>();
    }

    public async Task<IEnumerable<IPyPiMetadataEntry>> GetPackageVersionStatesAsync(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageName name,
      IEnumerable<PyPiPackageVersion> versions)
    {
      PyPiUpstreamOnlyMetadataStore sendInTheThisObject = this;
      IEnumerable<LimitedPyPiMetadata> limitedMetadataList = await sendInTheThisObject.upstreamPyPiClient.GetLimitedMetadataList(name, versions);
      List<IPyPiMetadataEntry> pyPiMetadataEntryList = new List<IPyPiMetadataEntry>();
      IEnumerable<IPyPiMetadataEntry> versionStatesAsync;
      using (ITracerBlock tracerBlock = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetPackageVersionStatesAsync)))
      {
        foreach (LimitedPyPiMetadata limitedPyPiMetadata1 in limitedMetadataList)
        {
          LimitedPyPiMetadata pypiMetadata = limitedPyPiMetadata1;
          LimitedPyPiMetadata limitedPyPiMetadata2 = pypiMetadata;
          if (((object) limitedPyPiMetadata2 != null ? limitedPyPiMetadata2.PackageFiles.FirstOrDefault<IUnstoredPyPiPackageFile>() : (IUnstoredPyPiPackageFile) null) != null)
          {
            try
            {
              IPyPiMetadataEntry upstreamMetadataEntry = sendInTheThisObject.CreateCachedUpstreamMetadataEntry(pypiMetadata);
              pyPiMetadataEntryList.Add(upstreamMetadataEntry);
            }
            catch (Exception ex)
            {
              tracerBlock.TraceConditionally((Func<string>) (() => string.Format("Package {0} failed with exception {1}", (object) pypiMetadata.Identity.DisplayStringForMessages, (object) ex)));
            }
          }
        }
        versionStatesAsync = (IEnumerable<IPyPiMetadataEntry>) pyPiMetadataEntryList;
      }
      return versionStatesAsync;
    }

    private IPyPiMetadataEntry CreateCachedUpstreamMetadataEntry(
      LimitedPyPiMetadata limitedPyPiMetadata)
    {
      PyPiMetadataEntry upstreamMetadataEntry = (PyPiMetadataEntry) null;
      DateTime now = this.timeProvider.Now;
      foreach (IUnstoredPyPiPackageFile packageFile1 in limitedPyPiMetadata.PackageFiles)
      {
        PyPiPackageFile packageFile2 = new PyPiPackageFile(packageFile1.Path, (IStorageId) new UpstreamStorageId(UpstreamSourceInfoUtils.CreateUpstreamSourceInfo(this.source)), packageFile1.Hashes, packageFile1.SizeInBytes, packageFile1.DateAdded, packageFile1.DistType);
        if (upstreamMetadataEntry == null)
        {
          VersionConstraintList versionConstraintList = !string.IsNullOrWhiteSpace(limitedPyPiMetadata.RequiresPython) ? RequirementParser.ParseVersionConstraintList(limitedPyPiMetadata.RequiresPython, TimeSpan.FromSeconds(3.0)) : (VersionConstraintList) null;
          upstreamMetadataEntry = new PyPiMetadataEntry(limitedPyPiMetadata.Identity, packageFile2, versionConstraintList, PackagingCommitId.Empty, Guid.Empty, now, Guid.Empty, now, (IEnumerable<UpstreamSourceInfo>) null);
        }
        else
          upstreamMetadataEntry.AddPackageFile(packageFile2);
      }
      return (IPyPiMetadataEntry) upstreamMetadataEntry;
    }
  }
}
