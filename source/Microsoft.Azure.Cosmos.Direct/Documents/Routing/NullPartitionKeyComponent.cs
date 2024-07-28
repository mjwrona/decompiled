// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.NullPartitionKeyComponent
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;
using System.IO;

namespace Microsoft.Azure.Documents.Routing
{
  internal sealed class NullPartitionKeyComponent : IPartitionKeyComponent
  {
    public static readonly NullPartitionKeyComponent Value = new NullPartitionKeyComponent();

    private NullPartitionKeyComponent()
    {
    }

    public int CompareTo(IPartitionKeyComponent other)
    {
      if (!(other is NullPartitionKeyComponent))
        throw new ArgumentException(nameof (other));
      return 0;
    }

    public int GetTypeOrdinal() => 1;

    public IPartitionKeyComponent Truncate() => (IPartitionKeyComponent) this;

    public void WriteForHashing(BinaryWriter writer) => writer.Write((byte) 1);

    public void WriteForHashingV2(BinaryWriter writer) => writer.Write((byte) 1);

    public void JsonEncode(JsonWriter writer) => writer.WriteValue((object) null);

    public object ToObject() => (object) null;

    public void WriteForBinaryEncoding(BinaryWriter binaryWriter) => binaryWriter.Write((byte) 1);
  }
}
