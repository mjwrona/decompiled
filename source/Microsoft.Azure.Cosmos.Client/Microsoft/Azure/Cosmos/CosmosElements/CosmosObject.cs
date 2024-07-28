// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.CosmosObject
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Distinct;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements
{
  internal abstract class CosmosObject : 
    CosmosElement,
    IReadOnlyDictionary<string, CosmosElement>,
    IEnumerable<KeyValuePair<string, CosmosElement>>,
    IEnumerable,
    IReadOnlyCollection<KeyValuePair<string, CosmosElement>>,
    IEquatable<CosmosObject>,
    IComparable<CosmosObject>
  {
    private const uint HashSeed = 1275696788;
    private const uint NameHashSeed = 263659187;

    public abstract CosmosObject.KeyCollection Keys { get; }

    IEnumerable<string> IReadOnlyDictionary<string, CosmosElement>.Keys => (IEnumerable<string>) this.Keys;

    public abstract CosmosObject.ValueCollection Values { get; }

    IEnumerable<CosmosElement> IReadOnlyDictionary<string, CosmosElement>.Values => (IEnumerable<CosmosElement>) this.Values;

    public abstract int Count { get; }

    public abstract CosmosElement this[string key] { get; }

    public abstract bool ContainsKey(string key);

    public abstract bool TryGetValue(string key, out CosmosElement value);

    public override void Accept(ICosmosElementVisitor cosmosElementVisitor) => cosmosElementVisitor.Visit(this);

    public override TResult Accept<TResult>(
      ICosmosElementVisitor<TResult> cosmosElementVisitor)
    {
      return cosmosElementVisitor.Visit(this);
    }

    public override TResult Accept<TArg, TResult>(
      ICosmosElementVisitor<TArg, TResult> cosmosElementVisitor,
      TArg input)
    {
      return cosmosElementVisitor.Visit(this, input);
    }

    public bool TryGetValue<TCosmosElement>(string key, out TCosmosElement typedCosmosElement) where TCosmosElement : CosmosElement
    {
      CosmosElement cosmosElement1;
      if (!this.TryGetValue(key, out cosmosElement1))
      {
        typedCosmosElement = default (TCosmosElement);
        return false;
      }
      if (!(cosmosElement1 is TCosmosElement cosmosElement2))
      {
        typedCosmosElement = default (TCosmosElement);
        return false;
      }
      typedCosmosElement = cosmosElement2;
      return true;
    }

    public abstract CosmosObject.Enumerator GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    IEnumerator<KeyValuePair<string, CosmosElement>> IEnumerable<KeyValuePair<string, CosmosElement>>.GetEnumerator() => (IEnumerator<KeyValuePair<string, CosmosElement>>) this.GetEnumerator();

    public override bool Equals(CosmosElement cosmosElement) => cosmosElement is CosmosObject cosmosObject && this.Equals(cosmosObject);

    public bool Equals(CosmosObject cosmosObject)
    {
      if (this.Count != cosmosObject.Count)
        return false;
      foreach (KeyValuePair<string, CosmosElement> keyValuePair in this)
      {
        string key = keyValuePair.Key;
        CosmosElement cosmosElement1 = keyValuePair.Value;
        CosmosElement cosmosElement2;
        if (!cosmosObject.TryGetValue(key, out cosmosElement2) || cosmosElement1 != cosmosElement2)
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      uint hashCode = 1275696788;
      foreach (KeyValuePair<string, CosmosElement> keyValuePair in this)
      {
        uint seed = MurmurHash3.Hash32(keyValuePair.Key, 263659187U);
        uint num = MurmurHash3.Hash32<int>(keyValuePair.Value.GetHashCode(), seed);
        hashCode ^= num;
      }
      return (int) hashCode;
    }

    public int CompareTo(CosmosObject cosmosObject)
    {
      UInt128 hash1 = DistinctHash.GetHash((CosmosElement) this);
      UInt128 hash2 = DistinctHash.GetHash((CosmosElement) cosmosObject);
      return UInt128BinaryComparer.Singleton.Compare(hash1, hash2);
    }

    public static CosmosObject Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosObject) new CosmosObject.LazyCosmosObject(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosObject Create(
      IReadOnlyDictionary<string, CosmosElement> dictionary)
    {
      return (CosmosObject) new CosmosObject.EagerCosmosObject(dictionary);
    }

    public static CosmosObject CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.CreateFromBuffer<CosmosObject>(buffer);

    public static CosmosObject Parse(string json) => CosmosElement.Parse<CosmosObject>(json);

    public static bool TryCreateFromBuffer(
      ReadOnlyMemory<byte> buffer,
      out CosmosObject cosmosObject)
    {
      return CosmosElement.TryCreateFromBuffer<CosmosObject>(buffer, out cosmosObject);
    }

    public static bool TryParse(string json, out CosmosObject cosmosObject) => CosmosElement.TryParse<CosmosObject>(json, out cosmosObject);

    public new static class Monadic
    {
      public static TryCatch<CosmosObject> CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.Monadic.CreateFromBuffer<CosmosObject>(buffer);

      public static TryCatch<CosmosObject> Parse(string json) => CosmosElement.Monadic.Parse<CosmosObject>(json);
    }

    public struct Enumerator : 
      IEnumerator<KeyValuePair<string, CosmosElement>>,
      IEnumerator,
      IDisposable
    {
      private Dictionary<string, CosmosElement>.Enumerator innerEnumerator;

      internal Enumerator(
        Dictionary<string, CosmosElement>.Enumerator innerEnumerator)
      {
        this.innerEnumerator = innerEnumerator;
      }

      public KeyValuePair<string, CosmosElement> Current => this.innerEnumerator.Current;

      object IEnumerator.Current => (object) this.innerEnumerator.Current;

      public void Dispose() => this.innerEnumerator.Dispose();

      public bool MoveNext() => this.innerEnumerator.MoveNext();

      public void Reset() => throw new NotImplementedException();
    }

    public struct KeyCollection : IEnumerable<string>, IEnumerable
    {
      private Dictionary<string, CosmosElement>.KeyCollection innerCollection;

      internal KeyCollection(
        Dictionary<string, CosmosElement>.KeyCollection innerCollection)
      {
        this.innerCollection = innerCollection;
      }

      public CosmosObject.KeyCollection.Enumerator GetEnumerator() => new CosmosObject.KeyCollection.Enumerator(this.innerCollection.GetEnumerator());

      IEnumerator<string> IEnumerable<string>.GetEnumerator() => (IEnumerator<string>) this.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

      public struct Enumerator : IEnumerator<string>, IEnumerator, IDisposable
      {
        private Dictionary<string, CosmosElement>.KeyCollection.Enumerator innerEnumerator;

        internal Enumerator(
          Dictionary<string, CosmosElement>.KeyCollection.Enumerator innerEnumerator)
        {
          this.innerEnumerator = innerEnumerator;
        }

        public string Current => this.innerEnumerator.Current;

        object IEnumerator.Current => (object) this.innerEnumerator.Current;

        public void Dispose() => this.innerEnumerator.Dispose();

        public bool MoveNext() => this.innerEnumerator.MoveNext();

        public void Reset() => throw new NotImplementedException();
      }
    }

    public struct ValueCollection : IEnumerable<CosmosElement>, IEnumerable
    {
      private Dictionary<string, CosmosElement>.ValueCollection innerCollection;

      internal ValueCollection(
        Dictionary<string, CosmosElement>.ValueCollection innerCollection)
      {
        this.innerCollection = innerCollection;
      }

      public CosmosObject.ValueCollection.Enumerator GetEnumerator() => new CosmosObject.ValueCollection.Enumerator(this.innerCollection.GetEnumerator());

      IEnumerator<CosmosElement> IEnumerable<CosmosElement>.GetEnumerator() => (IEnumerator<CosmosElement>) this.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

      public struct Enumerator : IEnumerator<CosmosElement>, IEnumerator, IDisposable
      {
        private Dictionary<string, CosmosElement>.ValueCollection.Enumerator innerEnumerator;

        internal Enumerator(
          Dictionary<string, CosmosElement>.ValueCollection.Enumerator innerEnumerator)
        {
          this.innerEnumerator = innerEnumerator;
        }

        public CosmosElement Current => this.innerEnumerator.Current;

        object IEnumerator.Current => (object) this.innerEnumerator.Current;

        public void Dispose() => this.innerEnumerator.Dispose();

        public bool MoveNext() => this.innerEnumerator.MoveNext();

        public void Reset() => throw new NotImplementedException();
      }
    }

    private sealed class EagerCosmosObject : CosmosObject
    {
      private readonly Dictionary<string, CosmosElement> dictionary;

      public EagerCosmosObject(
        IReadOnlyDictionary<string, CosmosElement> dictionary)
      {
        this.dictionary = new Dictionary<string, CosmosElement>((IDictionary<string, CosmosElement>) dictionary.ToDictionary<KeyValuePair<string, CosmosElement>, string, CosmosElement>((Func<KeyValuePair<string, CosmosElement>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, CosmosElement>, CosmosElement>) (kvp => kvp.Value)));
      }

      public override CosmosElement this[string key] => this.dictionary[key];

      public override int Count => this.dictionary.Count;

      public override CosmosObject.KeyCollection Keys => new CosmosObject.KeyCollection(this.dictionary.Keys);

      public override CosmosObject.ValueCollection Values => new CosmosObject.ValueCollection(this.dictionary.Values);

      public override bool ContainsKey(string key) => this.dictionary.ContainsKey(key);

      public override CosmosObject.Enumerator GetEnumerator() => new CosmosObject.Enumerator(this.dictionary.GetEnumerator());

      public override bool TryGetValue(string key, out CosmosElement value) => this.dictionary.TryGetValue(key, out value);

      public override void WriteTo(IJsonWriter jsonWriter)
      {
        jsonWriter.WriteObjectStart();
        foreach (KeyValuePair<string, CosmosElement> keyValuePair in this.dictionary)
        {
          if (!(keyValuePair.Value is CosmosUndefined))
          {
            jsonWriter.WriteFieldName(keyValuePair.Key);
            keyValuePair.Value.WriteTo(jsonWriter);
          }
        }
        jsonWriter.WriteObjectEnd();
      }
    }

    private sealed class LazyCosmosObject : CosmosObject
    {
      private readonly IJsonNavigator jsonNavigator;
      private readonly IJsonNavigatorNode jsonNavigatorNode;
      private readonly Lazy<Dictionary<string, CosmosElement>> lazyCache;

      public LazyCosmosObject(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        JsonNodeType nodeType = jsonNavigator.GetNodeType(jsonNavigatorNode);
        if (nodeType != JsonNodeType.Object)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be a {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.Object, (object) nodeType));
        this.jsonNavigator = jsonNavigator;
        this.jsonNavigatorNode = jsonNavigatorNode;
        this.lazyCache = new Lazy<Dictionary<string, CosmosElement>>((Func<Dictionary<string, CosmosElement>>) (() =>
        {
          Dictionary<string, CosmosElement> dictionary = new Dictionary<string, CosmosElement>(this.jsonNavigator.GetObjectPropertyCount(this.jsonNavigatorNode));
          foreach (ObjectProperty objectProperty in this.jsonNavigator.GetObjectProperties(this.jsonNavigatorNode))
          {
            string key = UtfAnyString.op_Implicit(this.jsonNavigator.GetStringValue(objectProperty.NameNode));
            CosmosElement cosmosElement = CosmosElement.Dispatch(this.jsonNavigator, objectProperty.ValueNode);
            dictionary[key] = cosmosElement;
          }
          return dictionary;
        }));
      }

      public override CosmosObject.KeyCollection Keys => new CosmosObject.KeyCollection(this.lazyCache.Value.Keys);

      public override CosmosObject.ValueCollection Values => new CosmosObject.ValueCollection(this.lazyCache.Value.Values);

      public override int Count => this.lazyCache.Value.Count;

      public override CosmosElement this[string key] => this.lazyCache.Value[key];

      public override bool ContainsKey(string key) => this.lazyCache.Value.ContainsKey(key);

      public override CosmosObject.Enumerator GetEnumerator() => new CosmosObject.Enumerator(this.lazyCache.Value.GetEnumerator());

      public override bool TryGetValue(string key, out CosmosElement value) => this.lazyCache.Value.TryGetValue(key, out value);

      public override void WriteTo(IJsonWriter jsonWriter) => this.jsonNavigator.WriteNode(this.jsonNavigatorNode, jsonWriter);

      public override IJsonReader CreateReader() => this.jsonNavigator.CreateReader(this.jsonNavigatorNode);
    }
  }
}
