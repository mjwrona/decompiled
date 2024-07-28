// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiVersionMetadataDocumentSerializerFactory
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public class PyPiVersionMetadataDocumentSerializerFactory : 
    PyPiMetadataDocumentSerializerBase<IPyPiMetadataEntryWithRawMetadataWritable, IPyPiMetadataEntryWithRawMetadata, PyPiPackageFileWithRawMetadata>
  {
    public PyPiVersionMetadataDocumentSerializerFactory(
      ICache<string, object> telemetryCache,
      ITracerService tracerService)
      : base(telemetryCache, tracerService)
    {
    }

    protected override PyPiPackageFileWithRawMetadata CreateFile(
      JsonReader reader,
      string path,
      IStorageId storageId,
      IReadOnlyCollection<HashAndType> hashes,
      long size,
      DateTime dateAdded,
      PyPiDistType distType)
    {
      IReadOnlyDictionary<string, string[]> rawMetadata = this.ReadMetadata(reader);
      string deflatedBase64String = reader.ReadExpectedStringOrNullProperty("GpgSig");
      DeflateCompressibleBytes gpgSignature = !string.IsNullOrWhiteSpace(deflatedBase64String) ? DeflateCompressibleBytes.FromDeflatedBase64String(deflatedBase64String) : (DeflateCompressibleBytes) null;
      return new PyPiPackageFileWithRawMetadata(path, storageId, hashes, size, dateAdded, distType, rawMetadata, gpgSignature);
    }

    private IReadOnlyDictionary<string, string[]> ReadMetadata(JsonReader reader)
    {
      JsonSerializerSettings settings = new JsonSerializerSettings();
      settings.TypeNameHandling = TypeNameHandling.None;
      settings.CheckAdditionalContent = false;
      reader.ReadExpectedPropertyNameOnly("rawMetadata");
      reader.Read();
      return (IReadOnlyDictionary<string, string[]>) JsonSerializer.Create(settings).Deserialize<Dictionary<string, string[]>>(reader);
    }

    protected override IPyPiMetadataEntryWithRawMetadataWritable CreateEntry(
      PyPiPackageIdentity packageIdentity)
    {
      return (IPyPiMetadataEntryWithRawMetadataWritable) new PyPiMetadataEntryWithRawMetadata(packageIdentity);
    }

    protected override void WriteFileExtras(JsonWriter writer, PyPiPackageFileWithRawMetadata file)
    {
      writer.WritePropertyName("rawMetadata");
      JsonSerializer.Create().Serialize(writer, (object) file.RawMetadata);
      writer.WritePropertyName("GpgSig");
      writer.WriteValue(file.GpgSignature?.AsDeflatedBase64String());
    }
  }
}
