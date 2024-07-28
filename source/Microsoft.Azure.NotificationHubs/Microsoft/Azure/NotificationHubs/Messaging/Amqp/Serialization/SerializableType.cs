// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization.SerializableType
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization
{
  internal abstract class SerializableType
  {
    private readonly AmqpContractSerializer serializer;
    private readonly Type type;
    private readonly bool hasDefaultCtor;

    protected SerializableType(AmqpContractSerializer serializer, Type type)
    {
      this.serializer = serializer;
      this.type = type;
      this.hasDefaultCtor = type.GetConstructor(Type.EmptyTypes) != (ConstructorInfo) null;
    }

    public virtual EncodingType Encoding => throw new InvalidOperationException();

    public virtual SerialiableMember[] Members => throw new InvalidOperationException();

    public static SerializableType CreateSingleValueType(Type type)
    {
      EncodingBase encoding = AmqpEncoding.GetEncoding(type);
      return (SerializableType) new SerializableType.SingleValueType(type, encoding);
    }

    public static SerializableType CreateDescribedValueType<TValue, TAs>(
      string symbol,
      Func<TValue, TAs> getter,
      Func<TAs, TValue> setter)
    {
      return (SerializableType) new SerializableType.DescribedValueType<TValue, TAs>(symbol, getter, setter);
    }

    public static SerializableType CreateObjectType(Type type) => (SerializableType) new SerializableType.AmqpObjectType(type);

    public static SerializableType CreateListType(
      AmqpContractSerializer serializer,
      Type type,
      Type itemType,
      MethodAccessor addAccessor)
    {
      return (SerializableType) new SerializableType.ListType(serializer, type, itemType, addAccessor);
    }

    public static SerializableType CreateMapType(
      AmqpContractSerializer serializer,
      Type type,
      MemberAccessor keyAccessor,
      MemberAccessor valueAccessor,
      MethodAccessor addAccessor)
    {
      return (SerializableType) new SerializableType.MapType(serializer, type, keyAccessor, valueAccessor, addAccessor);
    }

    public static SerializableType CreateDescribedListType(
      AmqpContractSerializer serializer,
      Type type,
      SerializableType baseType,
      string descriptorName,
      ulong? descriptorCode,
      SerialiableMember[] members,
      Dictionary<Type, SerializableType> knownTypes,
      MethodAccessor onDesrialized)
    {
      return (SerializableType) new SerializableType.DescribedListType(serializer, type, baseType, descriptorName, descriptorCode, members, knownTypes, onDesrialized);
    }

    public static SerializableType CreateDescribedMapType(
      AmqpContractSerializer serializer,
      Type type,
      SerializableType baseType,
      string descriptorName,
      ulong? descriptorCode,
      SerialiableMember[] members,
      Dictionary<Type, SerializableType> knownTypes,
      MethodAccessor onDesrialized)
    {
      return (SerializableType) new SerializableType.DescribedMapType(serializer, type, baseType, descriptorName, descriptorCode, members, knownTypes, onDesrialized);
    }

    public static SerializableType CreateAmqpSerializableType(
      AmqpContractSerializer serializer,
      Type type)
    {
      return (SerializableType) new SerializableType.AmqpSerializableType(serializer, type);
    }

    public abstract void WriteObject(ByteBuffer buffer, object graph);

    public abstract object ReadObject(ByteBuffer buffer);

    private sealed class SingleValueType : SerializableType
    {
      private readonly EncodingBase encoder;

      public SingleValueType(Type type, EncodingBase encoder)
        : base((AmqpContractSerializer) null, type)
      {
        this.encoder = encoder;
      }

      public override void WriteObject(ByteBuffer buffer, object value) => this.encoder.EncodeObject(value, false, buffer);

      public override object ReadObject(ByteBuffer buffer) => this.encoder.DecodeObject(buffer, (FormatCode) (byte) 0);
    }

    private sealed class AmqpSerializableType : SerializableType
    {
      public AmqpSerializableType(AmqpContractSerializer serializer, Type type)
        : base(serializer, type)
      {
      }

      public override void WriteObject(ByteBuffer buffer, object value)
      {
        if (value == null)
          AmqpEncoding.EncodeNull(buffer);
        else
          ((IAmqpSerializable) value).Encode(buffer);
      }

      public override object ReadObject(ByteBuffer buffer)
      {
        buffer.Validate(false, 1);
        if ((FormatCode) buffer.Buffer[buffer.Offset] == (FormatCode) (byte) 64)
        {
          buffer.Complete(1);
          return (object) null;
        }
        object obj = this.hasDefaultCtor ? Activator.CreateInstance(this.type) : FormatterServices.GetUninitializedObject(this.type);
        ((IAmqpSerializable) obj).Decode(buffer);
        return obj;
      }
    }

    private sealed class DescribedValueType<TValue, TAs> : SerializableType
    {
      private readonly AmqpSymbol symbol;
      private readonly EncodingBase encoder;
      private readonly Func<TValue, TAs> getter;
      private readonly Func<TAs, TValue> setter;

      public DescribedValueType(string symbol, Func<TValue, TAs> getter, Func<TAs, TValue> setter)
        : base((AmqpContractSerializer) null, typeof (TValue))
      {
        this.symbol = (AmqpSymbol) symbol;
        this.encoder = AmqpEncoding.GetEncoding(typeof (TAs));
        this.getter = getter;
        this.setter = setter;
      }

      public override void WriteObject(ByteBuffer buffer, object value)
      {
        if (value == null)
        {
          AmqpEncoding.EncodeNull(buffer);
        }
        else
        {
          AmqpBitConverter.WriteUByte(buffer, (byte) 0);
          SymbolEncoding.Encode(this.symbol, buffer);
          this.encoder.EncodeObject((object) this.getter((TValue) value), false, buffer);
        }
      }

      public override object ReadObject(ByteBuffer buffer)
      {
        Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.DescribedType describedType = DescribedEncoding.Decode(buffer);
        if (describedType == null)
          return (object) null;
        if (!this.symbol.Equals(describedType.Descriptor))
          throw new SerializationException(describedType.Descriptor.ToString());
        return (object) this.setter((TAs) describedType.Value);
      }
    }

    private sealed class AmqpObjectType : SerializableType
    {
      public AmqpObjectType(Type type)
        : base((AmqpContractSerializer) null, type)
      {
      }

      public override void WriteObject(ByteBuffer buffer, object value) => AmqpEncoding.EncodeObject(value, buffer);

      public override object ReadObject(ByteBuffer buffer) => AmqpEncoding.DecodeObject(buffer);
    }

    private abstract class CollectionType : SerializableType
    {
      protected CollectionType(AmqpContractSerializer serializer, Type type)
        : base(serializer, type)
      {
      }

      public abstract int WriteMembers(ByteBuffer buffer, object container);

      public abstract void ReadMembers(ByteBuffer buffer, object container, ref int count);

      protected abstract bool WriteFormatCode(ByteBuffer buffer);

      protected abstract void Initialize(
        ByteBuffer buffer,
        FormatCode formatCode,
        out int size,
        out int count,
        out int encodeWidth,
        out SerializableType.CollectionType effectiveType);

      public override void WriteObject(ByteBuffer buffer, object graph)
      {
        if (graph == null)
        {
          AmqpEncoding.EncodeNull(buffer);
        }
        else
        {
          if (!this.WriteFormatCode(buffer))
            return;
          int writePos = buffer.WritePos;
          AmqpBitConverter.WriteULong(buffer, 0UL);
          int data = this.WriteMembers(buffer, graph);
          AmqpBitConverter.WriteUInt(buffer.Buffer, writePos, (uint) (buffer.WritePos - writePos - 4));
          AmqpBitConverter.WriteUInt(buffer.Buffer, writePos + 4, (uint) data);
        }
      }

      public override object ReadObject(ByteBuffer buffer)
      {
        FormatCode formatCode = AmqpEncoding.ReadFormatCode(buffer);
        if (formatCode == (FormatCode) (byte) 64)
          return (object) null;
        int size;
        int count;
        int encodeWidth;
        SerializableType.CollectionType effectiveType;
        this.Initialize(buffer, formatCode, out size, out count, out encodeWidth, out effectiveType);
        int offset = buffer.Offset;
        object container = effectiveType.hasDefaultCtor ? Activator.CreateInstance(effectiveType.type) : FormatterServices.GetUninitializedObject(effectiveType.type);
        if (count > 0)
        {
          effectiveType.ReadMembers(buffer, container, ref count);
          if (count > 0)
            buffer.Complete(size - (buffer.Offset - offset) - encodeWidth);
        }
        return container;
      }
    }

    private sealed class ListType : SerializableType.CollectionType
    {
      private readonly SerializableType itemType;
      private readonly MethodAccessor addMethodAccessor;

      public ListType(
        AmqpContractSerializer serializer,
        Type type,
        Type itemType,
        MethodAccessor addAccessor)
        : base(serializer, type)
      {
        this.itemType = serializer.GetType(itemType);
        this.addMethodAccessor = addAccessor;
      }

      protected override bool WriteFormatCode(ByteBuffer buffer)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 208);
        return true;
      }

      public override int WriteMembers(ByteBuffer buffer, object container)
      {
        int num = 0;
        foreach (object graph in (IEnumerable) container)
        {
          if (graph == null)
          {
            AmqpEncoding.EncodeNull(buffer);
          }
          else
          {
            SerializableType serializableType = this.itemType;
            if (graph.GetType() != serializableType.type)
              serializableType = this.serializer.GetType(graph.GetType());
            serializableType.WriteObject(buffer, graph);
          }
          ++num;
        }
        return num;
      }

      protected override void Initialize(
        ByteBuffer buffer,
        FormatCode formatCode,
        out int size,
        out int count,
        out int encodeWidth,
        out SerializableType.CollectionType effectiveType)
      {
        if (formatCode == (FormatCode) (byte) 69)
        {
          size = count = encodeWidth = 0;
          effectiveType = (SerializableType.CollectionType) this;
        }
        else
        {
          if (formatCode != (FormatCode) (byte) 208 && formatCode != (FormatCode) (byte) 192)
            throw new AmqpException(AmqpError.InvalidField, SRAmqp.AmqpInvalidFormatCode((object) formatCode, (object) buffer.Offset));
          encodeWidth = formatCode == (FormatCode) (byte) 192 ? 1 : 4;
          AmqpEncoding.ReadSizeAndCount(buffer, formatCode, (FormatCode) (byte) 192, (FormatCode) (byte) 208, out size, out count);
          effectiveType = (SerializableType.CollectionType) this;
        }
      }

      public override void ReadMembers(ByteBuffer buffer, object container, ref int count)
      {
        while (count > 0)
        {
          object obj = this.itemType.ReadObject(buffer);
          this.addMethodAccessor.Invoke(container, new object[1]
          {
            obj
          });
          --count;
        }
      }
    }

    private sealed class MapType : SerializableType.CollectionType
    {
      private readonly SerializableType keyType;
      private readonly SerializableType valueType;
      private readonly MemberAccessor keyAccessor;
      private readonly MemberAccessor valueAccessor;
      private readonly MethodAccessor addMethodAccessor;

      public MapType(
        AmqpContractSerializer serializer,
        Type type,
        MemberAccessor keyAccessor,
        MemberAccessor valueAccessor,
        MethodAccessor addAccessor)
        : base(serializer, type)
      {
        this.keyType = this.serializer.GetType(keyAccessor.Type);
        this.valueType = this.serializer.GetType(valueAccessor.Type);
        this.keyAccessor = keyAccessor;
        this.valueAccessor = valueAccessor;
        this.addMethodAccessor = addAccessor;
      }

      protected override bool WriteFormatCode(ByteBuffer buffer)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 209);
        return true;
      }

      public override int WriteMembers(ByteBuffer buffer, object container)
      {
        int num = 0;
        foreach (object container1 in (IEnumerable) container)
        {
          object graph1 = this.keyAccessor.Get(container1);
          object graph2 = this.valueAccessor.Get(container1);
          if (graph2 != null)
          {
            this.keyType.WriteObject(buffer, graph1);
            SerializableType serializableType = this.valueType;
            if (graph2.GetType() != serializableType.type)
              serializableType = this.serializer.GetType(graph2.GetType());
            serializableType.WriteObject(buffer, graph2);
            num += 2;
          }
        }
        return num;
      }

      protected override void Initialize(
        ByteBuffer buffer,
        FormatCode formatCode,
        out int size,
        out int count,
        out int encodeWidth,
        out SerializableType.CollectionType effectiveType)
      {
        if (formatCode != (FormatCode) (byte) 209 && formatCode != (FormatCode) (byte) 193)
          throw new AmqpException(AmqpError.InvalidField, SRAmqp.AmqpInvalidFormatCode((object) formatCode, (object) buffer.Offset));
        encodeWidth = formatCode == (FormatCode) (byte) 193 ? 1 : 4;
        AmqpEncoding.ReadSizeAndCount(buffer, formatCode, (FormatCode) (byte) 193, (FormatCode) (byte) 209, out size, out count);
        effectiveType = (SerializableType.CollectionType) this;
      }

      public override void ReadMembers(ByteBuffer buffer, object container, ref int count)
      {
        while (count > 0)
        {
          object obj1 = this.keyType.ReadObject(buffer);
          object obj2 = this.valueType.ReadObject(buffer);
          this.addMethodAccessor.Invoke(container, new object[2]
          {
            obj1,
            obj2
          });
          count -= 2;
        }
      }
    }

    private abstract class DescribedType : SerializableType.CollectionType
    {
      private readonly SerializableType.DescribedType baseType;
      private readonly AmqpSymbol descriptorName;
      private readonly ulong? descriptorCode;
      private readonly SerialiableMember[] members;
      private readonly MethodAccessor onDeserialized;
      private readonly KeyValuePair<Type, SerializableType>[] knownTypes;

      protected DescribedType(
        AmqpContractSerializer serializer,
        Type type,
        SerializableType baseType,
        string descriptorName,
        ulong? descriptorCode,
        SerialiableMember[] members,
        Dictionary<Type, SerializableType> knownTypes,
        MethodAccessor onDesrialized)
        : base(serializer, type)
      {
        this.baseType = (SerializableType.DescribedType) baseType;
        this.descriptorName = (AmqpSymbol) descriptorName;
        this.descriptorCode = descriptorCode;
        this.members = members;
        this.onDeserialized = onDesrialized;
        this.knownTypes = SerializableType.DescribedType.GetKnownTypes(knownTypes);
      }

      public override SerialiableMember[] Members => this.members;

      protected abstract byte Code { get; }

      protected SerializableType.DescribedType BaseType => this.baseType;

      protected override bool WriteFormatCode(ByteBuffer buffer)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 0);
        if (this.descriptorCode.HasValue)
          ULongEncoding.Encode(this.descriptorCode, buffer);
        else
          SymbolEncoding.Encode(this.descriptorName, buffer);
        AmqpBitConverter.WriteUByte(buffer, this.Code);
        return true;
      }

      protected override void Initialize(
        ByteBuffer buffer,
        FormatCode formatCode,
        out int size,
        out int count,
        out int encodeWidth,
        out SerializableType.CollectionType effectiveType)
      {
        if (formatCode != (FormatCode) (byte) 0)
          throw new AmqpException(AmqpError.InvalidField, SRAmqp.AmqpInvalidFormatCode((object) formatCode, (object) buffer.Offset));
        effectiveType = (SerializableType.CollectionType) null;
        formatCode = AmqpEncoding.ReadFormatCode(buffer);
        ulong? code2 = new ulong?();
        AmqpSymbol symbol2 = new AmqpSymbol();
        if (formatCode == (FormatCode) (byte) 68)
          code2 = new ulong?(0UL);
        else if (formatCode == (FormatCode) (byte) 128 || formatCode == (FormatCode) (byte) 83)
          code2 = ULongEncoding.Decode(buffer, formatCode);
        else if (formatCode == (FormatCode) (byte) 163 || formatCode == (FormatCode) (byte) 179)
          symbol2 = SymbolEncoding.Decode(buffer, formatCode);
        if (this.AreEqual(this.descriptorCode, this.descriptorName, code2, symbol2))
          effectiveType = (SerializableType.CollectionType) this;
        else if (this.knownTypes != null)
        {
          for (int index1 = 0; index1 < this.knownTypes.Length; ++index1)
          {
            KeyValuePair<Type, SerializableType> keyValuePair1 = this.knownTypes[index1];
            if (keyValuePair1.Value == null)
            {
              SerializableType type = this.serializer.GetType(keyValuePair1.Key);
              KeyValuePair<Type, SerializableType>[] knownTypes = this.knownTypes;
              int index2 = index1;
              keyValuePair1 = new KeyValuePair<Type, SerializableType>(keyValuePair1.Key, type);
              KeyValuePair<Type, SerializableType> keyValuePair2 = keyValuePair1;
              knownTypes[index2] = keyValuePair2;
            }
            SerializableType.DescribedType describedType = (SerializableType.DescribedType) keyValuePair1.Value;
            if (this.AreEqual(describedType.descriptorCode, describedType.descriptorName, code2, symbol2))
            {
              effectiveType = (SerializableType.CollectionType) describedType;
              break;
            }
          }
        }
        if (effectiveType == null)
        {
          ulong? nullable = code2;
          throw new SerializationException(SRAmqp.AmqpUnknownDescriptor(nullable.HasValue ? (object) nullable.GetValueOrDefault() : (object) symbol2.Value, (object) this.type.Name));
        }
        formatCode = AmqpEncoding.ReadFormatCode(buffer);
        if (this.Code == (byte) 208)
        {
          if (formatCode == (FormatCode) (byte) 69)
          {
            size = count = encodeWidth = 0;
          }
          else
          {
            encodeWidth = formatCode == (FormatCode) (byte) 192 ? 1 : 4;
            AmqpEncoding.ReadSizeAndCount(buffer, formatCode, (FormatCode) (byte) 192, (FormatCode) (byte) 208, out size, out count);
          }
        }
        else
        {
          encodeWidth = formatCode == (FormatCode) (byte) 193 ? 1 : 4;
          AmqpEncoding.ReadSizeAndCount(buffer, formatCode, (FormatCode) (byte) 193, (FormatCode) (byte) 209, out size, out count);
        }
      }

      protected void InvokeDeserialized(object container)
      {
        if (this.baseType != null)
          this.baseType.InvokeDeserialized(container);
        if (this.onDeserialized == null)
          return;
        this.onDeserialized.Invoke(container, new object[1]
        {
          (object) new StreamingContext()
        });
      }

      private static KeyValuePair<Type, SerializableType>[] GetKnownTypes(
        Dictionary<Type, SerializableType> types)
      {
        if (types == null || types.Count == 0)
          return (KeyValuePair<Type, SerializableType>[]) null;
        KeyValuePair<Type, SerializableType>[] knownTypes = new KeyValuePair<Type, SerializableType>[types.Count];
        int num = 0;
        foreach (KeyValuePair<Type, SerializableType> type in types)
          knownTypes[num++] = type;
        return knownTypes;
      }

      private bool AreEqual(ulong? code1, AmqpSymbol symbol1, ulong? code2, AmqpSymbol symbol2)
      {
        if (code1.HasValue && code2.HasValue)
          return (long) code1.Value == (long) code2.Value;
        return symbol1.Value != null && symbol2.Value != null && symbol1.Value == symbol2.Value;
      }
    }

    private sealed class DescribedListType : SerializableType.DescribedType
    {
      public DescribedListType(
        AmqpContractSerializer serializer,
        Type type,
        SerializableType baseType,
        string descriptorName,
        ulong? descriptorCode,
        SerialiableMember[] members,
        Dictionary<Type, SerializableType> knownTypes,
        MethodAccessor onDesrialized)
        : base(serializer, type, baseType, descriptorName, descriptorCode, members, knownTypes, onDesrialized)
      {
      }

      public override EncodingType Encoding => EncodingType.List;

      protected override byte Code => 208;

      public override int WriteMembers(ByteBuffer buffer, object container)
      {
        foreach (SerialiableMember member in this.Members)
        {
          object graph = member.Accessor.Get(container);
          if (graph == null)
          {
            AmqpEncoding.EncodeNull(buffer);
          }
          else
          {
            SerializableType type = member.Type;
            if (graph.GetType() != type.type)
              type = this.serializer.GetType(graph.GetType());
            type.WriteObject(buffer, graph);
          }
        }
        return this.Members.Length;
      }

      public override void ReadMembers(ByteBuffer buffer, object container, ref int count)
      {
        int index = 0;
        while (index < this.Members.Length && count > 0)
        {
          object obj = this.Members[index].Type.ReadObject(buffer);
          this.Members[index].Accessor.Set(container, obj);
          ++index;
          --count;
        }
        this.InvokeDeserialized(container);
      }
    }

    private sealed class DescribedMapType : SerializableType.DescribedType
    {
      private readonly Dictionary<string, SerialiableMember> membersMap;

      public DescribedMapType(
        AmqpContractSerializer serializer,
        Type type,
        SerializableType baseType,
        string descriptorName,
        ulong? descriptorCode,
        SerialiableMember[] members,
        Dictionary<Type, SerializableType> knownTypes,
        MethodAccessor onDesrialized)
        : base(serializer, type, baseType, descriptorName, descriptorCode, members, knownTypes, onDesrialized)
      {
        this.membersMap = new Dictionary<string, SerialiableMember>();
        foreach (SerialiableMember member in members)
          this.membersMap.Add(member.Name, member);
      }

      public override EncodingType Encoding => EncodingType.Map;

      protected override byte Code => 209;

      public override int WriteMembers(ByteBuffer buffer, object container)
      {
        int num = 0;
        if (this.BaseType != null)
          this.BaseType.WriteMembers(buffer, container);
        foreach (SerialiableMember member in this.Members)
        {
          object graph = member.Accessor.Get(container);
          if (graph != null)
          {
            AmqpCodec.EncodeSymbol((AmqpSymbol) member.Name, buffer);
            SerializableType type = member.Type;
            if (graph.GetType() != type.type)
              type = this.serializer.GetType(graph.GetType());
            type.WriteObject(buffer, graph);
            num += 2;
          }
        }
        return num;
      }

      public override void ReadMembers(ByteBuffer buffer, object container, ref int count)
      {
        if (this.BaseType != null)
          this.BaseType.ReadMembers(buffer, container, ref count);
        int num = 0;
        while (num < this.membersMap.Count && count > 0)
        {
          SerialiableMember serialiableMember;
          if (this.membersMap.TryGetValue(AmqpCodec.DecodeSymbol(buffer).Value, out serialiableMember))
          {
            object obj = serialiableMember.Type.ReadObject(buffer);
            serialiableMember.Accessor.Set(container, obj);
          }
          ++num;
          count -= 2;
        }
        this.InvokeDeserialized(container);
      }
    }
  }
}
