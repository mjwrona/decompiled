// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination.QueryState
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using System;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination
{
  internal sealed class QueryState : State
  {
    public QueryState(CosmosElement value) => this.Value = value ?? throw new ArgumentNullException(nameof (value));

    public CosmosElement Value { get; }
  }
}
