// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Tokens.PipelineContinuationTokenV1_1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.QueryPlan;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Tokens
{
  internal sealed class PipelineContinuationTokenV1_1 : PipelineContinuationToken
  {
    public static readonly System.Version VersionNumber = new System.Version(1, 1);
    private const string SourceContinuationTokenPropertyName = "SourceContinuationToken";
    private const string QueryPlanPropertyName = "QueryPlan";

    public PipelineContinuationTokenV1_1(
      PartitionedQueryExecutionInfo queryPlan,
      CosmosElement sourceContinuationToken)
      : base(PipelineContinuationTokenV1_1.VersionNumber)
    {
      this.QueryPlan = queryPlan;
      this.SourceContinuationToken = sourceContinuationToken ?? throw new ArgumentNullException(nameof (sourceContinuationToken));
    }

    public CosmosElement SourceContinuationToken { get; }

    public PartitionedQueryExecutionInfo QueryPlan { get; }

    public override string ToString() => this.ToString(int.MaxValue);

    public string ToString(int lengthLimitInBytes)
    {
      string str = this.QueryPlan?.ToString();
      bool flag = str != null && str.Length + this.SourceContinuationToken.ToString().Length < lengthLimitInBytes;
      return CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
      {
        {
          "Version",
          (CosmosElement) CosmosString.Create(this.Version.ToString())
        },
        {
          "QueryPlan",
          flag ? (CosmosElement) CosmosString.Create(str) : (CosmosElement) CosmosNull.Create()
        },
        {
          "SourceContinuationToken",
          this.SourceContinuationToken
        }
      }).ToString();
    }

    public static bool TryCreateFromCosmosElement(
      CosmosObject parsedContinuationToken,
      out PipelineContinuationTokenV1_1 pipelinedContinuationToken)
    {
      if ((CosmosElement) parsedContinuationToken == (CosmosElement) null)
        throw new ArgumentNullException(nameof (parsedContinuationToken));
      System.Version version;
      if (!PipelineContinuationToken.TryParseVersion(parsedContinuationToken, out version))
      {
        pipelinedContinuationToken = (PipelineContinuationTokenV1_1) null;
        return false;
      }
      if (version != PipelineContinuationTokenV1_1.VersionNumber)
      {
        pipelinedContinuationToken = (PipelineContinuationTokenV1_1) null;
        return false;
      }
      PartitionedQueryExecutionInfo queryPlan;
      if (!PipelineContinuationTokenV1_1.TryParseQueryPlan(parsedContinuationToken, out queryPlan))
      {
        pipelinedContinuationToken = (PipelineContinuationTokenV1_1) null;
        return false;
      }
      CosmosElement sourceContinuationToken;
      if (!parsedContinuationToken.TryGetValue("SourceContinuationToken", out sourceContinuationToken))
      {
        pipelinedContinuationToken = (PipelineContinuationTokenV1_1) null;
        return false;
      }
      pipelinedContinuationToken = new PipelineContinuationTokenV1_1(queryPlan, sourceContinuationToken);
      return true;
    }

    private static bool TryParseQueryPlan(
      CosmosObject parsedContinuationToken,
      out PartitionedQueryExecutionInfo queryPlan)
    {
      if ((CosmosElement) parsedContinuationToken == (CosmosElement) null)
        throw new ArgumentNullException(nameof (parsedContinuationToken));
      CosmosElement cosmosElement;
      if (!parsedContinuationToken.TryGetValue("QueryPlan", out cosmosElement))
      {
        queryPlan = (PartitionedQueryExecutionInfo) null;
        return false;
      }
      switch (cosmosElement)
      {
        case CosmosNull _:
          queryPlan = (PartitionedQueryExecutionInfo) null;
          return true;
        case CosmosString cosmosString:
          if (PartitionedQueryExecutionInfo.TryParse(UtfAnyString.op_Implicit(cosmosString.Value), out queryPlan))
            return true;
          queryPlan = (PartitionedQueryExecutionInfo) null;
          return false;
        default:
          queryPlan = (PartitionedQueryExecutionInfo) null;
          return false;
      }
    }
  }
}
