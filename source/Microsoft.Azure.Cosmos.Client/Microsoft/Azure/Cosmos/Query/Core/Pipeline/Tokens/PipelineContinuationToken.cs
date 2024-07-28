// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Tokens.PipelineContinuationToken
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.QueryPlan;
using System;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Tokens
{
  internal abstract class PipelineContinuationToken
  {
    protected const string VersionPropertyName = "Version";
    private static readonly System.Version CurrentVersion = new System.Version(1, 1);

    protected PipelineContinuationToken(System.Version version) => this.Version = version;

    public System.Version Version { get; }

    public static bool TryCreateFromCosmosElement(
      CosmosElement cosmosElement,
      out PipelineContinuationToken pipelineContinuationToken)
    {
      if (cosmosElement == (CosmosElement) null)
        throw new ArgumentNullException(nameof (cosmosElement));
      if (!(cosmosElement is CosmosObject cosmosObject))
      {
        pipelineContinuationToken = (PipelineContinuationToken) new PipelineContinuationTokenV0(cosmosElement);
        return true;
      }
      System.Version version;
      if (!PipelineContinuationToken.TryParseVersion(cosmosObject, out version))
      {
        pipelineContinuationToken = (PipelineContinuationToken) null;
        return false;
      }
      if (version == PipelineContinuationTokenV0.VersionNumber)
      {
        PipelineContinuationTokenV0 pipelineContinuationTokenV0;
        if (!PipelineContinuationTokenV0.TryCreateFromCosmosElement(cosmosElement, out pipelineContinuationTokenV0))
        {
          pipelineContinuationToken = (PipelineContinuationToken) null;
          return false;
        }
        pipelineContinuationToken = (PipelineContinuationToken) pipelineContinuationTokenV0;
      }
      else if (version == PipelineContinuationTokenV1.VersionNumber)
      {
        PipelineContinuationTokenV1 pipelinedContinuationTokenV1;
        if (!PipelineContinuationTokenV1.TryCreateFromCosmosElement(cosmosObject, out pipelinedContinuationTokenV1))
        {
          pipelineContinuationToken = (PipelineContinuationToken) null;
          return false;
        }
        pipelineContinuationToken = (PipelineContinuationToken) pipelinedContinuationTokenV1;
      }
      else if (version == PipelineContinuationTokenV1_1.VersionNumber)
      {
        PipelineContinuationTokenV1_1 pipelinedContinuationToken;
        if (!PipelineContinuationTokenV1_1.TryCreateFromCosmosElement(cosmosObject, out pipelinedContinuationToken))
        {
          pipelineContinuationToken = (PipelineContinuationToken) null;
          return false;
        }
        pipelineContinuationToken = (PipelineContinuationToken) pipelinedContinuationToken;
      }
      else
      {
        pipelineContinuationToken = (PipelineContinuationToken) null;
        return false;
      }
      return true;
    }

    public static bool TryConvertToLatest(
      PipelineContinuationToken pipelinedContinuationToken,
      out PipelineContinuationTokenV1_1 pipelineContinuationTokenV1_1)
    {
      if (pipelinedContinuationToken == null)
        throw new ArgumentNullException(nameof (pipelinedContinuationToken));
      if (pipelinedContinuationToken is PipelineContinuationTokenV0 continuationTokenV0)
        pipelinedContinuationToken = (PipelineContinuationToken) new PipelineContinuationTokenV1(continuationTokenV0.SourceContinuationToken);
      if (pipelinedContinuationToken is PipelineContinuationTokenV1 continuationTokenV1)
        pipelinedContinuationToken = (PipelineContinuationToken) new PipelineContinuationTokenV1_1((PartitionedQueryExecutionInfo) null, continuationTokenV1.SourceContinuationToken);
      if (!(pipelinedContinuationToken is PipelineContinuationTokenV1_1 continuationTokenV11))
      {
        pipelineContinuationTokenV1_1 = (PipelineContinuationTokenV1_1) null;
        return false;
      }
      pipelineContinuationTokenV1_1 = continuationTokenV11;
      return true;
    }

    public static bool IsTokenFromTheFuture(
      PipelineContinuationToken versionedContinuationToken)
    {
      return versionedContinuationToken.Version > PipelineContinuationToken.CurrentVersion;
    }

    protected static bool TryParseVersion(
      CosmosObject parsedVersionedContinuationToken,
      out System.Version version)
    {
      if ((CosmosElement) parsedVersionedContinuationToken == (CosmosElement) null)
        throw new ArgumentNullException(nameof (parsedVersionedContinuationToken));
      CosmosString typedCosmosElement;
      if (!parsedVersionedContinuationToken.TryGetValue<CosmosString>("Version", out typedCosmosElement))
      {
        version = PipelineContinuationTokenV0.VersionNumber;
        return true;
      }
      System.Version result;
      if (!System.Version.TryParse(UtfAnyString.op_Implicit(typedCosmosElement.Value), out result))
      {
        version = (System.Version) null;
        return false;
      }
      version = result;
      return true;
    }
  }
}
