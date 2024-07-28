// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosSerializationFormatOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using System;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class CosmosSerializationFormatOptions
  {
    public string ContentSerializationFormat { get; }

    public CosmosSerializationFormatOptions.CreateCustomNavigator CreateCustomNavigatorCallback { get; }

    public CosmosSerializationFormatOptions.CreateCustomWriter CreateCustomWriterCallback { get; }

    public CosmosSerializationFormatOptions(
      string contentSerializationFormat,
      CosmosSerializationFormatOptions.CreateCustomNavigator createCustomNavigator,
      CosmosSerializationFormatOptions.CreateCustomWriter createCustomWriter)
    {
      if (contentSerializationFormat == null)
        throw new ArgumentNullException(nameof (contentSerializationFormat));
      if (createCustomNavigator == null)
        throw new ArgumentNullException(nameof (createCustomNavigator));
      if (createCustomWriter == null)
        throw new ArgumentNullException(nameof (createCustomWriter));
      this.ContentSerializationFormat = contentSerializationFormat;
      this.CreateCustomNavigatorCallback = createCustomNavigator;
      this.CreateCustomWriterCallback = createCustomWriter;
    }

    public delegate IJsonNavigator CreateCustomNavigator(ReadOnlyMemory<byte> content);

    public delegate IJsonWriter CreateCustomWriter();
  }
}
