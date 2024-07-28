// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.IJsonNavigator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Json
{
  internal interface IJsonNavigator
  {
    JsonSerializationFormat SerializationFormat { get; }

    IJsonNavigatorNode GetRootNode();

    JsonNodeType GetNodeType(IJsonNavigatorNode node);

    Number64 GetNumber64Value(IJsonNavigatorNode numberNode);

    bool TryGetBufferedStringValue(IJsonNavigatorNode stringNode, out Utf8Memory value);

    UtfAnyString GetStringValue(IJsonNavigatorNode stringNode);

    sbyte GetInt8Value(IJsonNavigatorNode numberNode);

    short GetInt16Value(IJsonNavigatorNode numberNode);

    int GetInt32Value(IJsonNavigatorNode numberNode);

    long GetInt64Value(IJsonNavigatorNode numberNode);

    float GetFloat32Value(IJsonNavigatorNode numberNode);

    double GetFloat64Value(IJsonNavigatorNode numberNode);

    uint GetUInt32Value(IJsonNavigatorNode numberNode);

    Guid GetGuidValue(IJsonNavigatorNode guidNode);

    ReadOnlyMemory<byte> GetBinaryValue(IJsonNavigatorNode binaryNode);

    bool TryGetBufferedBinaryValue(
      IJsonNavigatorNode binaryNode,
      out ReadOnlyMemory<byte> bufferedBinaryValue);

    int GetArrayItemCount(IJsonNavigatorNode arrayNode);

    IJsonNavigatorNode GetArrayItemAt(IJsonNavigatorNode arrayNode, int index);

    IEnumerable<IJsonNavigatorNode> GetArrayItems(IJsonNavigatorNode arrayNode);

    int GetObjectPropertyCount(IJsonNavigatorNode objectNode);

    bool TryGetObjectProperty(
      IJsonNavigatorNode objectNode,
      string propertyName,
      out ObjectProperty objectProperty);

    IEnumerable<ObjectProperty> GetObjectProperties(IJsonNavigatorNode objectNode);

    IJsonReader CreateReader(IJsonNavigatorNode jsonNavigatorNode);

    void WriteNode(IJsonNavigatorNode jsonNavigatorNode, IJsonWriter jsonWriter);
  }
}
