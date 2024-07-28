// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.EncryptionKeyWrapMetadata
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  public class EncryptionKeyWrapMetadata : IEquatable<EncryptionKeyWrapMetadata>
  {
    private EncryptionKeyWrapMetadata()
    {
    }

    public EncryptionKeyWrapMetadata(string type, string name, string value, string algorithm)
    {
      this.Type = type ?? throw new ArgumentNullException(nameof (type));
      this.Name = name ?? throw new ArgumentNullException(nameof (name));
      this.Value = value ?? throw new ArgumentNullException(nameof (value));
      this.Algorithm = algorithm ?? throw new ArgumentNullException(nameof (algorithm));
    }

    public EncryptionKeyWrapMetadata(EncryptionKeyWrapMetadata source)
      : this(source?.Type, source?.Name, source?.Value, source?.Algorithm)
    {
    }

    [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
    public string Type { get; private set; }

    [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; private set; }

    [JsonProperty(PropertyName = "value", NullValueHandling = NullValueHandling.Ignore)]
    public string Value { get; private set; }

    [JsonProperty(PropertyName = "algorithm", NullValueHandling = NullValueHandling.Ignore)]
    public string Algorithm { get; private set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as EncryptionKeyWrapMetadata);

    public override int GetHashCode() => (((1265339359 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Type)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Name)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Value)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Algorithm);

    public bool Equals(EncryptionKeyWrapMetadata other) => other != null && this.Type == other.Type && this.Name == other.Name && this.Value == other.Value && this.Algorithm == other.Algorithm && this.AdditionalProperties.EqualsTo(other.AdditionalProperties);
  }
}
