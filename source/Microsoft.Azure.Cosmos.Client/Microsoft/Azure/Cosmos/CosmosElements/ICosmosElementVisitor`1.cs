// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.ICosmosElementVisitor`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements
{
  internal interface ICosmosElementVisitor<TResult>
  {
    TResult Visit(CosmosArray cosmosArray);

    TResult Visit(CosmosBinary cosmosBinary);

    TResult Visit(CosmosBoolean cosmosBoolean);

    TResult Visit(CosmosGuid cosmosGuid);

    TResult Visit(CosmosNull cosmosNull);

    TResult Visit(CosmosNumber cosmosNumber);

    TResult Visit(CosmosObject cosmosObject);

    TResult Visit(CosmosString cosmosString);

    TResult Visit(CosmosUndefined cosmosUndefined);
  }
}
