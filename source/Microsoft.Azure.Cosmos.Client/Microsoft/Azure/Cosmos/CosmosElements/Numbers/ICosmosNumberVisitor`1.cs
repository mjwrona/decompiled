// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.Numbers.ICosmosNumberVisitor`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements.Numbers
{
  internal interface ICosmosNumberVisitor<TResult>
  {
    TResult Visit(CosmosNumber64 cosmosNumber64);

    TResult Visit(CosmosInt8 cosmosInt8);

    TResult Visit(CosmosInt16 cosmosInt16);

    TResult Visit(CosmosInt32 cosmosInt32);

    TResult Visit(CosmosInt64 cosmosInt64);

    TResult Visit(CosmosUInt32 cosmosUInt32);

    TResult Visit(CosmosFloat32 cosmosFloat32);

    TResult Visit(CosmosFloat64 cosmosFloat64);
  }
}
