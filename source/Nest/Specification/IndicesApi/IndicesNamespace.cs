// Decompiled with JetBrains decompiler
// Type: Nest.Specification.IndicesApi.IndicesNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.IndicesApi
{
  public class IndicesNamespace : Nest.NamespacedClientProxy
  {
    internal IndicesNamespace(ElasticClient client)
      : base(client)
    {
    }

    public AddIndexBlockResponse AddBlock(
      Indices index,
      IndexBlock block,
      Func<AddIndexBlockDescriptor, IAddIndexBlockRequest> selector = null)
    {
      return this.AddBlock(selector.InvokeOrDefault<AddIndexBlockDescriptor, IAddIndexBlockRequest>(new AddIndexBlockDescriptor(index, block)));
    }

    public Task<AddIndexBlockResponse> AddBlockAsync(
      Indices index,
      IndexBlock block,
      Func<AddIndexBlockDescriptor, IAddIndexBlockRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.AddBlockAsync(selector.InvokeOrDefault<AddIndexBlockDescriptor, IAddIndexBlockRequest>(new AddIndexBlockDescriptor(index, block)), ct);
    }

    public AddIndexBlockResponse AddBlock(IAddIndexBlockRequest request) => this.DoRequest<IAddIndexBlockRequest, AddIndexBlockResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<AddIndexBlockResponse> AddBlockAsync(
      IAddIndexBlockRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IAddIndexBlockRequest, AddIndexBlockResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public AnalyzeResponse Analyze(Func<AnalyzeDescriptor, IAnalyzeRequest> selector = null) => this.Analyze(selector.InvokeOrDefault<AnalyzeDescriptor, IAnalyzeRequest>(new AnalyzeDescriptor()));

    public Task<AnalyzeResponse> AnalyzeAsync(
      Func<AnalyzeDescriptor, IAnalyzeRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.AnalyzeAsync(selector.InvokeOrDefault<AnalyzeDescriptor, IAnalyzeRequest>(new AnalyzeDescriptor()), ct);
    }

    public AnalyzeResponse Analyze(IAnalyzeRequest request) => this.DoRequest<IAnalyzeRequest, AnalyzeResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<AnalyzeResponse> AnalyzeAsync(IAnalyzeRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IAnalyzeRequest, AnalyzeResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public ClearCacheResponse ClearCache(
      Indices index = null,
      Func<ClearCacheDescriptor, IClearCacheRequest> selector = null)
    {
      return this.ClearCache(selector.InvokeOrDefault<ClearCacheDescriptor, IClearCacheRequest>(new ClearCacheDescriptor().Index(index)));
    }

    public Task<ClearCacheResponse> ClearCacheAsync(
      Indices index = null,
      Func<ClearCacheDescriptor, IClearCacheRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ClearCacheAsync(selector.InvokeOrDefault<ClearCacheDescriptor, IClearCacheRequest>(new ClearCacheDescriptor().Index(index)), ct);
    }

    public ClearCacheResponse ClearCache(IClearCacheRequest request) => this.DoRequest<IClearCacheRequest, ClearCacheResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClearCacheResponse> ClearCacheAsync(
      IClearCacheRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClearCacheRequest, ClearCacheResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public CloneIndexResponse Clone(
      IndexName index,
      IndexName target,
      Func<CloneIndexDescriptor, ICloneIndexRequest> selector = null)
    {
      return this.Clone(selector.InvokeOrDefault<CloneIndexDescriptor, ICloneIndexRequest>(new CloneIndexDescriptor(index, target)));
    }

    public Task<CloneIndexResponse> CloneAsync(
      IndexName index,
      IndexName target,
      Func<CloneIndexDescriptor, ICloneIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CloneAsync(selector.InvokeOrDefault<CloneIndexDescriptor, ICloneIndexRequest>(new CloneIndexDescriptor(index, target)), ct);
    }

    public CloneIndexResponse Clone(ICloneIndexRequest request) => this.DoRequest<ICloneIndexRequest, CloneIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CloneIndexResponse> CloneAsync(ICloneIndexRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ICloneIndexRequest, CloneIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public CloseIndexResponse Close(
      Indices index,
      Func<CloseIndexDescriptor, ICloseIndexRequest> selector = null)
    {
      return this.Close(selector.InvokeOrDefault<CloseIndexDescriptor, ICloseIndexRequest>(new CloseIndexDescriptor(index)));
    }

    public Task<CloseIndexResponse> CloseAsync(
      Indices index,
      Func<CloseIndexDescriptor, ICloseIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CloseAsync(selector.InvokeOrDefault<CloseIndexDescriptor, ICloseIndexRequest>(new CloseIndexDescriptor(index)), ct);
    }

    public CloseIndexResponse Close(ICloseIndexRequest request) => this.DoRequest<ICloseIndexRequest, CloseIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CloseIndexResponse> CloseAsync(ICloseIndexRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ICloseIndexRequest, CloseIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public CreateIndexResponse Create(
      IndexName index,
      Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null)
    {
      return this.Create(selector.InvokeOrDefault<CreateIndexDescriptor, ICreateIndexRequest>(new CreateIndexDescriptor(index)));
    }

    public Task<CreateIndexResponse> CreateAsync(
      IndexName index,
      Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CreateAsync(selector.InvokeOrDefault<CreateIndexDescriptor, ICreateIndexRequest>(new CreateIndexDescriptor(index)), ct);
    }

    public CreateIndexResponse Create(ICreateIndexRequest request) => this.DoRequest<ICreateIndexRequest, CreateIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CreateIndexResponse> CreateAsync(ICreateIndexRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ICreateIndexRequest, CreateIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public CreateDataStreamResponse CreateDataStream(
      Name name,
      Func<CreateDataStreamDescriptor, ICreateDataStreamRequest> selector = null)
    {
      return this.CreateDataStream(selector.InvokeOrDefault<CreateDataStreamDescriptor, ICreateDataStreamRequest>(new CreateDataStreamDescriptor(name)));
    }

    public Task<CreateDataStreamResponse> CreateDataStreamAsync(
      Name name,
      Func<CreateDataStreamDescriptor, ICreateDataStreamRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CreateDataStreamAsync(selector.InvokeOrDefault<CreateDataStreamDescriptor, ICreateDataStreamRequest>(new CreateDataStreamDescriptor(name)), ct);
    }

    public CreateDataStreamResponse CreateDataStream(ICreateDataStreamRequest request) => this.DoRequest<ICreateDataStreamRequest, CreateDataStreamResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CreateDataStreamResponse> CreateDataStreamAsync(
      ICreateDataStreamRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ICreateDataStreamRequest, CreateDataStreamResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DataStreamsStatsResponse DataStreamsStats(
      Names name = null,
      Func<DataStreamsStatsDescriptor, IDataStreamsStatsRequest> selector = null)
    {
      return this.DataStreamsStats(selector.InvokeOrDefault<DataStreamsStatsDescriptor, IDataStreamsStatsRequest>(new DataStreamsStatsDescriptor().Name(name)));
    }

    public Task<DataStreamsStatsResponse> DataStreamsStatsAsync(
      Names name = null,
      Func<DataStreamsStatsDescriptor, IDataStreamsStatsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DataStreamsStatsAsync(selector.InvokeOrDefault<DataStreamsStatsDescriptor, IDataStreamsStatsRequest>(new DataStreamsStatsDescriptor().Name(name)), ct);
    }

    public DataStreamsStatsResponse DataStreamsStats(IDataStreamsStatsRequest request) => this.DoRequest<IDataStreamsStatsRequest, DataStreamsStatsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DataStreamsStatsResponse> DataStreamsStatsAsync(
      IDataStreamsStatsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDataStreamsStatsRequest, DataStreamsStatsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteIndexResponse Delete(
      Indices index,
      Func<DeleteIndexDescriptor, IDeleteIndexRequest> selector = null)
    {
      return this.Delete(selector.InvokeOrDefault<DeleteIndexDescriptor, IDeleteIndexRequest>(new DeleteIndexDescriptor(index)));
    }

    public Task<DeleteIndexResponse> DeleteAsync(
      Indices index,
      Func<DeleteIndexDescriptor, IDeleteIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteAsync(selector.InvokeOrDefault<DeleteIndexDescriptor, IDeleteIndexRequest>(new DeleteIndexDescriptor(index)), ct);
    }

    public DeleteIndexResponse Delete(IDeleteIndexRequest request) => this.DoRequest<IDeleteIndexRequest, DeleteIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteIndexResponse> DeleteAsync(IDeleteIndexRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IDeleteIndexRequest, DeleteIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public DeleteAliasResponse DeleteAlias(
      Indices index,
      Names name,
      Func<DeleteAliasDescriptor, IDeleteAliasRequest> selector = null)
    {
      return this.DeleteAlias(selector.InvokeOrDefault<DeleteAliasDescriptor, IDeleteAliasRequest>(new DeleteAliasDescriptor(index, name)));
    }

    public Task<DeleteAliasResponse> DeleteAliasAsync(
      Indices index,
      Names name,
      Func<DeleteAliasDescriptor, IDeleteAliasRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteAliasAsync(selector.InvokeOrDefault<DeleteAliasDescriptor, IDeleteAliasRequest>(new DeleteAliasDescriptor(index, name)), ct);
    }

    public DeleteAliasResponse DeleteAlias(IDeleteAliasRequest request) => this.DoRequest<IDeleteAliasRequest, DeleteAliasResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteAliasResponse> DeleteAliasAsync(
      IDeleteAliasRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteAliasRequest, DeleteAliasResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteDataStreamResponse DeleteDataStream(
      Names name,
      Func<DeleteDataStreamDescriptor, IDeleteDataStreamRequest> selector = null)
    {
      return this.DeleteDataStream(selector.InvokeOrDefault<DeleteDataStreamDescriptor, IDeleteDataStreamRequest>(new DeleteDataStreamDescriptor(name)));
    }

    public Task<DeleteDataStreamResponse> DeleteDataStreamAsync(
      Names name,
      Func<DeleteDataStreamDescriptor, IDeleteDataStreamRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteDataStreamAsync(selector.InvokeOrDefault<DeleteDataStreamDescriptor, IDeleteDataStreamRequest>(new DeleteDataStreamDescriptor(name)), ct);
    }

    public DeleteDataStreamResponse DeleteDataStream(IDeleteDataStreamRequest request) => this.DoRequest<IDeleteDataStreamRequest, DeleteDataStreamResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteDataStreamResponse> DeleteDataStreamAsync(
      IDeleteDataStreamRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteDataStreamRequest, DeleteDataStreamResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteIndexTemplateV2Response DeleteTemplateV2(
      Name name,
      Func<DeleteIndexTemplateV2Descriptor, IDeleteIndexTemplateV2Request> selector = null)
    {
      return this.DeleteTemplateV2(selector.InvokeOrDefault<DeleteIndexTemplateV2Descriptor, IDeleteIndexTemplateV2Request>(new DeleteIndexTemplateV2Descriptor(name)));
    }

    public Task<DeleteIndexTemplateV2Response> DeleteTemplateV2Async(
      Name name,
      Func<DeleteIndexTemplateV2Descriptor, IDeleteIndexTemplateV2Request> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteTemplateV2Async(selector.InvokeOrDefault<DeleteIndexTemplateV2Descriptor, IDeleteIndexTemplateV2Request>(new DeleteIndexTemplateV2Descriptor(name)), ct);
    }

    public DeleteIndexTemplateV2Response DeleteTemplateV2(IDeleteIndexTemplateV2Request request) => this.DoRequest<IDeleteIndexTemplateV2Request, DeleteIndexTemplateV2Response>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteIndexTemplateV2Response> DeleteTemplateV2Async(
      IDeleteIndexTemplateV2Request request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteIndexTemplateV2Request, DeleteIndexTemplateV2Response>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteIndexTemplateResponse DeleteTemplate(
      Name name,
      Func<DeleteIndexTemplateDescriptor, IDeleteIndexTemplateRequest> selector = null)
    {
      return this.DeleteTemplate(selector.InvokeOrDefault<DeleteIndexTemplateDescriptor, IDeleteIndexTemplateRequest>(new DeleteIndexTemplateDescriptor(name)));
    }

    public Task<DeleteIndexTemplateResponse> DeleteTemplateAsync(
      Name name,
      Func<DeleteIndexTemplateDescriptor, IDeleteIndexTemplateRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteTemplateAsync(selector.InvokeOrDefault<DeleteIndexTemplateDescriptor, IDeleteIndexTemplateRequest>(new DeleteIndexTemplateDescriptor(name)), ct);
    }

    public DeleteIndexTemplateResponse DeleteTemplate(IDeleteIndexTemplateRequest request) => this.DoRequest<IDeleteIndexTemplateRequest, DeleteIndexTemplateResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteIndexTemplateResponse> DeleteTemplateAsync(
      IDeleteIndexTemplateRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteIndexTemplateRequest, DeleteIndexTemplateResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ExistsResponse Exists(
      Indices index,
      Func<IndexExistsDescriptor, IIndexExistsRequest> selector = null)
    {
      return this.Exists(selector.InvokeOrDefault<IndexExistsDescriptor, IIndexExistsRequest>(new IndexExistsDescriptor(index)));
    }

    public Task<ExistsResponse> ExistsAsync(
      Indices index,
      Func<IndexExistsDescriptor, IIndexExistsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ExistsAsync(selector.InvokeOrDefault<IndexExistsDescriptor, IIndexExistsRequest>(new IndexExistsDescriptor(index)), ct);
    }

    public ExistsResponse Exists(IIndexExistsRequest request) => this.DoRequest<IIndexExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExistsResponse> ExistsAsync(IIndexExistsRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IIndexExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public ExistsResponse AliasExists(
      Names name,
      Func<AliasExistsDescriptor, IAliasExistsRequest> selector = null)
    {
      return this.AliasExists(selector.InvokeOrDefault<AliasExistsDescriptor, IAliasExistsRequest>(new AliasExistsDescriptor(name)));
    }

    public Task<ExistsResponse> AliasExistsAsync(
      Names name,
      Func<AliasExistsDescriptor, IAliasExistsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.AliasExistsAsync(selector.InvokeOrDefault<AliasExistsDescriptor, IAliasExistsRequest>(new AliasExistsDescriptor(name)), ct);
    }

    public ExistsResponse AliasExists(IAliasExistsRequest request) => this.DoRequest<IAliasExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExistsResponse> AliasExistsAsync(IAliasExistsRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IAliasExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public ExistsResponse TemplateV2Exists(
      Name name,
      Func<IndexTemplateV2ExistsDescriptor, IIndexTemplateV2ExistsRequest> selector = null)
    {
      return this.TemplateV2Exists(selector.InvokeOrDefault<IndexTemplateV2ExistsDescriptor, IIndexTemplateV2ExistsRequest>(new IndexTemplateV2ExistsDescriptor(name)));
    }

    public Task<ExistsResponse> TemplateV2ExistsAsync(
      Name name,
      Func<IndexTemplateV2ExistsDescriptor, IIndexTemplateV2ExistsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.TemplateV2ExistsAsync(selector.InvokeOrDefault<IndexTemplateV2ExistsDescriptor, IIndexTemplateV2ExistsRequest>(new IndexTemplateV2ExistsDescriptor(name)), ct);
    }

    public ExistsResponse TemplateV2Exists(IIndexTemplateV2ExistsRequest request) => this.DoRequest<IIndexTemplateV2ExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExistsResponse> TemplateV2ExistsAsync(
      IIndexTemplateV2ExistsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IIndexTemplateV2ExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ExistsResponse TemplateExists(
      Names name,
      Func<IndexTemplateExistsDescriptor, IIndexTemplateExistsRequest> selector = null)
    {
      return this.TemplateExists(selector.InvokeOrDefault<IndexTemplateExistsDescriptor, IIndexTemplateExistsRequest>(new IndexTemplateExistsDescriptor(name)));
    }

    public Task<ExistsResponse> TemplateExistsAsync(
      Names name,
      Func<IndexTemplateExistsDescriptor, IIndexTemplateExistsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.TemplateExistsAsync(selector.InvokeOrDefault<IndexTemplateExistsDescriptor, IIndexTemplateExistsRequest>(new IndexTemplateExistsDescriptor(name)), ct);
    }

    public ExistsResponse TemplateExists(IIndexTemplateExistsRequest request) => this.DoRequest<IIndexTemplateExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExistsResponse> TemplateExistsAsync(
      IIndexTemplateExistsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IIndexTemplateExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ExistsResponse TypeExists(
      Indices index,
      Names type,
      Func<TypeExistsDescriptor, ITypeExistsRequest> selector = null)
    {
      return this.TypeExists(selector.InvokeOrDefault<TypeExistsDescriptor, ITypeExistsRequest>(new TypeExistsDescriptor(index, type)));
    }

    public Task<ExistsResponse> TypeExistsAsync(
      Indices index,
      Names type,
      Func<TypeExistsDescriptor, ITypeExistsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.TypeExistsAsync(selector.InvokeOrDefault<TypeExistsDescriptor, ITypeExistsRequest>(new TypeExistsDescriptor(index, type)), ct);
    }

    public ExistsResponse TypeExists(ITypeExistsRequest request) => this.DoRequest<ITypeExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExistsResponse> TypeExistsAsync(ITypeExistsRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ITypeExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public FlushResponse Flush(Indices index = null, Func<FlushDescriptor, IFlushRequest> selector = null) => this.Flush(selector.InvokeOrDefault<FlushDescriptor, IFlushRequest>(new FlushDescriptor().Index(index)));

    public Task<FlushResponse> FlushAsync(
      Indices index = null,
      Func<FlushDescriptor, IFlushRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.FlushAsync(selector.InvokeOrDefault<FlushDescriptor, IFlushRequest>(new FlushDescriptor().Index(index)), ct);
    }

    public FlushResponse Flush(IFlushRequest request) => this.DoRequest<IFlushRequest, FlushResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<FlushResponse> FlushAsync(IFlushRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IFlushRequest, FlushResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public SyncedFlushResponse SyncedFlush(
      Indices index = null,
      Func<SyncedFlushDescriptor, ISyncedFlushRequest> selector = null)
    {
      return this.SyncedFlush(selector.InvokeOrDefault<SyncedFlushDescriptor, ISyncedFlushRequest>(new SyncedFlushDescriptor().Index(index)));
    }

    public Task<SyncedFlushResponse> SyncedFlushAsync(
      Indices index = null,
      Func<SyncedFlushDescriptor, ISyncedFlushRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.SyncedFlushAsync(selector.InvokeOrDefault<SyncedFlushDescriptor, ISyncedFlushRequest>(new SyncedFlushDescriptor().Index(index)), ct);
    }

    public SyncedFlushResponse SyncedFlush(ISyncedFlushRequest request) => this.DoRequest<ISyncedFlushRequest, SyncedFlushResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<SyncedFlushResponse> SyncedFlushAsync(
      ISyncedFlushRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ISyncedFlushRequest, SyncedFlushResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ForceMergeResponse ForceMerge(
      Indices index = null,
      Func<ForceMergeDescriptor, IForceMergeRequest> selector = null)
    {
      return this.ForceMerge(selector.InvokeOrDefault<ForceMergeDescriptor, IForceMergeRequest>(new ForceMergeDescriptor().Index(index)));
    }

    public Task<ForceMergeResponse> ForceMergeAsync(
      Indices index = null,
      Func<ForceMergeDescriptor, IForceMergeRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ForceMergeAsync(selector.InvokeOrDefault<ForceMergeDescriptor, IForceMergeRequest>(new ForceMergeDescriptor().Index(index)), ct);
    }

    public ForceMergeResponse ForceMerge(IForceMergeRequest request) => this.DoRequest<IForceMergeRequest, ForceMergeResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ForceMergeResponse> ForceMergeAsync(
      IForceMergeRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IForceMergeRequest, ForceMergeResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public FreezeIndexResponse Freeze(
      IndexName index,
      Func<FreezeIndexDescriptor, IFreezeIndexRequest> selector = null)
    {
      return this.Freeze(selector.InvokeOrDefault<FreezeIndexDescriptor, IFreezeIndexRequest>(new FreezeIndexDescriptor(index)));
    }

    public Task<FreezeIndexResponse> FreezeAsync(
      IndexName index,
      Func<FreezeIndexDescriptor, IFreezeIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.FreezeAsync(selector.InvokeOrDefault<FreezeIndexDescriptor, IFreezeIndexRequest>(new FreezeIndexDescriptor(index)), ct);
    }

    public FreezeIndexResponse Freeze(IFreezeIndexRequest request) => this.DoRequest<IFreezeIndexRequest, FreezeIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<FreezeIndexResponse> FreezeAsync(IFreezeIndexRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IFreezeIndexRequest, FreezeIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public GetIndexResponse Get(
      Indices index,
      Func<GetIndexDescriptor, IGetIndexRequest> selector = null)
    {
      return this.Get(selector.InvokeOrDefault<GetIndexDescriptor, IGetIndexRequest>(new GetIndexDescriptor(index)));
    }

    public Task<GetIndexResponse> GetAsync(
      Indices index,
      Func<GetIndexDescriptor, IGetIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetAsync(selector.InvokeOrDefault<GetIndexDescriptor, IGetIndexRequest>(new GetIndexDescriptor(index)), ct);
    }

    public GetIndexResponse Get(IGetIndexRequest request) => this.DoRequest<IGetIndexRequest, GetIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetIndexResponse> GetAsync(IGetIndexRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IGetIndexRequest, GetIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public GetAliasResponse GetAlias(
      Indices index = null,
      Func<GetAliasDescriptor, IGetAliasRequest> selector = null)
    {
      return this.GetAlias(selector.InvokeOrDefault<GetAliasDescriptor, IGetAliasRequest>(new GetAliasDescriptor().Index(index)));
    }

    public Task<GetAliasResponse> GetAliasAsync(
      Indices index = null,
      Func<GetAliasDescriptor, IGetAliasRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetAliasAsync(selector.InvokeOrDefault<GetAliasDescriptor, IGetAliasRequest>(new GetAliasDescriptor().Index(index)), ct);
    }

    public GetAliasResponse GetAlias(IGetAliasRequest request) => this.DoRequest<IGetAliasRequest, GetAliasResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetAliasResponse> GetAliasAsync(IGetAliasRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IGetAliasRequest, GetAliasResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public GetDataStreamResponse GetDataStream(
      Names name = null,
      Func<GetDataStreamDescriptor, IGetDataStreamRequest> selector = null)
    {
      return this.GetDataStream(selector.InvokeOrDefault<GetDataStreamDescriptor, IGetDataStreamRequest>(new GetDataStreamDescriptor().Name(name)));
    }

    public Task<GetDataStreamResponse> GetDataStreamAsync(
      Names name = null,
      Func<GetDataStreamDescriptor, IGetDataStreamRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetDataStreamAsync(selector.InvokeOrDefault<GetDataStreamDescriptor, IGetDataStreamRequest>(new GetDataStreamDescriptor().Name(name)), ct);
    }

    public GetDataStreamResponse GetDataStream(IGetDataStreamRequest request) => this.DoRequest<IGetDataStreamRequest, GetDataStreamResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetDataStreamResponse> GetDataStreamAsync(
      IGetDataStreamRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetDataStreamRequest, GetDataStreamResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetFieldMappingResponse GetFieldMapping<TDocument>(
      Fields fields,
      Func<GetFieldMappingDescriptor<TDocument>, IGetFieldMappingRequest> selector = null)
      where TDocument : class
    {
      return this.GetFieldMapping(selector.InvokeOrDefault<GetFieldMappingDescriptor<TDocument>, IGetFieldMappingRequest>(new GetFieldMappingDescriptor<TDocument>(fields)));
    }

    public Task<GetFieldMappingResponse> GetFieldMappingAsync<TDocument>(
      Fields fields,
      Func<GetFieldMappingDescriptor<TDocument>, IGetFieldMappingRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.GetFieldMappingAsync(selector.InvokeOrDefault<GetFieldMappingDescriptor<TDocument>, IGetFieldMappingRequest>(new GetFieldMappingDescriptor<TDocument>(fields)), ct);
    }

    public GetFieldMappingResponse GetFieldMapping(IGetFieldMappingRequest request) => this.DoRequest<IGetFieldMappingRequest, GetFieldMappingResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetFieldMappingResponse> GetFieldMappingAsync(
      IGetFieldMappingRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetFieldMappingRequest, GetFieldMappingResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetIndexTemplateV2Response GetTemplateV2(
      Name name = null,
      Func<GetIndexTemplateV2Descriptor, IGetIndexTemplateV2Request> selector = null)
    {
      return this.GetTemplateV2(selector.InvokeOrDefault<GetIndexTemplateV2Descriptor, IGetIndexTemplateV2Request>(new GetIndexTemplateV2Descriptor().Name(name)));
    }

    public Task<GetIndexTemplateV2Response> GetTemplateV2Async(
      Name name = null,
      Func<GetIndexTemplateV2Descriptor, IGetIndexTemplateV2Request> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetTemplateV2Async(selector.InvokeOrDefault<GetIndexTemplateV2Descriptor, IGetIndexTemplateV2Request>(new GetIndexTemplateV2Descriptor().Name(name)), ct);
    }

    public GetIndexTemplateV2Response GetTemplateV2(IGetIndexTemplateV2Request request) => this.DoRequest<IGetIndexTemplateV2Request, GetIndexTemplateV2Response>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetIndexTemplateV2Response> GetTemplateV2Async(
      IGetIndexTemplateV2Request request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetIndexTemplateV2Request, GetIndexTemplateV2Response>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetMappingResponse GetMapping<TDocument>(
      Func<GetMappingDescriptor<TDocument>, IGetMappingRequest> selector = null)
      where TDocument : class
    {
      return this.GetMapping(selector.InvokeOrDefault<GetMappingDescriptor<TDocument>, IGetMappingRequest>(new GetMappingDescriptor<TDocument>()));
    }

    public Task<GetMappingResponse> GetMappingAsync<TDocument>(
      Func<GetMappingDescriptor<TDocument>, IGetMappingRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.GetMappingAsync(selector.InvokeOrDefault<GetMappingDescriptor<TDocument>, IGetMappingRequest>(new GetMappingDescriptor<TDocument>()), ct);
    }

    public GetMappingResponse GetMapping(IGetMappingRequest request) => this.DoRequest<IGetMappingRequest, GetMappingResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetMappingResponse> GetMappingAsync(
      IGetMappingRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetMappingRequest, GetMappingResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetIndexSettingsResponse GetSettings(
      Indices index = null,
      Func<GetIndexSettingsDescriptor, IGetIndexSettingsRequest> selector = null)
    {
      return this.GetSettings(selector.InvokeOrDefault<GetIndexSettingsDescriptor, IGetIndexSettingsRequest>(new GetIndexSettingsDescriptor().Index(index)));
    }

    public Task<GetIndexSettingsResponse> GetSettingsAsync(
      Indices index = null,
      Func<GetIndexSettingsDescriptor, IGetIndexSettingsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetSettingsAsync(selector.InvokeOrDefault<GetIndexSettingsDescriptor, IGetIndexSettingsRequest>(new GetIndexSettingsDescriptor().Index(index)), ct);
    }

    public GetIndexSettingsResponse GetSettings(IGetIndexSettingsRequest request) => this.DoRequest<IGetIndexSettingsRequest, GetIndexSettingsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetIndexSettingsResponse> GetSettingsAsync(
      IGetIndexSettingsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetIndexSettingsRequest, GetIndexSettingsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetIndexTemplateResponse GetTemplate(
      Names name = null,
      Func<GetIndexTemplateDescriptor, IGetIndexTemplateRequest> selector = null)
    {
      return this.GetTemplate(selector.InvokeOrDefault<GetIndexTemplateDescriptor, IGetIndexTemplateRequest>(new GetIndexTemplateDescriptor().Name(name)));
    }

    public Task<GetIndexTemplateResponse> GetTemplateAsync(
      Names name = null,
      Func<GetIndexTemplateDescriptor, IGetIndexTemplateRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetTemplateAsync(selector.InvokeOrDefault<GetIndexTemplateDescriptor, IGetIndexTemplateRequest>(new GetIndexTemplateDescriptor().Name(name)), ct);
    }

    public GetIndexTemplateResponse GetTemplate(IGetIndexTemplateRequest request) => this.DoRequest<IGetIndexTemplateRequest, GetIndexTemplateResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetIndexTemplateResponse> GetTemplateAsync(
      IGetIndexTemplateRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetIndexTemplateRequest, GetIndexTemplateResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public MigrateToDataStreamResponse MigrateToDataStream(
      Name name,
      Func<MigrateToDataStreamDescriptor, IMigrateToDataStreamRequest> selector = null)
    {
      return this.MigrateToDataStream(selector.InvokeOrDefault<MigrateToDataStreamDescriptor, IMigrateToDataStreamRequest>(new MigrateToDataStreamDescriptor(name)));
    }

    public Task<MigrateToDataStreamResponse> MigrateToDataStreamAsync(
      Name name,
      Func<MigrateToDataStreamDescriptor, IMigrateToDataStreamRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.MigrateToDataStreamAsync(selector.InvokeOrDefault<MigrateToDataStreamDescriptor, IMigrateToDataStreamRequest>(new MigrateToDataStreamDescriptor(name)), ct);
    }

    public MigrateToDataStreamResponse MigrateToDataStream(IMigrateToDataStreamRequest request) => this.DoRequest<IMigrateToDataStreamRequest, MigrateToDataStreamResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<MigrateToDataStreamResponse> MigrateToDataStreamAsync(
      IMigrateToDataStreamRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IMigrateToDataStreamRequest, MigrateToDataStreamResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public OpenIndexResponse Open(
      Indices index,
      Func<OpenIndexDescriptor, IOpenIndexRequest> selector = null)
    {
      return this.Open(selector.InvokeOrDefault<OpenIndexDescriptor, IOpenIndexRequest>(new OpenIndexDescriptor(index)));
    }

    public Task<OpenIndexResponse> OpenAsync(
      Indices index,
      Func<OpenIndexDescriptor, IOpenIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.OpenAsync(selector.InvokeOrDefault<OpenIndexDescriptor, IOpenIndexRequest>(new OpenIndexDescriptor(index)), ct);
    }

    public OpenIndexResponse Open(IOpenIndexRequest request) => this.DoRequest<IOpenIndexRequest, OpenIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<OpenIndexResponse> OpenAsync(IOpenIndexRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IOpenIndexRequest, OpenIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public PromoteDataStreamResponse PromoteDataStream(
      Name name,
      Func<PromoteDataStreamDescriptor, IPromoteDataStreamRequest> selector = null)
    {
      return this.PromoteDataStream(selector.InvokeOrDefault<PromoteDataStreamDescriptor, IPromoteDataStreamRequest>(new PromoteDataStreamDescriptor(name)));
    }

    public Task<PromoteDataStreamResponse> PromoteDataStreamAsync(
      Name name,
      Func<PromoteDataStreamDescriptor, IPromoteDataStreamRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PromoteDataStreamAsync(selector.InvokeOrDefault<PromoteDataStreamDescriptor, IPromoteDataStreamRequest>(new PromoteDataStreamDescriptor(name)), ct);
    }

    public PromoteDataStreamResponse PromoteDataStream(IPromoteDataStreamRequest request) => this.DoRequest<IPromoteDataStreamRequest, PromoteDataStreamResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PromoteDataStreamResponse> PromoteDataStreamAsync(
      IPromoteDataStreamRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPromoteDataStreamRequest, PromoteDataStreamResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutAliasResponse PutAlias(
      Indices index,
      Name name,
      Func<PutAliasDescriptor, IPutAliasRequest> selector = null)
    {
      return this.PutAlias(selector.InvokeOrDefault<PutAliasDescriptor, IPutAliasRequest>(new PutAliasDescriptor(index, name)));
    }

    public Task<PutAliasResponse> PutAliasAsync(
      Indices index,
      Name name,
      Func<PutAliasDescriptor, IPutAliasRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutAliasAsync(selector.InvokeOrDefault<PutAliasDescriptor, IPutAliasRequest>(new PutAliasDescriptor(index, name)), ct);
    }

    public PutAliasResponse PutAlias(IPutAliasRequest request) => this.DoRequest<IPutAliasRequest, PutAliasResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutAliasResponse> PutAliasAsync(IPutAliasRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IPutAliasRequest, PutAliasResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public PutIndexTemplateV2Response PutTemplateV2(
      Name name,
      Func<PutIndexTemplateV2Descriptor, IPutIndexTemplateV2Request> selector)
    {
      return this.PutTemplateV2(selector.InvokeOrDefault<PutIndexTemplateV2Descriptor, IPutIndexTemplateV2Request>(new PutIndexTemplateV2Descriptor(name)));
    }

    public Task<PutIndexTemplateV2Response> PutTemplateV2Async(
      Name name,
      Func<PutIndexTemplateV2Descriptor, IPutIndexTemplateV2Request> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutTemplateV2Async(selector.InvokeOrDefault<PutIndexTemplateV2Descriptor, IPutIndexTemplateV2Request>(new PutIndexTemplateV2Descriptor(name)), ct);
    }

    public PutIndexTemplateV2Response PutTemplateV2(IPutIndexTemplateV2Request request) => this.DoRequest<IPutIndexTemplateV2Request, PutIndexTemplateV2Response>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutIndexTemplateV2Response> PutTemplateV2Async(
      IPutIndexTemplateV2Request request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutIndexTemplateV2Request, PutIndexTemplateV2Response>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutMappingResponse PutMapping<TDocument>(
      Func<PutMappingDescriptor<TDocument>, IPutMappingRequest> selector)
      where TDocument : class
    {
      return this.PutMapping(selector.InvokeOrDefault<PutMappingDescriptor<TDocument>, IPutMappingRequest>(new PutMappingDescriptor<TDocument>()));
    }

    public Task<PutMappingResponse> PutMappingAsync<TDocument>(
      Func<PutMappingDescriptor<TDocument>, IPutMappingRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.PutMappingAsync(selector.InvokeOrDefault<PutMappingDescriptor<TDocument>, IPutMappingRequest>(new PutMappingDescriptor<TDocument>()), ct);
    }

    public PutMappingResponse PutMapping(IPutMappingRequest request) => this.DoRequest<IPutMappingRequest, PutMappingResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutMappingResponse> PutMappingAsync(
      IPutMappingRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutMappingRequest, PutMappingResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public UpdateIndexSettingsResponse UpdateSettings(
      Indices index,
      Func<UpdateIndexSettingsDescriptor, IUpdateIndexSettingsRequest> selector)
    {
      return this.UpdateSettings(selector.InvokeOrDefault<UpdateIndexSettingsDescriptor, IUpdateIndexSettingsRequest>(new UpdateIndexSettingsDescriptor().Index(index)));
    }

    public Task<UpdateIndexSettingsResponse> UpdateSettingsAsync(
      Indices index,
      Func<UpdateIndexSettingsDescriptor, IUpdateIndexSettingsRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.UpdateSettingsAsync(selector.InvokeOrDefault<UpdateIndexSettingsDescriptor, IUpdateIndexSettingsRequest>(new UpdateIndexSettingsDescriptor().Index(index)), ct);
    }

    public UpdateIndexSettingsResponse UpdateSettings(IUpdateIndexSettingsRequest request) => this.DoRequest<IUpdateIndexSettingsRequest, UpdateIndexSettingsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<UpdateIndexSettingsResponse> UpdateSettingsAsync(
      IUpdateIndexSettingsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IUpdateIndexSettingsRequest, UpdateIndexSettingsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutIndexTemplateResponse PutTemplate(
      Name name,
      Func<PutIndexTemplateDescriptor, IPutIndexTemplateRequest> selector)
    {
      return this.PutTemplate(selector.InvokeOrDefault<PutIndexTemplateDescriptor, IPutIndexTemplateRequest>(new PutIndexTemplateDescriptor(name)));
    }

    public Task<PutIndexTemplateResponse> PutTemplateAsync(
      Name name,
      Func<PutIndexTemplateDescriptor, IPutIndexTemplateRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutTemplateAsync(selector.InvokeOrDefault<PutIndexTemplateDescriptor, IPutIndexTemplateRequest>(new PutIndexTemplateDescriptor(name)), ct);
    }

    public PutIndexTemplateResponse PutTemplate(IPutIndexTemplateRequest request) => this.DoRequest<IPutIndexTemplateRequest, PutIndexTemplateResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutIndexTemplateResponse> PutTemplateAsync(
      IPutIndexTemplateRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutIndexTemplateRequest, PutIndexTemplateResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public RecoveryStatusResponse RecoveryStatus(
      Indices index = null,
      Func<RecoveryStatusDescriptor, IRecoveryStatusRequest> selector = null)
    {
      return this.RecoveryStatus(selector.InvokeOrDefault<RecoveryStatusDescriptor, IRecoveryStatusRequest>(new RecoveryStatusDescriptor().Index(index)));
    }

    public Task<RecoveryStatusResponse> RecoveryStatusAsync(
      Indices index = null,
      Func<RecoveryStatusDescriptor, IRecoveryStatusRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RecoveryStatusAsync(selector.InvokeOrDefault<RecoveryStatusDescriptor, IRecoveryStatusRequest>(new RecoveryStatusDescriptor().Index(index)), ct);
    }

    public RecoveryStatusResponse RecoveryStatus(IRecoveryStatusRequest request) => this.DoRequest<IRecoveryStatusRequest, RecoveryStatusResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<RecoveryStatusResponse> RecoveryStatusAsync(
      IRecoveryStatusRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IRecoveryStatusRequest, RecoveryStatusResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public RefreshResponse Refresh(Indices index = null, Func<RefreshDescriptor, IRefreshRequest> selector = null) => this.Refresh(selector.InvokeOrDefault<RefreshDescriptor, IRefreshRequest>(new RefreshDescriptor().Index(index)));

    public Task<RefreshResponse> RefreshAsync(
      Indices index = null,
      Func<RefreshDescriptor, IRefreshRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RefreshAsync(selector.InvokeOrDefault<RefreshDescriptor, IRefreshRequest>(new RefreshDescriptor().Index(index)), ct);
    }

    public RefreshResponse Refresh(IRefreshRequest request) => this.DoRequest<IRefreshRequest, RefreshResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<RefreshResponse> RefreshAsync(IRefreshRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IRefreshRequest, RefreshResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public ReloadSearchAnalyzersResponse ReloadSearchAnalyzers(
      Indices index,
      Func<ReloadSearchAnalyzersDescriptor, IReloadSearchAnalyzersRequest> selector = null)
    {
      return this.ReloadSearchAnalyzers(selector.InvokeOrDefault<ReloadSearchAnalyzersDescriptor, IReloadSearchAnalyzersRequest>(new ReloadSearchAnalyzersDescriptor(index)));
    }

    public Task<ReloadSearchAnalyzersResponse> ReloadSearchAnalyzersAsync(
      Indices index,
      Func<ReloadSearchAnalyzersDescriptor, IReloadSearchAnalyzersRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ReloadSearchAnalyzersAsync(selector.InvokeOrDefault<ReloadSearchAnalyzersDescriptor, IReloadSearchAnalyzersRequest>(new ReloadSearchAnalyzersDescriptor(index)), ct);
    }

    public ReloadSearchAnalyzersResponse ReloadSearchAnalyzers(IReloadSearchAnalyzersRequest request) => this.DoRequest<IReloadSearchAnalyzersRequest, ReloadSearchAnalyzersResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ReloadSearchAnalyzersResponse> ReloadSearchAnalyzersAsync(
      IReloadSearchAnalyzersRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IReloadSearchAnalyzersRequest, ReloadSearchAnalyzersResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ResolveIndexResponse Resolve(
      Names name,
      Func<ResolveIndexDescriptor, IResolveIndexRequest> selector = null)
    {
      return this.Resolve(selector.InvokeOrDefault<ResolveIndexDescriptor, IResolveIndexRequest>(new ResolveIndexDescriptor(name)));
    }

    public Task<ResolveIndexResponse> ResolveAsync(
      Names name,
      Func<ResolveIndexDescriptor, IResolveIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ResolveAsync(selector.InvokeOrDefault<ResolveIndexDescriptor, IResolveIndexRequest>(new ResolveIndexDescriptor(name)), ct);
    }

    public ResolveIndexResponse Resolve(IResolveIndexRequest request) => this.DoRequest<IResolveIndexRequest, ResolveIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ResolveIndexResponse> ResolveAsync(
      IResolveIndexRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IResolveIndexRequest, ResolveIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public RolloverIndexResponse Rollover(
      Name alias,
      Func<RolloverIndexDescriptor, IRolloverIndexRequest> selector = null)
    {
      return this.Rollover(selector.InvokeOrDefault<RolloverIndexDescriptor, IRolloverIndexRequest>(new RolloverIndexDescriptor(alias)));
    }

    public Task<RolloverIndexResponse> RolloverAsync(
      Name alias,
      Func<RolloverIndexDescriptor, IRolloverIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RolloverAsync(selector.InvokeOrDefault<RolloverIndexDescriptor, IRolloverIndexRequest>(new RolloverIndexDescriptor(alias)), ct);
    }

    public RolloverIndexResponse Rollover(IRolloverIndexRequest request) => this.DoRequest<IRolloverIndexRequest, RolloverIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<RolloverIndexResponse> RolloverAsync(
      IRolloverIndexRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IRolloverIndexRequest, RolloverIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public SegmentsResponse Segments(
      Indices index = null,
      Func<SegmentsDescriptor, ISegmentsRequest> selector = null)
    {
      return this.Segments(selector.InvokeOrDefault<SegmentsDescriptor, ISegmentsRequest>(new SegmentsDescriptor().Index(index)));
    }

    public Task<SegmentsResponse> SegmentsAsync(
      Indices index = null,
      Func<SegmentsDescriptor, ISegmentsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.SegmentsAsync(selector.InvokeOrDefault<SegmentsDescriptor, ISegmentsRequest>(new SegmentsDescriptor().Index(index)), ct);
    }

    public SegmentsResponse Segments(ISegmentsRequest request) => this.DoRequest<ISegmentsRequest, SegmentsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<SegmentsResponse> SegmentsAsync(ISegmentsRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ISegmentsRequest, SegmentsResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public IndicesShardStoresResponse ShardStores(
      Indices index = null,
      Func<IndicesShardStoresDescriptor, IIndicesShardStoresRequest> selector = null)
    {
      return this.ShardStores(selector.InvokeOrDefault<IndicesShardStoresDescriptor, IIndicesShardStoresRequest>(new IndicesShardStoresDescriptor().Index(index)));
    }

    public Task<IndicesShardStoresResponse> ShardStoresAsync(
      Indices index = null,
      Func<IndicesShardStoresDescriptor, IIndicesShardStoresRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ShardStoresAsync(selector.InvokeOrDefault<IndicesShardStoresDescriptor, IIndicesShardStoresRequest>(new IndicesShardStoresDescriptor().Index(index)), ct);
    }

    public IndicesShardStoresResponse ShardStores(IIndicesShardStoresRequest request) => this.DoRequest<IIndicesShardStoresRequest, IndicesShardStoresResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<IndicesShardStoresResponse> ShardStoresAsync(
      IIndicesShardStoresRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IIndicesShardStoresRequest, IndicesShardStoresResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ShrinkIndexResponse Shrink(
      IndexName index,
      IndexName target,
      Func<ShrinkIndexDescriptor, IShrinkIndexRequest> selector = null)
    {
      return this.Shrink(selector.InvokeOrDefault<ShrinkIndexDescriptor, IShrinkIndexRequest>(new ShrinkIndexDescriptor(index, target)));
    }

    public Task<ShrinkIndexResponse> ShrinkAsync(
      IndexName index,
      IndexName target,
      Func<ShrinkIndexDescriptor, IShrinkIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ShrinkAsync(selector.InvokeOrDefault<ShrinkIndexDescriptor, IShrinkIndexRequest>(new ShrinkIndexDescriptor(index, target)), ct);
    }

    public ShrinkIndexResponse Shrink(IShrinkIndexRequest request) => this.DoRequest<IShrinkIndexRequest, ShrinkIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ShrinkIndexResponse> ShrinkAsync(IShrinkIndexRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IShrinkIndexRequest, ShrinkIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public SplitIndexResponse Split(
      IndexName index,
      IndexName target,
      Func<SplitIndexDescriptor, ISplitIndexRequest> selector = null)
    {
      return this.Split(selector.InvokeOrDefault<SplitIndexDescriptor, ISplitIndexRequest>(new SplitIndexDescriptor(index, target)));
    }

    public Task<SplitIndexResponse> SplitAsync(
      IndexName index,
      IndexName target,
      Func<SplitIndexDescriptor, ISplitIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.SplitAsync(selector.InvokeOrDefault<SplitIndexDescriptor, ISplitIndexRequest>(new SplitIndexDescriptor(index, target)), ct);
    }

    public SplitIndexResponse Split(ISplitIndexRequest request) => this.DoRequest<ISplitIndexRequest, SplitIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<SplitIndexResponse> SplitAsync(ISplitIndexRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ISplitIndexRequest, SplitIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public IndicesStatsResponse Stats(
      Indices index = null,
      Func<IndicesStatsDescriptor, IIndicesStatsRequest> selector = null)
    {
      return this.Stats(selector.InvokeOrDefault<IndicesStatsDescriptor, IIndicesStatsRequest>(new IndicesStatsDescriptor().Index(index)));
    }

    public Task<IndicesStatsResponse> StatsAsync(
      Indices index = null,
      Func<IndicesStatsDescriptor, IIndicesStatsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StatsAsync(selector.InvokeOrDefault<IndicesStatsDescriptor, IIndicesStatsRequest>(new IndicesStatsDescriptor().Index(index)), ct);
    }

    public IndicesStatsResponse Stats(IIndicesStatsRequest request) => this.DoRequest<IIndicesStatsRequest, IndicesStatsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<IndicesStatsResponse> StatsAsync(IIndicesStatsRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IIndicesStatsRequest, IndicesStatsResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public UnfreezeIndexResponse Unfreeze(
      IndexName index,
      Func<UnfreezeIndexDescriptor, IUnfreezeIndexRequest> selector = null)
    {
      return this.Unfreeze(selector.InvokeOrDefault<UnfreezeIndexDescriptor, IUnfreezeIndexRequest>(new UnfreezeIndexDescriptor(index)));
    }

    public Task<UnfreezeIndexResponse> UnfreezeAsync(
      IndexName index,
      Func<UnfreezeIndexDescriptor, IUnfreezeIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.UnfreezeAsync(selector.InvokeOrDefault<UnfreezeIndexDescriptor, IUnfreezeIndexRequest>(new UnfreezeIndexDescriptor(index)), ct);
    }

    public UnfreezeIndexResponse Unfreeze(IUnfreezeIndexRequest request) => this.DoRequest<IUnfreezeIndexRequest, UnfreezeIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<UnfreezeIndexResponse> UnfreezeAsync(
      IUnfreezeIndexRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IUnfreezeIndexRequest, UnfreezeIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public BulkAliasResponse BulkAlias(
      Func<BulkAliasDescriptor, IBulkAliasRequest> selector)
    {
      return this.BulkAlias(selector.InvokeOrDefault<BulkAliasDescriptor, IBulkAliasRequest>(new BulkAliasDescriptor()));
    }

    public Task<BulkAliasResponse> BulkAliasAsync(
      Func<BulkAliasDescriptor, IBulkAliasRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.BulkAliasAsync(selector.InvokeOrDefault<BulkAliasDescriptor, IBulkAliasRequest>(new BulkAliasDescriptor()), ct);
    }

    public BulkAliasResponse BulkAlias(IBulkAliasRequest request) => this.DoRequest<IBulkAliasRequest, BulkAliasResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<BulkAliasResponse> BulkAliasAsync(IBulkAliasRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IBulkAliasRequest, BulkAliasResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public ValidateQueryResponse ValidateQuery<TDocument>(
      Func<ValidateQueryDescriptor<TDocument>, IValidateQueryRequest> selector = null)
      where TDocument : class
    {
      return this.ValidateQuery(selector.InvokeOrDefault<ValidateQueryDescriptor<TDocument>, IValidateQueryRequest>(new ValidateQueryDescriptor<TDocument>()));
    }

    public Task<ValidateQueryResponse> ValidateQueryAsync<TDocument>(
      Func<ValidateQueryDescriptor<TDocument>, IValidateQueryRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.ValidateQueryAsync(selector.InvokeOrDefault<ValidateQueryDescriptor<TDocument>, IValidateQueryRequest>(new ValidateQueryDescriptor<TDocument>()), ct);
    }

    public ValidateQueryResponse ValidateQuery(IValidateQueryRequest request) => this.DoRequest<IValidateQueryRequest, ValidateQueryResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ValidateQueryResponse> ValidateQueryAsync(
      IValidateQueryRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IValidateQueryRequest, ValidateQueryResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
