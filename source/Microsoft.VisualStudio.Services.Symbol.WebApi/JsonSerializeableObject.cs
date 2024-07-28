// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.JsonSerializeableObject
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  public abstract class JsonSerializeableObject
  {
    public static T FromJson<T>(string json) => JsonConvert.DeserializeObject<T>(json);

    public T Clone<T>() => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject((object) this));

    public string ToJson() => JsonConvert.SerializeObject((object) this, new JsonSerializerSettings()
    {
      ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
    });
  }
}
