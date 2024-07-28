// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Tokens.PipelineContinuationTokenV0
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using System;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Tokens
{
  internal sealed class PipelineContinuationTokenV0 : PipelineContinuationToken
  {
    public static readonly System.Version VersionNumber = new System.Version(0, 0);

    public PipelineContinuationTokenV0(CosmosElement sourceContinuationToken)
      : base(PipelineContinuationTokenV0.VersionNumber)
    {
      this.SourceContinuationToken = sourceContinuationToken ?? throw new ArgumentNullException(nameof (sourceContinuationToken));
    }

    public CosmosElement SourceContinuationToken { get; }

    public override string ToString() => this.SourceContinuationToken.ToString();

    public static bool TryCreateFromCosmosElement(
      CosmosElement cosmosElement,
      out PipelineContinuationTokenV0 pipelineContinuationTokenV0)
    {
      pipelineContinuationTokenV0 = !(cosmosElement == (CosmosElement) null) ? new PipelineContinuationTokenV0(cosmosElement) : throw new ArgumentNullException(nameof (cosmosElement));
      return true;
    }
  }
}
