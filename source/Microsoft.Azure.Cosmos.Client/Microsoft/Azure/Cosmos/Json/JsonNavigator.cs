// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.JsonNavigator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Azure.Cosmos.Json
{
  internal abstract class JsonNavigator : IJsonNavigator
  {
    public abstract JsonSerializationFormat SerializationFormat { get; }

    public static IJsonNavigator Create(ReadOnlyMemory<byte> buffer)
    {
      if (buffer.IsEmpty)
        throw new ArgumentOutOfRangeException("buffer can not be empty.");
      return buffer.Span[0] != (byte) 128 ? (IJsonNavigator) new JsonNavigator.JsonTextNavigator(buffer) : (IJsonNavigator) new JsonNavigator.JsonBinaryNavigator(buffer);
    }

    public abstract IJsonNavigatorNode GetRootNode();

    public abstract JsonNodeType GetNodeType(IJsonNavigatorNode node);

    public abstract Number64 GetNumber64Value(IJsonNavigatorNode numberNode);

    public abstract bool TryGetBufferedStringValue(
      IJsonNavigatorNode stringNode,
      out Utf8Memory bufferedStringValue);

    public abstract UtfAnyString GetStringValue(IJsonNavigatorNode stringNode);

    public abstract sbyte GetInt8Value(IJsonNavigatorNode numberNode);

    public abstract short GetInt16Value(IJsonNavigatorNode numberNode);

    public abstract int GetInt32Value(IJsonNavigatorNode numberNode);

    public abstract long GetInt64Value(IJsonNavigatorNode numberNode);

    public abstract float GetFloat32Value(IJsonNavigatorNode numberNode);

    public abstract double GetFloat64Value(IJsonNavigatorNode numberNode);

    public abstract uint GetUInt32Value(IJsonNavigatorNode numberNode);

    public abstract Guid GetGuidValue(IJsonNavigatorNode guidNode);

    public abstract ReadOnlyMemory<byte> GetBinaryValue(IJsonNavigatorNode binaryNode);

    public abstract bool TryGetBufferedBinaryValue(
      IJsonNavigatorNode binaryNode,
      out ReadOnlyMemory<byte> bufferedBinaryValue);

    public abstract int GetArrayItemCount(IJsonNavigatorNode arrayNode);

    public abstract IJsonNavigatorNode GetArrayItemAt(IJsonNavigatorNode arrayNode, int index);

    public abstract IEnumerable<IJsonNavigatorNode> GetArrayItems(IJsonNavigatorNode arrayNode);

    public abstract int GetObjectPropertyCount(IJsonNavigatorNode objectNode);

    public abstract bool TryGetObjectProperty(
      IJsonNavigatorNode objectNode,
      string propertyName,
      out ObjectProperty objectProperty);

    public abstract IEnumerable<ObjectProperty> GetObjectProperties(IJsonNavigatorNode objectNode);

    public virtual void WriteNode(IJsonNavigatorNode jsonNavigatorNode, IJsonWriter jsonWriter)
    {
      JsonNodeType nodeType = this.GetNodeType(jsonNavigatorNode);
      switch (nodeType)
      {
        case JsonNodeType.Null:
          jsonWriter.WriteNullValue();
          break;
        case JsonNodeType.False:
          jsonWriter.WriteBoolValue(false);
          break;
        case JsonNodeType.True:
          jsonWriter.WriteBoolValue(true);
          break;
        case JsonNodeType.Number64:
          Number64 number64Value = this.GetNumber64Value(jsonNavigatorNode);
          jsonWriter.WriteNumber64Value(number64Value);
          break;
        case JsonNodeType.String:
        case JsonNodeType.FieldName:
          bool flag = nodeType == JsonNodeType.FieldName;
          Utf8Memory bufferedStringValue;
          if (this.TryGetBufferedStringValue(jsonNavigatorNode, out bufferedStringValue))
          {
            if (flag)
            {
              jsonWriter.WriteFieldName(bufferedStringValue.Span);
              break;
            }
            jsonWriter.WriteStringValue(bufferedStringValue.Span);
            break;
          }
          string fieldName = UtfAnyString.op_Implicit(this.GetStringValue(jsonNavigatorNode));
          if (flag)
          {
            jsonWriter.WriteFieldName(fieldName);
            break;
          }
          jsonWriter.WriteStringValue(fieldName);
          break;
        case JsonNodeType.Array:
          jsonWriter.WriteArrayStart();
          foreach (IJsonNavigatorNode arrayItem in this.GetArrayItems(jsonNavigatorNode))
            this.WriteNode(arrayItem, jsonWriter);
          jsonWriter.WriteArrayEnd();
          break;
        case JsonNodeType.Object:
          jsonWriter.WriteObjectStart();
          foreach (ObjectProperty objectProperty in this.GetObjectProperties(jsonNavigatorNode))
          {
            this.WriteNode(objectProperty.NameNode, jsonWriter);
            this.WriteNode(objectProperty.ValueNode, jsonWriter);
          }
          jsonWriter.WriteObjectEnd();
          break;
        case JsonNodeType.Int8:
          sbyte int8Value = this.GetInt8Value(jsonNavigatorNode);
          jsonWriter.WriteInt8Value(int8Value);
          break;
        case JsonNodeType.Int16:
          short int16Value = this.GetInt16Value(jsonNavigatorNode);
          jsonWriter.WriteInt16Value(int16Value);
          break;
        case JsonNodeType.Int32:
          int int32Value = this.GetInt32Value(jsonNavigatorNode);
          jsonWriter.WriteInt32Value(int32Value);
          break;
        case JsonNodeType.Int64:
          long int64Value = this.GetInt64Value(jsonNavigatorNode);
          jsonWriter.WriteInt64Value(int64Value);
          break;
        case JsonNodeType.UInt32:
          uint uint32Value = this.GetUInt32Value(jsonNavigatorNode);
          jsonWriter.WriteUInt32Value(uint32Value);
          break;
        case JsonNodeType.Float32:
          float float32Value = this.GetFloat32Value(jsonNavigatorNode);
          jsonWriter.WriteFloat32Value(float32Value);
          break;
        case JsonNodeType.Float64:
          double float64Value = this.GetFloat64Value(jsonNavigatorNode);
          jsonWriter.WriteFloat64Value(float64Value);
          break;
        case JsonNodeType.Binary:
          ReadOnlyMemory<byte> binaryValue = this.GetBinaryValue(jsonNavigatorNode);
          jsonWriter.WriteBinaryValue(binaryValue.Span);
          break;
        case JsonNodeType.Guid:
          Guid guidValue = this.GetGuidValue(jsonNavigatorNode);
          jsonWriter.WriteGuidValue(guidValue);
          break;
        default:
          throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}.", (object) "JsonNodeType", (object) nodeType));
      }
    }

    public abstract IJsonReader CreateReader(IJsonNavigatorNode jsonNavigatorNode);

    private sealed class JsonBinaryNavigator : JsonNavigator
    {
      private readonly ReadOnlyMemory<byte> rootBuffer;
      private readonly IJsonNavigatorNode rootNode;

      public JsonBinaryNavigator(ReadOnlyMemory<byte> buffer)
      {
        if (buffer.Length < 2)
          throw new ArgumentException("buffer must have at least two byte.");
        this.rootBuffer = buffer.Span[0] == (byte) 128 ? buffer : throw new ArgumentNullException("buffer must be binary encoded.");
        buffer = buffer.Slice(1);
        int valueLength = JsonBinaryEncoding.GetValueLength(buffer.Span);
        if (buffer.Length < valueLength)
          throw new ArgumentException("buffer is shorter than the length prefix.");
        buffer = buffer.Slice(0, valueLength);
        this.rootNode = (IJsonNavigatorNode) new JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode(buffer, JsonBinaryEncoding.NodeTypes.Lookup[(int) buffer.Span[0]]);
      }

      public override JsonSerializationFormat SerializationFormat => JsonSerializationFormat.Binary;

      public override IJsonNavigatorNode GetRootNode() => this.rootNode;

      public override JsonNodeType GetNodeType(IJsonNavigatorNode node) => node is JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode binaryNavigatorNode ? binaryNavigatorNode.JsonNodeType : throw new ArgumentException("node must be a BinaryNavigatorNode");

      public override Number64 GetNumber64Value(IJsonNavigatorNode numberNode) => JsonBinaryEncoding.GetNumberValue(JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Number64, numberNode).Span);

      public override bool TryGetBufferedStringValue(
        IJsonNavigatorNode stringNode,
        out Utf8Memory value)
      {
        return JsonBinaryEncoding.TryGetBufferedStringValue(this.rootBuffer, JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.String, stringNode), out value);
      }

      public override UtfAnyString GetStringValue(IJsonNavigatorNode stringNode) => Utf8String.op_Implicit(JsonBinaryEncoding.GetUtf8StringValue(this.rootBuffer, JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.String, stringNode)));

      public override sbyte GetInt8Value(IJsonNavigatorNode numberNode) => JsonBinaryEncoding.GetInt8Value(JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Int8, numberNode).Span);

      public override short GetInt16Value(IJsonNavigatorNode numberNode) => JsonBinaryEncoding.GetInt16Value(JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Int16, numberNode).Span);

      public override int GetInt32Value(IJsonNavigatorNode numberNode) => JsonBinaryEncoding.GetInt32Value(JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Int32, numberNode).Span);

      public override long GetInt64Value(IJsonNavigatorNode numberNode) => JsonBinaryEncoding.GetInt64Value(JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Int64, numberNode).Span);

      public override float GetFloat32Value(IJsonNavigatorNode numberNode) => JsonBinaryEncoding.GetFloat32Value(JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Float32, numberNode).Span);

      public override double GetFloat64Value(IJsonNavigatorNode numberNode) => JsonBinaryEncoding.GetFloat64Value(JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Float64, numberNode).Span);

      public override uint GetUInt32Value(IJsonNavigatorNode numberNode) => JsonBinaryEncoding.GetUInt32Value(JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.UInt32, numberNode).Span);

      public override Guid GetGuidValue(IJsonNavigatorNode guidNode) => JsonBinaryEncoding.GetGuidValue(JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Guid, guidNode).Span);

      public override ReadOnlyMemory<byte> GetBinaryValue(IJsonNavigatorNode binaryNode)
      {
        ReadOnlyMemory<byte> bufferedBinaryValue;
        if (!this.TryGetBufferedBinaryValue(binaryNode, out bufferedBinaryValue))
          throw new JsonInvalidTokenException();
        return bufferedBinaryValue;
      }

      public override bool TryGetBufferedBinaryValue(
        IJsonNavigatorNode binaryNode,
        out ReadOnlyMemory<byte> bufferedBinaryValue)
      {
        ReadOnlyMemory<byte> nodeOfType = JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Binary, binaryNode);
        bufferedBinaryValue = JsonBinaryEncoding.GetBinaryValue(nodeOfType);
        return true;
      }

      public override int GetArrayItemCount(IJsonNavigatorNode arrayNode)
      {
        ReadOnlyMemory<byte> nodeOfType = JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Array, arrayNode);
        byte typeMarker = nodeOfType.Span[0];
        int firstValueOffset = JsonBinaryEncoding.GetFirstValueOffset(typeMarker);
        long num;
        switch (typeMarker)
        {
          case 224:
            num = 0L;
            break;
          case 225:
            num = 1L;
            break;
          case 226:
          case 227:
          case 228:
            num = (long) JsonNavigator.JsonBinaryNavigator.GetValueCount(nodeOfType.Slice(firstValueOffset).Span);
            break;
          case 229:
            num = (long) MemoryMarshal.Read<byte>(nodeOfType.Slice(2).Span);
            break;
          case 230:
            num = (long) MemoryMarshal.Read<ushort>(nodeOfType.Slice(3).Span);
            break;
          case 231:
            num = (long) MemoryMarshal.Read<uint>(nodeOfType.Slice(5).Span);
            break;
          default:
            throw new InvalidOperationException(string.Format("Unexpected array type marker: {0}", (object) typeMarker));
        }
        return num <= (long) int.MaxValue ? (int) num : throw new InvalidOperationException("count can not be more than int.MaxValue");
      }

      public override IJsonNavigatorNode GetArrayItemAt(IJsonNavigatorNode arrayNode, int index)
      {
        ReadOnlyMemory<byte> nodeOfType = JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Array, arrayNode);
        long index1 = index >= 0 ? (long) index : throw new IndexOutOfRangeException();
        ReadOnlyMemory<byte> buffer;
        ref ReadOnlyMemory<byte> local = ref buffer;
        if (!JsonNavigator.JsonBinaryNavigator.TryGetValueAt(nodeOfType, index1, out local))
          throw new IndexOutOfRangeException(string.Format("Tried to access index: {0} in an array.", (object) index));
        return (IJsonNavigatorNode) new JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode(buffer, JsonBinaryEncoding.NodeTypes.Lookup[(int) buffer.Span[0]]);
      }

      public override IEnumerable<IJsonNavigatorNode> GetArrayItems(IJsonNavigatorNode arrayNode) => this.GetArrayItemsInternal(JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Array, arrayNode)).Select<JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode, IJsonNavigatorNode>((Func<JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode, IJsonNavigatorNode>) (node => (IJsonNavigatorNode) node));

      private IEnumerable<JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode> GetArrayItemsInternal(
        ReadOnlyMemory<byte> buffer)
      {
        return JsonBinaryEncoding.Enumerator.GetArrayItems(buffer).Select<ReadOnlyMemory<byte>, JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode>((Func<ReadOnlyMemory<byte>, JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode>) (arrayItem => new JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode(arrayItem, JsonBinaryEncoding.NodeTypes.Lookup[(int) arrayItem.Span[0]])));
      }

      public override int GetObjectPropertyCount(IJsonNavigatorNode objectNode)
      {
        ReadOnlyMemory<byte> nodeOfType = JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Object, objectNode);
        byte typeMarker = nodeOfType.Span[0];
        int firstValueOffset = JsonBinaryEncoding.GetFirstValueOffset(typeMarker);
        long num1;
        switch (typeMarker)
        {
          case 232:
            num1 = 0L;
            break;
          case 233:
            num1 = 2L;
            break;
          case 234:
          case 235:
          case 236:
            num1 = (long) JsonNavigator.JsonBinaryNavigator.GetValueCount(nodeOfType.Slice(firstValueOffset).Span);
            break;
          case 237:
            num1 = (long) MemoryMarshal.Read<byte>(nodeOfType.Slice(2).Span);
            break;
          case 238:
            num1 = (long) MemoryMarshal.Read<ushort>(nodeOfType.Slice(3).Span);
            break;
          case 239:
            num1 = (long) MemoryMarshal.Read<uint>(nodeOfType.Slice(5).Span);
            break;
          default:
            throw new InvalidOperationException(string.Format("Unexpected object type marker: {0}", (object) typeMarker));
        }
        long num2 = num1 / 2L;
        return num2 <= (long) int.MaxValue ? (int) num2 : throw new InvalidOperationException("count can not be more than int.MaxValue");
      }

      public override bool TryGetObjectProperty(
        IJsonNavigatorNode objectNode,
        string propertyName,
        out ObjectProperty objectProperty)
      {
        JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Object, objectNode);
        Utf8Span utf8Span = Utf8Span.TranscodeUtf16(propertyName);
        foreach (ObjectProperty objectProperty1 in this.GetObjectProperties(objectNode))
        {
          Utf8Memory bufferedStringValue;
          if (this.TryGetBufferedStringValue(objectProperty1.NameNode, out bufferedStringValue))
          {
            if (((Utf8Span) ref utf8Span).Equals(bufferedStringValue.Span))
            {
              objectProperty = objectProperty1;
              return true;
            }
          }
          else if (UtfAnyString.op_Equality(this.GetStringValue(objectProperty1.NameNode), propertyName))
          {
            objectProperty = objectProperty1;
            return true;
          }
        }
        objectProperty = new ObjectProperty();
        return false;
      }

      public override IEnumerable<ObjectProperty> GetObjectProperties(IJsonNavigatorNode objectNode) => this.GetObjectPropertiesInternal(JsonNavigator.JsonBinaryNavigator.GetNodeOfType(JsonNodeType.Object, objectNode)).Select<JsonNavigator.JsonBinaryNavigator.ObjectPropertyInternal, ObjectProperty>((Func<JsonNavigator.JsonBinaryNavigator.ObjectPropertyInternal, ObjectProperty>) (objectPropertyInternal => new ObjectProperty((IJsonNavigatorNode) objectPropertyInternal.NameNode, (IJsonNavigatorNode) objectPropertyInternal.ValueNode)));

      private IEnumerable<JsonNavigator.JsonBinaryNavigator.ObjectPropertyInternal> GetObjectPropertiesInternal(
        ReadOnlyMemory<byte> buffer)
      {
        return JsonBinaryEncoding.Enumerator.GetObjectProperties(buffer).Select<JsonBinaryEncoding.Enumerator.ObjectProperty, JsonNavigator.JsonBinaryNavigator.ObjectPropertyInternal>((Func<JsonBinaryEncoding.Enumerator.ObjectProperty, JsonNavigator.JsonBinaryNavigator.ObjectPropertyInternal>) (property => new JsonNavigator.JsonBinaryNavigator.ObjectPropertyInternal(new JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode(property.Name, JsonNodeType.FieldName), new JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode(property.Value, JsonBinaryEncoding.NodeTypes.Lookup[(int) property.Value.Span[0]]))));
      }

      public override IJsonReader CreateReader(IJsonNavigatorNode jsonNavigatorNode)
      {
        if (!(jsonNavigatorNode is JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode binaryNavigatorNode))
          throw new ArgumentException("jsonNavigatorNode must be a BinaryNavigatorNode");
        ArraySegment<byte> segment;
        if (!MemoryMarshal.TryGetArray<byte>(binaryNavigatorNode.Buffer, out segment))
          throw new InvalidOperationException("Failed to get segment");
        return JsonReader.CreateBinaryFromOffset(this.rootBuffer, segment.Offset);
      }

      private static int GetValueCount(ReadOnlySpan<byte> node)
      {
        int valueCount = 0;
        int valueLength;
        for (; !node.IsEmpty; node = node.Slice(valueLength))
        {
          ++valueCount;
          valueLength = JsonBinaryEncoding.GetValueLength(node);
        }
        return valueCount;
      }

      private static bool TryGetValueAt(
        ReadOnlyMemory<byte> arrayNode,
        long index,
        out ReadOnlyMemory<byte> arrayItem)
      {
        if (index > (long) int.MaxValue)
        {
          arrayItem = new ReadOnlyMemory<byte>();
          return false;
        }
        IEnumerable<ReadOnlyMemory<byte>> source = JsonBinaryEncoding.Enumerator.GetArrayItems(arrayNode).Skip<ReadOnlyMemory<byte>>((int) index);
        if (!source.Any<ReadOnlyMemory<byte>>())
        {
          arrayItem = new ReadOnlyMemory<byte>();
          return false;
        }
        arrayItem = source.First<ReadOnlyMemory<byte>>();
        return true;
      }

      private static ReadOnlyMemory<byte> GetNodeOfType(
        JsonNodeType expected,
        IJsonNavigatorNode node)
      {
        if (node == null)
          throw new ArgumentNullException(nameof (node));
        ReadOnlyMemory<byte> nodeOfType = node is JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode binaryNavigatorNode ? binaryNavigatorNode.Buffer : throw new ArgumentException("node must be a BinaryNavigatorNode");
        if (nodeOfType.IsEmpty)
          throw new ArgumentException("Node must not be empty.");
        if (JsonBinaryEncoding.NodeTypes.Lookup[(int) nodeOfType.Span[0]] != expected)
          throw new ArgumentException(string.Format("Node needs to be of type {0}.", (object) expected));
        return nodeOfType;
      }

      public override void WriteNode(IJsonNavigatorNode jsonNavigatorNode, IJsonWriter jsonWriter)
      {
        if (!(jsonNavigatorNode is JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode binaryNavigatorNode))
          throw new ArgumentOutOfRangeException("Expected jsonNavigatorNode to be a BinaryNavigatorNode.");
        if (this.SerializationFormat == jsonWriter.SerializationFormat)
        {
          bool isFieldName = binaryNavigatorNode.JsonNodeType == JsonNodeType.FieldName;
          if (!(jsonWriter is IJsonBinaryWriterExtensions writerExtensions))
            throw new InvalidOperationException("Expected writer to implement: IJsonBinaryWriterExtensions.");
          if (!(this.rootNode is JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode _))
            throw new InvalidOperationException("Expected rootNode to be a BinaryNavigatorNode.");
          writerExtensions.WriteRawJsonValue(this.rootBuffer, binaryNavigatorNode.Buffer, jsonNavigatorNode == this.rootNode, isFieldName);
        }
        else
          this.WriteToInternal(binaryNavigatorNode, jsonWriter);
      }

      private void WriteToInternal(
        JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode binaryNavigatorNode,
        IJsonWriter jsonWriter)
      {
        ReadOnlyMemory<byte> buffer = binaryNavigatorNode.Buffer;
        JsonNodeType jsonNodeType = binaryNavigatorNode.JsonNodeType;
        switch (jsonNodeType)
        {
          case JsonNodeType.Null:
            jsonWriter.WriteNullValue();
            break;
          case JsonNodeType.False:
            jsonWriter.WriteBoolValue(false);
            break;
          case JsonNodeType.True:
            jsonWriter.WriteBoolValue(true);
            break;
          case JsonNodeType.Number64:
            Number64 numberValue = JsonBinaryEncoding.GetNumberValue(buffer.Span);
            jsonWriter.WriteNumber64Value(numberValue);
            break;
          case JsonNodeType.String:
          case JsonNodeType.FieldName:
            bool flag = binaryNavigatorNode.JsonNodeType == JsonNodeType.FieldName;
            Utf8Memory utf8Memory;
            if (JsonBinaryEncoding.TryGetBufferedStringValue(this.rootBuffer, buffer, out utf8Memory))
            {
              if (flag)
              {
                jsonWriter.WriteFieldName(utf8Memory.Span);
                break;
              }
              jsonWriter.WriteStringValue(utf8Memory.Span);
              break;
            }
            string stringValue = JsonBinaryEncoding.GetStringValue(this.rootBuffer, buffer);
            if (flag)
            {
              jsonWriter.WriteFieldName(stringValue);
              break;
            }
            jsonWriter.WriteStringValue(stringValue);
            break;
          case JsonNodeType.Array:
            jsonWriter.WriteArrayStart();
            foreach (JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode binaryNavigatorNode1 in this.GetArrayItemsInternal(buffer))
              this.WriteToInternal(binaryNavigatorNode1, jsonWriter);
            jsonWriter.WriteArrayEnd();
            break;
          case JsonNodeType.Object:
            jsonWriter.WriteObjectStart();
            foreach (JsonNavigator.JsonBinaryNavigator.ObjectPropertyInternal propertyInternal in this.GetObjectPropertiesInternal(buffer))
            {
              this.WriteToInternal(propertyInternal.NameNode, jsonWriter);
              this.WriteToInternal(propertyInternal.ValueNode, jsonWriter);
            }
            jsonWriter.WriteObjectEnd();
            break;
          case JsonNodeType.Int8:
            sbyte int8Value = JsonBinaryEncoding.GetInt8Value(buffer.Span);
            jsonWriter.WriteInt8Value(int8Value);
            break;
          case JsonNodeType.Int16:
            short int16Value = JsonBinaryEncoding.GetInt16Value(buffer.Span);
            jsonWriter.WriteInt16Value(int16Value);
            break;
          case JsonNodeType.Int32:
            int int32Value = JsonBinaryEncoding.GetInt32Value(buffer.Span);
            jsonWriter.WriteInt32Value(int32Value);
            break;
          case JsonNodeType.Int64:
            long int64Value = JsonBinaryEncoding.GetInt64Value(buffer.Span);
            jsonWriter.WriteInt64Value(int64Value);
            break;
          case JsonNodeType.UInt32:
            uint uint32Value = JsonBinaryEncoding.GetUInt32Value(buffer.Span);
            jsonWriter.WriteUInt32Value(uint32Value);
            break;
          case JsonNodeType.Float32:
            float float32Value = JsonBinaryEncoding.GetFloat32Value(buffer.Span);
            jsonWriter.WriteFloat32Value(float32Value);
            break;
          case JsonNodeType.Float64:
            double float64Value = JsonBinaryEncoding.GetFloat64Value(buffer.Span);
            jsonWriter.WriteFloat64Value(float64Value);
            break;
          case JsonNodeType.Binary:
            ReadOnlyMemory<byte> binaryValue = JsonBinaryEncoding.GetBinaryValue(buffer);
            jsonWriter.WriteBinaryValue(binaryValue.Span);
            break;
          case JsonNodeType.Guid:
            Guid guidValue = JsonBinaryEncoding.GetGuidValue(buffer.Span);
            jsonWriter.WriteGuidValue(guidValue);
            break;
          default:
            throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}.", (object) "JsonNodeType", (object) jsonNodeType));
        }
      }

      private readonly struct BinaryNavigatorNode : IJsonNavigatorNode
      {
        public BinaryNavigatorNode(ReadOnlyMemory<byte> buffer, JsonNodeType jsonNodeType)
        {
          this.Buffer = buffer;
          this.JsonNodeType = jsonNodeType;
        }

        public ReadOnlyMemory<byte> Buffer { get; }

        public JsonNodeType JsonNodeType { get; }
      }

      private readonly struct ObjectPropertyInternal
      {
        public ObjectPropertyInternal(
          JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode nameNode,
          JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode valueNode)
        {
          this.NameNode = nameNode;
          this.ValueNode = valueNode;
        }

        public JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode NameNode { get; }

        public JsonNavigator.JsonBinaryNavigator.BinaryNavigatorNode ValueNode { get; }
      }
    }

    private sealed class JsonTextNavigator : JsonNavigator
    {
      private static readonly Utf8Memory ReverseSoldius = Utf8Memory.Create("\\");
      private readonly JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode rootNode;

      public JsonTextNavigator(ReadOnlyMemory<byte> buffer)
      {
        ReadOnlySpan<byte> readOnlySpan = !buffer.IsEmpty ? buffer.Span : throw new ArgumentOutOfRangeException("buffer can not be empty.");
        int num1 = (int) readOnlySpan[0];
        readOnlySpan = buffer.Span;
        byte num2 = readOnlySpan[buffer.Span.Length - 1];
        bool flag1 = num1 == 123 && num2 == (byte) 125;
        bool flag2 = num1 == 91 && num2 == (byte) 93;
        JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode textNavigatorNode;
        if (flag1 | flag2)
        {
          Lazy<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode> lazyNode = new Lazy<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode>(new Func<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode>(CreateRootNode));
          textNavigatorNode = !flag2 ? (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) new JsonNavigator.JsonTextNavigator.LazyObjectNode(lazyNode, buffer) : (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) new JsonNavigator.JsonTextNavigator.LazyArrayNode(lazyNode, buffer);
        }
        else
          textNavigatorNode = CreateRootNode();
        this.rootNode = textNavigatorNode;

        JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode CreateRootNode() => JsonReader.Create(buffer) is IJsonTextReaderPrivateImplementation jsonTextReader ? JsonNavigator.JsonTextNavigator.Parser.Parse(jsonTextReader) : throw new InvalidOperationException("jsonTextReader needs to implement IJsonTextReaderPrivateImplementation.");
      }

      public override JsonSerializationFormat SerializationFormat => JsonSerializationFormat.Text;

      public override IJsonNavigatorNode GetRootNode() => (IJsonNavigatorNode) this.rootNode;

      public override JsonNodeType GetNodeType(IJsonNavigatorNode node) => node is JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode textNavigatorNode ? textNavigatorNode.Type : throw new ArgumentException("node must actually be a text node.");

      public override Number64 GetNumber64Value(IJsonNavigatorNode node)
      {
        if (!(node is JsonNavigator.JsonTextNavigator.NumberNode numberNode))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "NumberNode"));
        return JsonTextParser.GetNumberValue(numberNode.BufferedToken.Span);
      }

      public override bool TryGetBufferedStringValue(IJsonNavigatorNode node, out Utf8Memory value)
      {
        if (!(node is JsonNavigator.JsonTextNavigator.StringNodeBase stringNodeBase))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "StringNodeBase"));
        Utf8Span span = stringNodeBase.BufferedValue.Span;
        if (((Utf8Span) ref span).Contains(JsonNavigator.JsonTextNavigator.ReverseSoldius.Span))
        {
          value = new Utf8Memory();
          return false;
        }
        value = stringNodeBase.BufferedValue.Slice(1, stringNodeBase.BufferedValue.Length - 2);
        return true;
      }

      public override UtfAnyString GetStringValue(IJsonNavigatorNode node) => node is JsonNavigator.JsonTextNavigator.StringNodeBase stringNodeBase ? Utf8String.op_Implicit(JsonTextParser.GetStringValue(stringNodeBase.BufferedValue)) : throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "StringNodeBase"));

      public override sbyte GetInt8Value(IJsonNavigatorNode node)
      {
        if (!(node is JsonNavigator.JsonTextNavigator.Int8Node int8Node))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "Int8Node"));
        return JsonTextParser.GetInt8Value(int8Node.BufferedToken.Span);
      }

      public override short GetInt16Value(IJsonNavigatorNode node)
      {
        if (!(node is JsonNavigator.JsonTextNavigator.Int16Node int16Node))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "Int16Node"));
        return JsonTextParser.GetInt16Value(int16Node.BufferedToken.Span);
      }

      public override int GetInt32Value(IJsonNavigatorNode node)
      {
        if (!(node is JsonNavigator.JsonTextNavigator.Int32Node int32Node))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "Int32Node"));
        return JsonTextParser.GetInt32Value(int32Node.BufferedToken.Span);
      }

      public override long GetInt64Value(IJsonNavigatorNode node)
      {
        if (!(node is JsonNavigator.JsonTextNavigator.Int64Node int64Node))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "Int64Node"));
        return JsonTextParser.GetInt64Value(int64Node.BufferedToken.Span);
      }

      public override float GetFloat32Value(IJsonNavigatorNode node)
      {
        if (!(node is JsonNavigator.JsonTextNavigator.Float32Node float32Node))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "Float32Node"));
        return JsonTextParser.GetFloat32Value(float32Node.BufferedToken.Span);
      }

      public override double GetFloat64Value(IJsonNavigatorNode node)
      {
        if (!(node is JsonNavigator.JsonTextNavigator.Float64Node float64Node))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "Float64Node"));
        return JsonTextParser.GetFloat64Value(float64Node.BufferedToken.Span);
      }

      public override uint GetUInt32Value(IJsonNavigatorNode node)
      {
        if (!(node is JsonNavigator.JsonTextNavigator.UInt32Node uint32Node))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "UInt32Node"));
        return JsonTextParser.GetUInt32Value(uint32Node.BufferedToken.Span);
      }

      public override Guid GetGuidValue(IJsonNavigatorNode node)
      {
        if (!(node is JsonNavigator.JsonTextNavigator.GuidNode guidNode))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "GuidNode"));
        return JsonTextParser.GetGuidValue(guidNode.BufferedToken.Span);
      }

      public override ReadOnlyMemory<byte> GetBinaryValue(IJsonNavigatorNode node)
      {
        if (!(node is JsonNavigator.JsonTextNavigator.BinaryNode binaryNode))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "BinaryNode"));
        return JsonTextParser.GetBinaryValue(binaryNode.BufferedToken.Span);
      }

      public override bool TryGetBufferedBinaryValue(
        IJsonNavigatorNode binaryNode,
        out ReadOnlyMemory<byte> bufferedBinaryValue)
      {
        bufferedBinaryValue = new ReadOnlyMemory<byte>();
        return false;
      }

      public override int GetArrayItemCount(IJsonNavigatorNode node)
      {
        if (node is JsonNavigator.JsonTextNavigator.LazyNode lazyNode)
          node = (IJsonNavigatorNode) lazyNode.Value;
        if (!(node is JsonNavigator.JsonTextNavigator.ArrayNode arrayNode))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "ArrayNode"));
        return arrayNode.Items.Count;
      }

      public override IJsonNavigatorNode GetArrayItemAt(IJsonNavigatorNode node, int index)
      {
        if (node is JsonNavigator.JsonTextNavigator.LazyNode lazyNode)
          node = (IJsonNavigatorNode) lazyNode.Value;
        if (!(node is JsonNavigator.JsonTextNavigator.ArrayNode arrayNode))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "ArrayNode"));
        return (IJsonNavigatorNode) arrayNode.Items[index];
      }

      public override IEnumerable<IJsonNavigatorNode> GetArrayItems(IJsonNavigatorNode node)
      {
        if (node is JsonNavigator.JsonTextNavigator.LazyNode lazyNode)
          node = (IJsonNavigatorNode) lazyNode.Value;
        return node is JsonNavigator.JsonTextNavigator.ArrayNode arrayNode ? (IEnumerable<IJsonNavigatorNode>) arrayNode.Items : throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "ArrayNode"));
      }

      public override int GetObjectPropertyCount(IJsonNavigatorNode node)
      {
        if (node is JsonNavigator.JsonTextNavigator.LazyNode lazyNode)
          node = (IJsonNavigatorNode) lazyNode.Value;
        if (!(node is JsonNavigator.JsonTextNavigator.ObjectNode objectNode))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "ObjectNode"));
        return objectNode.Properties.Count;
      }

      public override bool TryGetObjectProperty(
        IJsonNavigatorNode node,
        string propertyName,
        out ObjectProperty objectProperty)
      {
        if (node is JsonNavigator.JsonTextNavigator.LazyNode lazyNode)
          node = (IJsonNavigatorNode) lazyNode.Value;
        if (!(node is JsonNavigator.JsonTextNavigator.ObjectNode objectNode))
          throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "ObjectNode"));
        Utf8Memory utf8Memory = Utf8Memory.Create(propertyName);
        foreach (ObjectProperty property in (IEnumerable<ObjectProperty>) objectNode.Properties)
        {
          Utf8Memory bufferedStringValue;
          if (!this.TryGetBufferedStringValue(property.NameNode, out bufferedStringValue))
            throw new InvalidOperationException("Failed to get property name buffered value.");
          if (Utf8Span.op_Equality(utf8Memory.Span, bufferedStringValue.Span))
          {
            objectProperty = property;
            return true;
          }
        }
        objectProperty = new ObjectProperty();
        return false;
      }

      public override IEnumerable<ObjectProperty> GetObjectProperties(IJsonNavigatorNode node)
      {
        if (node is JsonNavigator.JsonTextNavigator.LazyNode lazyNode)
          node = (IJsonNavigatorNode) lazyNode.Value;
        return node is JsonNavigator.JsonTextNavigator.ObjectNode objectNode ? (IEnumerable<ObjectProperty>) objectNode.Properties : throw new ArgumentException(string.Format("{0} was not of type: {1}.", (object) node, (object) "ObjectNode"));
      }

      public override void WriteNode(IJsonNavigatorNode jsonNavigatorNode, IJsonWriter jsonWriter)
      {
        if (!(jsonNavigatorNode is JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode jsonTextNavigatorNode))
          throw new ArgumentOutOfRangeException("Expected jsonNavigatorNode to be a JsonTextNavigatorNode.");
        if (this.SerializationFormat == jsonWriter.SerializationFormat && jsonWriter is IJsonTextWriterExtensions writerExtensions)
        {
          bool isFieldName = jsonTextNavigatorNode.Type == JsonNodeType.FieldName;
          writerExtensions.WriteRawJsonValue(JsonNavigator.JsonTextNavigator.GetNodeBuffer(jsonTextNavigatorNode), isFieldName);
        }
        else
          base.WriteNode(jsonNavigatorNode, jsonWriter);
      }

      public override IJsonReader CreateReader(IJsonNavigatorNode jsonNavigatorNode) => jsonNavigatorNode is JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode jsonTextNavigatorNode ? JsonReader.Create(JsonSerializationFormat.Text, JsonNavigator.JsonTextNavigator.GetNodeBuffer(jsonTextNavigatorNode)) : throw new ArgumentException("jsonNavigatorNode must be a JsonTextNavigatorNode.");

      private static ReadOnlyMemory<byte> GetNodeBuffer(
        JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode jsonTextNavigatorNode)
      {
        switch (jsonTextNavigatorNode)
        {
          case JsonNavigator.JsonTextNavigator.LazyNode lazyNode:
            return lazyNode.BufferedValue;
          case JsonNavigator.JsonTextNavigator.ArrayNode arrayNode:
            return arrayNode.BufferedValue;
          case JsonNavigator.JsonTextNavigator.FalseNode _:
            return JsonNavigator.JsonTextNavigator.SingletonBuffers.False;
          case JsonNavigator.JsonTextNavigator.StringNodeBase stringNodeBase:
            return stringNodeBase.BufferedValue.Memory;
          case JsonNavigator.JsonTextNavigator.NullNode _:
            return JsonNavigator.JsonTextNavigator.SingletonBuffers.Null;
          case JsonNavigator.JsonTextNavigator.NumberNode numberNode:
            return numberNode.BufferedToken;
          case JsonNavigator.JsonTextNavigator.ObjectNode objectNode:
            return objectNode.BufferedValue;
          case JsonNavigator.JsonTextNavigator.TrueNode _:
            return JsonNavigator.JsonTextNavigator.SingletonBuffers.True;
          case JsonNavigator.JsonTextNavigator.GuidNode guidNode:
            return guidNode.BufferedToken;
          case JsonNavigator.JsonTextNavigator.BinaryNode binaryNode:
            return binaryNode.BufferedToken;
          case JsonNavigator.JsonTextNavigator.IntegerNode integerNode:
            return integerNode.BufferedToken;
          case JsonNavigator.JsonTextNavigator.FloatNode floatNode:
            return floatNode.BufferedToken;
          default:
            throw new ArgumentOutOfRangeException(string.Format("Unknown {0} type: {1}.", (object) "JsonTextNavigatorNode", (object) jsonTextNavigatorNode.GetType()));
        }
      }

      private static class SingletonBuffers
      {
        public static readonly ReadOnlyMemory<byte> True = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("true");
        public static readonly ReadOnlyMemory<byte> False = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("false");
        public static readonly ReadOnlyMemory<byte> Null = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("null");
      }

      private static class Parser
      {
        public static JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode Parse(
          IJsonTextReaderPrivateImplementation jsonTextReader)
        {
          JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode textNavigatorNode = jsonTextReader.Read() ? JsonNavigator.JsonTextNavigator.Parser.ParseNode(jsonTextReader) : throw new InvalidOperationException("Failed to read from reader");
          if (!jsonTextReader.Read())
            return textNavigatorNode;
          throw new ArgumentException("Did not fully parse json");
        }

        private static JsonNavigator.JsonTextNavigator.ArrayNode ParseArrayNode(
          IJsonTextReaderPrivateImplementation jsonTextReader)
        {
          List<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode> items = new List<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode>();
          ArraySegment<byte> segment1;
          if (!MemoryMarshal.TryGetArray<byte>(jsonTextReader.GetBufferedJsonToken().Memory, out segment1))
            throw new InvalidOperationException("Failed to get startArrayArraySegment.");
          jsonTextReader.Read();
          while (jsonTextReader.CurrentTokenType != JsonTokenType.EndArray)
            items.Add(JsonNavigator.JsonTextNavigator.Parser.ParseNode(jsonTextReader));
          ArraySegment<byte> segment2;
          if (!MemoryMarshal.TryGetArray<byte>(jsonTextReader.GetBufferedJsonToken().Memory, out segment2))
            throw new InvalidOperationException("Failed to get endArrayArraySegment.");
          jsonTextReader.Read();
          ReadOnlyMemory<byte> bufferedValue = ((ReadOnlyMemory<byte>) segment1.Array).Slice(segment1.Offset, segment2.Offset - segment1.Offset + 1);
          return JsonNavigator.JsonTextNavigator.ArrayNode.Create((IReadOnlyList<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode>) items, bufferedValue);
        }

        private static JsonNavigator.JsonTextNavigator.ObjectNode ParseObjectNode(
          IJsonTextReaderPrivateImplementation jsonTextReader)
        {
          List<ObjectProperty> properties = new List<ObjectProperty>();
          ArraySegment<byte> segment1;
          if (!MemoryMarshal.TryGetArray<byte>(jsonTextReader.GetBufferedJsonToken().Memory, out segment1))
            throw new InvalidOperationException("Failed to get startObjectArraySegment.");
          jsonTextReader.Read();
          while (jsonTextReader.CurrentTokenType != JsonTokenType.EndObject)
          {
            ObjectProperty propertyNode = JsonNavigator.JsonTextNavigator.Parser.ParsePropertyNode(jsonTextReader);
            properties.Add(propertyNode);
          }
          ArraySegment<byte> segment2;
          if (!MemoryMarshal.TryGetArray<byte>(jsonTextReader.GetBufferedJsonToken().Memory, out segment2))
            throw new InvalidOperationException("Failed to get endObjectArraySegment.");
          jsonTextReader.Read();
          ReadOnlyMemory<byte> bufferedValue = ((ReadOnlyMemory<byte>) segment1.Array).Slice(segment1.Offset, segment2.Offset - segment1.Offset + 1);
          return JsonNavigator.JsonTextNavigator.ObjectNode.Create((IReadOnlyList<ObjectProperty>) properties, bufferedValue);
        }

        private static JsonNavigator.JsonTextNavigator.StringNode ParseStringNode(
          IJsonTextReaderPrivateImplementation jsonTextReader)
        {
          JsonNavigator.JsonTextNavigator.StringNode stringNode = JsonNavigator.JsonTextNavigator.StringNode.Create(jsonTextReader.GetBufferedJsonToken());
          jsonTextReader.Read();
          return stringNode;
        }

        private static JsonNavigator.JsonTextNavigator.NumberNode ParseNumberNode(
          IJsonTextReaderPrivateImplementation jsonTextReader)
        {
          JsonNavigator.JsonTextNavigator.NumberNode numberNode = JsonNavigator.JsonTextNavigator.NumberNode.Create(jsonTextReader.GetBufferedJsonToken().Memory);
          jsonTextReader.Read();
          return numberNode;
        }

        private static JsonNavigator.JsonTextNavigator.IntegerNode ParseIntegerNode(
          IJsonTextReaderPrivateImplementation jsonTextReader,
          JsonTokenType jsonTokenType)
        {
          ReadOnlyMemory<byte> memory = jsonTextReader.GetBufferedJsonToken().Memory;
          JsonNavigator.JsonTextNavigator.IntegerNode integerNode1;
          switch (jsonTokenType)
          {
            case JsonTokenType.Int8:
              integerNode1 = (JsonNavigator.JsonTextNavigator.IntegerNode) JsonNavigator.JsonTextNavigator.Int8Node.Create(memory);
              break;
            case JsonTokenType.Int16:
              integerNode1 = (JsonNavigator.JsonTextNavigator.IntegerNode) JsonNavigator.JsonTextNavigator.Int16Node.Create(memory);
              break;
            case JsonTokenType.Int32:
              integerNode1 = (JsonNavigator.JsonTextNavigator.IntegerNode) JsonNavigator.JsonTextNavigator.Int32Node.Create(memory);
              break;
            case JsonTokenType.Int64:
              integerNode1 = (JsonNavigator.JsonTextNavigator.IntegerNode) JsonNavigator.JsonTextNavigator.Int64Node.Create(memory);
              break;
            case JsonTokenType.UInt32:
              integerNode1 = (JsonNavigator.JsonTextNavigator.IntegerNode) JsonNavigator.JsonTextNavigator.UInt32Node.Create(memory);
              break;
            default:
              throw new ArgumentException(string.Format("Unknown {0}: {1}", (object) "JsonTokenType", (object) jsonTokenType));
          }
          JsonNavigator.JsonTextNavigator.IntegerNode integerNode2 = integerNode1;
          jsonTextReader.Read();
          return integerNode2;
        }

        private static JsonNavigator.JsonTextNavigator.FloatNode ParseFloatNode(
          IJsonTextReaderPrivateImplementation jsonTextReader,
          JsonTokenType jsonTokenType)
        {
          ReadOnlyMemory<byte> memory = jsonTextReader.GetBufferedJsonToken().Memory;
          JsonNavigator.JsonTextNavigator.FloatNode floatNode1;
          if (jsonTokenType != JsonTokenType.Float32)
          {
            if (jsonTokenType != JsonTokenType.Float64)
              throw new ArgumentException(string.Format("Unknown {0}: {1}", (object) "JsonTokenType", (object) jsonTokenType));
            floatNode1 = (JsonNavigator.JsonTextNavigator.FloatNode) JsonNavigator.JsonTextNavigator.Float64Node.Create(memory);
          }
          else
            floatNode1 = (JsonNavigator.JsonTextNavigator.FloatNode) JsonNavigator.JsonTextNavigator.Float32Node.Create(memory);
          JsonNavigator.JsonTextNavigator.FloatNode floatNode2 = floatNode1;
          jsonTextReader.Read();
          return floatNode2;
        }

        private static JsonNavigator.JsonTextNavigator.TrueNode ParseTrueNode(
          IJsonReader jsonTextReader)
        {
          jsonTextReader.Read();
          return JsonNavigator.JsonTextNavigator.TrueNode.Create();
        }

        private static JsonNavigator.JsonTextNavigator.FalseNode ParseFalseNode(
          IJsonReader jsonTextReader)
        {
          jsonTextReader.Read();
          return JsonNavigator.JsonTextNavigator.FalseNode.Create();
        }

        private static JsonNavigator.JsonTextNavigator.NullNode ParseNullNode(
          IJsonReader jsonTextReader)
        {
          jsonTextReader.Read();
          return JsonNavigator.JsonTextNavigator.NullNode.Create();
        }

        private static ObjectProperty ParsePropertyNode(
          IJsonTextReaderPrivateImplementation jsonTextReader)
        {
          JsonNavigator.JsonTextNavigator.FieldNameNode nameNode = JsonNavigator.JsonTextNavigator.FieldNameNode.Create(Utf8Memory.UnsafeCreateNoValidation(jsonTextReader.GetBufferedJsonToken().Memory));
          jsonTextReader.Read();
          JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode node = JsonNavigator.JsonTextNavigator.Parser.ParseNode(jsonTextReader);
          return new ObjectProperty((IJsonNavigatorNode) nameNode, (IJsonNavigatorNode) node);
        }

        private static JsonNavigator.JsonTextNavigator.GuidNode ParseGuidNode(
          IJsonTextReaderPrivateImplementation jsonTextReader)
        {
          JsonNavigator.JsonTextNavigator.GuidNode guidNode = JsonNavigator.JsonTextNavigator.GuidNode.Create(jsonTextReader.GetBufferedJsonToken().Memory);
          jsonTextReader.Read();
          return guidNode;
        }

        private static JsonNavigator.JsonTextNavigator.BinaryNode ParseBinaryNode(
          IJsonTextReaderPrivateImplementation jsonTextReader)
        {
          JsonNavigator.JsonTextNavigator.BinaryNode binaryNode = JsonNavigator.JsonTextNavigator.BinaryNode.Create(jsonTextReader.GetBufferedJsonToken().Memory);
          jsonTextReader.Read();
          return binaryNode;
        }

        private static JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode ParseNode(
          IJsonTextReaderPrivateImplementation jsonTextReader)
        {
          switch (jsonTextReader.CurrentTokenType)
          {
            case JsonTokenType.BeginArray:
              return (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) JsonNavigator.JsonTextNavigator.Parser.ParseArrayNode(jsonTextReader);
            case JsonTokenType.BeginObject:
              return (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) JsonNavigator.JsonTextNavigator.Parser.ParseObjectNode(jsonTextReader);
            case JsonTokenType.String:
              return (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) JsonNavigator.JsonTextNavigator.Parser.ParseStringNode(jsonTextReader);
            case JsonTokenType.Number:
              return (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) JsonNavigator.JsonTextNavigator.Parser.ParseNumberNode(jsonTextReader);
            case JsonTokenType.True:
              return (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) JsonNavigator.JsonTextNavigator.Parser.ParseTrueNode((IJsonReader) jsonTextReader);
            case JsonTokenType.False:
              return (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) JsonNavigator.JsonTextNavigator.Parser.ParseFalseNode((IJsonReader) jsonTextReader);
            case JsonTokenType.Null:
              return (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) JsonNavigator.JsonTextNavigator.Parser.ParseNullNode((IJsonReader) jsonTextReader);
            case JsonTokenType.Int8:
            case JsonTokenType.Int16:
            case JsonTokenType.Int32:
            case JsonTokenType.Int64:
            case JsonTokenType.UInt32:
              return (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) JsonNavigator.JsonTextNavigator.Parser.ParseIntegerNode(jsonTextReader, jsonTextReader.CurrentTokenType);
            case JsonTokenType.Float32:
            case JsonTokenType.Float64:
              return (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) JsonNavigator.JsonTextNavigator.Parser.ParseFloatNode(jsonTextReader, jsonTextReader.CurrentTokenType);
            case JsonTokenType.Guid:
              return (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) JsonNavigator.JsonTextNavigator.Parser.ParseGuidNode(jsonTextReader);
            case JsonTokenType.Binary:
              return (JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode) JsonNavigator.JsonTextNavigator.Parser.ParseBinaryNode(jsonTextReader);
            default:
              throw new JsonInvalidTokenException();
          }
        }
      }

      private abstract class JsonTextNavigatorNode : IJsonNavigatorNode
      {
        public abstract JsonNodeType Type { get; }
      }

      private sealed class ArrayNode : JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode
      {
        private ArrayNode(
          IReadOnlyList<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode> items,
          ReadOnlyMemory<byte> bufferedValue)
        {
          this.Items = items;
          this.BufferedValue = bufferedValue;
        }

        public IReadOnlyList<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode> Items { get; }

        public ReadOnlyMemory<byte> BufferedValue { get; }

        public override JsonNodeType Type => JsonNodeType.Array;

        public static JsonNavigator.JsonTextNavigator.ArrayNode Create(
          IReadOnlyList<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode> items,
          ReadOnlyMemory<byte> bufferedValue)
        {
          return new JsonNavigator.JsonTextNavigator.ArrayNode(items, bufferedValue);
        }
      }

      private sealed class FalseNode : JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode
      {
        private static readonly JsonNavigator.JsonTextNavigator.FalseNode Instance = new JsonNavigator.JsonTextNavigator.FalseNode();

        private FalseNode()
        {
        }

        public override JsonNodeType Type => JsonNodeType.False;

        public static JsonNavigator.JsonTextNavigator.FalseNode Create() => JsonNavigator.JsonTextNavigator.FalseNode.Instance;
      }

      private sealed class FieldNameNode : JsonNavigator.JsonTextNavigator.StringNodeBase
      {
        private static readonly JsonNavigator.JsonTextNavigator.FieldNameNode Empty = new JsonNavigator.JsonTextNavigator.FieldNameNode(Utf8Memory.Empty);

        private FieldNameNode(Utf8Memory bufferedValue)
          : base(bufferedValue)
        {
        }

        public override JsonNodeType Type => JsonNodeType.FieldName;

        public static JsonNavigator.JsonTextNavigator.FieldNameNode Create(Utf8Memory bufferedToken) => bufferedToken.Length == 0 ? JsonNavigator.JsonTextNavigator.FieldNameNode.Empty : new JsonNavigator.JsonTextNavigator.FieldNameNode(bufferedToken);
      }

      private sealed class NullNode : JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode
      {
        private static readonly JsonNavigator.JsonTextNavigator.NullNode Instance = new JsonNavigator.JsonTextNavigator.NullNode();

        private NullNode()
        {
        }

        public override JsonNodeType Type => JsonNodeType.Null;

        public static JsonNavigator.JsonTextNavigator.NullNode Create() => JsonNavigator.JsonTextNavigator.NullNode.Instance;
      }

      private sealed class NumberNode : JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode
      {
        private static readonly JsonNavigator.JsonTextNavigator.NumberNode[] LiteralNumberNodes = new JsonNavigator.JsonTextNavigator.NumberNode[32]
        {
          new JsonNavigator.JsonTextNavigator.NumberNode(0),
          new JsonNavigator.JsonTextNavigator.NumberNode(1),
          new JsonNavigator.JsonTextNavigator.NumberNode(2),
          new JsonNavigator.JsonTextNavigator.NumberNode(3),
          new JsonNavigator.JsonTextNavigator.NumberNode(4),
          new JsonNavigator.JsonTextNavigator.NumberNode(5),
          new JsonNavigator.JsonTextNavigator.NumberNode(6),
          new JsonNavigator.JsonTextNavigator.NumberNode(7),
          new JsonNavigator.JsonTextNavigator.NumberNode(8),
          new JsonNavigator.JsonTextNavigator.NumberNode(9),
          new JsonNavigator.JsonTextNavigator.NumberNode(10),
          new JsonNavigator.JsonTextNavigator.NumberNode(11),
          new JsonNavigator.JsonTextNavigator.NumberNode(12),
          new JsonNavigator.JsonTextNavigator.NumberNode(13),
          new JsonNavigator.JsonTextNavigator.NumberNode(14),
          new JsonNavigator.JsonTextNavigator.NumberNode(15),
          new JsonNavigator.JsonTextNavigator.NumberNode(16),
          new JsonNavigator.JsonTextNavigator.NumberNode(17),
          new JsonNavigator.JsonTextNavigator.NumberNode(18),
          new JsonNavigator.JsonTextNavigator.NumberNode(19),
          new JsonNavigator.JsonTextNavigator.NumberNode(20),
          new JsonNavigator.JsonTextNavigator.NumberNode(21),
          new JsonNavigator.JsonTextNavigator.NumberNode(22),
          new JsonNavigator.JsonTextNavigator.NumberNode(23),
          new JsonNavigator.JsonTextNavigator.NumberNode(24),
          new JsonNavigator.JsonTextNavigator.NumberNode(25),
          new JsonNavigator.JsonTextNavigator.NumberNode(26),
          new JsonNavigator.JsonTextNavigator.NumberNode(27),
          new JsonNavigator.JsonTextNavigator.NumberNode(28),
          new JsonNavigator.JsonTextNavigator.NumberNode(29),
          new JsonNavigator.JsonTextNavigator.NumberNode(30),
          new JsonNavigator.JsonTextNavigator.NumberNode(31)
        };

        private NumberNode(ReadOnlyMemory<byte> bufferedToken) => this.BufferedToken = bufferedToken;

        private NumberNode(int value)
          : this((ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(value.ToString()))
        {
        }

        public ReadOnlyMemory<byte> BufferedToken { get; }

        public override JsonNodeType Type => JsonNodeType.Number64;

        public static JsonNavigator.JsonTextNavigator.NumberNode Create(
          ReadOnlyMemory<byte> bufferedToken)
        {
          ReadOnlySpan<byte> span = bufferedToken.Span;
          if (span.Length == 1 && span[0] >= (byte) 48 && span[0] <= (byte) 57)
            return JsonNavigator.JsonTextNavigator.NumberNode.LiteralNumberNodes[(int) span[0] - 48];
          if (span.Length == 2 && span[0] >= (byte) 48 && span[0] <= (byte) 57 && span[1] >= (byte) 48 && span[1] <= (byte) 57)
          {
            int index = ((int) span[0] - 48) * 10 + ((int) span[1] - 48);
            if (index >= 0 && index < JsonNavigator.JsonTextNavigator.NumberNode.LiteralNumberNodes.Length)
              return JsonNavigator.JsonTextNavigator.NumberNode.LiteralNumberNodes[index];
          }
          return new JsonNavigator.JsonTextNavigator.NumberNode(bufferedToken);
        }
      }

      private sealed class ObjectNode : JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode
      {
        private ObjectNode(
          IReadOnlyList<ObjectProperty> properties,
          ReadOnlyMemory<byte> bufferedValue)
        {
          this.Properties = properties;
          this.BufferedValue = bufferedValue;
        }

        public IReadOnlyList<ObjectProperty> Properties { get; }

        public ReadOnlyMemory<byte> BufferedValue { get; }

        public override JsonNodeType Type => JsonNodeType.Object;

        public static JsonNavigator.JsonTextNavigator.ObjectNode Create(
          IReadOnlyList<ObjectProperty> properties,
          ReadOnlyMemory<byte> bufferedValue)
        {
          return new JsonNavigator.JsonTextNavigator.ObjectNode(properties, bufferedValue);
        }
      }

      private sealed class StringNode : JsonNavigator.JsonTextNavigator.StringNodeBase
      {
        private static readonly JsonNavigator.JsonTextNavigator.StringNode Empty = new JsonNavigator.JsonTextNavigator.StringNode(Utf8Memory.Empty);

        private StringNode(Utf8Memory bufferedValue)
          : base(bufferedValue)
        {
        }

        public override JsonNodeType Type => JsonNodeType.String;

        public static JsonNavigator.JsonTextNavigator.StringNode Create(Utf8Memory bufferedToken) => bufferedToken.Length == 0 ? JsonNavigator.JsonTextNavigator.StringNode.Empty : new JsonNavigator.JsonTextNavigator.StringNode(bufferedToken);
      }

      private abstract class StringNodeBase : JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode
      {
        protected StringNodeBase(Utf8Memory bufferedValue) => this.BufferedValue = bufferedValue;

        public Utf8Memory BufferedValue { get; }
      }

      private sealed class TrueNode : JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode
      {
        private static readonly JsonNavigator.JsonTextNavigator.TrueNode Instance = new JsonNavigator.JsonTextNavigator.TrueNode();

        private TrueNode()
        {
        }

        public override JsonNodeType Type => JsonNodeType.True;

        public static JsonNavigator.JsonTextNavigator.TrueNode Create() => JsonNavigator.JsonTextNavigator.TrueNode.Instance;
      }

      private abstract class IntegerNode : JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode
      {
        protected IntegerNode(ReadOnlyMemory<byte> bufferedToken) => this.BufferedToken = bufferedToken;

        public ReadOnlyMemory<byte> BufferedToken { get; }
      }

      private sealed class Int8Node : JsonNavigator.JsonTextNavigator.IntegerNode
      {
        private Int8Node(ReadOnlyMemory<byte> bufferedToken)
          : base(bufferedToken)
        {
        }

        public override JsonNodeType Type => JsonNodeType.Int8;

        public static JsonNavigator.JsonTextNavigator.Int8Node Create(
          ReadOnlyMemory<byte> bufferedToken)
        {
          return new JsonNavigator.JsonTextNavigator.Int8Node(bufferedToken);
        }
      }

      private sealed class Int16Node : JsonNavigator.JsonTextNavigator.IntegerNode
      {
        private Int16Node(ReadOnlyMemory<byte> bufferedToken)
          : base(bufferedToken)
        {
        }

        public override JsonNodeType Type => JsonNodeType.Int16;

        public static JsonNavigator.JsonTextNavigator.Int16Node Create(
          ReadOnlyMemory<byte> bufferedToken)
        {
          return new JsonNavigator.JsonTextNavigator.Int16Node(bufferedToken);
        }
      }

      private sealed class Int32Node : JsonNavigator.JsonTextNavigator.IntegerNode
      {
        private Int32Node(ReadOnlyMemory<byte> bufferedToken)
          : base(bufferedToken)
        {
        }

        public override JsonNodeType Type => JsonNodeType.Int32;

        public static JsonNavigator.JsonTextNavigator.Int32Node Create(
          ReadOnlyMemory<byte> bufferedToken)
        {
          return new JsonNavigator.JsonTextNavigator.Int32Node(bufferedToken);
        }
      }

      private sealed class Int64Node : JsonNavigator.JsonTextNavigator.IntegerNode
      {
        private Int64Node(ReadOnlyMemory<byte> bufferedToken)
          : base(bufferedToken)
        {
        }

        public override JsonNodeType Type => JsonNodeType.Int64;

        public static JsonNavigator.JsonTextNavigator.Int64Node Create(
          ReadOnlyMemory<byte> bufferedToken)
        {
          return new JsonNavigator.JsonTextNavigator.Int64Node(bufferedToken);
        }
      }

      private sealed class UInt32Node : JsonNavigator.JsonTextNavigator.IntegerNode
      {
        private UInt32Node(ReadOnlyMemory<byte> bufferedToken)
          : base(bufferedToken)
        {
        }

        public override JsonNodeType Type => JsonNodeType.UInt32;

        public static JsonNavigator.JsonTextNavigator.UInt32Node Create(
          ReadOnlyMemory<byte> bufferedToken)
        {
          return new JsonNavigator.JsonTextNavigator.UInt32Node(bufferedToken);
        }
      }

      private abstract class FloatNode : JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode
      {
        protected FloatNode(ReadOnlyMemory<byte> bufferedToken) => this.BufferedToken = bufferedToken;

        public ReadOnlyMemory<byte> BufferedToken { get; }
      }

      private sealed class Float32Node : JsonNavigator.JsonTextNavigator.FloatNode
      {
        private Float32Node(ReadOnlyMemory<byte> bufferedToken)
          : base(bufferedToken)
        {
        }

        public override JsonNodeType Type => JsonNodeType.Float32;

        public static JsonNavigator.JsonTextNavigator.Float32Node Create(
          ReadOnlyMemory<byte> bufferedToken)
        {
          return new JsonNavigator.JsonTextNavigator.Float32Node(bufferedToken);
        }
      }

      private sealed class Float64Node : JsonNavigator.JsonTextNavigator.FloatNode
      {
        private Float64Node(ReadOnlyMemory<byte> bufferedToken)
          : base(bufferedToken)
        {
        }

        public override JsonNodeType Type => JsonNodeType.Float64;

        public static JsonNavigator.JsonTextNavigator.Float64Node Create(
          ReadOnlyMemory<byte> bufferedToken)
        {
          return new JsonNavigator.JsonTextNavigator.Float64Node(bufferedToken);
        }
      }

      private sealed class GuidNode : JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode
      {
        private GuidNode(ReadOnlyMemory<byte> bufferedToken) => this.BufferedToken = bufferedToken;

        public ReadOnlyMemory<byte> BufferedToken { get; }

        public override JsonNodeType Type => JsonNodeType.Guid;

        public static JsonNavigator.JsonTextNavigator.GuidNode Create(ReadOnlyMemory<byte> value) => new JsonNavigator.JsonTextNavigator.GuidNode(value);
      }

      private sealed class BinaryNode : JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode
      {
        private BinaryNode(ReadOnlyMemory<byte> bufferedToken) => this.BufferedToken = bufferedToken;

        public ReadOnlyMemory<byte> BufferedToken { get; }

        public override JsonNodeType Type => JsonNodeType.Binary;

        public static JsonNavigator.JsonTextNavigator.BinaryNode Create(ReadOnlyMemory<byte> value) => new JsonNavigator.JsonTextNavigator.BinaryNode(value);
      }

      private abstract class LazyNode : JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode
      {
        private readonly Lazy<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode> lazyNode;

        protected LazyNode(
          Lazy<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode> lazyNode,
          ReadOnlyMemory<byte> bufferedValue)
        {
          this.lazyNode = lazyNode;
          this.BufferedValue = bufferedValue;
        }

        public ReadOnlyMemory<byte> BufferedValue { get; }

        public JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode Value => this.lazyNode.Value;
      }

      private sealed class LazyArrayNode : JsonNavigator.JsonTextNavigator.LazyNode
      {
        public LazyArrayNode(
          Lazy<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode> lazyNode,
          ReadOnlyMemory<byte> bufferedValue)
          : base(lazyNode, bufferedValue)
        {
        }

        public override JsonNodeType Type => JsonNodeType.Array;
      }

      private sealed class LazyObjectNode : JsonNavigator.JsonTextNavigator.LazyNode
      {
        public LazyObjectNode(
          Lazy<JsonNavigator.JsonTextNavigator.JsonTextNavigatorNode> lazyNode,
          ReadOnlyMemory<byte> bufferedValue)
          : base(lazyNode, bufferedValue)
        {
        }

        public override JsonNodeType Type => JsonNodeType.Object;
      }
    }
  }
}
