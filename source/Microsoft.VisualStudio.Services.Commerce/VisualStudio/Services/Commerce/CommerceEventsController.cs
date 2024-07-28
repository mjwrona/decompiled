// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceEventsController
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [XmlFormatter]
  public class CommerceEventsController : CommerceControllerBase
  {
    internal override string Layer => nameof (CommerceEventsController);

    [HttpPost]
    [TraceFilter(5103201, 5103250)]
    [TraceExceptions(5103249)]
    [TraceRequest(5103202)]
    [TraceResponse(5103248)]
    [ClientResponseType(typeof (string), null, null)]
    public HttpResponseMessage HandleSubscriptionNotifications()
    {
      ArgumentUtility.CheckForNull<HttpRequestMessage>(this.Request, "Request");
      CommerceValidators.CheckForMissingContentType(this.TfsRequestContext, this.Request);
      EntityEvent entityFromRequest = this.GetEntityFromRequest();
      CommerceCommons.SetRequestContextUniqueIdentifier(this.TfsRequestContext, entityFromRequest.OperationId);
      string resourceType = (string) null;
      if (entityFromRequest.Properties != null)
        resourceType = ((IEnumerable<EntityProperty>) entityFromRequest.Properties).Where<EntityProperty>((Func<EntityProperty, bool>) (e => string.Compare(e.PropertyName, "ResourceType", StringComparison.InvariantCultureIgnoreCase) == 0)).Select<EntityProperty, string>((Func<EntityProperty, string>) (p => p.PropertyValue)).FirstOrDefault<string>();
      if (resourceType == null)
        resourceType = "account";
      CommerceValidators.CheckResourceTypeIsAccount(this.TfsRequestContext, resourceType);
      Guid guid = new Guid(entityFromRequest?.EntityId?.Id ?? new Guid().ToString());
      ArgumentUtility.CheckForEmptyGuid(guid, "subscriptionId");
      PlatformSubscriptionService service = this.TfsRequestContext.GetService<PlatformSubscriptionService>();
      try
      {
        switch (entityFromRequest.EntityState)
        {
          case EntityState.Deleted:
          case EntityState.Enabled:
          case EntityState.Disabled:
            SubscriptionStatus statusIdMapping = this.getStatusIdMapping(entityFromRequest.EntityState);
            PlatformSubscriptionService subscriptionService = service;
            IVssRequestContext tfsRequestContext = this.TfsRequestContext;
            AzureSubscriptionInternal subscription = new AzureSubscriptionInternal();
            subscription.AzureSubscriptionId = guid;
            subscription.AzureSubscriptionStatusId = statusIdMapping;
            AccountProviderNamespace? providerNamespaceId = new AccountProviderNamespace?();
            subscriptionService.UpdateAzureSubscription(tfsRequestContext, subscription, providerNamespaceId);
            break;
          case EntityState.Registered:
            service.CreateAzureSubscription(this.TfsRequestContext, new AzureSubscriptionInternal()
            {
              AzureSubscriptionId = guid,
              AzureSubscriptionStatusId = SubscriptionStatus.Active
            });
            break;
          default:
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
              ReasonPhrase = HostingResources.InvalidEntityState((object) entityFromRequest.EntityState)
            });
        }
      }
      catch (AzureSubscriptionDoesNotExistException ex)
      {
        this.TfsRequestContext.TraceException(5103249, this.Area, this.Layer, (Exception) ex);
      }
      return new HttpResponseMessage(HttpStatusCode.OK);
    }

    private EntityEvent GetEntityFromRequest()
    {
      EntityEvent result = this.Request.Content.ReadAsAsync<EntityEvent>((IEnumerable<MediaTypeFormatter>) this.Configuration.Formatters, (IFormatterLogger) new CommerceFormatterLogger(this.TfsRequestContext, 5103204)).Result;
      if (result != null)
        return result;
      this.TfsRequestContext.Trace(5103249, TraceLevel.Error, this.Area, this.Layer, "Incoming request failed to deserialize as EntityEvent");
      throw new HttpResponseException(HttpStatusCode.InternalServerError);
    }

    private SubscriptionStatus getStatusIdMapping(EntityState entityState)
    {
      SubscriptionStatus statusIdMapping = SubscriptionStatus.Unknown;
      switch (entityState)
      {
        case EntityState.Deleted:
          statusIdMapping = SubscriptionStatus.Deleted;
          break;
        case EntityState.Enabled:
          statusIdMapping = SubscriptionStatus.Active;
          break;
        case EntityState.Disabled:
          statusIdMapping = SubscriptionStatus.Disabled;
          break;
      }
      return statusIdMapping;
    }
  }
}
