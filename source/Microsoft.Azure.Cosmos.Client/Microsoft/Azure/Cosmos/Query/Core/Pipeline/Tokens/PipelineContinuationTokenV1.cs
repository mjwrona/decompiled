// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Tokens.PipelineContinuationTokenV1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Tokens
{
  internal sealed class PipelineContinuationTokenV1 : PipelineContinuationToken
  {
    public static readonly System.Version VersionNumber = new System.Version(1, 0);
    private static readonly string SourceContinuationTokenPropertyName = nameof (SourceContinuationToken);

    public PipelineContinuationTokenV1(CosmosElement sourceContinuationToken)
      : base(PipelineContinuationTokenV1.VersionNumber)
    {
      this.SourceContinuationToken = sourceContinuationToken ?? throw new ArgumentNullException(nameof (sourceContinuationToken));
    }

    public CosmosElement SourceContinuationToken { get; }

    public override string ToString() => CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
    {
      {
        "Version",
        (CosmosElement) CosmosString.Create(this.Version.ToString())
      },
      {
        PipelineContinuationTokenV1.SourceContinuationTokenPropertyName,
        this.SourceContinuationToken
      }
    }).ToString();

    public static bool TryCreateFromCosmosElement(
      CosmosObject parsedContinuationToken,
      out PipelineContinuationTokenV1 pipelinedContinuationTokenV1)
    {
      if ((CosmosElement) parsedContinuationToken == (CosmosElement) null)
        throw new ArgumentNullException(nameof (parsedContinuationToken));
      System.Version version;
      if (!PipelineContinuationToken.TryParseVersion(parsedContinuationToken, out version))
      {
        pipelinedContinuationTokenV1 = (PipelineContinuationTokenV1) null;
        return false;
      }
      if (version != PipelineContinuationTokenV1.VersionNumber)
      {
        pipelinedContinuationTokenV1 = (PipelineContinuationTokenV1) null;
        return false;
      }
      CosmosElement sourceContinuationToken;
      if (!parsedContinuationToken.TryGetValue(PipelineContinuationTokenV1.SourceContinuationTokenPropertyName, out sourceContinuationToken))
      {
        pipelinedContinuationTokenV1 = (PipelineContinuationTokenV1) null;
        return false;
      }
      pipelinedContinuationTokenV1 = new PipelineContinuationTokenV1(sourceContinuationToken);
      return true;
    }

    public static PipelineContinuationTokenV1 CreateFromV0Token(
      PipelineContinuationTokenV0 pipelinedContinuationTokenV0)
    {
      return pipelinedContinuationTokenV0 != null ? new PipelineContinuationTokenV1(pipelinedContinuationTokenV0.SourceContinuationToken) : throw new ArgumentNullException(nameof (pipelinedContinuationTokenV0));
    }
  }
}
