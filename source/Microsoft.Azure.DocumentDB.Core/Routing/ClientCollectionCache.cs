// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.ClientCollectionCache
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Routing
{
  internal sealed class ClientCollectionCache : CollectionCache
  {
    private readonly IStoreModel storeModel;
    private readonly IAuthorizationTokenProvider tokenProvider;
    private readonly IRetryPolicyFactory retryPolicy;
    private readonly ISessionContainer sessionContainer;

    public ClientCollectionCache(
      ISessionContainer sessionContainer,
      IStoreModel storeModel,
      IAuthorizationTokenProvider tokenProvider,
      IRetryPolicyFactory retryPolicy)
    {
      this.storeModel = storeModel != null ? storeModel : throw new ArgumentNullException(nameof (storeModel));
      this.tokenProvider = tokenProvider;
      this.retryPolicy = retryPolicy;
      this.sessionContainer = sessionContainer;
    }

    protected override Task<DocumentCollection> GetByRidAsync(
      string apiVersion,
      string collectionRid,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      IDocumentClientRetryPolicy retryPolicyInstance = (IDocumentClientRetryPolicy) new ClearingSessionContainerClientRetryPolicy(this.sessionContainer, this.retryPolicy.GetRequestPolicy());
      return TaskHelper.InlineIfPossible<DocumentCollection>((Func<Task<DocumentCollection>>) (() => this.ReadCollectionAsync(PathsHelper.GeneratePath(ResourceType.Collection, collectionRid, false), cancellationToken, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    protected override Task<DocumentCollection> GetByNameAsync(
      string apiVersion,
      string resourceAddress,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      IDocumentClientRetryPolicy retryPolicyInstance = (IDocumentClientRetryPolicy) new ClearingSessionContainerClientRetryPolicy(this.sessionContainer, this.retryPolicy.GetRequestPolicy());
      return TaskHelper.InlineIfPossible<DocumentCollection>((Func<Task<DocumentCollection>>) (() => this.ReadCollectionAsync(resourceAddress, cancellationToken, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<DocumentCollection> ReadCollectionAsync(
      string collectionLink,
      CancellationToken cancellationToken,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      cancellationToken.ThrowIfCancellationRequested();
      DocumentCollection resource;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Collection, collectionLink, AuthorizationTokenType.PrimaryMasterKey, (INameValueCollection) new DictionaryNameValueCollection()))
      {
        request.Headers["x-ms-date"] = DateTime.UtcNow.ToString("r");
        request.Headers["authorization"] = this.tokenProvider.GetUserAuthorizationToken(request.ResourceAddress, PathsHelper.GetResourcePath(request.ResourceType), "GET", request.Headers, AuthorizationTokenType.PrimaryMasterKey, out string _);
        using (new ActivityScope(Guid.NewGuid()))
        {
          retryPolicyInstance?.OnBeforeSendRequest(request);
          using (DocumentServiceResponse response = await this.storeModel.ProcessMessageAsync(request))
            resource = new ResourceResponse<DocumentCollection>(response).Resource;
        }
      }
      return resource;
    }
  }
}
