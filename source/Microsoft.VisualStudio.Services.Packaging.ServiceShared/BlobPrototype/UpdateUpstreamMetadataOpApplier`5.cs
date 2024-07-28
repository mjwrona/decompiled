// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.UpdateUpstreamMetadataOpApplier`5
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class UpdateUpstreamMetadataOpApplier<TPackageIdentity, TPackageName, TPackageVersion, TMetadataEntry, TMetadataEntryWritable> : 
    IMetadataDocumentOpApplier<MetadataDocument<TMetadataEntry>>
    where TPackageIdentity : IPackageIdentity<TPackageName, TPackageVersion>
    where TPackageName : IPackageName
    where TPackageVersion : class, IPackageVersion
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>, ICreateWriteable<TMetadataEntryWritable>
    where TMetadataEntryWritable : IMetadataEntryWriteable<TPackageIdentity>, TMetadataEntry
  {
    private readonly IComparer<IPackageVersion> versionComparer;
    private readonly IEqualityComparer<IPackageVersion> versionEqualityComparer;
    private readonly ITracerService tracerService;

    public UpdateUpstreamMetadataOpApplier(
      IComparer<IPackageVersion> versionComparer,
      IEqualityComparer<IPackageVersion> versionEqualityComparer,
      ITracerService tracerService)
    {
      this.versionComparer = versionComparer;
      this.versionEqualityComparer = versionEqualityComparer;
      this.tracerService = tracerService;
    }

    public MetadataDocument<TMetadataEntry> Apply(
      ICommitLogEntry commitLogEntry,
      MetadataDocument<TMetadataEntry> currentDoc)
    {
      IUpdateUpstreamMetadataOperationData<TPackageIdentity, TPackageName, TPackageVersion, TMetadataEntry> operationData = (IUpdateUpstreamMetadataOperationData<TPackageIdentity, TPackageName, TPackageVersion, TMetadataEntry>) commitLogEntry.CommitOperationData;
      List<TMetadataEntry> localEntries = currentDoc.Entries.Where<TMetadataEntry>((Func<TMetadataEntry, bool>) (x => x.IsLocal)).ToList<TMetadataEntry>();
      HashSet<TPackageVersion> localEntryVersions = new HashSet<TPackageVersion>(localEntries.Select<TMetadataEntry, TPackageVersion>((Func<TMetadataEntry, TPackageVersion>) (x => x.PackageIdentity.Version)), (IEqualityComparer<TPackageVersion>) this.versionEqualityComparer);
      List<TMetadataEntry> upstreamEntries = operationData.UpstreamEntries.Where<TMetadataEntry>((Func<TMetadataEntry, bool>) (x => !localEntryVersions.Contains(x.PackageIdentity.Version))).ToList<TMetadataEntry>();
      MetadataVersionComparer<TPackageIdentity, TMetadataEntry> metadataVersionComparer = new MetadataVersionComparer<TPackageIdentity, TMetadataEntry>(this.versionComparer);
      upstreamEntries.Sort((IComparer<TMetadataEntry>) metadataVersionComparer);
      List<TMetadataEntry> mergedSortedEntries = localEntries.Merge<TMetadataEntry>((IEnumerable<TMetadataEntry>) upstreamEntries, new Func<TMetadataEntry, TMetadataEntry, int>(metadataVersionComparer.Compare)).ToList<TMetadataEntry>();
      mergedSortedEntries = mergedSortedEntries.Select<TMetadataEntry, TMetadataEntry>((Func<TMetadataEntry, TMetadataEntry>) (entry =>
      {
        TerrapinIngestionValidationReason validationReason;
        if (!operationData.EntriesToUpdateWithTerrapinData.TryGetValue(entry.PackageIdentity.Version, out validationReason))
          return entry;
        TMetadataEntryWritable writeable = entry.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
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
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (Apply)))
      {
        tracerBlock.TraceConditionally(new string[1]
        {
          "Local entries"
        }, (Func<string>) (() => "Local package version entries: " + string.Join<TPackageVersion>(", ", localEntries.Select<TMetadataEntry, TPackageVersion>((Func<TMetadataEntry, TPackageVersion>) (x => x.PackageIdentity.Version)))));
        tracerBlock.TraceConditionally(new string[1]
        {
          "Upstream entries"
        }, (Func<string>) (() => "Upstream package version entries: " + string.Join<TPackageVersion>(", ", upstreamEntries.Select<TMetadataEntry, TPackageVersion>((Func<TMetadataEntry, TPackageVersion>) (x => x.PackageIdentity.Version)))));
        tracerBlock.TraceConditionally(new string[1]
        {
          "Merged sorted entries"
        }, (Func<string>) (() => "Local and upstream package version sorted entries: " + string.Join<TPackageVersion>(", ", mergedSortedEntries.Select<TMetadataEntry, TPackageVersion>((Func<TMetadataEntry, TPackageVersion>) (x => x.PackageIdentity.Version)))));
        return new MetadataDocument<TMetadataEntry>(mergedSortedEntries, (IMetadataDocumentProperties) new MetadataDocumentProperties((IPackageName) operationData.PackageName, operationData.UpstreamsConfigurationHash, new DateTime?(operationData.UpstreamsLastRefreshedUtc), currentDoc.Properties?.SourceChainMap, operationData.PackageNameMetadata));
      }
    }
  }
}
