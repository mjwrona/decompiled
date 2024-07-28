// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.ClientCollectionCache
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal class ClientCollectionCache : CollectionCache
  {
    private readonly IStoreModel storeModel;
    private readonly ICosmosAuthorizationTokenProvider tokenProvider;
    private readonly IRetryPolicyFactory retryPolicy;
    private readonly ISessionContainer sessionContainer;

    public ClientCollectionCache(
      ISessionContainer sessionContainer,
      IStoreModel storeModel,
      ICosmosAuthorizationTokenProvider tokenProvider,
      IRetryPolicyFactory retryPolicy)
    {
      this.storeModel = storeModel ?? throw new ArgumentNullException(nameof (storeModel));
      this.tokenProvider = tokenProvider;
      this.retryPolicy = retryPolicy;
      this.sessionContainer = sessionContainer;
    }

    protected override Task<ContainerProperties> GetByRidAsync(
      string apiVersion,
      string collectionRid,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      IDocumentClientRetryPolicy retryPolicyInstance = (IDocumentClientRetryPolicy) new ClearingSessionContainerClientRetryPolicy(this.sessionContainer, this.retryPolicy.GetRequestPolicy());
      return TaskHelper.InlineIfPossible<ContainerProperties>((Func<Task<ContainerProperties>>) (() => this.ReadCollectionAsync(PathsHelper.GeneratePath(ResourceType.Collection, collectionRid, false), retryPolicyInstance, trace, clientSideRequestStatistics, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    protected override Task<ContainerProperties> GetByNameAsync(
      string apiVersion,
      string resourceAddress,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      IDocumentClientRetryPolicy retryPolicyInstance = (IDocumentClientRetryPolicy) new ClearingSessionContainerClientRetryPolicy(this.sessionContainer, this.retryPolicy.GetRequestPolicy());
      return TaskHelper.InlineIfPossible<ContainerProperties>((Func<Task<ContainerProperties>>) (() => this.ReadCollectionAsync(resourceAddress, retryPolicyInstance, trace, clientSideRequestStatistics, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    internal override Task<ContainerProperties> ResolveByNameAsync(
      string apiVersion,
      string resourceAddress,
      bool forceRefesh,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken)
    {
      return forceRefesh && this.sessionContainer != null ? TaskHelper.InlineIfPossible<ContainerProperties>((Func<Task<ContainerProperties>>) (async () =>
      {
        string oldRid = (await base.ResolveByNameAsync(apiVersion, resourceAddress, false, trace, clientSideRequestStatistics, cancellationToken))?.ResourceId;
        ContainerProperties containerProperties1 = await base.ResolveByNameAsync(apiVersion, resourceAddress, forceRefesh, trace, clientSideRequestStatistics, cancellationToken);
        if (oldRid != null && oldRid != containerProperties1?.ResourceId)
          this.sessionContainer.ClearTokenByCollectionFullname(PathsHelper.GetCollectionPath(resourceAddress));
        ContainerProperties containerProperties2 = containerProperties1;
        oldRid = (string) null;
        return containerProperties2;
      }), (IRetryPolicy) null, cancellationToken) : TaskHelper.InlineIfPossible<ContainerProperties>((Func<Task<ContainerProperties>>) (() => base.ResolveByNameAsync(apiVersion, resourceAddress, forceRefesh, trace, clientSideRequestStatistics, cancellationToken)), (IRetryPolicy) null, cancellationToken);
    }

    public override Task<ContainerProperties> ResolveCollectionAsync(
      DocumentServiceRequest request,
      CancellationToken cancellationToken,
      ITrace trace)
    {
      return TaskHelper.InlineIfPossible<ContainerProperties>((Func<Task<ContainerProperties>>) (() => this.ResolveCollectionWithSessionContainerCleanupAsync(request, (Func<Task<ContainerProperties>>) (() => base.ResolveCollectionAsync(request, cancellationToken, trace)))), (IRetryPolicy) null, cancellationToken);
    }

    public override Task<ContainerProperties> ResolveCollectionAsync(
      DocumentServiceRequest request,
      TimeSpan refreshAfter,
      CancellationToken cancellationToken,
      ITrace trace)
    {
      return TaskHelper.InlineIfPossible<ContainerProperties>((Func<Task<ContainerProperties>>) (() => this.ResolveCollectionWithSessionContainerCleanupAsync(request, (Func<Task<ContainerProperties>>) (() => base.ResolveCollectionAsync(request, refreshAfter, cancellationToken, trace)))), (IRetryPolicy) null, cancellationToken);
    }

    private async Task<ContainerProperties> ResolveCollectionWithSessionContainerCleanupAsync(
      DocumentServiceRequest request,
      Func<Task<ContainerProperties>> resolveContainerProvider)
    {
      string previouslyResolvedCollectionRid = request?.RequestContext?.ResolvedCollectionRid;
      ContainerProperties containerProperties1 = await resolveContainerProvider();
      if (this.sessionContainer != null && previouslyResolvedCollectionRid != null && previouslyResolvedCollectionRid != containerProperties1.ResourceId)
        this.sessionContainer.ClearTokenByResourceId(previouslyResolvedCollectionRid);
      ContainerProperties containerProperties2 = containerProperties1;
      previouslyResolvedCollectionRid = (string) null;
      return containerProperties2;
    }

    private async Task<ContainerProperties> ReadCollectionAsync(
      string collectionLink,
      IDocumentClientRetryPolicy retryPolicyInstance,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken)
    {
      ContainerProperties containerProperties;
      using (ITrace childTrace = trace.StartChild("Read Collection", TraceComponent.Transport, TraceLevel.Info))
      {
        cancellationToken.ThrowIfCancellationRequested();
        RequestNameValueCollection headers = new RequestNameValueCollection();
        using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Collection, collectionLink, AuthorizationTokenType.PrimaryMasterKey, (INameValueCollection) headers))
        {
          headers.XDate = Rfc1123DateTimeCache.UtcNow();
          request.RequestContext.ClientRequestStatistics = clientSideRequestStatistics ?? (IClientSideRequestStatistics) new ClientSideRequestStatisticsTraceDatum(DateTime.UtcNow, trace.Summary);
          if (clientSideRequestStatistics == null)
            childTrace.AddDatum("Client Side Request Stats", (object) request.RequestContext.ClientRequestStatistics);
          headers.Authorization = await this.tokenProvider.GetUserAuthorizationTokenAsync(request.ResourceAddress, PathsHelper.GetResourcePath(request.ResourceType), "GET", request.Headers, AuthorizationTokenType.PrimaryMasterKey, childTrace);
          using (new ActivityScope(Guid.NewGuid()))
          {
            retryPolicyInstance?.OnBeforeSendRequest(request);
            try
            {
              using (DocumentServiceResponse response = await this.storeModel.ProcessMessageAsync(request))
                containerProperties = CosmosResource.FromStream<ContainerProperties>(response);
            }
            catch (DocumentClientException ex)
            {
              childTrace.AddDatum("Exception Message", (object) ex.Message);
              throw;
            }
          }
        }
      }
      return containerProperties;
    }
  }
}
