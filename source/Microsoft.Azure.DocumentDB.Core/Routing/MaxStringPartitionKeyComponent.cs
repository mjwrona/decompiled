// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.MaxStringPartitionKeyComponent
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.IO;

namespace Microsoft.Azure.Documents.Routing
{
  internal sealed class MaxStringPartitionKeyComponent : IPartitionKeyComponent
  {
    public static readonly MaxStringPartitionKeyComponent Value = new MaxStringPartitionKeyComponent();

    private MaxStringPartitionKeyComponent()
    {
    }

    public int CompareTo(IPartitionKeyComponent other)
    {
      if (!(other is MaxStringPartitionKeyComponent))
        throw new ArgumentException(nameof (other));
      return 0;
    }

    public int GetTypeOrdinal() => 9;

    public override int GetHashCode() => 0;

    public IPartitionKeyComponent Truncate() => throw new InvalidOperationException();

    public void WriteForHashing(BinaryWriter writer) => throw new InvalidOperationException();

    public void WriteForHashingV2(BinaryWriter writer) => throw new InvalidOperationException();

    public void JsonEncode(JsonWriter writer) => PartitionKeyInternalJsonConverter.JsonEncode(this, writer);

    public object ToObject() => (object) MaxString.Value;

    public void WriteForBinaryEncoding(BinaryWriter binaryWriter) => binaryWriter.Write((byte) 9);
  }
}
