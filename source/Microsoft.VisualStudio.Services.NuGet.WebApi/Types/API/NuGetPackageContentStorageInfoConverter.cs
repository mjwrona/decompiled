// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.NuGetPackageContentStorageInfoConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API
{
  public class NuGetPackageContentStorageInfoConverter : VssSecureJsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JObject json = JObject.Load(reader);
      string storageType = (string) json["storageType"];
      PackageContentStorageType result;
      if (!Enum.TryParse<PackageContentStorageType>(storageType, true, out result))
        result = PackageContentStorageType.Unknown;
      switch (result)
      {
        case PackageContentStorageType.Blob:
          return (object) json.ToObject<NuGetBlobPackageContentStorageInfo>();
        case PackageContentStorageType.Drop:
          return (object) json.ToObject<NuGetDropPackageContentStorageInfo>();
        default:
          return (object) new NuGetUnknownTypePackageContentStorageInfo(storageType, json);
      }
    }

    public override bool CanConvert(Type objectType) => objectType == typeof (NuGetPackageContentStorageInfo);

    public override bool CanWrite => false;
  }
}
