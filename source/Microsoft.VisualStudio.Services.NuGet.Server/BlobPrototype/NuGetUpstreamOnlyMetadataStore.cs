// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetUpstreamOnlyMetadataStore
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using BuildXL.Cache.ContentStore.UtilitiesCore.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetUpstreamOnlyMetadataStore : 
    IUpstreamMetadataService<
    #nullable disable
    VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>
  {
    private readonly UpstreamSource source;
    private readonly IUpstreamNuGetClient upstreamNuGetClient;
    private readonly ITimeProvider timeProvider;
    private readonly IConverter<string, Exception> versionValidatingConverter;
    private readonly ITracerService tracerService;
    private IOrgLevelPackagingSetting<bool> propagateDelistSetting;

    public NuGetUpstreamOnlyMetadataStore(
      UpstreamSource source,
      IUpstreamNuGetClient upstreamNuGetClient,
      ITimeProvider timeProvider,
      IConverter<string, Exception> versionValidatingConverter,
      ITracerService tracerService,
      IOrgLevelPackagingSetting<bool> propagateDelistSetting)
    {
      this.source = source;
      this.upstreamNuGetClient = upstreamNuGetClient;
      this.timeProvider = timeProvider;
      this.versionValidatingConverter = versionValidatingConverter;
      this.tracerService = tracerService;
      this.propagateDelistSetting = propagateDelistSetting;
    }

    public async Task<IEnumerable<INuGetMetadataEntry>> UpdateEntriesWithTransientStateAsync(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName,
      IEnumerable<INuGetMetadataEntry> entries,
      ICommitLogEntryHeader fakeCommitHeader)
    {
      if (!this.propagateDelistSetting.Get())
        return Enumerable.Empty<INuGetMetadataEntry>();
      List<INuGetMetadataEntry> entriesList = entries.ToList<INuGetMetadataEntry>();
      if (!entriesList.Any<INuGetMetadataEntry>())
        return Enumerable.Empty<INuGetMetadataEntry>();
      if (entriesList.Any<INuGetMetadataEntry>((Func<INuGetMetadataEntry, bool>) (x => !x.PackageIdentity.Name.Equals(packageName))))
        throw new InvalidOperationException("UpdateEntriesWithTransientStateAsync can only handle entries from a single package name");
      NuGetPackageRegistrationState registrationState1 = await this.upstreamNuGetClient.GetRegistrationState(downstreamFeedRequest, packageName, entriesList.Select<INuGetMetadataEntry, VssNuGetPackageVersion>((Func<INuGetMetadataEntry, VssNuGetPackageVersion>) (x => x.PackageIdentity.Version)));
      List<INuGetMetadataEntry> getMetadataEntryList = new List<INuGetMetadataEntry>();
      foreach (INuGetMetadataEntry getMetadataEntry in entriesList)
      {
        if (getMetadataEntry.IsLocal)
          throw new InvalidOperationException("Do not send local entries to UpdateEntriesWithTransientStateAsync");
        NuGetRegistrationState registrationState2;
        if (registrationState1.Versions.TryGetValue(getMetadataEntry.PackageIdentity.Version, out registrationState2))
        {
          INuGetMetadataEntryWriteable writeable = getMetadataEntry.CreateWriteable(fakeCommitHeader);
          bool flag = false;
          bool listed1 = registrationState2.Listed;
          bool listed2 = getMetadataEntry.Listed;
          if (listed1 != listed2)
          {
            writeable.Listed = listed1;
            flag = true;
          }
          if (flag)
            getMetadataEntryList.Add((INuGetMetadataEntry) writeable);
        }
      }
      return (IEnumerable<INuGetMetadataEntry>) getMetadataEntryList;
    }

    public Task<object> GetPackageNameMetadata(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName name)
    {
      return Task.FromResult<object>((object) null);
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<VssNuGetPackageVersion>>> GetPackageVersionsAsync(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName packageName)
    {
      NuGetUpstreamOnlyMetadataStore onlyMetadataStore = this;
      // ISSUE: reference to a compiler-generated method
      return (IReadOnlyList<VersionWithSourceChain<VssNuGetPackageVersion>>) (await onlyMetadataStore.upstreamNuGetClient.GetPackageVersions(downstreamFeedRequest, packageName)).Where<VersionWithSourceChain<VssNuGetPackageVersion>>(new Func<VersionWithSourceChain<VssNuGetPackageVersion>, bool>(onlyMetadataStore.\u003CGetPackageVersionsAsync\u003Eb__9_0)).ToList<VersionWithSourceChain<VssNuGetPackageVersion>>();
    }

    public async Task<IEnumerable<INuGetMetadataEntry>> GetPackageVersionStatesAsync(
      IFeedRequest downstreamFeedRequest,
      VssNuGetPackageName name,
      IEnumerable<VssNuGetPackageVersion> versions)
    {
      NuGetUpstreamOnlyMetadataStore sendInTheThisObject = this;
      IEnumerable<INuGetMetadataEntry> versionStatesAsync;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetPackageVersionStatesAsync)))
      {
        IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes> nuspecs = await sendInTheThisObject.upstreamNuGetClient.GetNuspecs(downstreamFeedRequest, name, versions);
        List<INuGetMetadataEntry> getMetadataEntryList = new List<INuGetMetadataEntry>();
        foreach (KeyValuePair<VssNuGetPackageVersion, ContentBytes> source1 in (IEnumerable<KeyValuePair<VssNuGetPackageVersion, ContentBytes>>) nuspecs)
        {
          VssNuGetPackageVersion key;
          ContentBytes contentBytes;
          source1.Deconstruct<VssNuGetPackageVersion, ContentBytes>(out key, out contentBytes);
          VssNuGetPackageVersion getPackageVersion = key;
          ContentBytes nuspec = contentBytes;
          if (nuspec != null)
          {
            try
            {
              INuGetMetadataEntry upstreamMetadataEntry = sendInTheThisObject.CreateCachedUpstreamMetadataEntry(nuspec);
              if (sendInTheThisObject.IsValidVersion(upstreamMetadataEntry.PackageIdentity.Version.NormalizedVersion))
              {
                if (getPackageVersion != null)
                {
                  if (getPackageVersion != null)
                  {
                    if (!(upstreamMetadataEntry.PackageIdentity.Version.NormalizedVersion == getPackageVersion.NormalizedVersion))
                      continue;
                  }
                  else
                    continue;
                }
                getMetadataEntryList.Add(upstreamMetadataEntry);
              }
            }
            catch (Exception ex)
            {
              tracer.TraceConditionally((Func<string>) (() =>
              {
                byte[] source2 = nuspec.Content;
                if (source2 == null)
                  return "Exception encountered with getting PackageVersionStates. nuspec Content is null.";
                if (nuspec.AreBytesCompressed)
                  source2 = CompressionHelper.InflateByteArray(nuspec.Content);
                return "Exception encountered with getting PackageVersionStates. First xmlCharacters: " + string.Join(" ", ((IEnumerable<byte>) source2).Take<byte>(5).Select<byte, string>((Func<byte, string>) (x => x.ToString("X2"))));
              }));
              tracer.TraceException(ex);
              throw;
            }
          }
        }
        versionStatesAsync = (IEnumerable<INuGetMetadataEntry>) getMetadataEntryList;
      }
      return versionStatesAsync;
    }

    private INuGetMetadataEntry CreateCachedUpstreamMetadataEntry(ContentBytes nuspecBytes)
    {
      DateTime now = this.timeProvider.Now;
      return (INuGetMetadataEntry) new NuGetMetadataEntry(PackagingCommitId.Empty, now, now, Guid.Empty, Guid.Empty, (IStorageId) new UpstreamStorageId(UpstreamSourceInfoUtils.CreateUpstreamSourceInfo(this.source)), 0L, nuspecBytes, true, (IEnumerable<UpstreamSourceInfo>) null);
    }

    private bool IsValidVersion(string x) => this.versionValidatingConverter.Convert(x) == null;
  }
}
