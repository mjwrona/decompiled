// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Operations.PyPiVersionMetadataDocumentApplierFactory
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Operations
{
  public class PyPiVersionMetadataDocumentApplierFactory : 
    IMetadataDocumentOpApplierFactory<MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>
  {
    private readonly Dictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>> applierDictionary;

    public PyPiVersionMetadataDocumentApplierFactory() => this.applierDictionary = new Dictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>>()
    {
      {
        typeof (PyPiAddOperationData),
        (IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>) new AddSourceChainOpApplier<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>()
      }
    };

    public IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntryWithRawMetadata>> Get(
      ICommitOperationData operationData,
      bool throwIfNotFound = true)
    {
      IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntryWithRawMetadata>> documentOpApplier;
      if (this.applierDictionary.TryGetValue(operationData.GetType(), out documentOpApplier))
        return documentOpApplier;
      if (throwIfNotFound)
        throw new InvalidOperationException(operationData.GetType().Name);
      return (IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>) null;
    }
  }
}
