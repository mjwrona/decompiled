// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ArtifactJsonConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  public class ArtifactJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (ArtifactDownloadInputBase).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override bool CanWrite => false;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      if (reader.TokenType != JsonToken.StartObject || !(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract jsonObjectContract))
        return existingValue;
      JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("ArtifactType");
      if (closestMatchProperty == null)
        return existingValue;
      JObject jobject = JObject.Load(reader);
      JToken jtoken;
      if (!jobject.TryGetValue(closestMatchProperty.PropertyName, StringComparison.OrdinalIgnoreCase, out jtoken) || jtoken.Type != JTokenType.String)
        return existingValue;
      string artifactType = (string) jtoken;
      ArtifactDownloadInputBase target;
      switch (artifactType)
      {
        case "Build":
          target = (ArtifactDownloadInputBase) new BuildArtifactDownloadInput();
          break;
        case "Git":
          target = (ArtifactDownloadInputBase) new GitArtifactDownloadInput();
          break;
        case "GitHub":
          target = (ArtifactDownloadInputBase) new GitHubArtifactDownloadInput();
          break;
        case "Jenkins":
          target = (ArtifactDownloadInputBase) new JenkinsArtifactDownloadInput();
          break;
        case "TFVC":
          target = (ArtifactDownloadInputBase) new TfvcArtifactDownloadInput();
          break;
        default:
          target = (ArtifactDownloadInputBase) new CustomArtifactDownloadInput(artifactType);
          break;
      }
      if (target != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, (object) target);
      }
      return (object) target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
