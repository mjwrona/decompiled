// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.Namespace
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  [JsonObject]
  public sealed class Namespace
  {
    private static readonly JsonSerializerSettings NamespaceParseSettings = new JsonSerializerSettings()
    {
      CheckAdditionalContent = true,
      NullValueHandling = NullValueHandling.Ignore,
      Formatting = Formatting.Indented
    };
    private List<EnumSchema> enums;
    private List<Schema> schemas;

    public Namespace()
    {
      this.Version = SchemaLanguageVersion.V2;
      this.enums = new List<EnumSchema>();
      this.schemas = new List<Schema>();
    }

    [DefaultValue(SchemaLanguageVersion.V2)]
    [JsonProperty(PropertyName = "version", DefaultValueHandling = DefaultValueHandling.Populate)]
    public SchemaLanguageVersion Version { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "comment", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string Comment { get; set; }

    [JsonProperty(PropertyName = "cppNamespace", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string CppNamespace { get; set; }

    [JsonProperty(PropertyName = "enums", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public List<EnumSchema> Enums
    {
      get => this.enums;
      set => this.enums = value ?? new List<EnumSchema>();
    }

    [JsonProperty(PropertyName = "schemas")]
    public List<Schema> Schemas
    {
      get => this.schemas;
      set => this.schemas = value ?? new List<Schema>();
    }

    public static Namespace Parse(string json)
    {
      Namespace ns = JsonConvert.DeserializeObject<Namespace>(json, Namespace.NamespaceParseSettings);
      SchemaValidator.Validate(ns);
      return ns;
    }

    public static string ToJson(Namespace n) => JsonConvert.SerializeObject((object) n, Namespace.NamespaceParseSettings);

    public static Result Read(ref RowBuffer row, out Namespace value)
    {
      Contract.Requires(row.Header.SchemaId == new SchemaId(2147473651));
      RowCursor scope = RowCursor.Create(ref row);
      return new NamespaceHybridRowSerializer().Read(ref row, ref scope, true, out value);
    }

    public Result Write(ref RowBuffer row)
    {
      Contract.Requires(row.Header.SchemaId == new SchemaId(2147473651));
      RowCursor scope = RowCursor.Create(ref row);
      return new NamespaceHybridRowSerializer().Write(ref row, ref scope, true, (TypeArgumentList) new SchemaId(2147473651), this);
    }

    internal SchemaLanguageVersion GetEffectiveSdlVersion() => this.Version == SchemaLanguageVersion.Unspecified ? SchemaLanguageVersion.V2 : this.Version;

    private bool ShouldSerializeEnums() => this.enums.Count > 0;
  }
}
