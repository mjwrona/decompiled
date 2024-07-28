// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.ProxyDocumentQueryExecutionContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query
{
  internal sealed class ProxyDocumentQueryExecutionContext : 
    IDocumentQueryExecutionContext,
    IDisposable
  {
    private readonly IDocumentQueryClient client;
    private readonly ResourceType resourceTypeEnum;
    private readonly Type resourceType;
    private readonly Expression expression;
    private readonly FeedOptions feedOptions;
    private readonly string resourceLink;
    private readonly ContainerProperties collection;
    private readonly bool isContinuationExpected;
    private readonly Guid correlatedActivityId;
    private IDocumentQueryExecutionContext innerExecutionContext;

    private ProxyDocumentQueryExecutionContext(
      IDocumentQueryExecutionContext innerExecutionContext,
      IDocumentQueryClient client,
      ResourceType resourceTypeEnum,
      Type resourceType,
      Expression expression,
      FeedOptions feedOptions,
      string resourceLink,
      ContainerProperties collection,
      bool isContinuationExpected,
      Guid correlatedActivityId)
    {
      this.innerExecutionContext = innerExecutionContext;
      this.client = client;
      this.resourceTypeEnum = resourceTypeEnum;
      this.resourceType = resourceType;
      this.expression = expression;
      this.feedOptions = feedOptions;
      this.resourceLink = resourceLink;
      this.collection = collection;
      this.isContinuationExpected = isContinuationExpected;
      this.correlatedActivityId = correlatedActivityId;
    }

    public static ProxyDocumentQueryExecutionContext Create(
      IDocumentQueryClient client,
      ResourceType resourceTypeEnum,
      Type resourceType,
      Expression expression,
      FeedOptions feedOptions,
      string resourceLink,
      CancellationToken token,
      ContainerProperties collection,
      bool isContinuationExpected,
      Guid correlatedActivityId)
    {
      token.ThrowIfCancellationRequested();
      return new ProxyDocumentQueryExecutionContext((IDocumentQueryExecutionContext) new DefaultDocumentQueryExecutionContext(new DocumentQueryExecutionContextBase.InitParams(client, resourceTypeEnum, resourceType, expression, feedOptions, resourceLink, false, correlatedActivityId), isContinuationExpected), client, resourceTypeEnum, resourceType, expression, feedOptions, resourceLink, collection, isContinuationExpected, correlatedActivityId);
    }

    public bool IsDone => this.innerExecutionContext.IsDone;

    public void Dispose() => this.innerExecutionContext.Dispose();

    public async Task<DocumentFeedResponse<CosmosElement>> ExecuteNextFeedResponseAsync(
      CancellationToken token)
    {
      if (this.IsDone)
        throw new InvalidOperationException(RMResources.DocumentQueryExecutionContextIsDone);
      DocumentFeedResponse<CosmosElement> documentFeedResponse;
      Error error;
      try
      {
        documentFeedResponse = await this.innerExecutionContext.ExecuteNextFeedResponseAsync(token);
        goto label_10;
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
        if (!(statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue) || ex.GetSubStatus() != SubStatusCodes.CrossPartitionQueryNotServable)
          throw;
        else
          error = ex.Error;
      }
      List<PartitionKeyRange> partitionKeyRangesAsync = await ((DocumentQueryExecutionContextBase) this.innerExecutionContext).GetTargetPartitionKeyRangesAsync(this.collection.ResourceId, JsonConvert.DeserializeObject<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo>(error.AdditionalErrorInfo).QueryRanges);
      DocumentQueryExecutionContextBase.InitParams initParams = new DocumentQueryExecutionContextBase.InitParams(this.client, this.resourceTypeEnum, this.resourceType, this.expression, this.feedOptions, this.resourceLink, false, this.correlatedActivityId);
      throw new NotSupportedException("v2 query excution context is currently not supported.");
label_10:
      return documentFeedResponse;
    }
  }
}
