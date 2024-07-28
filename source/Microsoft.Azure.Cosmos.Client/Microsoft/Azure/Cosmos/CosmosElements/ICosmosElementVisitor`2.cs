// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.ICosmosElementVisitor`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements
{
  internal interface ICosmosElementVisitor<TArg, TResult>
  {
    TResult Visit(CosmosArray cosmosArray, TArg input);

    TResult Visit(CosmosBinary cosmosBinary, TArg input);

    TResult Visit(CosmosBoolean cosmosBoolean, TArg input);

    TResult Visit(CosmosGuid cosmosGuid, TArg input);

    TResult Visit(CosmosNull cosmosNull, TArg input);

    TResult Visit(CosmosNumber cosmosNumber, TArg input);

    TResult Visit(CosmosObject cosmosObject, TArg input);

    TResult Visit(CosmosString cosmosString, TArg input);

    TResult Visit(CosmosUndefined cosmosUndefined, TArg input);
  }
}
