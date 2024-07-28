// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.MonadicCreatePipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System.Threading;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline
{
  internal delegate TryCatch<IQueryPipelineStage> MonadicCreatePipelineStage(
    CosmosElement continuationToken,
    CancellationToken cancellationToken);
}
