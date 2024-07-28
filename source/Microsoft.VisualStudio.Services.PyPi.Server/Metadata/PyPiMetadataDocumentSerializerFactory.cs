// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiMetadataDocumentSerializerFactory
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public class PyPiMetadataDocumentSerializerFactory : 
    PyPiMetadataDocumentSerializerBase<IPyPiMetadataEntryWritable, IPyPiMetadataEntry, PyPiPackageFile>
  {
    public PyPiMetadataDocumentSerializerFactory(
      ICache<string, object> telemetryCache,
      ITracerService tracerService)
      : base(telemetryCache, tracerService)
    {
    }

    protected override IPyPiMetadataEntryWritable CreateEntry(PyPiPackageIdentity packageIdentity) => (IPyPiMetadataEntryWritable) new PyPiMetadataEntry(packageIdentity);

    protected override void WriteFileExtras(JsonWriter writer, PyPiPackageFile file)
    {
    }

    protected override PyPiPackageFile CreateFile(
      JsonReader reader,
      string path,
      IStorageId storageId,
      IReadOnlyCollection<HashAndType> hashes,
      long size,
      DateTime dateAdded,
      PyPiDistType distType)
    {
      return new PyPiPackageFile(path, storageId, hashes, size, dateAdded, distType);
    }
  }
}
