// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.Schema
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
  public sealed class Schema
  {
    private List<PartitionKey> partitionKeys;
    private List<PrimarySortKey> primaryKeys;
    private List<StaticKey> staticKeys;
    private List<Property> properties;

    public Schema()
    {
      this.Version = SchemaLanguageVersion.Unspecified;
      this.Type = TypeKind.Schema;
      this.properties = new List<Property>();
      this.partitionKeys = new List<PartitionKey>();
      this.primaryKeys = new List<PrimarySortKey>();
      this.staticKeys = new List<StaticKey>();
    }

    [DefaultValue(SchemaLanguageVersion.Unspecified)]
    [JsonProperty(PropertyName = "version", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public SchemaLanguageVersion Version { get; set; }

    [JsonProperty(PropertyName = "comment", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string Comment { get; set; }

    [JsonProperty(PropertyName = "name", Required = Required.Always)]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "id", Required = Required.Always)]
    public SchemaId SchemaId { get; set; }

    [JsonProperty(PropertyName = "baseName", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string BaseName { get; set; }

    [JsonProperty(PropertyName = "baseId", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public SchemaId BaseSchemaId { get; set; }

    [JsonProperty(PropertyName = "options")]
    public SchemaOptions Options { get; set; }

    [DefaultValue(TypeKind.Schema)]
    [JsonProperty(PropertyName = "type", Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Populate)]
    public TypeKind Type { get; set; }

    [JsonProperty(PropertyName = "properties")]
    public List<Property> Properties
    {
      get => this.properties;
      set => this.properties = value ?? new List<Property>();
    }

    [JsonProperty(PropertyName = "partitionkeys")]
    public List<PartitionKey> PartitionKeys
    {
      get => this.partitionKeys;
      set => this.partitionKeys = value ?? new List<PartitionKey>();
    }

    [JsonProperty(PropertyName = "primarykeys")]
    public List<PrimarySortKey> PrimaryKeys
    {
      get => this.primaryKeys;
      set => this.primaryKeys = value ?? new List<PrimarySortKey>();
    }

    [JsonProperty(PropertyName = "statickeys", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public List<StaticKey> StaticKeys
    {
      get => this.staticKeys;
      set => this.staticKeys = value ?? new List<StaticKey>();
    }

    public Layout Compile(Namespace ns)
    {
      Contract.Requires(ns != null);
      Contract.Requires(ns.Schemas.Contains(this));
      return LayoutCompiler.Compile(ns, this);
    }

    internal SchemaLanguageVersion GetEffectiveSdlVersion(Namespace ns) => this.Version != SchemaLanguageVersion.Unspecified ? this.Version : ns.GetEffectiveSdlVersion();
  }
}
