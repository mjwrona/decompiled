// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.LegacyEleadSecurityUtility
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Security.Principal;

namespace Microsoft.Azure.Boards.CssNodes
{
  public class LegacyEleadSecurityUtility
  {
    public static string[] ListLocalizedActionNames(string objectClassId, string[] actionId)
    {
      TeamFoundationTrace.Verbose(TraceKeywordSets.Authorization, "Starting ListLocalizedActionNames('{0}', actions[])", (object) objectClassId);
      ResourceManager manager = ServerResources.Manager;
      if (objectClassId.Equals(PermissionObjectClasses.IterationNode, StringComparison.OrdinalIgnoreCase))
        objectClassId = PermissionObjectClasses.CssNode;
      else if (objectClassId.Equals(PermissionObjectClasses.Namespace, StringComparison.OrdinalIgnoreCase) || objectClassId.Equals(PermissionObjectClasses.Project, StringComparison.OrdinalIgnoreCase))
        manager = TFCommonResources.Manager;
      else if (objectClassId.Equals(PermissionObjectClasses.EventSubscription, StringComparison.OrdinalIgnoreCase))
        manager = Microsoft.TeamFoundation.Framework.Server.ServerResources.Manager;
      string[] strArray = new string[actionId.Length];
      for (int index = 0; index < actionId.Length; ++index)
      {
        string name = objectClassId + "_" + actionId[index];
        try
        {
          strArray[index] = manager.GetString(name);
        }
        catch
        {
        }
        if (string.IsNullOrEmpty(strArray[index]))
          throw new BadClassIdActionIdPairException(objectClassId, actionId[index]);
      }
      return strArray;
    }

    public static AccessControlEntry[] GetAuthorizationStyleACEsFromACL(
      IAccessControlList accessControlList,
      Dictionary<int, ActionDefinition> m_bitMaskToActions)
    {
      List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
      foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
        accessControlEntryList.AddRange((IEnumerable<AccessControlEntry>) LegacyEleadSecurityUtility.GetAuthorizationStyleACEs(accessControlEntry, m_bitMaskToActions));
      return accessControlEntryList.ToArray();
    }

    public static void RemoveAccessControlEntry(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string securityToken,
      AccessControlEntry ace)
    {
      IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(ace.Sid);
      IAccessControlList acl = securityNamespace.QueryAccessControlList(requestContext, securityToken, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptorFromSid
      }, false);
      int bit = (securityNamespace.Description.Actions.AsEnumerable<ActionDefinition>().Where<ActionDefinition>((Func<ActionDefinition, bool>) (action => action.Name.Equals(ace.ActionId))).FirstOrDefault<ActionDefinition>() ?? throw new GroupSecuritySubsystemServiceException(ServerResources.GSS_SECURITYACTIONDOESNOTEXISTERROR((object) ace.ActionId))).Bit;
      IAccessControlEntry accessControlEntry = acl.QueryAccessControlEntry(descriptorFromSid);
      if (ace.Deny && (accessControlEntry.Deny & bit) != 0 || !ace.Deny && (accessControlEntry.Allow & bit) != 0)
        accessControlEntry = acl.RemovePermissions(descriptorFromSid, bit);
      securityNamespace.SetAccessControlEntry(requestContext, securityToken, accessControlEntry, false);
    }

    public static List<AccessControlEntry> GetAuthorizationStyleACEs(
      IAccessControlEntry namespaceACE,
      Dictionary<int, ActionDefinition> m_bitMaskToActions)
    {
      List<AccessControlEntry> authorizationStyleAcEs = new List<AccessControlEntry>();
      SecurityIdentifier securityIdentifier = new SecurityIdentifier(namespaceACE.Descriptor.Identifier);
      for (int index = 0; index < 32; ++index)
      {
        int key = 1 << index;
        if (m_bitMaskToActions.ContainsKey(key))
        {
          if ((namespaceACE.Allow & key) == key)
            authorizationStyleAcEs.Add(new AccessControlEntry(m_bitMaskToActions[key].Name, securityIdentifier.Value, false));
          else if ((namespaceACE.Deny & key) == key)
            authorizationStyleAcEs.Add(new AccessControlEntry(m_bitMaskToActions[key].Name, securityIdentifier.Value, true));
        }
      }
      return authorizationStyleAcEs;
    }

