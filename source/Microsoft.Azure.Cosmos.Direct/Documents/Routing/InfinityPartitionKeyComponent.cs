// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.InfinityPartitionKeyComponent
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;
using System.IO;

namespace Microsoft.Azure.Documents.Routing
{
  internal sealed class InfinityPartitionKeyComponent : IPartitionKeyComponent
  {
    public int CompareTo(IPartitionKeyComponent other)
    {
      if (!(other is InfinityPartitionKeyComponent))
        throw new ArgumentException(nameof (other));
      return 0;
    }

    public int GetTypeOrdinal() => (int) byte.MaxValue;

    public IPartitionKeyComponent Truncate() => throw new InvalidOperationException();

    public void WriteForHashing(BinaryWriter writer) => throw new InvalidOperationException();

    public void WriteForHashingV2(BinaryWriter writer) => throw new InvalidOperationException();

    public override int GetHashCode() => 0;

    public void JsonEncode(JsonWriter writer) => throw new NotImplementedException();

    public object ToObject() => throw new NotImplementedException();

    public void WriteForBinaryEncoding(BinaryWriter binaryWriter) => binaryWriter.Write(byte.MaxValue);
  }
}
