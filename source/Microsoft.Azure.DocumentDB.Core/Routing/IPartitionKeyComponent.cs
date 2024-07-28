// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.IPartitionKeyComponent
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System.IO;

namespace Microsoft.Azure.Documents.Routing
{
  internal interface IPartitionKeyComponent
  {
    int CompareTo(IPartitionKeyComponent other);

    int GetTypeOrdinal();

    void JsonEncode(JsonWriter writer);

    object ToObject();

    void WriteForHashing(BinaryWriter binaryWriter);

    void WriteForHashingV2(BinaryWriter binaryWriter);

    void WriteForBinaryEncoding(BinaryWriter binaryWriter);

    IPartitionKeyComponent Truncate();
  }
}
