// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.SessionMetadataService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public class SessionMetadataService : 
    VssMemoryCacheService<SessionKey, SessionRequest>,
    ISessionMetadataService,
    IVssFrameworkService
  {
    public SessionMetadataService()
      : base((IEqualityComparer<SessionKey>) SessionKeyComparer.Instance, TimeSpan.FromMinutes(10.0))
    {
    }

    public SessionRequest GetSessionMetadata(
      IVssRequestContext requestContext,
      SessionKey sessionKey)
    {
      using (requestContext.GetTracerFacade().Enter((object) this, nameof (GetSessionMetadata)))
      {
        SessionRequest requestFromItemStore;
        if (!this.TryGetValue(requestContext, sessionKey, out requestFromItemStore))
        {
          requestFromItemStore = this.GetSessionRequestFromItemStore(requestContext, sessionKey);
          this.CacheSessionMetadata(requestContext, sessionKey, requestFromItemStore);
        }
        return requestFromItemStore;
      }
    }

    public async Task StoreSessionMetadataAsync(
      IVssRequestContext requestContext,
      SessionKey sessionKey,
      SessionRequest metadata)
    {
      SessionMetadataService sendInTheThisObject = this;
      using (requestContext.GetTracerFacade().Enter((object) sendInTheThisObject, nameof (StoreSessionMetadataAsync)))
      {
        IItemStore itemStore = sendInTheThisObject.GetItemStore(requestContext);
        Locator sessionContainer = sendInTheThisObject.ComputeContainerName(sessionKey.SessionId);
        Locator metadataLocator = sendInTheThisObject.GetMetadataLocator(sessionKey.Protocol);
        SessionItem sessionItem = new SessionItem()
        {
          SessionName = sessionKey.SessionId.Name,
          SessionMetadata = metadata.Data,
          Feed = metadata.Feed,
          SourceType = metadata.Source,
          SessionCreator = new Guid?(requestContext.GetAuthenticatedIdentity().Id)
        };
        ContainerItem addContainerAsync = await itemStore.GetOrAddContainerAsync(requestContext, new ContainerItem()
        {
          Name = sessionContainer,
          ExpirationTime = new DateTime?(DateTime.UtcNow.AddMinutes(30.0))
        });
        if (await itemStore.GetItemAsync<SessionItem>(requestContext, sessionContainer, metadataLocator) != null)
          throw new ProvenanceSessionAlreadyExistsException(string.Format("Provenance session {0} already exists.", (object) sessionKey.SessionId));
        if (!await itemStore.CompareSwapItemAsync(requestContext, sessionContainer, metadataLocator, (StoredItem) sessionItem))
          throw new TargetModifiedAfterReadException(nameof (StoreSessionMetadataAsync));
        sendInTheThisObject.CacheSessionMetadata(requestContext, sessionKey, metadata);
        itemStore = (IItemStore) null;
        sessionContainer = (Locator) null;
        metadataLocator = (Locator) null;
        sessionItem = (SessionItem) null;
      }
    }

    private void CacheSessionMetadata(
      IVssRequestContext requestContext,
      SessionKey sessionKey,
      SessionRequest sessionRequest)
    {
      using (ITracerBlock tracerBlock = requestContext.GetTracerFacade().Enter((object) this, nameof (CacheSessionMetadata)))
      {
        if (this.TryAdd(requestContext, sessionKey, sessionRequest))
          tracerBlock.TraceInfo(string.Format("Cached session: {0}", (object) sessionKey));
        else
          tracerBlock.TraceInfo(string.Format("Could not cache session: {0}", (object) sessionKey));
      }
    }

    private SessionRequest GetSessionRequestFromItemStore(
      IVssRequestContext requestContext,
      SessionKey sessionKey)
    {
      using (requestContext.GetTracerFacade().Enter((object) this, nameof (GetSessionRequestFromItemStore)))
      {
        IItemStore itemStore = this.GetItemStore(requestContext);
        Locator sessionContainer = this.ComputeContainerName(sessionKey.SessionId);
        Locator metadataLocator = this.GetMetadataLocator(sessionKey.Protocol);
        SessionItem sessionItem = AsyncPump.Run<SessionItem>((Func<Task<SessionItem>>) (() => itemStore.GetItemAsync<SessionItem>(requestContext, sessionContainer, metadataLocator)));
        if (sessionItem == null)
          return (SessionRequest) null;
        Guid? sessionCreator = sessionItem.SessionCreator;
        Guid id = requestContext.GetAuthenticatedIdentity().Id;
        if (sessionCreator.HasValue)
        {
          Guid? nullable = sessionCreator;
          Guid guid = id;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != guid ? 1 : 0) : 0) : 1) != 0)
          {
            string message = string.Format("Provenance Session retrieved by non-creator. Session key: {0}; CreatorVSID: {1}; CurrentIdentityVSID: {2}", (object) sessionKey, (object) sessionCreator, (object) id);
            requestContext.Trace(5724040, TraceLevel.Info, "Packaging", "ProvenanceSession", message);
          }
        }
        return new SessionRequest()
        {
          Data = sessionItem.SessionMetadata,
          Feed = sessionItem.Feed,
          Source = sessionItem.SourceType
        };
      }
    }

    private IItemStore GetItemStore(IVssRequestContext requestContext) => (IItemStore) requestContext.GetService<SessionMetadataItemStore>();

    private Locator GetMetadataLocator(string protocol) => new Locator(new string[1]
    {
      protocol.ToLower() + "/metadata_V1"
    });

    private Locator ComputeContainerName(SessionId sessionId) => new Locator(new string[1]
    {
      string.Format("session/{0}", (object) sessionId.Id)
    });
  }
}