    public static void UpdateAccessControlEntries(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string securityToken,
      AccessControlEntry[] aces,
      string objectId,
      bool replace)
    {
      IdentityDescriptor[] descriptors = new IdentityDescriptor[aces.Length];
      int num = 0;
      foreach (AccessControlEntry ace in aces)
        descriptors[num++] = IdentityHelper.CreateDescriptorFromSid(ace.Sid);
      LegacyEleadSecurityUtility.ReadIdentities(requestContext, descriptors);
      Dictionary<IdentityDescriptor, IAccessControlEntry> dictionary = new Dictionary<IdentityDescriptor, IAccessControlEntry>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      IAccessControlList acl = replace ? (IAccessControlList) new AccessControlList(securityToken, true, (IEnumerable<Microsoft.TeamFoundation.Framework.Server.AccessControlEntry>) null) : securityNamespace.QueryAccessControlList(requestContext, securityToken, (IEnumerable<IdentityDescriptor>) null, false);
      for (int index = 0; index < aces.Length; ++index)
      {
        IdentityDescriptor identityDescriptor = descriptors[index];
        AccessControlEntry ace = aces[index];
        if (!(securityNamespace.Description.NamespaceId == AuthorizationSecurityConstants.IterationNodeSecurityGuid) || !StringComparer.OrdinalIgnoreCase.Equals(ace.ActionId, "WORK_ITEM_READ") && !StringComparer.OrdinalIgnoreCase.Equals(ace.ActionId, "WORK_ITEM_WRITE"))
        {
          LegacyEleadSecurityUtility.SetTaggingPermissions(requestContext, securityNamespace, ace, objectId);
          int bit = (securityNamespace.Description.Actions.AsEnumerable<ActionDefinition>().Where<ActionDefinition>((Func<ActionDefinition, bool>) (action => action.Name.Equals(ace.ActionId))).FirstOrDefault<ActionDefinition>() ?? throw new GroupSecuritySubsystemServiceException(ServerResources.GSS_SECURITYACTIONDOESNOTEXISTERROR((object) ace.ActionId))).Bit;
          int allow = ace.Deny ? 0 : bit;
          int deny = ace.Deny ? bit : 0;
          dictionary[identityDescriptor] = acl.SetPermissions(identityDescriptor, allow, deny, true);
        }
      }
      if (replace)
        securityNamespace.SetAccessControlLists(requestContext, (IEnumerable<IAccessControlList>) new IAccessControlList[1]
        {
          acl
        });
      else
        securityNamespace.SetAccessControlEntries(requestContext, securityToken, (IEnumerable<IAccessControlEntry>) dictionary.Values, false);
    }

    private static void SetTaggingPermissions(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      AccessControlEntry ace,
      string objectId)
    {
      try
      {
        if (securityNamespace.Description.NamespaceId != AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid)
          return;
        if (!StringComparer.OrdinalIgnoreCase.Equals(ace.ActionId, "WORK_ITEM_WRITE"))
          return;
        int create = TaggingPermissions.Create;
        IVssSecurityNamespace securityNamespace1 = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TaggingNamespaceId);
        if (securityNamespace1 == null)
          return;
        int allow = ace.Deny ? 0 : create;
        int deny = ace.Deny ? create : 0;
        ICommonStructureServiceProvider extension = requestContext.GetExtension<ICommonStructureServiceProvider>();
        Guid id;
        CommonStructureUtils.TryParseNodeUri(objectId, out id);
        IVssRequestContext requestContext1 = requestContext;
        Guid nodeId = id;
        Guid projectId = ProjectInfo.GetProjectId(extension.GetNode(requestContext1, nodeId).ProjectUri);
        ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
        if (project.State != ProjectState.CreatePending && project.State != ProjectState.New)
          return;
        string token = "/" + projectId.ToString();
        securityNamespace1.SetPermissions(requestContext, token, IdentityHelper.CreateDescriptorFromSid(ace.Sid), allow, deny, false);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDescriptor[] descriptors)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] array = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) descriptors, QueryMembership.None, (IEnumerable<string>) null).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
      for (int index = 0; index < array.Length; ++index)
      {
        if (array[index] == null)
          throw new IdentityNotFoundException(descriptors[index]);
        LegacyEleadSecurityUtility.Validate(requestContext, array[index]);
      }
      return array;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        throw new IdentityNotFoundException(descriptor);
      LegacyEleadSecurityUtility.Validate(requestContext, identity);
      return identity;
    }

    private static void Validate(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (!identity.IsActive)
        throw new ActiveDirectoryAccessException(ServerResources.GSS_ACTIVE_DIRECTORY_NO_IDENTITY_EXCEPTION());
      if (string.IsNullOrEmpty(identity.GetProperty<string>("SecurityGroup", (string) null)) && identity.IsContainer && identity.Descriptor.IdentityType == "System.Security.Principal.WindowsIdentity")
        throw new NotASecurityGroupException(identity.DisplayName);
    }
  }
}
