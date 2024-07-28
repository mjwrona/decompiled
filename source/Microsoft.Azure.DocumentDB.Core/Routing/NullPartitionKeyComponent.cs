// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.NullPartitionKeyComponent
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
