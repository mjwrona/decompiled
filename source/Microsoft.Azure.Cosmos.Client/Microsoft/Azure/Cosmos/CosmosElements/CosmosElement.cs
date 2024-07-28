// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.CosmosElement
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements.Numbers;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Json.Interop;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements
{
  [JsonConverter(typeof (CosmosElementJsonConverter))]
  internal abstract class CosmosElement : IEquatable<CosmosElement>, IComparable<CosmosElement>
  {
    protected static readonly Newtonsoft.Json.JsonSerializer DefaultSerializer = new Newtonsoft.Json.JsonSerializer()
    {
      Culture = CultureInfo.InvariantCulture,
      DateParseHandling = DateParseHandling.None
    };

    public override string ToString()
    {
      IJsonWriter jsonWriter = Microsoft.Azure.Cosmos.Json.JsonWriter.Create(JsonSerializationFormat.Text);
      this.WriteTo(jsonWriter);
      return Utf8StringHelpers.ToString(jsonWriter.GetResult());
    }

    public override bool Equals(object obj)
    {
      CosmosElement cosmosElement = obj as CosmosElement;
      return (object) cosmosElement != null && this.Equals(cosmosElement);
    }

    public abstract bool Equals(CosmosElement cosmosElement);

    public abstract override int GetHashCode();

    public int CompareTo(CosmosElement other)
    {
      int num1 = this.Accept<int>((ICosmosElementVisitor<int>) CosmosElement.CosmosElementToTypeOrder.Singleton);
      int num2 = other.Accept<int>((ICosmosElementVisitor<int>) CosmosElement.CosmosElementToTypeOrder.Singleton);
      return num1 != num2 ? num1.CompareTo(num2) : this.Accept<CosmosElement, int>((ICosmosElementVisitor<CosmosElement, int>) CosmosElement.CosmosElementWithinTypeComparer.Singleton, other);
    }

    public abstract void WriteTo(IJsonWriter jsonWriter);

    public abstract void Accept(ICosmosElementVisitor cosmosElementVisitor);

    public abstract TResult Accept<TResult>(
      ICosmosElementVisitor<TResult> cosmosElementVisitor);

    public abstract TResult Accept<TArg, TResult>(
      ICosmosElementVisitor<TArg, TResult> cosmosElementVisitor,
      TArg input);

    public virtual T Materialize<T>()
    {
      Newtonsoft.Json.JsonReader reader = (Newtonsoft.Json.JsonReader) new CosmosDBToNewtonsoftReader(this.CreateReader());
      return CosmosElement.DefaultSerializer.Deserialize<T>(reader);
    }

    public virtual IJsonReader CreateReader()
    {
      IJsonWriter jsonWriter = Microsoft.Azure.Cosmos.Json.JsonWriter.Create(JsonSerializationFormat.Binary);
      this.WriteTo(jsonWriter);
      return Microsoft.Azure.Cosmos.Json.JsonReader.Create(jsonWriter.GetResult());
    }

    public static TCosmosElement CreateFromBuffer<TCosmosElement>(ReadOnlyMemory<byte> buffer) where TCosmosElement : CosmosElement
    {
      TryCatch<TCosmosElement> fromBuffer = CosmosElement.Monadic.CreateFromBuffer<TCosmosElement>(buffer);
      fromBuffer.ThrowIfFailed();
      return fromBuffer.Result;
    }

    public static CosmosElement CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.CreateFromBuffer<CosmosElement>(buffer);

    public static bool TryCreateFromBuffer<TCosmosElement>(
      ReadOnlyMemory<byte> buffer,
      out TCosmosElement cosmosElement)
      where TCosmosElement : CosmosElement
    {
      TryCatch<TCosmosElement> fromBuffer = CosmosElement.Monadic.CreateFromBuffer<TCosmosElement>(buffer);
      if (fromBuffer.Failed)
      {
        cosmosElement = default (TCosmosElement);
        return false;
      }
      cosmosElement = fromBuffer.Result;
      return true;
    }

    public static CosmosElement Parse(string json)
    {
      TryCatch<CosmosElement> tryCatch = CosmosElement.Monadic.Parse(json);
      tryCatch.ThrowIfFailed();
      return tryCatch.Result;
    }

    public static TCosmosElement Parse<TCosmosElement>(string json) where TCosmosElement : CosmosElement
    {
      TryCatch<TCosmosElement> tryCatch = CosmosElement.Monadic.Parse<TCosmosElement>(json);
      tryCatch.ThrowIfFailed();
      return tryCatch.Result;
    }

    public static bool TryParse<TCosmosElement>(string json, out TCosmosElement cosmosElement) where TCosmosElement : CosmosElement
    {
      TryCatch<TCosmosElement> tryCatch = CosmosElement.Monadic.Parse<TCosmosElement>(json);
      if (tryCatch.Failed)
      {
        cosmosElement = default (TCosmosElement);
        return false;
      }
      cosmosElement = tryCatch.Result;
      return true;
    }

    public static CosmosElement Dispatch(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      JsonNodeType nodeType = jsonNavigator.GetNodeType(jsonNavigatorNode);
      switch (nodeType)
      {
        case JsonNodeType.Null:
          return (CosmosElement) CosmosNull.Create();
        case JsonNodeType.False:
          return (CosmosElement) CosmosBoolean.Create(false);
        case JsonNodeType.True:
          return (CosmosElement) CosmosBoolean.Create(true);
        case JsonNodeType.Number64:
          return (CosmosElement) CosmosNumber64.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.String:
          return (CosmosElement) CosmosString.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.Array:
          return (CosmosElement) CosmosArray.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.Object:
          return (CosmosElement) CosmosObject.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.FieldName:
          return (CosmosElement) CosmosString.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.Int8:
          return (CosmosElement) CosmosInt8.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.Int16:
          return (CosmosElement) CosmosInt16.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.Int32:
          return (CosmosElement) CosmosInt32.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.Int64:
          return (CosmosElement) CosmosInt64.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.UInt32:
          return (CosmosElement) CosmosUInt32.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.Float32:
          return (CosmosElement) CosmosFloat32.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.Float64:
          return (CosmosElement) CosmosFloat64.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.Binary:
          return (CosmosElement) CosmosBinary.Create(jsonNavigator, jsonNavigatorNode);
        case JsonNodeType.Guid:
          return (CosmosElement) CosmosGuid.Create(jsonNavigator, jsonNavigatorNode);
        default:
          throw new ArgumentException(string.Format("Unknown {0}: {1}", (object) "JsonNodeType", (object) nodeType));
      }
    }

    public static bool operator ==(CosmosElement a, CosmosElement b)
    {
      if ((object) a == (object) b)
        return true;
      return (object) a != null && (object) b != null && a.Equals(b);
    }

    public static bool operator !=(CosmosElement a, CosmosElement b) => !(a == b);

    public static class Monadic
    {
      public static TryCatch<TCosmosElement> CreateFromBuffer<TCosmosElement>(
        ReadOnlyMemory<byte> buffer)
        where TCosmosElement : CosmosElement
      {
        if (buffer.IsEmpty)
          TryCatch<TCosmosElement>.FromException((Exception) new ArgumentException("buffer must not be empty."));
        CosmosElement cosmosElement;
        try
        {
          IJsonNavigator jsonNavigator = JsonNavigator.Create(buffer);
          cosmosElement = CosmosElement.Dispatch(jsonNavigator, jsonNavigator.GetRootNode());
        }
        catch (JsonParseException ex)
        {
          return TryCatch<TCosmosElement>.FromException((Exception) ex);
        }
        return !(cosmosElement is TCosmosElement result) ? TryCatch<TCosmosElement>.FromException((Exception) new CosmosElementWrongTypeException(string.Format("buffer was incorrect cosmos element type: {0} when {1} was requested.", (object) cosmosElement.GetType(), (object) typeof (TCosmosElement)))) : TryCatch<TCosmosElement>.FromResult(result);
      }

      public static TryCatch<CosmosElement> CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.Monadic.CreateFromBuffer<CosmosElement>(buffer);

      public static TryCatch<TCosmosElement> Parse<TCosmosElement>(string serializedCosmosElement) where TCosmosElement : CosmosElement
      {
        if (serializedCosmosElement == null)
          throw new ArgumentNullException(nameof (serializedCosmosElement));
        return string.IsNullOrWhiteSpace(serializedCosmosElement) ? TryCatch<TCosmosElement>.FromException((Exception) new ArgumentException("'serializedCosmosElement' must not be null, empty, or whitespace.")) : CosmosElement.Monadic.CreateFromBuffer<TCosmosElement>((ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(serializedCosmosElement));
      }

      public static TryCatch<CosmosElement> Parse(string serializedCosmosElement) => CosmosElement.Monadic.Parse<CosmosElement>(serializedCosmosElement);
    }

    private sealed class CosmosElementToTypeOrder : ICosmosElementVisitor<int>
    {
      public static readonly CosmosElement.CosmosElementToTypeOrder Singleton = new CosmosElement.CosmosElementToTypeOrder();

      private CosmosElementToTypeOrder()
      {
      }

      public int Visit(CosmosUndefined cosmosUndefined) => 0;

      public int Visit(CosmosNull cosmosNull) => 1;

      public int Visit(CosmosBoolean cosmosBoolean) => 2;

      public int Visit(CosmosNumber cosmosNumber) => 3;

      public int Visit(CosmosString cosmosString) => 4;

      public int Visit(CosmosArray cosmosArray) => 5;

      public int Visit(CosmosObject cosmosObject) => 6;

      public int Visit(CosmosGuid cosmosGuid) => 7;

      public int Visit(CosmosBinary cosmosBinary) => 8;
    }

    private sealed class CosmosElementWithinTypeComparer : ICosmosElementVisitor<CosmosElement, int>
    {
      public static readonly CosmosElement.CosmosElementWithinTypeComparer Singleton = new CosmosElement.CosmosElementWithinTypeComparer();

      private CosmosElementWithinTypeComparer()
      {
      }

      public int Visit(CosmosUndefined cosmosUndefined, CosmosElement input) => cosmosUndefined.CompareTo((CosmosUndefined) input);

      public int Visit(CosmosArray cosmosArray, CosmosElement input) => cosmosArray.CompareTo((CosmosArray) input);

      public int Visit(CosmosBinary cosmosBinary, CosmosElement input) => cosmosBinary.CompareTo((CosmosBinary) input);

      public int Visit(CosmosBoolean cosmosBoolean, CosmosElement input) => cosmosBoolean.CompareTo((CosmosBoolean) input);

      public int Visit(CosmosGuid cosmosGuid, CosmosElement input) => cosmosGuid.CompareTo((CosmosGuid) input);

      public int Visit(CosmosNull cosmosNull, CosmosElement input) => cosmosNull.CompareTo((CosmosNull) input);

      public int Visit(CosmosNumber cosmosNumber, CosmosElement input) => cosmosNumber.CompareTo((CosmosNumber) input);

      public int Visit(CosmosObject cosmosObject, CosmosElement input) => cosmosObject.CompareTo((CosmosObject) input);

      public int Visit(CosmosString cosmosString, CosmosElement input) => cosmosString.CompareTo((CosmosString) input);
    }
  }
}
