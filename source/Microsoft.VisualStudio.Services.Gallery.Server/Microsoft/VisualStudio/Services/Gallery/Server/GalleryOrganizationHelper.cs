// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryOrganizationHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.HostManagement.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using Microsoft.VisualStudio.Services.UserMapping;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public static class GalleryOrganizationHelper
  {
    private const string c_msTenantRegistryKey = "/Configuration/Service/Gallery/MsTenantId";
    private const string c_msTenantDefaultValue = "72F988BF-86F1-41AF-91AB-2D7CD011DB47";

    public static Guid? GetOrganizationIdForCollection(
      IVssRequestContext requestContext,
      Guid collectionId)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      Guid? parentHostId = requestContext.GetService<IHostManagementService>().GetServiceHostProperties(requestContext, collectionId)?.ParentHostId;
      GalleryOrganizationHelper.PublishOrgQueryPerfCIEvent(requestContext, collectionId, stopwatch, nameof (GetOrganizationIdForCollection));
      return parentHostId;
    }

    public static IList<Collection> GetCollectionsForOrg(
      IVssRequestContext requestContext,
      Guid organizationId)
    {
      List<Collection> collectionsForOrg = (List<Collection>) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        IRemoteServiceClientFactory serviceClientFactory = (IRemoteServiceClientFactory) new RemoteServiceClientFactory();
        try
        {
          Stopwatch stopwatch = Stopwatch.StartNew();
          using (OrganizationHttpClient organizationClient = serviceClientFactory.GetOrganizationClient(requestContext, organizationId))
          {
            if (organizationClient != null)
              collectionsForOrg = organizationClient.GetCollectionsAsync().SyncResult<List<Collection>>();
          }
          GalleryOrganizationHelper.PublishOrgQueryPerfCIEvent(requestContext, organizationId, stopwatch, nameof (GetCollectionsForOrg));
        }
        catch (VssServiceResponseException ex) when (ex.HttpStatusCode == HttpStatusCode.Forbidden || ex.HttpStatusCode == HttpStatusCode.Unauthorized)
        {
          requestContext.TraceException(12061125, "gallery", nameof (GetCollectionsForOrg), (Exception) ex);
        }
        catch (VssServiceResponseException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
        {
          requestContext.TraceException(12061125, "gallery", nameof (GetCollectionsForOrg), (Exception) ex);
        }
      }
      return (IList<Collection>) collectionsForOrg;
    }

    public static bool IsMicrosoftTenantBackedAccount(
      IVssRequestContext requestContext,
      string accountId,
      List<ExtensionShare> sharedWith)
    {
      requestContext.TraceConditionally(12061134, TraceLevel.Info, "gallery", nameof (IsMicrosoftTenantBackedAccount), (Func<string>) (() =>
      {
        List<ExtensionShare> extensionShareList = sharedWith;
        return "SharedWith contains " + (extensionShareList != null ? extensionShareList.Serialize<List<ExtensionShare>>() : (string) null);
      }));
      IRemoteServiceClientFactory serviceClientFactory = (IRemoteServiceClientFactory) new RemoteServiceClientFactory();
      Collection collection = (Collection) null;
      try
      {
        using (OrganizationHttpClient organizationClient = serviceClientFactory.GetOrganizationClient(requestContext, Guid.Parse(accountId)))
        {
          if (organizationClient != null)
            collection = organizationClient.GetCollectionAsync("Me").SyncResult<Collection>();
        }
        if (collection == null)
        {
          requestContext.TraceConditionally(12061134, TraceLevel.Info, "gallery", nameof (IsMicrosoftTenantBackedAccount), (Func<string>) (() => "Collection " + accountId + " returned null from SPS"));
          return false;
        }
        Guid MsTenantId;
        if (!Guid.TryParse(GalleryOrganizationHelper.GetMsTenant(requestContext).FirstOrDefault<string>(), out MsTenantId))
          return false;
        Guid msOrgId = requestContext.GetService<IVssRegistryService>().GetValue<Guid>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/MsOrgId", Guid.Parse("b962ed4f-6670-4a37-ad24-8b9381530e67"));
        requestContext.TraceConditionally(12061134, TraceLevel.Info, "gallery", nameof (IsMicrosoftTenantBackedAccount), (Func<string>) (() => string.Format("Collection {0} has tenant id : {1}, with msOrgId as {2} and microsoft tenantId as {3}", (object) accountId, (object) collection.TenantId, (object) msOrgId, (object) MsTenantId)));
        if (collection.TenantId == Guid.Empty || !(collection.TenantId == MsTenantId))
          return false;
        bool resultValue = sharedWith?.Find((Predicate<ExtensionShare>) (share => share.Type == "organization" && share.Id.Equals(msOrgId.ToString(), StringComparison.OrdinalIgnoreCase))) != null;
        requestContext.TraceConditionally(12061134, TraceLevel.Info, "gallery", nameof (IsMicrosoftTenantBackedAccount), (Func<string>) (() => string.Format("For Collection {0} with tenant id : {1}, with msOrgId as {2} resulted in {3}", (object) accountId, (object) collection.TenantId, (object) msOrgId, (object) resultValue)));
        return requestContext.ExecutionEnvironment.IsDevFabricDeployment || resultValue;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061132, TraceLevel.Error, "gallery", nameof (IsMicrosoftTenantBackedAccount), ex);
      }
      return false;
    }

    private static IEnumerable<string> GetMsTenant(IVssRequestContext requestContext)
    {
      string str = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/MsTenantId", "72F988BF-86F1-41AF-91AB-2D7CD011DB47");
      IEnumerable<string> msTenant = (IEnumerable<string>) null;
      if (!string.IsNullOrEmpty(str))
      {
        if (str.Contains("|"))
          msTenant = (IEnumerable<string>) ((IEnumerable<string>) str.Split('|')).ToList<string>();
        else
          msTenant = (IEnumerable<string>) new List<string>()
          {
            str
          };
      }
      return msTenant;
    }

    public static IList<Guid> GetAccountsForUserInMicrosoftTenant(
      IVssRequestContext requestContext,
      Guid enterpriseId,
      Guid userId)
    {
      IFirstPartyPublisherAccessService service = requestContext.GetService<IFirstPartyPublisherAccessService>();
      if (requestContext.UserContext != (IdentityDescriptor) null && !ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) && service.IsInternalEmployee(requestContext) && !service.IsInternalPartner(requestContext))
      {
        string input = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/MsOrgId", "b962ed4f-6670-4a37-ad24-8b9381530e67");
        TimeSpan timeout = TimeSpan.FromSeconds(30.0);
        Guid guid;
        ref Guid local = ref guid;
        if (Guid.TryParse(input, out local) && enterpriseId == guid || requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        {
          using (new TraceWatch(requestContext, 12061133, TraceLevel.Error, timeout, "Gallery", "GetAccountsForUserInTenant", string.Format("Handler: QueryAccountIds for {0} took more than {1} seconds to complete.", (object) userId, (object) timeout.ToString()), Array.Empty<object>()))
            return UserAccountMappingMigrationHelper.QueryAccountIds(requestContext, userId, UserRole.Member);
        }
      }
      return (IList<Guid>) new List<Guid>();
    }

    public static List<Guid> GetOrganizationIdsForIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity user)
    {
      List<Guid> organizationIdsForIdentity = (List<Guid>) null;
      if ((requestContext.GetUserIdentity() ?? user) != null)
      {
        IGalleryAdminAuthorizer extension = requestContext.GetExtension<IGalleryAdminAuthorizer>();
        Guid result;
        if (extension != null && extension.IsDomainMSTenant(requestContext) && Guid.TryParse(requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/MsOrgId", "b962ed4f-6670-4a37-ad24-8b9381530e67"), out result))
          organizationIdsForIdentity = new List<Guid>()
          {
            result
          };
      }
      return organizationIdsForIdentity;
    }

    private static void PublishOrgQueryPerfCIEvent(
      IVssRequestContext requestContext,
      Guid hostId,
      Stopwatch stopwatch,
      string action)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Host", (object) hostId);
      stopwatch.Stop();
      properties.Add("Duration", (double) stopwatch.ElapsedMilliseconds);
      properties.Add(CustomerIntelligenceProperty.Action, action);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "ExtensionOrganizationSharing", properties);
    }
  }
}
