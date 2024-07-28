// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionRequestService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  public class ExtensionRequestService : 
    VssBaseService,
    IExtensionRequestService,
    IVssFrameworkService
  {
    private bool m_requestDeletionTaskQueued;
    private List<RequestedExtension> m_requestDeletionQueue = new List<RequestedExtension>();
    private ILockName m_requestDeletionLockName;
    private const string c_RequestsTabUri = "/_admin/_extensions?tab=requested&status=pending";
    private const int c_maxEmailRecipients = 100;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_requestDeletionLockName = this.CreateLockName(systemRequestContext, "requestDeletion");

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public List<RequestedExtension> GetRequests(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      IGalleryService service1 = vssRequestContext.GetService<IGalleryService>();
      List<RequestedExtension> items;
      using (ExtensionRequestComponent component = requestContext.CreateComponent<ExtensionRequestComponent>())
        items = component.GetRequests().GetCurrent<RequestedExtension>().Items;
      // ISSUE: explicit non-virtual call
      if (items != null && __nonvirtual (items.Count) > 0)
      {
        HashSet<string> stringSet = new HashSet<string>();
        foreach (RequestedExtension requestedExtension in items)
          stringSet.Add(GalleryUtil.CreateFullyQualifiedName(requestedExtension.PublisherName, requestedExtension.ExtensionName));
        List<FilterCriteria> filterCriteriaList = new List<FilterCriteria>();
        foreach (string str in stringSet)
          filterCriteriaList.Add(new FilterCriteria()
          {
            FilterType = 7,
            Value = str
          });
        foreach (string str in GalleryUtil.GetInstallationTargetsForProduct("vsts"))
          filterCriteriaList.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = str
          });
        ExtensionQuery query = new ExtensionQuery()
        {
          Filters = new List<QueryFilter>()
          {
            new QueryFilter() { Criteria = filterCriteriaList }
          },
          Flags = ExtensionQueryFlags.None
        };
        ExtensionQueryResult extensionQueryResult = service1.QueryExtensions(vssRequestContext, query);
        if (extensionQueryResult != null)
        {
          List<PublishedExtension> extensions = extensionQueryResult.Results[0].Extensions;
          List<RequestedExtension> collection = new List<RequestedExtension>();
          this.PopulateExtensionRequestIdentities(requestContext, items.SelectMany<RequestedExtension, ExtensionRequest>((Func<RequestedExtension, IEnumerable<ExtensionRequest>>) (requestedExtension => (IEnumerable<ExtensionRequest>) requestedExtension.ExtensionRequests)));
          foreach (RequestedExtension requestedExtension in items)
          {
            RequestedExtension request = requestedExtension;
            PublishedExtension publishedExtension = (PublishedExtension) null;
            if (extensions != null)
              publishedExtension = extensions.Find((Predicate<PublishedExtension>) (ext => ext.Publisher.PublisherName.Equals(request.PublisherName, StringComparison.OrdinalIgnoreCase) && ext.ExtensionName.Equals(request.ExtensionName, StringComparison.OrdinalIgnoreCase)));
            if (publishedExtension != null)
              request.PublisherDisplayName = publishedExtension.Publisher.DisplayName;
            else
              collection.Add(request);
          }
          if (collection.Count > 0)
          {
            IVssRequestContext context = requestContext.Elevate();
            TeamFoundationTaskService service2 = vssRequestContext.GetService<TeamFoundationTaskService>();
            ILockName deletionLockName = this.m_requestDeletionLockName;
            using (context.Lock(deletionLockName))
            {
              this.m_requestDeletionQueue.AddRange((IEnumerable<RequestedExtension>) collection);
              if (!this.m_requestDeletionTaskQueued)
              {
                service2.AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.RequestDeletionTask), (object) null, DateTime.UtcNow, 0));
                this.m_requestDeletionTaskQueued = true;
              }
            }
          }
        }
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (!requestContext.GetService<IExtensionPoliciesService>().HasManagePermission(requestContext))
        identity = requestContext.GetUserIdentity();
      return this.WrapRequests(items, identity);
    }

    public RequestedExtension RequestExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string requestMessage)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      PublishedExtension publishedExtension = vssRequestContext.GetService<IPublishedExtensionCache>().GetPublishedExtension(vssRequestContext, publisherName, extensionName, "latest");
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity = requestContext.GetUserIdentity();
      List<RequestedExtension> items;
      using (ExtensionRequestComponent component = requestContext.CreateComponent<ExtensionRequestComponent>())
        items = component.RequestExtension(publisherName, extensionName, requestIdentity.Id, requestMessage).GetCurrent<RequestedExtension>().Items;
      HashSet<Guid> source1 = new HashSet<Guid>();
      HashSet<Guid> source2 = new HashSet<Guid>();
      foreach (RequestedExtension requestedExtension in items)
      {
        source1.Add(new Guid(requestedExtension.ExtensionRequests[0].RequestedBy.Id));
        if (!string.IsNullOrEmpty(requestedExtension.ExtensionRequests[0].ResolvedBy.Id))
          source2.Add(new Guid(requestedExtension.ExtensionRequests[0].ResolvedBy.Id));
      }
      IdentityService service = requestContext.GetService<IdentityService>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source3 = service.ReadIdentities(requestContext, (IList<Guid>) source1.ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source4 = service.ReadIdentities(requestContext, (IList<Guid>) source2.ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null);
      foreach (RequestedExtension requestedExtension in items)
      {
        RequestedExtension request = requestedExtension;
        request.PublisherDisplayName = publishedExtension.Publisher.DisplayName;
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = source3.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && string.Equals(identity.Id.ToString(), request.ExtensionRequests[0].RequestedBy.Id))).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        request.ExtensionRequests[0].RequestedBy = identity1 == null ? (IdentityRef) null : this.ToIdentityRef(identity1);
        if (!string.IsNullOrEmpty(request.ExtensionRequests[0].ResolvedBy.Id))
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = source4.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && string.Equals(identity.Id.ToString(), request.ExtensionRequests[0].ResolvedBy.Id))).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          request.ExtensionRequests[0].ResolvedBy = identity2 == null ? (IdentityRef) null : this.ToIdentityRef(identity2);
        }
        else
          request.ExtensionRequests[0].ResolvedBy = (IdentityRef) null;
      }
      RequestedExtension requestedExtension1 = this.WrapRequests(items, requestIdentity).FirstOrDefault<RequestedExtension>();
      this.SendCustomerIntelligence(requestContext, publisherName, extensionName, requestIdentity.Id, nameof (RequestExtension));
      ExtensionRequest extensionRequest = requestedExtension1.ExtensionRequests.Find((Predicate<ExtensionRequest>) (req => req.RequestedBy.Id.Equals(requestIdentity.Id.ToString())));
      this.SendExtensionRequestEvents(requestContext, publisherName, extensionName, (IEnumerable<ExtensionRequest>) new ExtensionRequest[1]
      {
        extensionRequest
      }, ExtensionRequestUpdateType.Created);
      return requestedExtension1;
    }

    public IList<ExtensionRequest> ResolveRequests(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string requesterId,
      string rejectMessage,
      ExtensionRequestState state)
    {
      Guid requesterId1 = Guid.Empty;
      if (requesterId != null)
        requesterId1 = new Guid(requesterId);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      requestContext.GetService<IExtensionPoliciesService>().CheckManagePermission(requestContext);
      List<ExtensionRequest> requests = (List<ExtensionRequest>) null;
      using (ExtensionRequestComponent component = requestContext.CreateComponent<ExtensionRequestComponent>())
      {
        ResultCollection resultCollection = component.ResolveExtensionRequest(publisherName, extensionName, userIdentity.Id, requesterId1, rejectMessage, state);
        if (resultCollection != null)
          requests = resultCollection.GetCurrent<ExtensionRequest>().Items;
      }
      if (requests == null)
        requests = new List<ExtensionRequest>();
      if (requests.Count > 0)
      {
        this.PopulateExtensionRequestIdentities(requestContext, (IEnumerable<ExtensionRequest>) requests);
        if (state == ExtensionRequestState.Accepted || state == ExtensionRequestState.Rejected)
        {
          ExtensionRequestUpdateType updateType = state == ExtensionRequestState.Accepted ? ExtensionRequestUpdateType.Approved : ExtensionRequestUpdateType.Rejected;
          this.SendExtensionRequestEvents(requestContext, publisherName, extensionName, (IEnumerable<ExtensionRequest>) requests, updateType);
        }
        this.SendCustomerIntelligence(requestContext, publisherName, extensionName, requesterId1, "ResolveRequest", state, userIdentity.Id);
      }
      return (IList<ExtensionRequest>) requests;
    }

    public IList<ExtensionRequest> DeleteRequests(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      RequestedExtension requestedExtension = new RequestedExtension()
      {
        PublisherName = publisherName,
        ExtensionName = extensionName,
        ExtensionRequests = new List<ExtensionRequest>()
        {
          new ExtensionRequest()
          {
            RequestedBy = new IdentityRef()
            {
              Id = userIdentity.Id.ToString()
            }
          }
        }
      };
      List<ExtensionRequest> requests = (List<ExtensionRequest>) null;
      using (ExtensionRequestComponent component = requestContext.CreateComponent<ExtensionRequestComponent>())
      {
        ResultCollection resultCollection = component.DeleteExtensionRequests(new List<RequestedExtension>()
        {
          requestedExtension
        });
        if (resultCollection != null)
          requests = resultCollection.GetCurrent<ExtensionRequest>().Items;
      }
      if (requests == null)
        requests = new List<ExtensionRequest>();
      if (requests.Count > 0)
      {
        this.PopulateExtensionRequestIdentities(requestContext, (IEnumerable<ExtensionRequest>) requests);
        this.SendExtensionRequestEvents(requestContext, publisherName, extensionName, (IEnumerable<ExtensionRequest>) requests, ExtensionRequestUpdateType.Deleted);
        this.SendCustomerIntelligence(requestContext, publisherName, extensionName, userIdentity.Id, "DeleteExtensionRequests");
      }
      return (IList<ExtensionRequest>) requests;
    }

    private void PopulateExtensionRequestIdentities(
      IVssRequestContext requestContext,
      IEnumerable<ExtensionRequest> requests)
    {
      List<IdentityRef> identityRefList = new List<IdentityRef>();
      foreach (ExtensionRequest request in requests)
      {
        if (request.RequestedBy != null)
          identityRefList.Add(request.RequestedBy);
        if (request.ResolvedBy != null)
          identityRefList.Add(request.ResolvedBy);
      }
      this.PopulateIdentityRefsFromIds(requestContext, (IEnumerable<IdentityRef>) identityRefList);
    }

    private void PopulateIdentityRefsFromIds(
      IVssRequestContext requestContext,
      IEnumerable<IdentityRef> identityRefs)
    {
      List<IdentityRef> identityRefList = new List<IdentityRef>();
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (IdentityRef identityRef in identityRefs)
      {
        Guid identityRefId = this.GetIdentityRefId(identityRef);
        if (identityRefId != Guid.Empty)
        {
          identityRefList.Add(identityRef);
          dictionary[identityRefId] = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        }
      }
      IList<Guid> list = (IList<Guid>) dictionary.Keys.ToList<Guid>();
      if (list.Count <= 0)
        return;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) requestContext.GetService<IdentityService>().ReadIdentities(requestContext, list, QueryMembership.None, (IEnumerable<string>) null))
      {
        if (readIdentity != null)
          dictionary[readIdentity.Id] = readIdentity;
      }
      foreach (IdentityRef identityRef in identityRefList)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity;
        if (dictionary.TryGetValue(this.GetIdentityRefId(identityRef), out identity) && identityRef != null && identity != null)
          identityRef.DisplayName = identity.DisplayName;
      }
    }

    private List<RequestedExtension> WrapRequests(
      List<RequestedExtension> requests,
      Microsoft.VisualStudio.Services.Identity.Identity identity = null)
    {
      Dictionary<Tuple<string, string>, RequestedExtension> dictionary = new Dictionary<Tuple<string, string>, RequestedExtension>();
      foreach (RequestedExtension request in requests)
      {
        Tuple<string, string> key = Tuple.Create<string, string>(request.PublisherName, request.ExtensionName);
        if (dictionary.ContainsKey(key))
        {
          RequestedExtension requestedExtension;
          dictionary.TryGetValue(key, out requestedExtension);
          if (requestedExtension != null)
          {
            if (identity == null || string.Equals(identity.Id.ToString(), request.ExtensionRequests[0].RequestedBy.Id))
              requestedExtension.ExtensionRequests.Add(request.ExtensionRequests[0]);
            ++requestedExtension.RequestCount;
          }
        }
        else
        {
          dictionary.Add(key, request);
          if (identity != null && !string.Equals(identity.Id.ToString(), request.ExtensionRequests[0].RequestedBy.Id))
            request.ExtensionRequests.Clear();
        }
      }
      return new List<RequestedExtension>((IEnumerable<RequestedExtension>) dictionary.Values);
    }

    private Guid GetIdentityRefId(IdentityRef identityRef)
    {
      Guid result;
      return identityRef != null && Guid.TryParse(identityRef.Id, out result) ? result : Guid.Empty;
    }

    private void AddNotificationActor(
      VssNotificationEvent notificationEvent,
      string role,
      IdentityRef identityRef)
    {
      Guid identityRefId = this.GetIdentityRefId(identityRef);
      if (!(identityRefId != Guid.Empty))
        return;
      notificationEvent.AddActor(role, identityRefId);
    }

    private void SendExtensionRequestEvents(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      IEnumerable<ExtensionRequest> requests,
      ExtensionRequestUpdateType updateType)
    {
      IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      PublishedExtension publishedExtension1 = vssRequestContext1.GetService<IPublishedExtensionCache>().GetPublishedExtension(vssRequestContext1, publisherName, extensionName, "latest");
      if (publishedExtension1 == null)
        return;
      PublishedExtension publishedExtension2 = new PublishedExtension()
      {
        ExtensionName = publishedExtension1.ExtensionName,
        DisplayName = publishedExtension1.DisplayName,
        ShortDescription = publishedExtension1.ShortDescription,
        LongDescription = publishedExtension1.LongDescription,
        Publisher = publishedExtension1.Publisher,
        Flags = publishedExtension1.Flags
      };
      IVssRequestContext vssRequestContext2 = requestContext.Elevate();
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> extensionManagers = (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) vssRequestContext2.GetService<IExtensionPoliciesService>().GetExtensionManagers(vssRequestContext2, 100);
      INotificationEventService service = requestContext.GetService<INotificationEventService>();
      foreach (ExtensionRequest request in requests)
      {
        VssNotificationEvent notificationEvent = new VssNotificationEvent()
        {
          ItemId = publishedExtension2.ExtensionId.ToString(),
          SourceEventCreatedTime = new DateTime?(request.RequestDate)
        };
        notificationEvent.EventType = "ms.vss-extmgmt-web.extension-request-event";
        notificationEvent.Data = (object) new ExtensionRequestEvent()
        {
          UpdateType = updateType,
          Request = request,
          Extension = publishedExtension2,
          HostName = requestContext.ServiceHost.Name,
          Host = new ExtensionHost()
          {
            Id = requestContext.ServiceHost.InstanceId,
            Name = requestContext.ServiceHost.Name
          },
          Links = this.GetExtensionRequestUrls(requestContext, publishedExtension1)
        };
        this.AddNotificationActor(notificationEvent, "resolvedBy", request.ResolvedBy);
        this.AddNotificationActor(notificationEvent, "requestedBy", request.RequestedBy);
        notificationEvent.AddActor(VssNotificationEvent.Roles.Initiator, requestContext.GetUserId());
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in extensionManagers)
          notificationEvent.AddActor("approver", identity.Id);
        service.PublishSystemEvent(requestContext, notificationEvent);
      }
      if (updateType != ExtensionRequestUpdateType.Created && updateType != ExtensionRequestUpdateType.Approved && updateType != ExtensionRequestUpdateType.Rejected)
        return;
      VssNotificationEvent theEvent = new VssNotificationEvent();
      theEvent.EventType = "ms.vss-extmgmt-web.extension-requests-event";
      theEvent.Data = (object) new ExtensionRequestsEvent()
      {
        UpdateType = updateType,
        Requests = requests.ToList<ExtensionRequest>(),
        Extension = publishedExtension2,
        Host = new ExtensionHost()
        {
          Id = requestContext.ServiceHost.InstanceId,
          Name = requestContext.ServiceHost.Name
        },
        Links = this.GetExtensionRequestUrls(requestContext, publishedExtension1)
      };
      theEvent.AddActor(VssNotificationEvent.Roles.Initiator, requestContext.GetUserId());
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in extensionManagers)
        theEvent.AddActor("approver", identity.Id);
      service.PublishSystemEvent(requestContext, theEvent);
    }

    private ExtensionRequestUrls GetExtensionRequestUrls(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension)
    {
      IVssRequestContext context = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.TFS, AccessMappingConstants.ClientAccessMappingMoniker);
      string str1;
      if (locationServiceUrl == null)
        str1 = (string) null;
      else
        str1 = locationServiceUrl.TrimEnd('/');
      string str2 = str1;
      string str3 = context.GetClient<GalleryHttpClient>().BaseAddress.ToString().TrimEnd('/');
      string str4 = str3;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        str4 += "/_gallery";
      ExtensionRequestUrls extensionRequestUrls1 = new ExtensionRequestUrls();
      extensionRequestUrls1.ExtensionPage = str4 + "/items/" + publishedExtension.Publisher.PublisherName + "." + publishedExtension.ExtensionName;
      extensionRequestUrls1.RequestPage = str2 + "/_admin/_extensions?tab=requested&status=pending";
      ExtensionRequestUrls extensionRequestUrls2 = extensionRequestUrls1;
      if ((publishedExtension.Flags & PublishedExtensionFlags.Public) != PublishedExtensionFlags.None)
        extensionRequestUrls2.ExtensionIcon = str3 + "/_apis/public/gallery/publisher/" + publishedExtension.Publisher.PublisherName + "/extension/" + publishedExtension.ExtensionName + "/latest/assetbyname/Microsoft.VisualStudio.Services.Icons.Default";
      return extensionRequestUrls2;
    }

    private IdentityRef ToIdentityRef(Microsoft.VisualStudio.Services.Identity.Identity identity) => new IdentityRef()
    {
      Id = identity.Id.ToString(),
      DisplayName = identity.DisplayName
    };

    private void SendCustomerIntelligence(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid requesterId,
      string methodName,
      ExtensionRequestState state = ExtensionRequestState.Open,
      Guid resolverId = default (Guid))
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, methodName);
      properties.Add("PublisherName", publisherName);
      properties.Add("ExtensionName", extensionName);
      properties.Add("RequesterId", (object) requesterId);
      if (resolverId != Guid.Empty)
        properties.Add("ResolverId", (object) resolverId);
      if (state != ExtensionRequestState.Open)
        properties.Add("RequestState", (object) state);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.Contributions, ExtensionManagementCustomerIntelligenceFeature.ExtensionManagementRequests, properties);
    }

    private void RequestDeletionTask(IVssRequestContext requestContext, object taskArgs)
    {
      List<RequestedExtension> requestedExtensions = new List<RequestedExtension>();
      using (requestContext.Elevate().Lock(this.m_requestDeletionLockName))
      {
        if (this.m_requestDeletionQueue != null)
          requestedExtensions.AddRange((IEnumerable<RequestedExtension>) this.m_requestDeletionQueue);
        this.m_requestDeletionQueue = new List<RequestedExtension>();
        this.m_requestDeletionTaskQueued = false;
      }
      if (requestedExtensions.Count <= 0)
        return;
      using (ExtensionRequestComponent component = requestContext.CreateComponent<ExtensionRequestComponent>())
        component.DeleteExtensionRequests(requestedExtensions);
    }
  }
}
