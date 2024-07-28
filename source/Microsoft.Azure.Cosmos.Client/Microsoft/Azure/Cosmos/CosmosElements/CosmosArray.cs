// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.CosmosArray
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

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
  internal abstract class CosmosArray : 
    CosmosElement,
    IReadOnlyList<CosmosElement>,
    IEnumerable<CosmosElement>,
    IEnumerable,
    IReadOnlyCollection<CosmosElement>,
    IEquatable<CosmosArray>,
    IComparable<CosmosArray>
  {
    public static readonly CosmosArray Empty = (CosmosArray) new CosmosArray.EagerCosmosArray(Enumerable.Empty<CosmosElement>());
    private const uint HashSeed = 2533142560;

    public abstract int Count { get; }

    public abstract CosmosElement this[int index] { get; }

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

    public override bool Equals(CosmosElement cosmosElement) => cosmosElement is CosmosArray cosmosArray && this.Equals(cosmosArray);

    public bool Equals(CosmosArray cosmosArray)
    {
      if (this.Count != cosmosArray.Count)
        return false;
      foreach ((CosmosElement, CosmosElement) tuple in this.Zip<CosmosElement, CosmosElement, (CosmosElement, CosmosElement)>((IEnumerable<CosmosElement>) cosmosArray, (Func<CosmosElement, CosmosElement, (CosmosElement, CosmosElement)>) ((first, second) => (first, second))))
      {
        if (tuple.Item1 != tuple.Item2)
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      uint seed = 2533142560;
      for (int index = 0; index < this.Count; ++index)
        seed = MurmurHash3.Hash32<int>(this[index].GetHashCode(), seed);
      return (int) seed;
    }

    public int CompareTo(CosmosArray cosmosArray)
    {
      UInt128 hash1 = DistinctHash.GetHash((CosmosElement) this);
      UInt128 hash2 = DistinctHash.GetHash((CosmosElement) cosmosArray);
      return UInt128BinaryComparer.Singleton.Compare(hash1, hash2);
    }

    public static CosmosArray Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosArray) new CosmosArray.LazyCosmosArray(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosArray Create(IEnumerable<CosmosElement> cosmosElements) => (CosmosArray) new CosmosArray.EagerCosmosArray(cosmosElements);

    public static CosmosArray Create(params CosmosElement[] cosmosElements) => (CosmosArray) new CosmosArray.EagerCosmosArray((IEnumerable<CosmosElement>) cosmosElements);

    public static CosmosArray Create() => CosmosArray.Empty;

    public abstract CosmosArray.Enumerator GetEnumerator();

    IEnumerator<CosmosElement> IEnumerable<CosmosElement>.GetEnumerator() => (IEnumerator<CosmosElement>) this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public static CosmosArray CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.CreateFromBuffer<CosmosArray>(buffer);

    public static CosmosArray Parse(string json) => CosmosElement.Parse<CosmosArray>(json);

    public static bool TryCreateFromBuffer(ReadOnlyMemory<byte> buffer, out CosmosArray cosmosArray) => CosmosElement.TryCreateFromBuffer<CosmosArray>(buffer, out cosmosArray);

    public static bool TryParse(string json, out CosmosArray cosmosArray) => CosmosElement.TryParse<CosmosArray>(json, out cosmosArray);

    public new static class Monadic
    {
      public static TryCatch<CosmosArray> CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.Monadic.CreateFromBuffer<CosmosArray>(buffer);

      public static TryCatch<CosmosArray> Parse(string json) => CosmosElement.Monadic.Parse<CosmosArray>(json);
    }

    public struct Enumerator : IEnumerator<CosmosElement>, IEnumerator, IDisposable
    {
      private List<CosmosElement>.Enumerator innerEnumerator;

      internal Enumerator(List<CosmosElement>.Enumerator innerEnumerator) => this.innerEnumerator = innerEnumerator;

      public CosmosElement Current => this.innerEnumerator.Current;

      object IEnumerator.Current => (object) this.innerEnumerator.Current;

      public void Dispose() => this.innerEnumerator.Dispose();

      public bool MoveNext() => this.innerEnumerator.MoveNext();

      public void Reset() => throw new NotImplementedException();
    }

    private sealed class EagerCosmosArray : CosmosArray
    {
      private readonly List<CosmosElement> cosmosElements;

      public EagerCosmosArray(IEnumerable<CosmosElement> elements) => this.cosmosElements = new List<CosmosElement>(elements);

      public override int Count => this.cosmosElements.Count;

      public override CosmosElement this[int index] => this.cosmosElements[index];

      public override CosmosArray.Enumerator GetEnumerator() => new CosmosArray.Enumerator(this.cosmosElements.GetEnumerator());

      public override void WriteTo(IJsonWriter jsonWriter)
      {
        jsonWriter.WriteArrayStart();
        foreach (CosmosElement cosmosElement in (CosmosArray) this)
          cosmosElement.WriteTo(jsonWriter);
        jsonWriter.WriteArrayEnd();
      }
    }

    private sealed class LazyCosmosArray : CosmosArray
    {
      private readonly Lazy<List<CosmosElement>> lazyCosmosElementArray;
      private readonly IJsonNavigator jsonNavigator;
      private readonly IJsonNavigatorNode jsonNavigatorNode;

      public LazyCosmosArray(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        JsonNodeType nodeType = jsonNavigator.GetNodeType(jsonNavigatorNode);
        if (nodeType != JsonNodeType.Array)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be an {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.Array, (object) nodeType));
        this.jsonNavigator = jsonNavigator;
        this.jsonNavigatorNode = jsonNavigatorNode;
        this.lazyCosmosElementArray = new Lazy<List<CosmosElement>>((Func<List<CosmosElement>>) (() =>
        {
          List<CosmosElement> cosmosElementList = new List<CosmosElement>(jsonNavigator.GetArrayItemCount(jsonNavigatorNode));
          foreach (IJsonNavigatorNode arrayItem in jsonNavigator.GetArrayItems(jsonNavigatorNode))
            cosmosElementList.Add(CosmosElement.Dispatch(jsonNavigator, arrayItem));
          return cosmosElementList;
        }));
      }

      public override int Count => this.lazyCosmosElementArray.Value.Count;

      public override CosmosElement this[int index] => this.lazyCosmosElementArray.Value[index];

      public override CosmosArray.Enumerator GetEnumerator() => new CosmosArray.Enumerator(this.lazyCosmosElementArray.Value.GetEnumerator());

      public override void WriteTo(IJsonWriter jsonWriter) => this.jsonNavigator.WriteNode(this.jsonNavigatorNode, jsonWriter);

      public override IJsonReader CreateReader() => this.jsonNavigator.CreateReader(this.jsonNavigatorNode);
    }
  }
}
