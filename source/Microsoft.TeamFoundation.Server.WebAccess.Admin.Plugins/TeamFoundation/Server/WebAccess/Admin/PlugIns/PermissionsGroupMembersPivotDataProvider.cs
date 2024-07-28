// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.PermissionsGroupMembersPivotDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DiscoveryService;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class PermissionsGroupMembersPivotDataProvider : IExtensionDataProvider
  {
    private const int c_maxMembersToShow = 500;

    public string Name => "Admin.GroupMembersPivot";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      try
      {
        string subjectDescriptor = (string) null;
        PermissionsGroupMembersPivotDataProvider.GetPermissionsMembersPivotParams(providerContext, out subjectDescriptor);
        if (string.IsNullOrEmpty(subjectDescriptor))
        {
          requestContext.Trace(10050062, TraceLevel.Error, "OrgSettingsGroupsPivot", "DataProvider", "Descriptor is null.");
          return (object) null;
        }
        IdentityDescriptor identityDescriptor = SubjectDescriptor.FromString(subjectDescriptor).ToIdentityDescriptor(requestContext);
        TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
        TeamFoundationIdentity foundationIdentity = service.ReadIdentity(requestContext, identityDescriptor, MembershipQuery.None, ReadIdentityOptions.None);
        IdentityViewData identityViewData;
        if (!AadIdentityHelper.IsAadGroup(foundationIdentity.Descriptor))
        {
          TeamFoundationFilteredIdentitiesList filteredIdentities = service.ReadFilteredIdentitiesById(requestContext, new Guid[1]
          {
            foundationIdentity.TeamFoundationId
          }, IdentityManagementHelpers.GetPageSize(new int?(500)), (IEnumerable<IdentityFilter>) new List<IdentityFilter>(), (string) null, true, MembershipQuery.Direct, MembershipQuery.None, true, (IEnumerable<string>) null);
          identityViewData = TfsAdminIdentityHelper.JsonFromFilteredIdentitiesList(requestContext, filteredIdentities);
        }
        else
        {
          IEnumerable<IDirectoryEntity> directoryEntities = this.ReadMembersFromDDs(requestContext, (DirectoryDiscoveryServiceHelper.GetEntityFromTFIdentifier(requestContext, foundationIdentity.TeamFoundationId.ToString()) ?? throw new Exception(AdminServerResources.UnableToResolveGroup)).EntityId, 500);
          identityViewData = DirectoryDiscoveryServiceHelper.BuildFilteredIdentitiesViewModel(directoryEntities, false, directoryEntities.Count<IDirectoryEntity>(), requestContext.ServiceHost.CollectionServiceHost.InstanceId);
        }
        IEnumerable<GraphViewModel> graphViewModels = (IEnumerable<GraphViewModel>) null;
        int num = 0;
        if (identityViewData != null && identityViewData.Identities != null)
        {
          graphViewModels = identityViewData.Identities;
          num = identityViewData.Identities.Count<GraphViewModel>();
        }
        return (object) new GroupMembersViewData()
        {
          Identities = graphViewModels,
          HasMore = false,
          TotalIdentityCount = num
        };
      }
      catch (Exception ex)
      {
        requestContext.Trace(10050089, TraceLevel.Error, "MembersPivot", "DataProvider", ex.Message);
        return (object) null;
      }
    }

    private static void GetPermissionsMembersPivotParams(
      DataProviderContext providerContext,
      out string subjectDescriptor)
    {
      subjectDescriptor = (string) null;
      if (!providerContext.Properties.ContainsKey(nameof (subjectDescriptor)) || providerContext.Properties[nameof (subjectDescriptor)] == null)
        return;
      subjectDescriptor = providerContext.Properties[nameof (subjectDescriptor)].ToString();
    }

    private IEnumerable<IDirectoryEntity> ReadMembersFromDDs(
      IVssRequestContext requestContext,
      string groupId,
      int maxResults)
    {
      try
      {
        DirectoryDiscoveryService service = requestContext.GetService<DirectoryDiscoveryService>();
        string[] strArray1 = new string[1]{ "src" };
        string[] strArray2 = new string[6]
        {
          "DisplayName",
          "ScopeName",
          "SubjectDescriptor",
          "Mail",
          "SignInAddress",
          "Description"
        };
        DirectoryDiscoveryService discoveryService = service;
        IVssRequestContext context = requestContext;
        DirectoryGetRelatedEntitiesRequest relatedEntitiesRequest = new DirectoryGetRelatedEntitiesRequest();
        relatedEntitiesRequest.Directories = (IEnumerable<string>) strArray1;
        relatedEntitiesRequest.Depth = 1;
        relatedEntitiesRequest.MaxResults = new int?(maxResults);
        relatedEntitiesRequest.MinResults = new int?(maxResults);
        relatedEntitiesRequest.PropertiesToReturn = (IEnumerable<string>) strArray2;
        relatedEntitiesRequest.Relation = "Member";
        relatedEntitiesRequest.EntityIds = (IEnumerable<string>) new string[1]
        {
          groupId
        };
        relatedEntitiesRequest.PagingToken = (string) null;
        DirectoryGetRelatedEntitiesRequest request = relatedEntitiesRequest;
        DirectoryGetRelatedEntitiesResponse relatedEntities = discoveryService.GetRelatedEntities(context, request);
        if (relatedEntities == null || relatedEntities.Results == null || relatedEntities.Results.Count == 0 || !relatedEntities.Results.TryGetValue(groupId, out DirectoryGetRelatedEntitiesResult _))
          throw new Exception(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AdminServerResources.UnableToRetrieveRelatedIdentities, (object) groupId));
        IEnumerable<Exception> exceptions = relatedEntities.Results.Where<KeyValuePair<string, DirectoryGetRelatedEntitiesResult>>((Func<KeyValuePair<string, DirectoryGetRelatedEntitiesResult>, bool>) (kv => kv.Value.Exception != null)).Select<KeyValuePair<string, DirectoryGetRelatedEntitiesResult>, Exception>((Func<KeyValuePair<string, DirectoryGetRelatedEntitiesResult>, Exception>) (kv => kv.Value.Exception));
        if (exceptions.Any<Exception>())
        {
          AggregateException innerException = new AggregateException(exceptions);
          throw new Exception(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AdminServerResources.UnableToRetrieveRelatedIdentities, (object) groupId), (Exception) innerException);
        }
        return relatedEntities.Results.Single<KeyValuePair<string, DirectoryGetRelatedEntitiesResult>>().Value.Entities;
      }
      catch (Exception ex)
      {
        requestContext.Trace(10050088, TraceLevel.Error, "MembersPivot", "DataProvider", ex.Message);
        throw;
      }
    }
  }
}
