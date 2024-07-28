// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow
{
  [JsonConverter(typeof (SchemaId.SchemaIdConverter))]
  [DebuggerDisplay("{Id}")]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public readonly struct SchemaId : IEquatable<SchemaId>
  {
    public const int Size = 4;
    public static readonly SchemaId Invalid;

    public SchemaId(int id) => this.Id = id;

    public int Id { get; }

    [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Constructor")]
    public static explicit operator SchemaId(int id) => new SchemaId(id);

    [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Id property")]
    public static explicit operator int(SchemaId id) => id.Id;

    public static bool operator ==(SchemaId left, SchemaId right) => left.Equals(right);

    public static bool operator !=(SchemaId left, SchemaId right) => !left.Equals(right);

    public override bool Equals(object obj) => obj != null && obj is SchemaId other && this.Equals(other);

    public override int GetHashCode() => this.Id.GetHashCode();

    public bool Equals(SchemaId other) => this.Id == other.Id;

    public override string ToString() => this.Id.ToString();

    internal class SchemaIdConverter : JsonConverter
    {
      public override bool CanWrite => true;

      public override bool CanConvert(Type objectType) => typeof (SchemaId).IsAssignableFrom(objectType);

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        Contract.Requires(reader.TokenType == JsonToken.Integer);
        return (object) new SchemaId(checked ((int) (long) reader.Value));
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => writer.WriteValue((long) ((SchemaId) value).Id);
    }
  }
}
