// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.UpdateUpstreamMetadataWithFilesOpApplier`5
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class UpdateUpstreamMetadataWithFilesOpApplier<TPackageIdentity, TPackageName, TPackageVersion, TMetadataEntry, TMetadataEntryWritable> : 
    IMetadataDocumentOpApplier<MetadataDocument<TMetadataEntry>>
    where TPackageIdentity : IPackageIdentity<TPackageName, TPackageVersion>
    where TPackageName : IPackageName
    where TPackageVersion : class, IPackageVersion
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>, IPackageFiles, IMetadataEntryFilesUpdater<TMetadataEntry>, ICreateWriteable<TMetadataEntryWritable>
    where TMetadataEntryWritable : IMetadataEntryWriteable<TPackageIdentity>, TMetadataEntry
  {
    private readonly IComparer<IPackageVersion> versionComparer;
    private readonly IEqualityComparer<IPackageVersion> versionEqualityComparer;

    public UpdateUpstreamMetadataWithFilesOpApplier(
      IComparer<IPackageVersion> versionComparer,
      IEqualityComparer<IPackageVersion> versionEqualityComparer)
    {
      this.versionComparer = versionComparer;
      this.versionEqualityComparer = versionEqualityComparer;
    }

    public virtual MetadataDocument<TMetadataEntry> Apply(
      ICommitLogEntry commitLogEntry,
      MetadataDocument<TMetadataEntry> currentDoc)
    {
      if (!(commitLogEntry.CommitOperationData is IUpdateUpstreamMetadataOperationData<TPackageIdentity, TPackageName, TPackageVersion, TMetadataEntry> commitOperationData))
        throw new ArgumentException(Resources.Error_InvalidCommitEntryType());
      IUpdateUpstreamMetadataOperationData<TPackageIdentity, TPackageName, TPackageVersion, TMetadataEntry> operationData = commitOperationData;
      Dictionary<TPackageVersion, TMetadataEntry> dictionary1 = currentDoc.Entries.ToDictionary<TMetadataEntry, TPackageVersion, TMetadataEntry>((Func<TMetadataEntry, TPackageVersion>) (e => e.PackageIdentity.Version), (Func<TMetadataEntry, TMetadataEntry>) (e => e));
      HashSet<IPackageVersion> newUpstreamVersions = ((IEnumerable<IPackageVersion>) operationData.UpstreamEntries.Select<TMetadataEntry, TPackageVersion>((Func<TMetadataEntry, TPackageVersion>) (x => x.PackageIdentity.Version))).ToHashSet<IPackageVersion>(this.versionEqualityComparer);
      foreach (TMetadataEntry metadataEntry in dictionary1.Values.Where<TMetadataEntry>((Func<TMetadataEntry, bool>) (x => x.IsFromUpstream && !newUpstreamVersions.Contains((IPackageVersion) x.PackageIdentity.Version))).ToList<TMetadataEntry>())
      {
        List<IPackageFile> list = metadataEntry.PackageFiles.Where<IPackageFile>((Func<IPackageFile, bool>) (f => f.StorageId.IsLocal)).ToList<IPackageFile>();
        TPackageIdentity packageIdentity;
        if (!list.Any<IPackageFile>())
        {
          Dictionary<TPackageVersion, TMetadataEntry> dictionary2 = dictionary1;
          packageIdentity = metadataEntry.PackageIdentity;
          TPackageVersion version = packageIdentity.Version;
          dictionary2.Remove(version);
        }
        else if (list.Count != metadataEntry.PackageFiles.Count)
        {
          Dictionary<TPackageVersion, TMetadataEntry> dictionary3 = dictionary1;
          packageIdentity = metadataEntry.PackageIdentity;
          TPackageVersion version = packageIdentity.Version;
          TMetadataEntry withUpdatedFiles = metadataEntry.CreateEntryWithUpdatedFiles(list);
          dictionary3[version] = withUpdatedFiles;
        }
      }
      foreach (TMetadataEntry upstreamEntry in operationData.UpstreamEntries)
      {
        TPackageVersion version = upstreamEntry.PackageIdentity.Version;
        TMetadataEntry metadataEntry;
        if (dictionary1.TryGetValue(version, out metadataEntry))
        {
          Dictionary<string, IPackageFile> dictionary4 = metadataEntry.PackageFiles.Where<IPackageFile>((Func<IPackageFile, bool>) (f => f.StorageId.IsLocal)).ToList<IPackageFile>().ToDictionary<IPackageFile, string, IPackageFile>((Func<IPackageFile, string>) (f => f.Path), (Func<IPackageFile, IPackageFile>) (f => f), metadataEntry.PathEqualityComparer);
          foreach (IPackageFile packageFile in (IEnumerable<IPackageFile>) upstreamEntry.PackageFiles)
          {
            if (!dictionary4.ContainsKey(packageFile.Path))
              dictionary4[packageFile.Path] = packageFile;
          }
          dictionary1[version] = metadataEntry.CreateEntryWithUpdatedFiles(dictionary4.Values.ToList<IPackageFile>());
        }
        else
          dictionary1.Add(version, upstreamEntry);
      }
      List<TMetadataEntry> list1 = dictionary1.Values.Select<TMetadataEntry, TMetadataEntry>((Func<TMetadataEntry, TMetadataEntry>) (entry =>
      {
        if (!operationData.EntriesToUpdateWithTerrapinData.Keys.Any<TPackageVersion>((Func<TPackageVersion, bool>) (version => version.NormalizedVersion == entry.PackageIdentity.Version.NormalizedVersion)))
          return entry;
        TMetadataEntryWritable writeable = entry.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
        TerrapinIngestionValidationReason validationReason = operationData.EntriesToUpdateWithTerrapinData[entry.PackageIdentity.Version];
        if (validationReason == (TerrapinIngestionValidationReason) null)
        {
          ref TMetadataEntryWritable local = ref writeable;
          if ((object) default (TMetadataEntryWritable) == null)
          {
            TMetadataEntryWritable metadataEntryWritable = local;
            local = ref metadataEntryWritable;
          }
          DateTime? nullable = new DateTime?();
          local.QuarantineUntilDate = nullable;
        }
        else if (validationReason as TerrapinIngestionValidationReasonQuarantinePending != (TerrapinIngestionValidationReasonQuarantinePending) null)
        {
          ref TMetadataEntryWritable local = ref writeable;
          if ((object) default (TMetadataEntryWritable) == null)
          {
            TMetadataEntryWritable metadataEntryWritable = local;
            local = ref metadataEntryWritable;
          }
          DateTime? nullable = new DateTime?();
          local.QuarantineUntilDate = nullable;
        }
        else if (validationReason as TerrapinIngestionValidationReasonQuarantine != (TerrapinIngestionValidationReasonQuarantine) null)
        {
          ref TMetadataEntryWritable local = ref writeable;
          if ((object) default (TMetadataEntryWritable) == null)
          {
            TMetadataEntryWritable metadataEntryWritable = local;
            local = ref metadataEntryWritable;
          }
          DateTime? quarantinedUntilUtc = ((TerrapinIngestionValidationReasonQuarantine) validationReason).QuarantinedUntilUtc;
          local.QuarantineUntilDate = quarantinedUntilUtc;
        }
        else
        {
          ref TMetadataEntryWritable local = ref writeable;
          if ((object) default (TMetadataEntryWritable) == null)
          {
            TMetadataEntryWritable metadataEntryWritable = local;
            local = ref metadataEntryWritable;
          }
          DateTime? nullable = new DateTime?(DateTime.MaxValue);
          local.QuarantineUntilDate = nullable;
        }
        return (TMetadataEntry) writeable;
      })).ToList<TMetadataEntry>();
      list1.Sort((IComparer<TMetadataEntry>) new MetadataVersionComparer<TPackageIdentity, TMetadataEntry>(this.versionComparer));
      return new MetadataDocument<TMetadataEntry>(list1, (IMetadataDocumentProperties) new MetadataDocumentProperties((IPackageName) operationData.PackageName, operationData.UpstreamsConfigurationHash, new DateTime?(operationData.UpstreamsLastRefreshedUtc), currentDoc.Properties?.SourceChainMap, operationData.PackageNameMetadata));
    }
  }
}
