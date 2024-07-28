// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGet.NuGetJsonSerializerFactory
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Views;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGet
{
  public class NuGetJsonSerializerFactory : 
    IFactory<PackageNameQuery<INuGetMetadataEntry>, ISerializer<MetadataDocument<INuGetMetadataEntry>>>
  {
    private readonly Dictionary<string, PropertyDefinition<INuGetMetadataEntryWriteable>> nugetProperties;
    private readonly IFeatureFlagService featureFlagService;
    private readonly ICache<string, object> requestItemsAsCacheFacade;
    private readonly ITracerService tracerService;

    public NuGetJsonSerializerFactory(
      IFeatureFlagService featureFlagService,
      ICache<string, object> requestItemsAsCacheFacade,
      ITracerService tracerService)
    {
      this.featureFlagService = featureFlagService;
      this.requestItemsAsCacheFacade = requestItemsAsCacheFacade;
      this.tracerService = tracerService;
      this.nugetProperties = this.PopulateNuGetProperties();
    }

    private void ReadNuspecBytes(
      INuGetMetadataEntryWriteable entry,
      ICache<string, object> telemetryCache,
      string strVal)
    {
      bool flag = false;
      if (strVal.StartsWith("z:"))
      {
        strVal = strVal.Remove(0, 2);
        flag = true;
      }
      if (!telemetryCache.Has("Packaging.Properties.MetadataReadZip"))
      {
        telemetryCache.Set("Packaging.Properties.MetadataReadZip", (object) flag);
        telemetryCache.Set("Packaging.Properties.MetadataLengthSample", (object) strVal.Length);
      }
      entry.SetNuspecBytes(Convert.FromBase64String(strVal), flag);
    }

    private void WriteNuspecBytes(
      JsonWriter jsonWriter,
      ICache<string, object> telemetryCache,
      INuGetMetadataEntryWriteable entry)
    {
      byte[] numArray = entry.NuspecBytes;
      if (!entry.AreBytesCompressed)
        numArray = CompressionHelper.DeflateByteArray(numArray);
      if (!telemetryCache.Has("Packaging.Properties.MetadataWriteZip"))
        telemetryCache.Set("Packaging.Properties.MetadataWriteZip", (object) true);
      jsonWriter.WriteValue("z:" + Convert.ToBase64String(numArray));
    }

    private Dictionary<string, PropertyDefinition<INuGetMetadataEntryWriteable>> PopulateNuGetProperties()
    {
      Dictionary<string, PropertyDefinition<INuGetMetadataEntryWriteable>> properties = new Dictionary<string, PropertyDefinition<INuGetMetadataEntryWriteable>>();
      properties.Add("List", new PropertyDefinition<INuGetMetadataEntryWriteable>((Expression<Func<INuGetMetadataEntryWriteable, object>>) (e => (object) e.Listed), (Action<JsonWriter, ICache<string, object>, INuGetMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => w.WriteValue(e.Listed)), (Action<INuGetMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => e.Listed = (bool) val)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      properties.Add("MD", new PropertyDefinition<INuGetMetadataEntryWriteable>((Expression<Func<INuGetMetadataEntryWriteable, object>>) (e => e.NuspecBytes), (Action<JsonWriter, ICache<string, object>, INuGetMetadataEntryWriteable, IInternalMetadataDocumentProperties>) ((w, tel, e, dProps) => this.WriteNuspecBytes(w, tel, e)), (Action<INuGetMetadataEntryWriteable, MetadataDocumentProperties, ICache<string, object>, object>) ((e, dProps, tel, val) => this.ReadNuspecBytes(e, tel, (string) val)), skipSerializingCondition: NuGetJsonSerializerFactory.\u003C\u003EO.\u003C0\u003E__SkipOnDeleted ?? (NuGetJsonSerializerFactory.\u003C\u003EO.\u003C0\u003E__SkipOnDeleted = new Func<INuGetMetadataEntryWriteable, bool>(NuGetJsonSerializerFactory.SkipOnDeleted))));
      return properties;
    }

    private static bool SkipOnDeleted(INuGetMetadataEntryWriteable entry) => ((IMetadataEntryWritable) entry).DeletedDate.HasValue || ((IMetadataEntryWritable) entry).PermanentDeletedDate.HasValue;

    public ISerializer<MetadataDocument<INuGetMetadataEntry>> Get(
      PackageNameQuery<INuGetMetadataEntry> queryRequest)
    {
      StreamingPackageMetadataDocumentJsonConverter<VssNuGetPackageVersion, INuGetMetadataEntryWriteable, INuGetMetadataEntry> documentJsonConverter = new StreamingPackageMetadataDocumentJsonConverter<VssNuGetPackageVersion, INuGetMetadataEntryWriteable, INuGetMetadataEntry>(queryRequest.RequestData, (IConverter<FeedRequest<INuGetMetadataEntryWriteable>, INuGetMetadataEntryWriteable>) new LocalImplicitViewMetadataConverter<INuGetMetadataEntryWriteable>(), queryRequest.Options, (Func<IPackageName, VssNuGetPackageVersion, INuGetMetadataEntryWriteable>) ((packageName, packageVersion) => (INuGetMetadataEntryWriteable) new NuGetMetadataEntry(new VssNuGetPackageIdentity((VssNuGetPackageName) packageName, packageVersion))), (Func<string, IPackageName>) (nameString => (IPackageName) new VssNuGetPackageName(nameString)), (Func<string, VssNuGetPackageVersion>) (versionString => new VssNuGetPackageVersion(versionString)), (IEnumerable<KeyValuePair<string, PropertyDefinition<INuGetMetadataEntryWriteable>>>) this.nugetProperties, this.requestItemsAsCacheFacade, this.tracerService);
      JsonSerializerSettings settings = new JsonSerializerSettings();
      settings.Converters.Add((JsonConverter) documentJsonConverter);
      return (ISerializer<MetadataDocument<INuGetMetadataEntry>>) new JsonSerializer<MetadataDocument<INuGetMetadataEntry>>(settings);
    }
  }
}
