// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.CosmosUndefined
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements
{
  internal sealed class CosmosUndefined : 
    CosmosElement,
    IEquatable<CosmosUndefined>,
    IComparable<CosmosUndefined>
  {
    private static readonly CosmosUndefined Instance = new CosmosUndefined();

    private CosmosUndefined()
    {
    }

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

    public int CompareTo(CosmosUndefined other) => 0;

    public override bool Equals(CosmosElement cosmosElement) => cosmosElement is CosmosUndefined;

    public bool Equals(CosmosUndefined other) => true;

    public override int GetHashCode() => 0;

    public override void WriteTo(IJsonWriter jsonWriter)
    {
    }

    public static CosmosUndefined Create() => CosmosUndefined.Instance;
  }
}
