// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.UndefinedPartitionKeyComponent
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.IO;

namespace Microsoft.Azure.Documents.Routing
{
  internal sealed class UndefinedPartitionKeyComponent : IPartitionKeyComponent
  {
    public static readonly UndefinedPartitionKeyComponent Value = new UndefinedPartitionKeyComponent();

    internal UndefinedPartitionKeyComponent()
    {
    }

    public int CompareTo(IPartitionKeyComponent other)
    {
      if (!(other is UndefinedPartitionKeyComponent))
        throw new ArgumentException(nameof (other));
      return 0;
    }

    public int GetTypeOrdinal() => 0;

    public double GetHashValue() => 0.0;

    public override int GetHashCode() => 0;

    public void JsonEncode(JsonWriter writer)
    {
      writer.WriteStartObject();
      writer.WriteEndObject();
    }

    public object ToObject() => (object) Undefined.Value;

    public IPartitionKeyComponent Truncate() => (IPartitionKeyComponent) this;

    public void WriteForHashing(BinaryWriter writer) => writer.Write((byte) 0);

    public void WriteForHashingV2(BinaryWriter writer) => writer.Write((byte) 0);

    public void WriteForBinaryEncoding(BinaryWriter binaryWriter) => binaryWriter.Write((byte) 0);
  }
}
