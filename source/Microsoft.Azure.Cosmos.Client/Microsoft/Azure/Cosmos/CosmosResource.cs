// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosResource
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.IO;

namespace Microsoft.Azure.Cosmos
{
  internal static class CosmosResource
  {
    private static readonly CosmosJsonDotNetSerializer cosmosDefaultJsonSerializer = new CosmosJsonDotNetSerializer();

    internal static T FromStream<T>(DocumentServiceResponse response)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      return response.ResponseBody != null && (!response.ResponseBody.CanSeek || response.ResponseBody.Length > 0L) ? CosmosResource.FromStream<T>(response.ResponseBody) : default (T);
    }

    internal static Stream ToStream<T>(T input) => CosmosResource.cosmosDefaultJsonSerializer.ToStream<T>(input);

    internal static T FromStream<T>(Stream stream) => CosmosResource.cosmosDefaultJsonSerializer.FromStream<T>(stream);
  }
}
