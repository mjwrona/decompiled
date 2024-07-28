// Decompiled with JetBrains decompiler
// Type: Nest.Specification.TasksApi.TasksNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.TasksApi
{
  public class TasksNamespace : Nest.NamespacedClientProxy
  {
    internal TasksNamespace(ElasticClient client)
      : base(client)
    {
    }

    public CancelTasksResponse Cancel(
      Func<CancelTasksDescriptor, ICancelTasksRequest> selector = null)
    {
      return this.Cancel(selector.InvokeOrDefault<CancelTasksDescriptor, ICancelTasksRequest>(new CancelTasksDescriptor()));
    }

    public Task<CancelTasksResponse> CancelAsync(
      Func<CancelTasksDescriptor, ICancelTasksRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CancelAsync(selector.InvokeOrDefault<CancelTasksDescriptor, ICancelTasksRequest>(new CancelTasksDescriptor()), ct);
    }

    public CancelTasksResponse Cancel(ICancelTasksRequest request) => this.DoRequest<ICancelTasksRequest, CancelTasksResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CancelTasksResponse> CancelAsync(ICancelTasksRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ICancelTasksRequest, CancelTasksResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public GetTaskResponse GetTask(TaskId taskId, Func<GetTaskDescriptor, IGetTaskRequest> selector = null) => this.GetTask(selector.InvokeOrDefault<GetTaskDescriptor, IGetTaskRequest>(new GetTaskDescriptor(taskId)));

    public Task<GetTaskResponse> GetTaskAsync(
      TaskId taskId,
      Func<GetTaskDescriptor, IGetTaskRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetTaskAsync(selector.InvokeOrDefault<GetTaskDescriptor, IGetTaskRequest>(new GetTaskDescriptor(taskId)), ct);
    }

    public GetTaskResponse GetTask(IGetTaskRequest request) => this.DoRequest<IGetTaskRequest, GetTaskResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetTaskResponse> GetTaskAsync(IGetTaskRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IGetTaskRequest, GetTaskResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public ListTasksResponse List(
      Func<ListTasksDescriptor, IListTasksRequest> selector = null)
    {
      return this.List(selector.InvokeOrDefault<ListTasksDescriptor, IListTasksRequest>(new ListTasksDescriptor()));
    }

    public Task<ListTasksResponse> ListAsync(
      Func<ListTasksDescriptor, IListTasksRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ListAsync(selector.InvokeOrDefault<ListTasksDescriptor, IListTasksRequest>(new ListTasksDescriptor()), ct);
    }

    public ListTasksResponse List(IListTasksRequest request) => this.DoRequest<IListTasksRequest, ListTasksResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ListTasksResponse> ListAsync(IListTasksRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IListTasksRequest, ListTasksResponse>(request, (IRequestParameters) request.RequestParameters, ct);
  }
}
