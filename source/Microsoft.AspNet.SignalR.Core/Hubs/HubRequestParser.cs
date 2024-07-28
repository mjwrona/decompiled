// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubRequestParser
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.SignalR.Hubs
{
  internal class HubRequestParser : IHubRequestParser
  {
    private static readonly IJsonValue[] _emptyArgs = new IJsonValue[0];

    public HubRequest Parse(string data, JsonSerializer serializer)
    {
      HubRequestParser.HubInvocation deserializedData = serializer.Parse<HubRequestParser.HubInvocation>(data);
      return new HubRequest()
      {
        Hub = deserializedData.Hub,
        Method = deserializedData.Method,
        Id = deserializedData.Id,
        State = HubRequestParser.GetState(deserializedData),
        ParameterValues = deserializedData.Args != null ? (IJsonValue[]) ((IEnumerable<JRaw>) deserializedData.Args).Select<JRaw, JRawValue>((Func<JRaw, JRawValue>) (value => new JRawValue(value))).ToArray<JRawValue>() : HubRequestParser._emptyArgs
      };
    }

    private static IDictionary<string, object> GetState(
      HubRequestParser.HubInvocation deserializedData)
    {
      if (deserializedData.State == null)
        return (IDictionary<string, object>) new Dictionary<string, object>();
      string json = deserializedData.State.ToString();
      if (json.Length > 4096)
        throw new InvalidOperationException(Resources.Error_StateExceededMaximumLength);
      JsonSerializerSettings serializerSettings = JsonUtility.CreateDefaultSerializerSettings();
      serializerSettings.Converters.Add((JsonConverter) new SipHashBasedDictionaryConverter());
      return JsonSerializer.Create(serializerSettings).Parse<IDictionary<string, object>>(json);
    }

    private class HubInvocation
    {
      [JsonProperty("H")]
      public string Hub { get; set; }

      [JsonProperty("M")]
      public string Method { get; set; }

      [JsonProperty("I")]
      public string Id { get; set; }

      [JsonProperty("S")]
      public JRaw State { get; set; }

      [JsonProperty("A")]
      public JRaw[] Args { get; set; }
    }
  }
}
