// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Operations.PyPiMetadataOpApplierFactory
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.PermanentDelete;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Operations
{
  public class PyPiMetadataOpApplierFactory : 
    IMetadataEntryOpApplierFactory<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>>
  {
    private readonly Dictionary<Type, IMetadataEntryOpApplier<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>>> applierDictionary;

    public PyPiMetadataOpApplierFactory() => this.applierDictionary = new Dictionary<Type, IMetadataEntryOpApplier<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>>>()
    {
      {
        typeof (PyPiAddOperationData),
        (IMetadataEntryOpApplier<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>>) new PyPiAddMetadataOpApplier()
      },
      {
        typeof (ViewOperationData),
        (IMetadataEntryOpApplier<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>>) new ViewMetadataOpApplier<IPyPiMetadataEntry, IPyPiMetadataEntryWritable>()
      },
      {
        typeof (DeleteOperationData),
        (IMetadataEntryOpApplier<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>>) new DeleteMetadataOpApplier<IPyPiMetadataEntry, IPyPiMetadataEntryWritable>()
      },
      {
        typeof (RestoreToFeedOperationData),
        (IMetadataEntryOpApplier<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>>) new RestoreToFeedMetadataOpApplier<IPyPiMetadataEntry, IPyPiMetadataEntryWritable, IRestoreToFeedOperationData>()
      },
      {
        typeof (PyPiPermanentDeleteOperationData),
        (IMetadataEntryOpApplier<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>>) new PermanentDeleteMetadataOpApplier<IPyPiMetadataEntry, IPyPiMetadataEntryWritable>()
      },
      {
        typeof (AddProblemPackageOperationData),
        (IMetadataEntryOpApplier<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>>) new NoChangeMetadataOpApplier<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>>()
      }
    };

    public IMetadataEntryOpApplier<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>> Get(
      ICommitOperationData operationData,
      bool throwIfNotFound = true)
    {
      IMetadataEntryOpApplier<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>> metadataEntryOpApplier;
      if (this.applierDictionary.TryGetValue(operationData.GetType(), out metadataEntryOpApplier))
        return metadataEntryOpApplier;
      if (throwIfNotFound)
        throw new InvalidOperationException(operationData.GetType().Name);
      return (IMetadataEntryOpApplier<IPyPiMetadataEntry, MetadataDocument<IPyPiMetadataEntry>>) null;
    }
  }
}
