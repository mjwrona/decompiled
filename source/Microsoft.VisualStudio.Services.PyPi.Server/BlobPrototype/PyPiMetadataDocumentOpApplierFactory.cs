// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.BlobPrototype.PyPiMetadataDocumentOpApplierFactory
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.Operations;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.BlobPrototype
{
  public class PyPiMetadataDocumentOpApplierFactory : 
    IMetadataDocumentOpApplierFactory<MetadataDocument<IPyPiMetadataEntry>>
  {
    private readonly Dictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntry>>> applierDictionary;

    public PyPiMetadataDocumentOpApplierFactory()
    {
      AddSourceChainOpApplier<PyPiPackageIdentity, IPyPiMetadataEntry> sourceChainOpApplier = new AddSourceChainOpApplier<PyPiPackageIdentity, IPyPiMetadataEntry>();
      ReverseVersionComparer<PyPiPackageVersion> reverseVersionComparer = new ReverseVersionComparer<PyPiPackageVersion>();
      this.applierDictionary = new Dictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntry>>>()
      {
        {
          typeof (UpdateUpstreamMetadataOperationData<PyPiPackageIdentity, PyPiPackageName, PyPiPackageVersion, IPyPiMetadataEntry>),
          (IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntry>>) new UpdateUpstreamMetadataWithFilesOpApplier<PyPiPackageIdentity, PyPiPackageName, PyPiPackageVersion, IPyPiMetadataEntry, IPyPiMetadataEntryWritable>((IComparer<IPackageVersion>) reverseVersionComparer, (IEqualityComparer<IPackageVersion>) reverseVersionComparer)
        },
        {
          typeof (PyPiAddOperationData),
          (IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntry>>) sourceChainOpApplier
        }
      };
    }

    public IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntry>> Get(
      ICommitOperationData operationData,
      bool throwIfNotFound = true)
    {
      IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntry>> documentOpApplier;
      if (this.applierDictionary.TryGetValue(operationData.GetType(), out documentOpApplier))
        return documentOpApplier;
      if (throwIfNotFound)
        throw new InvalidOperationException(operationData.GetType().Name);
      return (IMetadataDocumentOpApplier<MetadataDocument<IPyPiMetadataEntry>>) null;
    }
  }
}
