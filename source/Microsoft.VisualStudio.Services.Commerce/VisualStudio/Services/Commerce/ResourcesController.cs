// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourcesController
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [XmlFormatter]
  public class ResourcesController : CommerceControllerBase
  {
    internal Dictionary<SubscriptionStatus, ResourceState> SubscriptionStateToResourceStateMap = new Dictionary<SubscriptionStatus, ResourceState>()
    {
      {
        SubscriptionStatus.Active,
        ResourceState.Started
      },
      {
        SubscriptionStatus.Deleted,
        ResourceState.Stopped
      },
      {
        SubscriptionStatus.Disabled,
        ResourceState.Paused
      }
    };

    [HttpGet]
    [CaptureRdfeOperationId]
    [TraceFilter(5103401, 5103450)]
    [TraceExceptions(5103449)]
    [TraceResponse(5103448)]
    public HttpResponseMessage GetResource(
      string subscriptionId,
      string cloudServiceName,
      string resourceType,
      string resourceName)
    {
      Guid result;
      if (!Guid.TryParse(subscriptionId, out result))
        throw new ArgumentException("subscriptionId with value \"" + subscriptionId + "\" does not contain valid Guid");
      ArgumentUtility.CheckForEmptyGuid(result, nameof (subscriptionId));
      ArgumentUtility.CheckStringForNullOrEmpty(cloudServiceName, nameof (cloudServiceName));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceType, nameof (resourceType));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
      CommerceValidators.CheckResourceTypeIsAccount(this.TfsRequestContext, resourceType);
      PlatformSubscriptionService service = this.TfsRequestContext.GetService<PlatformSubscriptionService>();
      AzureSubscriptionInternal azureSubscription = service.GetAzureSubscription(this.TfsRequestContext, result);
      if (azureSubscription == null)
      {
        this.TfsRequestContext.Trace(5103449, TraceLevel.Error, this.Area, this.Layer, HostingResources.SubscriptionNotFound((object) result));
        return this.Request.CreateResponse<string>(HttpStatusCode.NotFound, HostingResources.SubscriptionNotFound((object) result));
      }
      ResourceState state;
      this.SubscriptionStateToResourceStateMap.TryGetValue(azureSubscription.AzureSubscriptionStatusId, out state);
      AzureResourceAccount azureResourceAccount = service.GetAzureResourceAccount(this.TfsRequestContext, result, AccountProviderNamespace.VisualStudioOnline, resourceName);
      if (azureResourceAccount == null)
        throw new AzureResourceAccountDoesNotExistException(resourceName);
      HttpResponseMessage response = this.Request.CreateResponse<ResourceOutput>(HttpStatusCode.OK, this.ResourceOutputBuilder.CreateResourceOutput(azureResourceAccount, this.TfsRequestContext, state));
      if (!string.IsNullOrEmpty(azureResourceAccount.ETag))
        response.Headers.ETag = new EntityTagHeaderValue("\"" + azureResourceAccount.ETag + "\"");
      else if (this.Request.Headers.IfMatch.Any<EntityTagHeaderValue>())
        response.Headers.ETag = this.Request.Headers.IfMatch.First<EntityTagHeaderValue>();
      string rdfeOperationId = CommerceCommons.GetRdfeOperationId(this.Request);
      if (!string.IsNullOrEmpty(rdfeOperationId))
        response.Headers.Add("x-ms-request-id", rdfeOperationId);
      return response;
    }

    [HttpPut]
    [CaptureRdfeOperationId]
    [TraceFilter(5103451, 5103500)]
    [TraceExceptions(5103499)]
    [TraceRequest(5103452)]
    [TraceResponse(5103496)]
    public HttpResponseMessage ProvisionOrUpdateResource(
      string subscriptionId,
      string cloudServiceName,
      string resourceType,
      string resourceName)
    {
      Guid result1;
      if (!Guid.TryParse(subscriptionId, out result1))
        throw new ArgumentException("subscriptionId with value \"" + subscriptionId + "\" does not contain valid Guid");
      ArgumentUtility.CheckForEmptyGuid(result1, nameof (subscriptionId));
      ArgumentUtility.CheckStringForNullOrEmpty(cloudServiceName, nameof (cloudServiceName));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceType, nameof (resourceType));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
      CommerceValidators.CheckForMissingContentType(this.TfsRequestContext, this.Request);
      CommerceValidators.CheckResourceTypeIsAccount(this.TfsRequestContext, resourceType);
      PlatformSubscriptionService service = this.TfsRequestContext.GetService<PlatformSubscriptionService>();
      ResourceInput inputFromXmlInput = this.GetResourceInputFromXMLInput();
      if (inputFromXmlInput == null)
      {
        this.TfsRequestContext.Trace(5103499, TraceLevel.Error, this.Area, this.Layer, "Incoming provision request with failed to deserialize request input");
        throw new HttpResponseException(HttpStatusCode.InternalServerError);
      }
      if (inputFromXmlInput.IntrinsicSettings != null)
      {
        XmlNode intrinsicSettingsXmlNode = ((IEnumerable<XmlNode>) inputFromXmlInput.IntrinsicSettings).FirstOrDefault<XmlNode>((System.Func<XmlNode, bool>) (xmlNode => string.Compare(xmlNode.Name, "CommerceIntrinsicSettings", StringComparison.InvariantCultureIgnoreCase) == 0));
        if (intrinsicSettingsXmlNode == null)
        {
          this.TfsRequestContext.Trace(5103499, TraceLevel.Error, this.Area, this.Layer, HostingResources.MissingRequiredIntrinsicSetting((object) "CommerceIntrinsicSettings", (object) inputFromXmlInput.IntrinsicSettings));
          throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }
        CommerceIntrinsicSettings intrinsicSettings = this.ParseCommerceIntrinsicSettings(intrinsicSettingsXmlNode);
        if (intrinsicSettings?.Items == null)
        {
          this.TfsRequestContext.Trace(5103499, TraceLevel.Error, this.Area, this.Layer, "Incoming provision request failed to deserialize intrinsic settings with request content");
          throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }
        AzureResourceAccount azureResourceAccount1 = new AzureResourceAccount()
        {
          AzureCloudServiceName = cloudServiceName,
          AzureResourceName = resourceName,
          AzureSubscriptionId = result1,
          ProviderNamespaceId = AccountProviderNamespace.VisualStudioOnline,
          AzureGeoRegion = inputFromXmlInput.CloudServiceSettings?.GeoRegion,
          ETag = inputFromXmlInput.ETag
        };
        Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        Dictionary<string, string> intrinsicSettingsDictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
        intrinsicSettings.Items.ForEach((Action<KeyValuePair>) (item => intrinsicSettingsDictionary.Add(item.Key, item.Value)));
        string str1;
        intrinsicSettingsDictionary.TryGetValue("operationType", out str1);
        string str2;
        intrinsicSettingsDictionary.TryGetValue("EMail", out str2);
        string str3;
        intrinsicSettingsDictionary.TryGetValue("AccountName", out str3);
        string str4;
        intrinsicSettingsDictionary.TryGetValue("SubscriptionSource", out str4);
        string identityDomain;
        intrinsicSettingsDictionary.TryGetValue("IdentityDomain", out identityDomain);
        string identityPuid;
        intrinsicSettingsDictionary.TryGetValue("Puid", out identityPuid);
        ArgumentUtility.CheckStringForNullOrEmpty(str1, "operationType");
        IVssRequestContext vssRequestContext1 = this.TfsRequestContext.Elevate();
        using (IVssRequestContext vssRequestContext2 = vssRequestContext1.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(vssRequestContext1, vssRequestContext1.ServiceHost.InstanceId, this.TfsRequestContext.UserContext))
        {
          vssRequestContext2.Items["Commerce.RequestSource"] = (object) "Azure";
          AzureResourceAccount azureResourceAccount2 = service.GetAzureResourceAccount(vssRequestContext2, result1, AccountProviderNamespace.VisualStudioOnline, resourceName, true);
          if (string.Compare(str1, "Update", StringComparison.InvariantCultureIgnoreCase) != 0)
          {
            if (azureResourceAccount2 != null)
              return this.Request.CreateResponse<ResourceOutput>(HttpStatusCode.OK, this.ResourceOutputBuilder.CreateResourceOutput(azureResourceAccount2, vssRequestContext2));
            try
            {
              service.InitializeInProgressOperation(vssRequestContext2, azureResourceAccount1);
            }
            catch (ConstraintException ex)
            {
              vssRequestContext1.Trace(5106085, TraceLevel.Verbose, this.Area, this.Layer, string.Format("Creating AzureResourceAccount failed with ConstraintException. SubscriptionId: {0}, CouldServiceName: {1}, ResourceType:{2}, AzureResourceName: {3} ", (object) subscriptionId, (object) cloudServiceName, (object) resourceType, (object) resourceName));
              AzureResourceAccount azureResourceAccount3 = service.GetAzureResourceAccount(vssRequestContext2, result1, AccountProviderNamespace.VisualStudioOnline, resourceName, true);
              if (azureResourceAccount3 != null)
                return this.Request.CreateResponse<ResourceOutput>(HttpStatusCode.OK, this.ResourceOutputBuilder.CreateResourceOutput(azureResourceAccount3, vssRequestContext2));
            }
          }
          if (!str1.Equals("Update", StringComparison.OrdinalIgnoreCase))
          {
            ArgumentUtility.CheckStringForNullOrEmpty(str2, "email");
            ArgumentUtility.CheckStringForNullOrEmpty(str3, "accountName");
            string domain = "Windows Live ID";
            string puid = (string) null;
            if (!string.IsNullOrEmpty(identityDomain))
              domain = identityDomain;
            if (domain.Equals("Windows Live ID", StringComparison.OrdinalIgnoreCase))
              puid = identityPuid;
            identity = IdentityHelper.GetOrCreateBindPendingIdentity(vssRequestContext2.Elevate(), domain, CommerceIdentityHelper.GetEmailFromUpn(str2), puid, callerName: nameof (ProvisionOrUpdateResource));
            SubscriptionSource result2;
            if (!string.IsNullOrEmpty(str4) && System.Enum.TryParse<SubscriptionSource>(str4, out result2) && (result2 == SubscriptionSource.EnterpriseAgreement || result2 == SubscriptionSource.Internal))
            {
              AzureSubscriptionInternal azureSubscription = service.GetAzureSubscription(vssRequestContext2, result1);
              azureSubscription.AzureSubscriptionSource = result2;
              service.UpdateAzureSubscription(vssRequestContext2, azureSubscription, new AccountProviderNamespace?(AccountProviderNamespace.VisualStudioOnline), false);
            }
          }
          AzureResourceAccount azureResourceAccount4;
          if (str1.Equals("Create", StringComparison.OrdinalIgnoreCase))
          {
            vssRequestContext2.Trace(5103454, TraceLevel.Verbose, this.Area, this.Layer, "Entering create mode, calling create and link account");
            azureResourceAccount4 = service.CreateAndLinkAccount(vssRequestContext2, azureResourceAccount1, str3, identity, CustomerIntelligenceEntryPoint.Azure);
          }
          else if (str1.Equals("Link", StringComparison.OrdinalIgnoreCase))
          {
            vssRequestContext2.Trace(5103454, TraceLevel.Verbose, this.Area, this.Layer, "Entering link mode, calling link account by name");
            azureResourceAccount4 = service.LinkCollectionByName(vssRequestContext2, azureResourceAccount1, str3, identity);
          }
          else if (str1.Equals("Update", StringComparison.OrdinalIgnoreCase))
          {
            vssRequestContext2.Trace(5103454, TraceLevel.Verbose, this.Area, this.Layer, "Entering update mode, calling update azure resource account associated resources");
            azureResourceAccount4 = service.GetAzureResourceAccount(vssRequestContext2, result1, AccountProviderNamespace.VisualStudioOnline, resourceName, true);
            if (azureResourceAccount4 == null)
            {
              vssRequestContext2.Trace(5103498, TraceLevel.Error, this.Area, this.Layer, "Incoming provision request not found for request content.");
              throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            service.UpdateAzureResourceAccountAssociatedResources(vssRequestContext2, azureResourceAccount4, intrinsicSettingsDictionary);
            if (!string.IsNullOrWhiteSpace(identityDomain))
            {
              try
              {
                this.CheckAccountOwner(vssRequestContext2, azureResourceAccount4, str2, identityPuid);
                identityDomain = this.EnableDisableAAD(vssRequestContext2, azureResourceAccount4, identityDomain);
              }
              catch (UserIsNotAccountOwnerException ex)
              {
                string message = "UserIsNotAccountOwnerException occurred for user with CUID " + identity?.GetProperty<string>("CUID", string.Empty);
                vssRequestContext2.Trace(5103455, TraceLevel.Error, this.Area, this.Layer, message);
                throw;
              }
              catch (InvalidIdentityIdTranslationException ex)
              {
                string message = string.Format("{0} occurred while linking/unlinking account {1} to the AAD directory", (object) "InvalidIdentityIdTranslationException", (object) azureResourceAccount4.AccountId);
                vssRequestContext2.Trace(5103456, TraceLevel.Error, this.Area, this.Layer, message);
                throw new HttpResponseException(new HttpResponseMessage()
                {
                  StatusCode = HttpStatusCode.BadRequest,
                  ReasonPhrase = HostingResources.AccountLinkingUnlinkingFailed((object) azureResourceAccount4.AccountId)
                });
              }
            }
            azureResourceAccount4.ETag = inputFromXmlInput.ETag;
            service.UpdateAzureResourceAccount(vssRequestContext2, azureResourceAccount4);
          }
          else
          {
            vssRequestContext2.Trace(5103499, TraceLevel.Error, this.Area, this.Layer, "Incoming provision request has invalid operation type " + str1);
            throw new HttpResponseException(new HttpResponseMessage()
            {
              StatusCode = HttpStatusCode.BadRequest,
              ReasonPhrase = HostingResources.InvalidOperationType((object) str1)
            });
          }
          if (!string.IsNullOrEmpty(identityDomain) && !identityDomain.Equals("Windows Live ID", StringComparison.OrdinalIgnoreCase) && azureResourceAccount4.AccountId != Guid.Empty)
          {
            HydrationHelper hydrationHelper = new HydrationHelper();
            if (str1.Equals("Create", StringComparison.OrdinalIgnoreCase))
              hydrationHelper.StartHydration(this.TfsRequestContext, azureResourceAccount4);
            else if (str1.Equals("Link", StringComparison.OrdinalIgnoreCase))
              hydrationHelper.StartHydration(this.TfsRequestContext, azureResourceAccount4);
          }
          HttpResponseMessage response = this.Request.CreateResponse<ResourceOutput>(HttpStatusCode.OK, this.ResourceOutputBuilder.CreateResourceOutput(azureResourceAccount4, vssRequestContext2));
          if (!string.IsNullOrEmpty(azureResourceAccount4.ETag))
            response.Headers.ETag = new EntityTagHeaderValue("\"" + azureResourceAccount4.ETag + "\"");
          string rdfeOperationId = CommerceCommons.GetRdfeOperationId(this.Request);
          if (!string.IsNullOrEmpty(rdfeOperationId))
            response.Headers.Add("x-ms-request-id", rdfeOperationId);
          return response;
        }
      }
      else
      {
        this.TfsRequestContext.Trace(5103454, TraceLevel.Verbose, this.Area, this.Layer, "Got a legacy create resource call on resource " + resourceName);
        AzureResourceAccount azureResourceAccount = service.GetAzureResourceAccount(this.TfsRequestContext, result1, AccountProviderNamespace.VisualStudioOnline, resourceName, true);
        if (azureResourceAccount == null)
          return this.Request.CreateResponse(HttpStatusCode.InternalServerError);
        if (azureResourceAccount.AccountId != Guid.Empty)
        {
          if (azureResourceAccount.AzureCloudServiceName != cloudServiceName)
          {
            azureResourceAccount.AlternateCloudServiceName = cloudServiceName;
            service.UpdateAzureResourceAccount(this.TfsRequestContext, azureResourceAccount);
          }
          if (string.IsNullOrEmpty(azureResourceAccount.ETag))
            azureResourceAccount.ETag = string.IsNullOrEmpty(inputFromXmlInput.ETag) ? Guid.NewGuid().ToString("D") : inputFromXmlInput.ETag;
          HttpResponseMessage response = this.Request.CreateResponse<ResourceOutput>(HttpStatusCode.OK, this.ResourceOutputBuilder.CreateResourceOutput(azureResourceAccount, this.TfsRequestContext));
          response.Headers.ETag = new EntityTagHeaderValue("\"" + azureResourceAccount.ETag + "\"");
          string rdfeOperationId = CommerceCommons.GetRdfeOperationId(this.Request);
          if (!string.IsNullOrEmpty(rdfeOperationId))
            response.Headers.Add("x-ms-request-id", rdfeOperationId);
          return response;
        }
        this.TfsRequestContext.Trace(5103499, TraceLevel.Error, this.Area, this.Layer, "Does not match the legacy resource name " + resourceName + " and cloud service name " + cloudServiceName);
        throw new HttpResponseException(HttpStatusCode.InternalServerError);
      }
    }

    [ExcludeFromCodeCoverage]
    protected virtual CommerceIntrinsicSettings ParseCommerceIntrinsicSettings(
      XmlNode intrinsicSettingsXmlNode)
    {
      return CommerceUtil.Deserialize<CommerceIntrinsicSettings>(intrinsicSettingsXmlNode.OuterXml);
    }

    [ExcludeFromCodeCoverage]
    protected virtual ResourceInput GetResourceInputFromXMLInput() => this.Request.Content.ReadAsAsync<ResourceInput>((IEnumerable<MediaTypeFormatter>) this.Configuration.Formatters, (IFormatterLogger) new CommerceFormatterLogger(this.TfsRequestContext, 5103499)).Result;

    [ExcludeFromCodeCoverage]
    internal virtual Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      string upn,
      string domain,
      string puid = null)
    {
      return IdentityHelper.GetOrCreateBindPendingIdentity(requestContext.Elevate(), domain, CommerceIdentityHelper.GetEmailFromUpn(upn), puid, callerName: nameof (GetIdentity));
    }

    internal void CheckAccountOwner(
      IVssRequestContext requestContext,
      AzureResourceAccount azureResourceAccount,
      string email,
      string identityPuid)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException();
      if (string.IsNullOrWhiteSpace(identityPuid) && string.IsNullOrWhiteSpace(email))
        return;
      Collection collection = (Collection) null;
      Guid organizationTenantId = Guid.Empty;
      CollectionHelper.WithCollectionContext(requestContext, azureResourceAccount.AccountId, (Action<IVssRequestContext>) (collectionContext =>
      {
        collection = collectionContext.GetService<ICollectionService>().GetCollection(collectionContext, (IEnumerable<string>) null);
        organizationTenantId = collectionContext.GetOrganizationAadTenantId();
      }), method: nameof (CheckAccountOwner));
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = this.GetIdentity(requestContext, email, "Windows Live ID", identityPuid);
      if (identity1.MasterId == collection.Owner)
        return;
      if (organizationTenantId == Guid.Empty)
      {
        requestContext.Trace(5105569, TraceLevel.Error, this.Area, this.Layer, string.Format("Validate account ownership failed to match exact for requestor with master id {0} ", (object) identity1) + string.Format("acting on unconnected account {0}.", (object) azureResourceAccount.AccountId));
        throw new UserIsNotAccountOwnerException(identity1.DisplayName, azureResourceAccount.AzureResourceName);
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        collection.Owner
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      string str = identity2 != null ? identity2.GetProperty<string>("Account", (string) null) : throw new UserIsNotAccountOwnerException(identity1.DisplayName, azureResourceAccount.AzureResourceName);
      if (email.Equals(str, StringComparison.OrdinalIgnoreCase))
        return;
      requestContext.Trace(5105569, TraceLevel.Error, this.Area, this.Layer, "Validate account ownership failed to match on email for CUID \"" + identity1.GetProperty<string>("CUID", string.Empty) + "\" " + string.Format("acting on account {0} in tenant {1} with actual owner CUID \"{2}\".", (object) azureResourceAccount.AccountId, (object) organizationTenantId, (object) identity2.GetProperty<string>("CUID", string.Empty)));
    }

    internal string EnableDisableAAD(
      IVssRequestContext newRequestContext,
      AzureResourceAccount azureResourceAccount,
      string identityDomain)
    {
      if (!string.IsNullOrEmpty(identityDomain))
      {
        Guid result;
        Guid.TryParse(identityDomain, out result);
        if (result == Guid.Empty)
          identityDomain = "Windows Live ID";
        if (newRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return identityDomain;
        Guid parentOrganizationId = CollectionHelper.GetParentOrganizationId(newRequestContext, azureResourceAccount.AccountId);
        Guid? tenantId;
        using (CommerceVssRequestContextExtensions.VssRequestContextHolder organization = newRequestContext.ToOrganization(parentOrganizationId))
        {
          IVssRequestContext requestContext = organization.RequestContext;
          tenantId = requestContext.GetService<IOrganizationService>().GetOrganization(requestContext, (IEnumerable<string>) null)?.TenantId;
        }
        Guid guid = result;
        Guid? nullable = tenantId;
        if ((nullable.HasValue ? (guid == nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          return identityDomain;
        newRequestContext.GetExtension<ICommerceAccountHandler>().UpdateAccountTenant(newRequestContext, result, parentOrganizationId);
        HydrationHelper hydrationHelper = this.GetHydrationHelper();
        if (result == Guid.Empty)
          hydrationHelper.StartDeHydration(newRequestContext, azureResourceAccount);
        else
          hydrationHelper.StartHydration(newRequestContext, azureResourceAccount);
      }
      return identityDomain;
    }

    internal virtual HydrationHelper GetHydrationHelper() => new HydrationHelper();

    [HttpDelete]
    [CaptureRdfeOperationId]
    [TraceFilter(5103501, 5103100)]
    [TraceExceptions(5103549)]
    public HttpResponseMessage DeleteResource(
      string subscriptionId,
      string cloudServiceName,
      string resourceType,
      string resourceName)
    {
      Guid result;
      if (!Guid.TryParse(subscriptionId, out result))
        throw new ArgumentException("subscriptionId with value \"" + subscriptionId + "\" does not contain valid Guid");
      ArgumentUtility.CheckForEmptyGuid(result, nameof (subscriptionId));
      ArgumentUtility.CheckStringForNullOrEmpty(cloudServiceName, nameof (cloudServiceName));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceType, nameof (resourceType));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
      PlatformSubscriptionService service = this.TfsRequestContext.GetService<PlatformSubscriptionService>();
      this.TfsRequestContext.Trace(5103502, TraceLevel.Info, this.Area, this.Layer, "Incoming DeleteResource request for " + resourceName + " resource of type " + resourceType + " " + string.Format("from subscription {0}, cloud service {1}", (object) result, (object) cloudServiceName));
      if (string.Compare("account", resourceType, StringComparison.InvariantCultureIgnoreCase) != 0)
      {
        resourceType = "account";
        this.TfsRequestContext.Trace(5103549, TraceLevel.Error, this.Area, this.Layer, "Incoming DeleteResource request has incorrect resource type " + resourceType + ", expected account");
      }
      AzureResourceAccount azureResourceAccount = service.GetAzureResourceAccount(this.TfsRequestContext, result, AccountProviderNamespace.VisualStudioOnline, resourceName, true);
      if (azureResourceAccount == null)
        throw new AzureResourceAccountDoesNotExistException(resourceName);
      service.DeleteAzureResourceAccount(this.TfsRequestContext, azureResourceAccount);
      if (azureResourceAccount.AccountId != Guid.Empty)
        this.GetHydrationHelper().StartDeHydration(this.TfsRequestContext, azureResourceAccount);
      return new HttpResponseMessage(HttpStatusCode.OK);
    }

    internal override string Layer => nameof (ResourcesController);

    internal IResourceOutputBuilder ResourceOutputBuilder { get; set; } = (IResourceOutputBuilder) new Microsoft.VisualStudio.Services.Commerce.ResourceOutputBuilder();
  }
}
