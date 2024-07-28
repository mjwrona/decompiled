// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Operations.PyPiVersionMetadataOpApplierFactory
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
  public class PyPiVersionMetadataOpApplierFactory : 
    IMetadataEntryOpApplierFactory<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>
  {
    private readonly Dictionary<Type, IMetadataEntryOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>> applierDictionary;

    public PyPiVersionMetadataOpApplierFactory() => this.applierDictionary = new Dictionary<Type, IMetadataEntryOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>>()
    {
      {
        typeof (PyPiAddOperationData),
        (IMetadataEntryOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>) new PyPiVersionMetadataAddOpApplier()
      },
      {
        typeof (ViewOperationData),
        (IMetadataEntryOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>) new ViewMetadataOpApplier<IPyPiMetadataEntryWithRawMetadata, IPyPiMetadataEntryWithRawMetadataWritable>()
      },
      {
        typeof (DeleteOperationData),
        (IMetadataEntryOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>) new DeleteMetadataOpApplier<IPyPiMetadataEntryWithRawMetadata, IPyPiMetadataEntryWithRawMetadataWritable>()
      },
      {
        typeof (RestoreToFeedOperationData),
        (IMetadataEntryOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>) new RestoreToFeedMetadataOpApplier<IPyPiMetadataEntryWithRawMetadata, IPyPiMetadataEntryWithRawMetadataWritable, IRestoreToFeedOperationData>()
      },
      {
        typeof (PyPiPermanentDeleteOperationData),
        (IMetadataEntryOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>) new PermanentDeleteMetadataOpApplier<IPyPiMetadataEntryWithRawMetadata, IPyPiMetadataEntryWithRawMetadataWritable>()
      },
      {
        typeof (AddProblemPackageOperationData),
        (IMetadataEntryOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>) new NoChangeMetadataOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>()
      }
    };

    public IMetadataEntryOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>> Get(
      ICommitOperationData operationData,
      bool throwIfNotFound = true)
    {
      IMetadataEntryOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>> metadataEntryOpApplier;
      if (this.applierDictionary.TryGetValue(operationData.GetType(), out metadataEntryOpApplier))
        return metadataEntryOpApplier;
      if (throwIfNotFound)
        throw new InvalidOperationException(operationData.GetType().Name);
      return (IMetadataEntryOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>) null;
    }
  }
}
