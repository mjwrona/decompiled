// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VariableGroupSecurityProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.SecurityRoles;
using Microsoft.VisualStudio.Services.SecurityRoles.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class VariableGroupSecurityProvider : LibrarySecurityProvider
  {
    private static readonly string VariableGroup = nameof (VariableGroup);

    public static string GetToken(string itemId) => VariableGroupSecurityProvider.VariableGroup + (object) DefaultSecurityProvider.NamespaceSeparator + itemId;

    public static void RemoveAllVariableGroupAdminAtCollectionLevel(
      IVssRequestContext requestContext,
      string token)
    {
      if (requestContext.IsFeatureEnabled("WebAccess.DistributedTask.ShareVariableGroups"))
        return;
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, LibrarySecurityProvider.LibraryNamespaceId).RemoveAccessControlLists(requestContext, (IEnumerable<string>) new List<string>()
      {
        token
      }, false);
    }

    public static void PromoteProjectLevelAdminsToCollectionLevelAdminsForVariableGroup(
      IVssRequestContext requestContext,
      ServicingContext servicingContext,
      string projectId,
      int groupId)
    {
      if (requestContext.IsFeatureEnabled("WebAccess.DistributedTask.ShareVariableGroups"))
        return;
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityMap = (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      VariableGroupSecurityProvider.PromoteProjectLevelAdminsToCollectionLevelAdminsForVariableGroupImpl(requestContext, servicingContext, identityMap, projectId, groupId);
    }

    public static void PromoteProjectLevelAdminsToCollectionLevelAdminsForVariableGroupForUpgrade(
      IVssRequestContext requestContext,
      ServicingContext servicingContext,
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      string projectId,
      int groupId)
    {
      VariableGroupSecurityProvider.PromoteProjectLevelAdminsToCollectionLevelAdminsForVariableGroupImpl(requestContext, servicingContext, identityMap, projectId, groupId);
    }

    private static void PromoteProjectLevelAdminsToCollectionLevelAdminsForVariableGroupImpl(
      IVssRequestContext requestContext,
      ServicingContext servicingContext,
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      string projectId,
      int groupId)
    {
      IdentityService service1 = requestContext.GetService<IdentityService>();
      ISecurityRoleMappingService service2 = requestContext.GetService<ISecurityRoleMappingService>();
      VariableGroupSecurityProvider securityProvider = new VariableGroupSecurityProvider();
      string str = projectId + "$" + groupId.ToString();
      IVssRequestContext requestContext1 = requestContext;
      string resourceId = str;
      foreach (RoleAssignment roleAssignment in service2.GetRoleAssignments(requestContext1, resourceId, "distributedtask.variablegroup"))
      {
        if (roleAssignment.Role.Name == "Administrator")
        {
          try
          {
            Guid guid = Guid.Parse(roleAssignment.Identity.Id);
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (identityMap.ContainsKey(guid))
            {
              identity = identityMap[guid];
            }
            else
            {
              identity = service1.GetIdentity(requestContext, guid);
              identityMap.Add(guid, identity);
            }
            if (identity.IsActive && identity.Descriptor != (IdentityDescriptor) null)
              securityProvider.AddVariableGroupAdminAtCollectionLevel(requestContext, identity.Descriptor, groupId.ToString());
            else
              servicingContext?.LogInfo("Identity {0} for variable group id {1} is not active", new object[2]
              {
                (object) identity.Id,
                (object) groupId
              });
            servicingContext?.LogInfo("Successfully set project level administrator of variable group {0} as collection level administrator", new object[1]
            {
              (object) groupId
            });
          }
          catch (Exception ex)
          {
            servicingContext?.Error("Failed to set project level admin of variable group {0} as collection level administrator with error: {1}", new object[2]
            {
              (object) groupId,
              (object) ex
            });
            throw;
          }
        }
      }
    }

    protected override string GetTokenSuffix(string itemId) => VariableGroupSecurityProvider.GetToken(itemId);

    protected override IList<IAccessControlEntry> GetCollectionAdminRoleAce(Microsoft.VisualStudio.Services.Identity.Identity ownerIdentity) => (IList<IAccessControlEntry>) new List<IAccessControlEntry>()
    {
      (IAccessControlEntry) new AccessControlEntry(ownerIdentity.Descriptor, 19, 0)
    };
  }
}
