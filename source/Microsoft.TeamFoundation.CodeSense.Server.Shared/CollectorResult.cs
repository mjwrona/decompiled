// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.CollectorResult
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class CollectorResult
  {
    public CollectorResult(string id, object data, string version)
    {
      this.Id = id;
      this.Version = version;
      this.Data = data;
    }

    [JsonConstructor]
    private CollectorResult()
    {
    }

    [JsonProperty]
    public string Id { get; private set; }

    [JsonProperty]
    public string Version { get; private set; }

    [JsonProperty]
    public object Data { get; set; }

    public T GetData<T>()
    {
      if (this.Data is T)
        return (T) this.Data;
      return this.Data is JToken ? ((JToken) this.Data).ToObject<T>() : throw new InvalidOperationException(string.Format("Data is not of type {0}", (object) typeof (T).FullName));
    }

    public object GetData(Type t)
    {
      if (t.IsAssignableFrom(this.Data.GetType()))
        return this.Data;
      return this.Data is JToken ? ((JToken) this.Data).ToObject(t) : throw new InvalidOperationException(string.Format("Data is not of type {0}", (object) t.FullName));
    }
  }
}
