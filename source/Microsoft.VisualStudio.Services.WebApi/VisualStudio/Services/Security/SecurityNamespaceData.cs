// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.SecurityNamespaceData
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Security
{
  [DataContract]
  public sealed class SecurityNamespaceData
  {
    public SecurityNamespaceData()
    {
    }

    public SecurityNamespaceData(
      Guid aclStoreId,
      long oldSequenceId,
      long[] newSequenceId,
      Guid identityDomain,
      IEnumerable<RemoteBackingStoreAccessControlEntry> accessControlEntries,
      IEnumerable<string> noInheritTokens)
    {
      this.AclStoreId = aclStoreId;
      this.OldSequenceId = oldSequenceId;
      this.NewSequenceId = newSequenceId;
      this.IdentityDomain = identityDomain;
      this.AccessControlEntries = accessControlEntries;
      this.NoInheritTokens = noInheritTokens;
    }

    public bool IsDelta => this.OldSequenceId != -1L;

    [DataMember]
    public Guid AclStoreId { get; set; }

    [DataMember]
    public long OldSequenceId { get; set; }

    [DataMember]
    [JsonConverter(typeof (SecurityNamespaceData.PluralSequenceIdJsonConverter))]
    public long[] NewSequenceId { get; set; }

    [DataMember]
    public Guid IdentityDomain { get; set; }

    [DataMember]
    public IEnumerable<RemoteBackingStoreAccessControlEntry> AccessControlEntries { get; set; }

    [DataMember]
    public IEnumerable<string> NoInheritTokens { get; set; }

    private class PluralSequenceIdJsonConverter : VssSecureJsonConverter
    {
      public override bool CanConvert(Type objectType) => objectType == typeof (long[]);

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        if (JsonToken.Null == reader.TokenType)
          return (object) null;
        List<long> longList = new List<long>();
        if (reader.TokenType == JsonToken.StartArray)
        {
          reader.Read();
          while (reader.TokenType == JsonToken.Integer)
          {
            longList.Add((long) reader.Value);
            reader.Read();
          }
          if (reader.TokenType != JsonToken.EndArray)
            throw new JsonSerializationException();
        }
        else if (reader.TokenType == JsonToken.Integer)
          longList.Add((long) reader.Value);
        return (object) longList.ToArray();
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        base.WriteJson(writer, value, serializer);
        long[] numArray = (long[]) value;
        switch (numArray.Length)
        {
          case 0:
            throw new InvalidOperationException();
          case 1:
            writer.WriteValue(numArray[0]);
            break;
          default:
            writer.WriteStartArray();
            for (int index = 0; index < numArray.Length; ++index)
              writer.WriteValue(numArray[index]);
            writer.WriteEndArray();
            break;
        }
      }
    }
  }
}
