// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.DocumentQueryProvider
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query;
using Microsoft.Azure.Documents;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal sealed class DocumentQueryProvider : IDocumentQueryProvider, IQueryProvider
  {
    private readonly IDocumentQueryClient client;
    private readonly ResourceType resourceTypeEnum;
    private readonly Type resourceType;
    private readonly string documentsFeedOrDatabaseLink;
    private readonly FeedOptions feedOptions;
    private readonly object partitionKey;
    private readonly Action<IQueryable> onExecuteScalarQueryCallback;

    public DocumentQueryProvider(
      IDocumentQueryClient client,
      ResourceType resourceTypeEnum,
      Type resourceType,
      string documentsFeedOrDatabaseLink,
      FeedOptions feedOptions,
      object partitionKey = null,
      Action<IQueryable> onExecuteScalarQueryCallback = null)
    {
      this.client = client;
      this.resourceTypeEnum = resourceTypeEnum;
      this.resourceType = resourceType;
      this.documentsFeedOrDatabaseLink = documentsFeedOrDatabaseLink;
      this.feedOptions = feedOptions;
      this.partitionKey = partitionKey;
      this.onExecuteScalarQueryCallback = onExecuteScalarQueryCallback;
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => (IQueryable<TElement>) new DocumentQuery<TElement>(this.client, this.resourceTypeEnum, this.resourceType, this.documentsFeedOrDatabaseLink, expression, this.feedOptions, this.partitionKey);

    public IQueryable CreateQuery(Expression expression)
    {
      Type elementType = TypeSystem.GetElementType(expression.Type);
      return (IQueryable) Activator.CreateInstance(typeof (DocumentQuery<bool>).GetGenericTypeDefinition().MakeGenericType(elementType), (object) this.client, (object) this.resourceTypeEnum, (object) this.resourceType, (object) this.documentsFeedOrDatabaseLink, (object) expression, (object) this.feedOptions, this.partitionKey);
    }

    public TResult Execute<TResult>(Expression expression)
    {
      DocumentQuery<TResult> instance = (DocumentQuery<TResult>) Activator.CreateInstance(typeof (DocumentQuery<bool>).GetGenericTypeDefinition().MakeGenericType(typeof (TResult)), (object) this.client, (object) this.resourceTypeEnum, (object) this.resourceType, (object) this.documentsFeedOrDatabaseLink, (object) expression, (object) this.feedOptions, this.partitionKey);
      Action<IQueryable> scalarQueryCallback = this.onExecuteScalarQueryCallback;
      if (scalarQueryCallback != null)
        scalarQueryCallback((IQueryable) instance);
      return instance.ToList<TResult>().FirstOrDefault<TResult>();
    }

    public object Execute(Expression expression)
    {
      DocumentQuery<object> instance = (DocumentQuery<object>) Activator.CreateInstance(typeof (DocumentQuery<bool>).GetGenericTypeDefinition().MakeGenericType(typeof (object)), (object) this.client, (object) this.resourceTypeEnum, (object) this.resourceType, (object) this.documentsFeedOrDatabaseLink, (object) expression, (object) this.feedOptions, this.partitionKey);
      Action<IQueryable> scalarQueryCallback = this.onExecuteScalarQueryCallback;
      if (scalarQueryCallback != null)
        scalarQueryCallback((IQueryable) instance);
      return instance.ToList<object>().FirstOrDefault<object>();
    }

    public async Task<TResult> ExecuteAsync<TResult>(
      Expression expression,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DocumentQuery<TResult> instance = (DocumentQuery<TResult>) Activator.CreateInstance(typeof (DocumentQuery<bool>).GetGenericTypeDefinition().MakeGenericType(typeof (TResult)), (object) this.client, (object) this.resourceTypeEnum, (object) this.resourceType, (object) this.documentsFeedOrDatabaseLink, (object) expression, (object) this.feedOptions, this.partitionKey);
      Action<IQueryable> scalarQueryCallback = this.onExecuteScalarQueryCallback;
      if (scalarQueryCallback != null)
        scalarQueryCallback((IQueryable) instance);
      return (await instance.ExecuteAllAsync()).FirstOrDefault<TResult>();
    }
  }
}
