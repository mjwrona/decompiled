// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.MultiCollectionDocumentQueryExecutionContext
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class MultiCollectionDocumentQueryExecutionContext : 
    IDocumentQueryExecutionContext,
    IDisposable
  {
    private readonly List<IDocumentQueryExecutionContext> childQueryExecutionContexts;
    private int currentChildQueryExecutionContextIndex;

    public static async Task<IDocumentQueryExecutionContext> CreateAsync(
      IDocumentQueryClient client,
      ResourceType resourceTypeEnum,
      Type resourceType,
      Expression expression,
      FeedOptions feedOptions,
      IEnumerable<string> documentFeedLinks,
      bool isContinuationExpected,
      CancellationToken token,
      Guid correlatedActivityId)
    {
      if (client == null)
        throw new ArgumentNullException(nameof (client));
      if (feedOptions == null)
        throw new ArgumentNullException(nameof (feedOptions));
      if (documentFeedLinks == null)
        throw new ArgumentNullException(nameof (documentFeedLinks));
      List<IDocumentQueryExecutionContext> childQueryExecutionContexts = new List<IDocumentQueryExecutionContext>();
      foreach (string documentFeedLink in documentFeedLinks)
      {
        List<IDocumentQueryExecutionContext> executionContextList = childQueryExecutionContexts;
        executionContextList.Add(await DocumentQueryExecutionContextFactory.CreateDocumentQueryExecutionContextAsync(client, resourceTypeEnum, resourceType, expression, feedOptions, documentFeedLink, isContinuationExpected, token, correlatedActivityId));
        executionContextList = (List<IDocumentQueryExecutionContext>) null;
      }
      return (IDocumentQueryExecutionContext) new MultiCollectionDocumentQueryExecutionContext(childQueryExecutionContexts);
    }

    private MultiCollectionDocumentQueryExecutionContext(
      List<IDocumentQueryExecutionContext> childQueryExecutionContexts)
    {
      this.childQueryExecutionContexts = childQueryExecutionContexts != null ? childQueryExecutionContexts : throw new ArgumentNullException(nameof (childQueryExecutionContexts));
    }

    public bool IsDone => this.currentChildQueryExecutionContextIndex >= this.childQueryExecutionContexts.Count<IDocumentQueryExecutionContext>();

    public void Dispose()
    {
      foreach (IDisposable executionContext in this.childQueryExecutionContexts)
        executionContext.Dispose();
    }

    public async Task<FeedResponse<object>> ExecuteNextAsync(CancellationToken token)
    {
      if (this.IsDone)
        throw new InvalidOperationException(RMResources.DocumentQueryExecutionContextIsDone);
      FeedResponse<object> feedResponse = await this.childQueryExecutionContexts[this.currentChildQueryExecutionContextIndex].ExecuteNextAsync(token);
      if (this.childQueryExecutionContexts[this.currentChildQueryExecutionContextIndex].IsDone)
        ++this.currentChildQueryExecutionContextIndex;
      return feedResponse;
    }
  }
}
