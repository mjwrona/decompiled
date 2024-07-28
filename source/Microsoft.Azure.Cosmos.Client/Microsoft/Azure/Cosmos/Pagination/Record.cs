// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.Record
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Documents;
using System;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal sealed class Record
  {
    public Record(
      ResourceId resourceIdentifier,
      DateTime timestamp,
      string identifier,
      CosmosObject payload)
    {
      this.ResourceIdentifier = resourceIdentifier;
      this.Timestamp = timestamp.Kind == DateTimeKind.Utc ? timestamp : throw new ArgumentOutOfRangeException("date time must be utc");
      this.Identifier = identifier ?? throw new ArgumentNullException(nameof (identifier));
      this.Payload = payload ?? throw new ArgumentNullException(nameof (payload));
    }

    public ResourceId ResourceIdentifier { get; }

    public DateTime Timestamp { get; }

    public string Identifier { get; }

    public CosmosObject Payload { get; }
  }
}
