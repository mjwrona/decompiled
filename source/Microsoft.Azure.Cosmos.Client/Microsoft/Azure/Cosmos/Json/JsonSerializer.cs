// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.JsonSerializer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Json
{
  internal static class JsonSerializer
  {
    public static ReadOnlyMemory<byte> Serialize(
      object value,
      JsonSerializationFormat jsonSerializationFormat = JsonSerializationFormat.Text)
    {
      IJsonWriter jsonWriter = JsonWriter.Create(jsonSerializationFormat);
      JsonSerializer.SerializeInternal(value, jsonWriter);
      return jsonWriter.GetResult();
    }

    public static void SerializeInternal(object value, IJsonWriter jsonWriter)
    {
      if (jsonWriter == null)
        throw new ArgumentNullException(nameof (jsonWriter));
      switch (value)
      {
        case null:
          jsonWriter.WriteNullValue();
          break;
        case bool flag:
          jsonWriter.WriteBoolValue(flag);
          break;
        case string str:
          jsonWriter.WriteStringValue(str);
          break;
        case Number64 number64:
          jsonWriter.WriteNumber64Value(number64);
          break;
        case sbyte num1:
          jsonWriter.WriteInt8Value(num1);
          break;
        case short num2:
          jsonWriter.WriteInt16Value(num2);
          break;
        case int num3:
          jsonWriter.WriteInt32Value(num3);
          break;
        case long num4:
          jsonWriter.WriteInt64Value(num4);
          break;
        case uint num5:
          jsonWriter.WriteUInt32Value(num5);
          break;
        case float num6:
          jsonWriter.WriteFloat32Value(num6);
          break;
        case double num7:
          jsonWriter.WriteFloat64Value(num7);
          break;
        case ReadOnlyMemory<byte> readOnlyMemory:
          jsonWriter.WriteBinaryValue(readOnlyMemory.Span);
          break;
        case Guid guid:
          jsonWriter.WriteGuidValue(guid);
          break;
        case IEnumerable enumerable:
          jsonWriter.WriteArrayStart();
          foreach (object obj in enumerable)
            JsonSerializer.SerializeInternal(obj, jsonWriter);
          jsonWriter.WriteArrayEnd();
          break;
        default:
          CosmosElement cosmosElement = value as CosmosElement;
          if ((object) cosmosElement == null)
          {
            PropertyInfo[] propertyInfoArray = !(value is ValueType valueType) ? value.GetType().GetProperties() : throw new ArgumentOutOfRangeException(string.Format("Unable to serialize type: {0}", (object) valueType.GetType()));
            jsonWriter.WriteObjectStart();
            foreach (PropertyInfo propertyInfo in propertyInfoArray)
            {
              jsonWriter.WriteFieldName(propertyInfo.Name);
              JsonSerializer.SerializeInternal(propertyInfo.GetValue(value), jsonWriter);
            }
            jsonWriter.WriteObjectEnd();
            break;
          }
          cosmosElement.WriteTo(jsonWriter);
          break;
      }
    }

    public static T Deserialize<T>(ReadOnlyMemory<byte> buffer)
    {
      TryCatch<T> tryCatch = JsonSerializer.Monadic.Deserialize<T>(buffer);
      tryCatch.ThrowIfFailed();
      return tryCatch.Result;
    }

    public static bool TryDeserialize<T>(ReadOnlyMemory<byte> buffer, out T result) => TryCatch<T>.ConvertToTryGet<T>(JsonSerializer.Monadic.Deserialize<T>(buffer), out result);

    public static class Monadic
    {
      public static TryCatch<T> Deserialize<T>(ReadOnlyMemory<byte> buffer)
      {
        TryCatch<CosmosElement> fromBuffer = CosmosElement.Monadic.CreateFromBuffer(buffer);
        if (fromBuffer.Failed)
          return TryCatch<T>.FromException(fromBuffer.Exception);
        TryCatch<object> tryCatch = fromBuffer.Result.Accept<Type, TryCatch<object>>((ICosmosElementVisitor<Type, TryCatch<object>>) JsonSerializer.DeserializationVisitor.Singleton, typeof (T));
        if (tryCatch.Failed)
          return TryCatch<T>.FromException(tryCatch.Exception);
        if (tryCatch.Result is T result)
          return TryCatch<T>.FromResult(result);
        Type type = typeof (T);
        if (tryCatch.Result == null && (!type.IsValueType || type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>)))
          return TryCatch<T>.FromResult(default (T));
        if (type == typeof (string))
          return TryCatch<T>.FromResult((T) tryCatch.Result.ToString());
        throw new InvalidOperationException("Could not cast to T.");
      }
    }

    private sealed class DeserializationVisitor : ICosmosElementVisitor<Type, TryCatch<object>>
    {
      public static readonly JsonSerializer.DeserializationVisitor Singleton = new JsonSerializer.DeserializationVisitor();

      private DeserializationVisitor()
      {
      }

      public TryCatch<object> Visit(CosmosArray cosmosArray, Type type)
      {
        if ((!type.IsGenericType ? 0 : (type.GetGenericTypeDefinition() == typeof (IReadOnlyList<>) ? 1 : 0)) == 0)
          return TryCatch<object>.FromException((Exception) JsonSerializer.DeserializationVisitor.Exceptions.ExpectedArray);
        Type input1 = ((IEnumerable<Type>) type.GenericTypeArguments).First<Type>();
        IList instance = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(input1));
        foreach (CosmosElement cosmos in cosmosArray)
        {
          TryCatch<object> tryCatch;
          if (input1 == typeof (object))
          {
            Type type1;
            switch (cosmos)
            {
              case CosmosArray _:
                type1 = typeof (IReadOnlyList<object>);
                break;
              case CosmosBoolean _:
                type1 = typeof (bool);
                break;
              case CosmosNull _:
                type1 = typeof (object);
                break;
              case CosmosNumber _:
                type1 = typeof (Number64);
                break;
              case CosmosObject _:
                type1 = typeof (object);
                break;
              case CosmosString _:
                type1 = typeof (string);
                break;
              case CosmosGuid _:
                type1 = typeof (Guid);
                break;
              case CosmosBinary _:
                type1 = typeof (ReadOnlyMemory<byte>);
                break;
              case CosmosUndefined _:
                type1 = typeof (object);
                break;
              default:
                throw new ArgumentOutOfRangeException("Unknown cosmos element type.");
            }
            Type input2 = type1;
            tryCatch = cosmos.Accept<Type, TryCatch<object>>((ICosmosElementVisitor<Type, TryCatch<object>>) this, input2);
          }
          else
            tryCatch = cosmos.Accept<Type, TryCatch<object>>((ICosmosElementVisitor<Type, TryCatch<object>>) this, input1);
          if (tryCatch.Failed)
            return tryCatch;
          instance.Add(tryCatch.Result);
        }
        return TryCatch<object>.FromResult((object) instance);
      }

      public TryCatch<object> Visit(CosmosBinary cosmosBinary, Type type) => type != typeof (ReadOnlyMemory<byte>) ? TryCatch<object>.FromException((Exception) JsonSerializer.DeserializationVisitor.Exceptions.ExpectedBinary) : TryCatch<object>.FromResult((object) cosmosBinary.Value);

      public TryCatch<object> Visit(CosmosBoolean cosmosBoolean, Type type) => type != typeof (bool) ? TryCatch<object>.FromException((Exception) JsonSerializer.DeserializationVisitor.Exceptions.ExpectedBoolean) : TryCatch<object>.FromResult(cosmosBoolean.Value ? JsonSerializer.DeserializationVisitor.BoxedValues.True : JsonSerializer.DeserializationVisitor.BoxedValues.False);

      public TryCatch<object> Visit(CosmosGuid cosmosGuid, Type type) => type != typeof (Guid) ? TryCatch<object>.FromException((Exception) JsonSerializer.DeserializationVisitor.Exceptions.ExpectedGuid) : TryCatch<object>.FromResult((object) cosmosGuid.Value);

      public TryCatch<object> Visit(CosmosNull cosmosNull, Type type) => type.IsValueType && (!type.IsGenericType || !(type.GetGenericTypeDefinition() == typeof (Nullable<>))) ? TryCatch<object>.FromException((Exception) JsonSerializer.DeserializationVisitor.Exceptions.ExpectedReferenceOrNullableType) : TryCatch<object>.FromResult((object) null);

      public TryCatch<object> Visit(CosmosUndefined cosmosUndefined, Type type) => TryCatch<object>.FromException((Exception) JsonSerializer.DeserializationVisitor.Exceptions.UnexpectedUndefined);

      public TryCatch<object> Visit(CosmosNumber cosmosNumber, Type type)
      {
        if (type == typeof (Number64))
          return TryCatch<object>.FromResult((object) cosmosNumber.Value);
        switch (Type.GetTypeCode(type))
        {
          case TypeCode.SByte:
            if (!cosmosNumber.Value.IsInteger)
              return TryCatch<object>.FromException((Exception) new CosmosElementWrongTypeException("Expected integral type for sbyte."));
            long result1 = Number64.ToLong(cosmosNumber.Value);
            return result1 < (long) sbyte.MinValue || result1 > (long) sbyte.MaxValue ? TryCatch<object>.FromException((Exception) new OverflowException(string.Format("{0} was out of range for sbyte.", (object) result1))) : TryCatch<object>.FromResult((object) (sbyte) result1);
          case TypeCode.Byte:
            if (!cosmosNumber.Value.IsInteger)
              return TryCatch<object>.FromException((Exception) new CosmosElementWrongTypeException("Expected integral type for byte."));
            long result2 = Number64.ToLong(cosmosNumber.Value);
            return result2 < 0L || result2 > (long) byte.MaxValue ? TryCatch<object>.FromException((Exception) new OverflowException(string.Format("{0} was out of range for byte.", (object) result2))) : TryCatch<object>.FromResult((object) (byte) result2);
          case TypeCode.Int16:
            if (!cosmosNumber.Value.IsInteger)
              return TryCatch<object>.FromException((Exception) new CosmosElementWrongTypeException("Expected integral type for short."));
            long result3 = Number64.ToLong(cosmosNumber.Value);
            return result3 < (long) short.MinValue || result3 > (long) short.MaxValue ? TryCatch<object>.FromException((Exception) new OverflowException(string.Format("{0} was out of range for short.", (object) result3))) : TryCatch<object>.FromResult((object) (short) result3);
          case TypeCode.UInt16:
            if (!cosmosNumber.Value.IsInteger)
              return TryCatch<object>.FromException((Exception) new CosmosElementWrongTypeException("Expected integral type for ushort."));
            long result4 = Number64.ToLong(cosmosNumber.Value);
            return result4 < 0L || result4 > (long) ushort.MaxValue ? TryCatch<object>.FromException((Exception) new OverflowException(string.Format("{0} was out of range for ushort.", (object) result4))) : TryCatch<object>.FromResult((object) (ushort) result4);
          case TypeCode.Int32:
            if (!cosmosNumber.Value.IsInteger)
              return TryCatch<object>.FromException((Exception) new CosmosElementWrongTypeException("Expected integral type for int."));
            long result5 = Number64.ToLong(cosmosNumber.Value);
            return result5 < (long) int.MinValue || result5 > (long) int.MaxValue ? TryCatch<object>.FromException((Exception) new OverflowException(string.Format("{0} was out of range for int.", (object) result5))) : TryCatch<object>.FromResult((object) (int) result5);
          case TypeCode.UInt32:
            if (!cosmosNumber.Value.IsInteger)
              return TryCatch<object>.FromException((Exception) new CosmosElementWrongTypeException("Expected integral type for uint."));
            long result6 = Number64.ToLong(cosmosNumber.Value);
            return result6 < 0L || result6 > (long) uint.MaxValue ? TryCatch<object>.FromException((Exception) new OverflowException(string.Format("{0} was out of range for uint.", (object) result6))) : TryCatch<object>.FromResult((object) (uint) result6);
          case TypeCode.Int64:
            return !cosmosNumber.Value.IsInteger ? TryCatch<object>.FromException((Exception) new CosmosElementWrongTypeException("Expected integral type for long.")) : TryCatch<object>.FromResult((object) Number64.ToLong(cosmosNumber.Value));
          case TypeCode.UInt64:
            return !cosmosNumber.Value.IsInteger ? TryCatch<object>.FromException((Exception) new CosmosElementWrongTypeException("Expected integral type for ulong.")) : TryCatch<object>.FromResult((object) (ulong) Number64.ToLong(cosmosNumber.Value));
          case TypeCode.Single:
            if (!cosmosNumber.Value.IsDouble)
              return TryCatch<object>.FromException((Exception) new CosmosElementWrongTypeException("Expected floating point type for float."));
            double result7 = Number64.ToDouble(cosmosNumber.Value);
            return result7 < -3.4028234663852886E+38 || result7 > 3.4028234663852886E+38 ? TryCatch<object>.FromException((Exception) new OverflowException(string.Format("{0} was out of range for float.", (object) result7))) : TryCatch<object>.FromResult((object) (float) result7);
          case TypeCode.Double:
            return !cosmosNumber.Value.IsDouble ? TryCatch<object>.FromException((Exception) new CosmosElementWrongTypeException("Expected floating point type for double.")) : TryCatch<object>.FromResult((object) Number64.ToDouble(cosmosNumber.Value));
          case TypeCode.Decimal:
            return TryCatch<object>.FromResult((object) (!cosmosNumber.Value.IsDouble ? (Decimal) Number64.ToLong(cosmosNumber.Value) : (Decimal) Number64.ToDouble(cosmosNumber.Value)));
          default:
            throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}.", (object) "TypeCode", (object) Type.GetTypeCode(type)));
        }
      }

      public TryCatch<object> Visit(CosmosObject cosmosObject, Type type)
      {
        ConstructorInfo[] constructors = type.GetConstructors();
        if (constructors.Length == 0)
          return TryCatch<object>.FromException((Exception) new CosmosElementNoPubliclyAccessibleConstructorException("Could not find publicly accessible constructors for type: " + type.FullName + "."));
        if (constructors.Length > 1)
          return TryCatch<object>.FromException((Exception) new CosmosElementCouldNotDetermineWhichConstructorToUseException("Could not determine which constructor to use for type: " + type.FullName + "."));
        ConstructorInfo constructorInfo = ((IEnumerable<ConstructorInfo>) constructors).First<ConstructorInfo>();
        ParameterInfo[] parameters = constructorInfo.GetParameters();
        List<object> objectList = new List<object>();
        foreach (ParameterInfo parameterInfo in parameters)
        {
          CosmosElement cosmosElement;
          if (!cosmosObject.TryGetValue(parameterInfo.Name, out cosmosElement))
            return TryCatch<object>.FromException((Exception) new CosmosElementFailedToFindPropertyException("Could not find property: '" + parameterInfo.Name + "'."));
          TryCatch<object> tryCatch = cosmosElement.Accept<Type, TryCatch<object>>((ICosmosElementVisitor<Type, TryCatch<object>>) this, parameterInfo.ParameterType);
          if (tryCatch.Failed)
            return TryCatch<object>.FromException(tryCatch.Exception);
          objectList.Add(tryCatch.Result);
        }
        object result;
        try
        {
          result = constructorInfo.Invoke(objectList.ToArray());
        }
        catch (Exception ex)
        {
          return TryCatch<object>.FromException(ex);
        }
        return TryCatch<object>.FromResult(result);
      }

      public TryCatch<object> Visit(CosmosString cosmosString, Type type) => type != typeof (string) ? TryCatch<object>.FromException((Exception) JsonSerializer.DeserializationVisitor.Exceptions.ExpectedString) : TryCatch<object>.FromResult((object) cosmosString.Value.ToString());

      private static class Exceptions
      {
        public static readonly CosmosElementWrongTypeException ExpectedArray = new CosmosElementWrongTypeException("Expected return type of 'IReadOnlyList'.");
        public static readonly CosmosElementWrongTypeException ExpectedBoolean = new CosmosElementWrongTypeException(string.Format("Expected return type of '{0}'.", (object) typeof (bool)));
        public static readonly CosmosElementWrongTypeException ExpectedBinary = new CosmosElementWrongTypeException(string.Format("Expected return type of '{0}'.", (object) typeof (ReadOnlyMemory<byte>)));
        public static readonly CosmosElementWrongTypeException ExpectedGuid = new CosmosElementWrongTypeException(string.Format("Expected return type of '{0}'.", (object) typeof (Guid)));
        public static readonly CosmosElementWrongTypeException ExpectedNumber = new CosmosElementWrongTypeException("Expected return type of number.");
        public static readonly CosmosElementWrongTypeException ExpectedReferenceOrNullableType = new CosmosElementWrongTypeException("Expected return type to be a reference or nullable type.");
        public static readonly CosmosElementWrongTypeException ExpectedString = new CosmosElementWrongTypeException(string.Format("Expected return type of '{0}'.", (object) typeof (string)));
        public static readonly CosmosElementWrongTypeException UnexpectedUndefined = new CosmosElementWrongTypeException(string.Format("Did not expect to encounter '{0}'.", (object) typeof (CosmosUndefined)));
      }

      private static class BoxedValues
      {
        public static readonly object True = (object) true;
        public static readonly object False = (object) false;
        public static readonly object Null = (object) null;
      }
    }

    private sealed class ReadOnlyListWrapper<T> : 
      IReadOnlyList<T>,
      IEnumerable<T>,
      IEnumerable,
      IReadOnlyCollection<T>
    {
      private readonly IList<T> list;

      public ReadOnlyListWrapper(IList<T> list) => this.list = list ?? throw new ArgumentNullException(nameof (list));

      public T this[int index] => this.list[index];

      public int Count => this.list.Count;

      public IEnumerator<T> GetEnumerator() => this.list.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.list.GetEnumerator();
    }
  }
}
