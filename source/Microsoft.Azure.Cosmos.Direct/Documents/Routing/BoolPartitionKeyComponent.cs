// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.BoolPartitionKeyComponent
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;
using System.IO;

namespace Microsoft.Azure.Documents.Routing
{
  internal sealed class BoolPartitionKeyComponent : IPartitionKeyComponent
  {
    private readonly bool value;

    public BoolPartitionKeyComponent(bool value) => this.value = value;

    public int CompareTo(IPartitionKeyComponent other) => other is BoolPartitionKeyComponent partitionKeyComponent ? Math.Sign((this.value ? 1 : 0) - (partitionKeyComponent.value ? 1 : 0)) : throw new ArgumentException(nameof (other));

    public int GetTypeOrdinal() => !this.value ? 2 : 3;

    public override int GetHashCode() => this.value.GetHashCode();

    public void JsonEncode(JsonWriter writer) => writer.WriteValue(this.value);

    public object ToObject() => (object) this.value;

    public IPartitionKeyComponent Truncate() => (IPartitionKeyComponent) this;

    public void WriteForHashing(BinaryWriter binaryWriter) => binaryWriter.Write(this.value ? (byte) 3 : (byte) 2);

    public void WriteForHashingV2(BinaryWriter binaryWriter) => binaryWriter.Write(this.value ? (byte) 3 : (byte) 2);

    public void WriteForBinaryEncoding(BinaryWriter binaryWriter) => binaryWriter.Write(this.value ? (byte) 3 : (byte) 2);
  }
}
