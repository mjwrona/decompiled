// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization.AmqpContractSerializer
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization
{
  internal sealed class AmqpContractSerializer
  {
    private static readonly Dictionary<Type, SerializableType> builtInTypes = new Dictionary<Type, SerializableType>()
    {
      {
        typeof (bool),
        SerializableType.CreateSingleValueType(typeof (bool))
      },
      {
        typeof (byte),
        SerializableType.CreateSingleValueType(typeof (byte))
      },
      {
        typeof (ushort),
        SerializableType.CreateSingleValueType(typeof (ushort))
      },
      {
        typeof (uint),
        SerializableType.CreateSingleValueType(typeof (uint))
      },
      {
        typeof (ulong),
        SerializableType.CreateSingleValueType(typeof (ulong))
      },
      {
        typeof (sbyte),
        SerializableType.CreateSingleValueType(typeof (sbyte))
      },
      {
        typeof (short),
        SerializableType.CreateSingleValueType(typeof (short))
      },
      {
        typeof (int),
        SerializableType.CreateSingleValueType(typeof (int))
      },
      {
        typeof (long),
        SerializableType.CreateSingleValueType(typeof (long))
      },
      {
        typeof (float),
        SerializableType.CreateSingleValueType(typeof (float))
      },
      {
        typeof (double),
        SerializableType.CreateSingleValueType(typeof (double))
      },
      {
        typeof (Decimal),
        SerializableType.CreateSingleValueType(typeof (Decimal))
      },
      {
        typeof (char),
        SerializableType.CreateSingleValueType(typeof (char))
      },
      {
        typeof (DateTime),
        SerializableType.CreateSingleValueType(typeof (DateTime))
      },
      {
        typeof (Guid),
        SerializableType.CreateSingleValueType(typeof (Guid))
      },
      {
        typeof (ArraySegment<byte>),
        SerializableType.CreateSingleValueType(typeof (ArraySegment<byte>))
      },
      {
        typeof (string),
        SerializableType.CreateSingleValueType(typeof (string))
      },
      {
        typeof (AmqpSymbol),
        SerializableType.CreateSingleValueType(typeof (AmqpSymbol))
      },
      {
        typeof (TimeSpan),
        SerializableType.CreateDescribedValueType<TimeSpan, long>("com.microsoft:timespan", (Func<TimeSpan, long>) (ts => ts.Ticks), (Func<long, TimeSpan>) (l => TimeSpan.FromTicks(l)))
      },
      {
        typeof (Uri),
        SerializableType.CreateDescribedValueType<Uri, string>("com.microsoft:uri", (Func<Uri, string>) (u => u.AbsoluteUri), (Func<string, Uri>) (s => new Uri(s)))
      },
      {
        typeof (DateTimeOffset),
        SerializableType.CreateDescribedValueType<DateTimeOffset, long>("com.microsoft:datetime-offset", (Func<DateTimeOffset, long>) (d => d.UtcTicks), (Func<long, DateTimeOffset>) (l => new DateTimeOffset(new DateTime(l, DateTimeKind.Utc))))
      },
      {
        typeof (object),
        SerializableType.CreateObjectType(typeof (object))
      }
    };
    private static readonly AmqpContractSerializer Instance = new AmqpContractSerializer();
    private readonly ConcurrentDictionary<Type, SerializableType> customTypeCache;

    internal AmqpContractSerializer() => this.customTypeCache = new ConcurrentDictionary<Type, SerializableType>();

    public static void WriteObject(Stream stream, object graph) => AmqpContractSerializer.Instance.WriteObjectInternal(stream, graph);

    public static T ReadObject<T>(Stream stream) => AmqpContractSerializer.Instance.ReadObjectInternal<T, T>(stream);

    public static TAs ReadObject<T, TAs>(Stream stream) => AmqpContractSerializer.Instance.ReadObjectInternal<T, TAs>(stream);

    internal void WriteObjectInternal(Stream stream, object graph)
    {
      if (graph == null)
      {
        stream.WriteByte((byte) 64);
      }
      else
      {
        using (ByteBuffer buffer = new ByteBuffer(1024, true))
        {
          this.GetType(graph.GetType()).WriteObject(buffer, graph);
          stream.Write(buffer.Buffer, buffer.Offset, buffer.Length);
        }
      }
    }

    internal void WriteObjectInternal(ByteBuffer buffer, object graph)
    {
      if (graph == null)
        AmqpEncoding.EncodeNull(buffer);
      else
        this.GetType(graph.GetType()).WriteObject(buffer, graph);
    }

    internal T ReadObjectInternal<T>(Stream stream) => this.ReadObjectInternal<T, T>(stream);

    internal TAs ReadObjectInternal<T, TAs>(Stream stream)
    {
      if (!stream.CanSeek)
        throw new AmqpException(AmqpError.DecodeError, "stream.CanSeek must be true.");
      SerializableType type = this.GetType(typeof (T));
      long position = stream.Position;
      ByteBuffer buffer;
      if (stream is BufferListStream bufferListStream)
      {
        ArraySegment<byte> arraySegment = bufferListStream.ReadBytes(int.MaxValue);
        buffer = new ByteBuffer(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
      }
      else
      {
        buffer = new ByteBuffer((int) stream.Length, false);
        int size = stream.Read(buffer.Buffer, 0, buffer.Capacity);
        buffer.Append(size);
      }
      using (buffer)
      {
        TAs @as = (TAs) type.ReadObject(buffer);
        if (buffer.Length > 0)
          stream.Position = position + (long) buffer.Offset;
        return @as;
      }
    }

    internal TAs ReadObjectInternal<T, TAs>(ByteBuffer buffer) => (TAs) this.GetType(typeof (T)).ReadObject(buffer);

    internal SerializableType GetType(Type type) => this.GetOrCompileType(type, false);

    private bool TryGetSerializableType(Type type, out SerializableType serializableType)
    {
      serializableType = (SerializableType) null;
      return AmqpContractSerializer.builtInTypes.TryGetValue(type, out serializableType) || this.customTypeCache.TryGetValue(type, out serializableType);
    }

    private SerializableType GetOrCompileType(Type type, bool describedOnly)
    {
      SerializableType serializableType = (SerializableType) null;
      if (!this.TryGetSerializableType(type, out serializableType))
      {
        serializableType = this.CompileType(type, describedOnly);
        if (serializableType != null)
          this.customTypeCache.TryAdd(type, serializableType);
      }
      return serializableType != null ? serializableType : throw new NotSupportedException(type.FullName);
    }

    private SerializableType CompileType(Type type, bool describedOnly)
    {
      object[] customAttributes1 = type.GetCustomAttributes(typeof (AmqpContractAttribute), false);
      if (customAttributes1.Length == 0)
        return describedOnly ? (SerializableType) null : this.CompileNonContractTypes(type);
      AmqpContractAttribute contractAttribute = (AmqpContractAttribute) customAttributes1[0];
      SerializableType baseType = (SerializableType) null;
      if (type.BaseType != typeof (object))
      {
        baseType = this.CompileType(type.BaseType, true);
        if (baseType != null)
        {
          if (baseType.Encoding != contractAttribute.Encoding)
            throw new SerializationException(SRAmqp.AmqpEncodingTypeMismatch((object) type.Name, (object) contractAttribute.Encoding, (object) type.BaseType.Name, (object) baseType.Encoding));
          this.customTypeCache.TryAdd(type.BaseType, baseType);
        }
      }
      string descriptorName = contractAttribute.Name;
      ulong? internalCode = contractAttribute.InternalCode;
      if (descriptorName == null && !internalCode.HasValue)
        descriptorName = type.FullName;
      List<SerialiableMember> serialiableMemberList = new List<SerialiableMember>();
      if (contractAttribute.Encoding == EncodingType.List && baseType != null)
        serialiableMemberList.AddRange((IEnumerable<SerialiableMember>) baseType.Members);
      int num1 = serialiableMemberList.Count + 1;
      MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      MethodAccessor onDesrialized = (MethodAccessor) null;
      foreach (MemberInfo memberInfo in members)
      {
        if (!(memberInfo.DeclaringType != type))
        {
          if (memberInfo.MemberType == MemberTypes.Field || memberInfo.MemberType == MemberTypes.Property)
          {
            object[] customAttributes2 = memberInfo.GetCustomAttributes(typeof (AmqpMemberAttribute), true);
            if (customAttributes2.Length == 1)
            {
              AmqpMemberAttribute amqpMemberAttribute = (AmqpMemberAttribute) customAttributes2[0];
              SerialiableMember serialiableMember = new SerialiableMember();
              serialiableMember.Name = amqpMemberAttribute.Name ?? memberInfo.Name;
              serialiableMember.Order = amqpMemberAttribute.InternalOrder ?? num1++;
              serialiableMember.Mandatory = amqpMemberAttribute.Mandatory;
              serialiableMember.Accessor = MemberAccessor.Create(memberInfo, true);
              Type type1 = memberInfo.MemberType == MemberTypes.Field ? ((FieldInfo) memberInfo).FieldType : ((PropertyInfo) memberInfo).PropertyType;
              serialiableMember.Type = this.GetType(type1);
              serialiableMemberList.Add(serialiableMember);
            }
          }
          else if (memberInfo.MemberType == MemberTypes.Method && memberInfo.GetCustomAttributes(typeof (OnDeserializedAttribute), false).Length == 1)
            onDesrialized = MethodAccessor.Create((MethodInfo) memberInfo);
        }
      }
      if (contractAttribute.Encoding == EncodingType.List)
      {
        serialiableMemberList.Sort((IComparer<SerialiableMember>) AmqpContractSerializer.MemberOrderComparer.Instance);
        int num2 = -1;
        foreach (SerialiableMember serialiableMember in serialiableMemberList)
        {
          if (num2 > 0 && serialiableMember.Order == num2)
            throw new SerializationException(SRAmqp.AmqpDuplicateMemberOrder((object) num2, (object) type.Name));
          num2 = serialiableMember.Order;
        }
      }
      SerialiableMember[] array = serialiableMemberList.ToArray();
      Dictionary<Type, SerializableType> knownTypes = (Dictionary<Type, SerializableType>) null;
      foreach (KnownTypeAttribute customAttribute in type.GetCustomAttributes(typeof (KnownTypeAttribute), false))
      {
        if (customAttribute.Type.GetCustomAttributes(typeof (AmqpContractAttribute), false).Length != 0)
        {
          if (knownTypes == null)
            knownTypes = new Dictionary<Type, SerializableType>();
          knownTypes.Add(customAttribute.Type, (SerializableType) null);
        }
      }
      if (contractAttribute.Encoding == EncodingType.List)
        return SerializableType.CreateDescribedListType(this, type, baseType, descriptorName, internalCode, array, knownTypes, onDesrialized);
      if (contractAttribute.Encoding == EncodingType.Map)
        return SerializableType.CreateDescribedMapType(this, type, baseType, descriptorName, internalCode, array, knownTypes, onDesrialized);
      throw new NotSupportedException(contractAttribute.Encoding.ToString());
    }

    private SerializableType CompileNonContractTypes(Type type) => this.CompileNullableTypes(type) ?? this.CompileInterfaceTypes(type);

    private SerializableType CompileNullableTypes(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>) ? this.GetType(type.GetGenericArguments()[0]) : (SerializableType) null;

    private SerializableType CompileInterfaceTypes(Type type)
    {
      bool isArray = type.IsArray;
      bool flag1 = false;
      bool flag2 = false;
      MemberAccessor keyAccessor = (MemberAccessor) null;
      MemberAccessor valueAccessor = (MemberAccessor) null;
      MethodAccessor addAccessor = (MethodAccessor) null;
      Type itemType = (Type) null;
      if (type.GetInterface(typeof (IAmqpSerializable).Name, false) != (Type) null)
        return SerializableType.CreateAmqpSerializableType(this, type);
      foreach (Type type1 in type.GetInterfaces())
      {
        if (type1.IsGenericType)
        {
          Type genericTypeDefinition = type1.GetGenericTypeDefinition();
          if (genericTypeDefinition == typeof (IDictionary<,>))
          {
            flag1 = true;
            Type[] genericArguments = type1.GetGenericArguments();
            itemType = typeof (KeyValuePair<,>).MakeGenericType(genericArguments);
            keyAccessor = MemberAccessor.Create((MemberInfo) itemType.GetProperty("Key"), false);
            valueAccessor = MemberAccessor.Create((MemberInfo) itemType.GetProperty("Value"), false);
            addAccessor = MethodAccessor.Create(type.GetMethod("Add", genericArguments));
            break;
          }
          if (genericTypeDefinition == typeof (IList<>))
          {
            flag2 = true;
            Type[] genericArguments = type1.GetGenericArguments();
            itemType = genericArguments[0];
            addAccessor = MethodAccessor.Create(type.GetMethod("Add", genericArguments));
            break;
          }
        }
      }
      if (flag1)
        return SerializableType.CreateMapType(this, type, keyAccessor, valueAccessor, addAccessor);
      return !isArray && flag2 ? SerializableType.CreateListType(this, type, itemType, addAccessor) : (SerializableType) null;
    }

    private sealed class MemberOrderComparer : IComparer<SerialiableMember>
    {
      public static readonly AmqpContractSerializer.MemberOrderComparer Instance = new AmqpContractSerializer.MemberOrderComparer();

      public int Compare(SerialiableMember m1, SerialiableMember m2)
      {
        if (m1.Order == m2.Order)
          return 0;
        return m1.Order <= m2.Order ? -1 : 1;
      }
    }
  }
}
