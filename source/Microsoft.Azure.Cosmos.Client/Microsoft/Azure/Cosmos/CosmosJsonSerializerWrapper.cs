// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosJsonSerializerWrapper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.IO;

namespace Microsoft.Azure.Cosmos
{
  internal class CosmosJsonSerializerWrapper : CosmosSerializer
  {
    internal CosmosSerializer InternalJsonSerializer { get; }

    public CosmosJsonSerializerWrapper(CosmosSerializer cosmosJsonSerializer) => this.InternalJsonSerializer = cosmosJsonSerializer ?? throw new ArgumentNullException(nameof (cosmosJsonSerializer));

    public override T FromStream<T>(Stream stream)
    {
      T obj = this.InternalJsonSerializer.FromStream<T>(stream);
      if (!stream.CanRead)
        return obj;
      throw new InvalidOperationException("Json Serializer left an open stream.");
    }

    public override Stream ToStream<T>(T input)
    {
      Stream stream = this.InternalJsonSerializer.ToStream<T>(input);
      if (stream == null)
        throw new InvalidOperationException("Json Serializer returned a null stream.");
      return stream.CanRead ? stream : throw new InvalidOperationException("Json Serializer returned a closed stream.");
    }
  }
}
