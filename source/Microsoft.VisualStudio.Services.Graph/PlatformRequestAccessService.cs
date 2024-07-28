// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.PlatformRequestAccessService
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph
{
  public class PlatformRequestAccessService : IPlatformRequestAccessService, IVssFrameworkService
  {
    private const string RequestAccessEventType = "ms.vss-sps-notifications.request-access-event";
    private const string DisableRequestAccessEmail = "VisualStudio.Services.Identity.RequestAccess.DisableRequestAccessEmail";
    private const string TraceArea = "Identity";
    private const string TraceLayer = "PlatformRequestAccessService";
    private const int MaxNoOfIdentities = 20;
    private const int MaxMessageLength = 200;

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public bool SendRequestAccessEmail(
      IVssRequestContext requestContext,
      string message,
      string urlRequested = null,
      string projectUri = null)
    {
      try
      {
        requestContext.TraceEnter(11983, "Identity", nameof (PlatformRequestAccessService), nameof (SendRequestAccessEmail));
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.RequestAccess.DisableRequestAccessEmail"))
        {
          requestContext.Trace(11991, TraceLevel.Info, "Identity", nameof (PlatformRequestAccessService), "SendRequestAccesEmail disabled");
          return false;
        }
        requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
        if (!requestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(requestContext.Elevate(), "Policy.AllowRequestAccessToken", true).EffectiveValue)
        {
          requestContext.Trace(11996, TraceLevel.Info, "Identity", nameof (PlatformRequestAccessService), "Request access policy validation enterprise policy was not enabled.");
          return false;
        }
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        if (userIdentity == null)
        {
          requestContext.Trace(11984, TraceLevel.Info, "Identity", nameof (PlatformRequestAccessService), "Unable to get the authenticated user, skipping email notification");
          return false;
        }
        Microsoft.VisualStudio.Services.Identity.Identity requesterIdentity = this.GetRequesterIdentityFromUserService(requestContext, userIdentity.Descriptor);
        if (requesterIdentity == null)
        {
          requestContext.Trace(11993, TraceLevel.Info, "Identity", nameof (PlatformRequestAccessService), "Unable to get the authenticated user in user service");
          return false;
        }
        if (message == null)
          message = string.Empty;
        if (message.Length > 200)
        {
          requestContext.Trace(11989, TraceLevel.Info, "Identity", nameof (PlatformRequestAccessService), "Message length is longer that accepted length.");
          return false;
        }
        requestContext.TraceDataConditionally(11982, TraceLevel.Info, "Identity", nameof (PlatformRequestAccessService), "Received Request Access Call", (Func<object>) (() => (object) new
        {
          Id = requesterIdentity.Id,
          message = message,
          Name = requestContext.ServiceHost.Name
        }), nameof (SendRequestAccessEmail));
        if (requesterIdentity.IsExternalUser && this.IsUserInSameTenant(requestContext, requesterIdentity))
        {
          IList<Guid> recipientIds = this.GetCollectionPCAUserIds(requestContext.Elevate());
          recipientIds.AddRange<Guid, IList<Guid>>((IEnumerable<Guid>) this.GetProjectAdministrators(requestContext.Elevate(), projectUri));
          if (recipientIds.Count<Guid>() == 0)
          {
            requestContext.TraceAlways(11985, TraceLevel.Info, "Identity", nameof (PlatformRequestAccessService), "Found no PCA identities for collection " + requestContext.ServiceHost.Name + ".");
            return false;
          }
          RequestAccessEvent requestAccessEvent = this.GetRequestAccessEvent(requestContext, requesterIdentity, message, urlRequested);
          this.SendEmailNotification(requestContext, requestAccessEvent, recipientIds);
          requestContext.TraceDataConditionally(11988, TraceLevel.Info, "Identity", nameof (PlatformRequestAccessService), "Successfully sent email to following admins", (Func<object>) (() => (object) new
          {
            recipientIds = recipientIds
          }), nameof (SendRequestAccessEmail));
          return true;
        }
        requestContext.Trace(11992, TraceLevel.Error, "Identity", nameof (PlatformRequestAccessService), "Failed to send request access email, since either user is not AAD user or not in same tenant");
        return false;
      }
      catch (Exception ex)
      {
        string message1 = "Failed to trigger request access notifications for collection " + requestContext.ServiceHost.Name;
        requestContext.Trace(11986, TraceLevel.Info, "Identity", nameof (PlatformRequestAccessService), message1);
        requestContext.TraceException(11987, TraceLevel.Error, "Identity", nameof (PlatformRequestAccessService), ex);
        return false;
      }
      finally
      {
        requestContext.TraceLeave(11983, "Identity", nameof (PlatformRequestAccessService), nameof (SendRequestAccessEmail));
      }
    }

    private IList<Guid> GetProjectAdministrators(
      IVssRequestContext requestContext,
      string projectUri)
    {
      if (projectUri == null)
      {
        requestContext.Trace(11994, TraceLevel.Info, "Identity", nameof (PlatformRequestAccessService), "ProjectUri:" + projectUri + " is empty.");
        return (IList<Guid>) Enumerable.Empty<Guid>().ToList<Guid>();
      }
      PlatformIdentityService service = requestContext.GetService<PlatformIdentityService>();
      IdentityScope scope = service.GetScope(requestContext, projectUri);
      if (scope == null)
        return (IList<Guid>) Enumerable.Empty<Guid>().ToList<Guid>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ListGroups(requestContext, new Guid[1]
      {
        scope.Id
      }, true, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && IdentityHelper.IsWellKnownGroup(x.Descriptor, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup))).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>()).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        return (IList<Guid>) Enumerable.Empty<Guid>().ToList<Guid>();
      ICollection<IdentityDescriptor> members = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identity.Descriptor
      }, QueryMembership.Direct, (IEnumerable<string>) null, false).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>().Members;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) members.ToList<IdentityDescriptor>(), QueryMembership.Direct, (IEnumerable<string>) null, false);
      requestContext.Trace(11995, TraceLevel.Info, "Identity", nameof (PlatformRequestAccessService), string.Format("Number of Project Administrators found are {0}", (object) source.Count));
      return (IList<Guid>) source.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id)).Take<Guid>(20).ToList<Guid>();
    }

    private Microsoft.VisualStudio.Services.Identity.Identity GetRequesterIdentityFromUserService(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      SubjectDescriptor legacyDescriptor = vssRequestContext.GetService<IGraphIdentifierConversionService>().GetCuidBasedDescriptorByLegacyDescriptor(vssRequestContext, identityDescriptor);
      return vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<SubjectDescriptor>) new List<SubjectDescriptor>()
      {
        legacyDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private bool IsUserInSameTenant(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity requestIdentity)
    {
      Guid property = requestIdentity.GetProperty<Guid>("Domain", Guid.Empty);
      Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
      return !property.Equals(Guid.Empty) && property.Equals(organizationAadTenantId);
    }

    private IList<Guid> GetCollectionPCAUserIds(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      }, QueryMembership.Direct, (IEnumerable<string>) null, true).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      return identity == null ? (IList<Guid>) new List<Guid>() : (IList<Guid>) identity.MemberIds.Take<Guid>(20).ToList<Guid>();
    }

    private void SendEmailNotification(
      IVssRequestContext requestContext,
      RequestAccessEvent requestAccessEvent,
      IList<Guid> recipients)
    {
      INotificationEventService service = requestContext.GetService<INotificationEventService>();
      VssNotificationEvent theEvent = new VssNotificationEvent(requestAccessEvent.Serialize<RequestAccessEvent>(), "ms.vss-sps-notifications.request-access-event");
      foreach (Guid recipient in (IEnumerable<Guid>) recipients)
        theEvent.AddActor("recipient", recipient);
      service.PublishSystemEvent(requestContext, theEvent);
    }

    private RequestAccessEvent GetRequestAccessEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity requesterIdentity,
      string message,
      string urlRequested)
    {
      string providerDisplayName = requesterIdentity.ProviderDisplayName;
      string property = requesterIdentity.GetProperty<string>("Account", "");
      string name = requestContext.ServiceHost.Name;
      string organizationUrl = this.GetOrganizationUrl(requestContext);
      return new RequestAccessEvent()
      {
        OrganizationUrl = organizationUrl,
        OrganizationName = name,
        SettingsUrl = organizationUrl + "_settings/users",
        Message = message,
        RequesterName = providerDisplayName,
        RequesterEmail = property,
        UrlRequested = urlRequested ?? organizationUrl
      };
    }

    private string GetOrganizationUrl(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.TFS, AccessMappingConstants.PublicAccessMappingMoniker);
  }
}
