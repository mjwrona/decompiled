// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.FrameworkIdentityService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Utility;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkIdentityService : 
    IdentityServiceBase,
    IInstallableIdentityService,
    IDisposable
  {
    private static readonly Regex m_globalScopeRegex = new Regex("^\\[SERVER\\]\\\\", RegexOptions.Compiled);
    private const string s_groupScopeFormat = "[{0}]\\";
    private const string s_Area = "IdentityService";
    private const string s_Layer = "FrameworkIdentityService";
    private VirtualizedGroupStore m_virtualizedGroupStore;
    private static readonly IdentityDescriptor AnonymousUsersGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(GroupWellKnownSecurityIds.AnonymousUsersGroup);

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      FrameworkIdentityStore parentIdentityStore = (FrameworkIdentityStore) null;
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        parentIdentityStore = systemRequestContext.To(TeamFoundationHostType.Parent).GetService<FrameworkIdentityService>().IdentityStore;
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        this.m_virtualizedGroupStore = new VirtualizedGroupStore(systemRequestContext, this.Domain.DomainId, this.IdentityMapper);
      if (this.Domain.IsMaster)
      {
        this.IdentityStore = new FrameworkIdentityStore(systemRequestContext, this.Domain, this.SyncAgents, parentIdentityStore);
        this.IdentityStore.DescriptorsChanged += new EventHandler<DescriptorChangeEventArgs>(((IdentityServiceBase) this).FireDescriptorsChanged);
        this.IdentityStore.DescriptorsChangedNotification += new EventHandler<DescriptorChangeNotificationEventArgs>(((IdentityServiceBase) this).FireDescriptorsChangedNotification);
      }
      else
      {
        this.IdentityStore = parentIdentityStore;
        this.IdentityStore.AddDomain(systemRequestContext, this.Domain);
      }
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      base.ServiceEnd(systemRequestContext);
      if (this.IdentityStore == null || !this.Domain.IsMaster)
        return;
      this.IdentityStore.DescriptorsChanged -= new EventHandler<DescriptorChangeEventArgs>(((IdentityServiceBase) this).FireDescriptorsChanged);
      this.IdentityStore.DescriptorsChangedNotification -= new EventHandler<DescriptorChangeNotificationEventArgs>(((IdentityServiceBase) this).FireDescriptorsChangedNotification);
      this.IdentityStore.Unload(systemRequestContext);
    }

    void IInstallableIdentityService.Install(IVssRequestContext requestContext)
    {
    }

    void IInstallableIdentityService.Uninstall(
      IVssRequestContext requestContext,
      IdentityDomain domain)
    {
      if (domain == null)
        domain = this.Domain;
      this.DeleteScope(requestContext, domain.DomainId);
    }

    public override IdentityScope CreateScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid parentScopeId,
      GroupScopeType scopeType,
      string scopeName,
      string adminGroupName,
      string adminGroupDescription,
      Guid creatorId)
    {
      return this.CreateScope(requestContext, scopeId, parentScopeId, scopeType, scopeName, adminGroupName, adminGroupDescription, creatorId, false);
    }

    private IdentityScope CreateScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid parentScopeId,
      GroupScopeType scopeType,
      string scopeName,
      string adminGroupName,
      string adminGroupDescription,
      Guid creatorId,
      bool idempotent)
    {
      TFCommonUtil.CheckGroupName(ref scopeName);
      TFCommonUtil.CheckGroupName(ref adminGroupName);
      TFCommonUtil.CheckGroupDescription(ref adminGroupDescription);
      ArgumentUtility.CheckForNull<IdentityDescriptor>(requestContext.UserContext, "requestContext.UserContext");
      if (this.IsVirtualizedDeploymentScope(requestContext))
        throw new NotImplementedException();
      try
      {
        requestContext.Trace(10002015, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityService), "Creating {0} scope {1} in parent scope {2} with name {3} and admin group name {4}", (object) scopeType, (object) scopeId, (object) parentScopeId, (object) scopeName, (object) adminGroupName);
        return this.IdentityStore.CreateScope(requestContext, this.Domain, scopeId, parentScopeId, scopeType, scopeName, adminGroupName, adminGroupDescription, creatorId);
      }
      catch (GroupScopeCreationException ex)
      {
        if (!idempotent)
        {
          throw;
        }
        else
        {
          requestContext.TraceException(10002015, "IdentityService", nameof (FrameworkIdentityService), (Exception) ex);
          return this.IdentityStore.GetScope(requestContext, this.Domain, scopeId);
        }
      }
    }

    public override IdentityScope GetScope(IVssRequestContext requestContext, Guid scopeId) => this.IsVirtualizedDeploymentScope(requestContext) ? this.m_virtualizedGroupStore.GetScope(requestContext, scopeId) : this.IdentityStore.GetScope(requestContext, this.Domain, scopeId);

    public override IdentityScope GetScope(IVssRequestContext requestContext, string scopeName)
    {
      ArgumentUtility.CheckForNull<string>(scopeName, nameof (scopeName));
      return this.IsVirtualizedDeploymentScope(requestContext) ? this.m_virtualizedGroupStore.GetScope(requestContext, scopeName) : this.IdentityStore.GetScope(requestContext, this.Domain, scopeName);
    }

    public override void DeleteScope(IVssRequestContext requestContext, Guid scopeId)
    {
      requestContext.Trace(10002025, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityService), "Deleting scopeId:{0}", (object) scopeId);
      if (this.IsVirtualizedDeploymentScope(requestContext))
        throw new NotImplementedException();
      this.IdentityStore.DeleteScope(requestContext, this.Domain, scopeId);
    }

    public override void RenameScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      string newName)
    {
      TFCommonUtil.CheckGroupName(ref newName);
      if (this.IsVirtualizedDeploymentScope(requestContext))
        throw new NotImplementedException();
      this.IdentityStore.RenameScope(requestContext, this.Domain, scopeId, newName);
    }

    public override void RestoreScope(IVssRequestContext requestContext, Guid scopeId)
    {
      requestContext.TraceConditionally(10002016, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityService), (Func<string>) (() => string.Format("restoring scopeId: {0} - framework", (object) scopeId)));
      if (this.IsVirtualizedDeploymentScope(requestContext))
        throw new NotImplementedException();
      this.IdentityStore.RestoreScope(requestContext, scopeId);
    }

    protected override Guid GetScopeParentId(IVssRequestContext requestContext, Guid scopeId) => this.IsVirtualizedDeploymentScope(requestContext) ? this.m_virtualizedGroupStore.GetScopeParentId(requestContext, scopeId) : this.IdentityStore.GetScopeParentId(requestContext, this.Domain, scopeId);

    protected override Microsoft.VisualStudio.Services.Identity.Identity CreateUser(
      IVssRequestContext requestContext,
      Guid scopeId,
      string userSid,
      string domainName,
      string accountName,
      string description)
    {
      if (scopeId == Guid.Empty)
        scopeId = this.Domain.DomainId;
      TFCommonUtil.CheckGroupName(ref accountName);
      TFCommonUtil.CheckGroupDescription(ref description);
      IdentityDescriptor descriptor = !string.IsNullOrEmpty(userSid) ? IdentityHelper.CreateUnauthenticatedDescriptor(userSid) : IdentityHelper.CreateUnauthenticatedDescriptor(SidIdentityHelper.NewSid(scopeId));
      return this.IdentityStore.CreateUnauthenticatedIdentity(requestContext, this.Domain, scopeId, descriptor, domainName, accountName, description);
    }

    public override Microsoft.VisualStudio.Services.Identity.Identity CreateFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      string mailAddress = null)
    {
      return this.IdentityStore.GetHttpClient(requestContext).CreateFrameworkIdentityAsync(identityType, role, identifier, displayName, (object) null, new CancellationToken()).SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public override Microsoft.VisualStudio.Services.Identity.Identity CreateGroup(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid groupId,
      string groupSid,
      string groupName,
      string groupDescription,
      SpecialGroupType specialType = SpecialGroupType.Generic,
      bool scopeLocal = true,
      bool hasRestrictedVisibility = false)
    {
      if (scopeId == Guid.Empty)
        scopeId = this.Domain.DomainId;
      TFCommonUtil.CheckGroupName(ref groupName);
      TFCommonUtil.CheckGroupDescription(ref groupDescription);
      ArgumentUtility.CheckForEmptyGuid(groupId, nameof (groupId));
      if (this.IsVirtualizedDeploymentScope(requestContext))
        return this.m_virtualizedGroupStore.CreateGroup(requestContext, scopeId, groupId, groupSid, groupName, groupDescription, specialType, scopeLocal, hasRestrictedVisibility);
      GroupDescription groupDescription1 = new GroupDescription(!string.IsNullOrEmpty(groupSid) ? IdentityHelper.CreateTeamFoundationDescriptor(groupSid) : IdentityHelper.CreateTeamFoundationDescriptor(SidIdentityHelper.NewSid(scopeId)), groupName, groupDescription, specialType, hasRestrictedVisibility, scopeLocal, scopeId, groupId);
      return this.IdentityStore.CreateGroups(requestContext, this.Domain, scopeId, groupDescription1)[0];
    }

    public override void DeleteGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      TFCommonUtil.CheckSid(groupDescriptor.Identifier, "groupSid");
      if (this.IsVirtualizedDeploymentScope(requestContext))
        throw new NotImplementedException();
      groupDescriptor = this.Domain.MapFromWellKnownIdentifier(groupDescriptor);
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = this.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        groupDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null, false)[0];
      if (readIdentity == null)
        throw new FindGroupSidDoesNotExistException(groupDescriptor.Identifier);
      if (readIdentity.Descriptor.Identifier.StartsWith(this.Domain.DomainSid, StringComparison.OrdinalIgnoreCase) && readIdentity.Descriptor.Identifier.Substring(this.Domain.DomainSid.Length).StartsWith(SidIdentityHelper.WellKnownSidType, StringComparison.OrdinalIgnoreCase))
      {
        SpecialGroupType specialGroupType = IdentityHelper.GetSpecialGroupType((IReadOnlyVssIdentity) readIdentity);
        switch (specialGroupType)
        {
          case SpecialGroupType.LicenseesApplicationGroup:
          case SpecialGroupType.AzureActiveDirectoryApplicationGroup:
            break;
          default:
            IdentityDescriptor foundationDescriptor = IdentityHelper.CreateTeamFoundationDescriptor(WebAccessConstants.WorkItemOnlyViewUsersGroup);
            IdentityDescriptor wellKnownIdentifier = this.Domain.MapToWellKnownIdentifier(readIdentity.Descriptor);
            if (!IdentityDescriptorComparer.Instance.Equals(wellKnownIdentifier, foundationDescriptor) && !IdentityDescriptorComparer.Instance.Equals(wellKnownIdentifier, FrameworkIdentityService.AnonymousUsersGroup))
              throw new RemoveSpecialGroupException(readIdentity.Descriptor.Identifier, specialGroupType);
            break;
        }
      }
      this.IdentityStore.DeleteGroup(requestContext, this.Domain, groupDescriptor);
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      return this.IsVirtualizedDeploymentScope(requestContext) ? this.m_virtualizedGroupStore.ListGroups(requestContext, scopeIds, recurse, propertyNameFilters) : this.ListGroups(requestContext, scopeIds, recurse, propertyNameFilters, false);
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters,
      bool listOnlyDeleted)
    {
      if (scopeIds == null)
        scopeIds = new Guid[1]{ this.Domain.DomainId };
      return this.IdentityStore.ListGroups(requestContext, this.Domain, scopeIds, recurse, listOnlyDeleted, propertyNameFilters);
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ListDeletedGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      return this.IsVirtualizedDeploymentScope(requestContext) ? this.m_virtualizedGroupStore.ListDeletedGroups(requestContext, scopeIds, recurse, propertyNameFilters) : this.ListGroups(requestContext, scopeIds, recurse, propertyNameFilters, true);
    }

    public override bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity member)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityValidation.CheckTeamFoundationType(groupDescriptor);
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(member, nameof (member));
      if (this.IsVirtualizedDeploymentScope(requestContext))
        return this.m_virtualizedGroupStore.AddMemberToGroup(requestContext, groupDescriptor, member);
      if (!IdentityValidation.IsTeamFoundationType(member.Descriptor) && member.Id == Guid.Empty)
        this.UpdateIdentities(requestContext.Elevate(), (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          member
        });
      groupDescriptor = this.Domain.MapFromWellKnownIdentifier(groupDescriptor);
      IdentityDescriptor memberDescriptor = this.Domain.MapFromWellKnownIdentifier(member.Descriptor);
      return this.IdentityStore.AddMemberToGroup(requestContext, this.Domain, groupDescriptor, memberDescriptor);
    }

    public override bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityValidation.CheckTeamFoundationType(groupDescriptor);
      IdentityValidation.CheckDescriptor(memberDescriptor, nameof (memberDescriptor));
      if (this.IsVirtualizedDeploymentScope(requestContext))
        return this.m_virtualizedGroupStore.AddMemberToGroup(requestContext, groupDescriptor, memberDescriptor);
      groupDescriptor = this.Domain.MapFromWellKnownIdentifier(groupDescriptor);
      memberDescriptor = this.Domain.MapFromWellKnownIdentifier(memberDescriptor);
      return this.IdentityStore.AddMemberToGroup(requestContext, this.Domain, groupDescriptor, memberDescriptor);
    }

    public override bool RemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityValidation.CheckDescriptor(memberDescriptor, nameof (memberDescriptor));
      if (this.IsVirtualizedDeploymentScope(requestContext))
        throw new NotImplementedException();
      groupDescriptor = this.Domain.MapFromWellKnownIdentifier(groupDescriptor);
      memberDescriptor = this.Domain.MapFromWellKnownIdentifier(memberDescriptor);
      return this.IdentityStore.RemoveMemberFromGroup(requestContext, this.Domain, groupDescriptor, memberDescriptor);
    }

    public override bool ForceRemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityValidation.CheckDescriptor(memberDescriptor, nameof (memberDescriptor));
      if (this.IsVirtualizedDeploymentScope(requestContext))
        throw new NotImplementedException();
      groupDescriptor = this.Domain.MapFromWellKnownIdentifier(groupDescriptor);
      memberDescriptor = this.Domain.MapFromWellKnownIdentifier(memberDescriptor);
      return this.IdentityStore.RemoveMemberFromGroup(requestContext, this.Domain, groupDescriptor, memberDescriptor, true);
    }

    public override bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      IdentityValidation.CheckDescriptor(memberDescriptor, nameof (memberDescriptor));
      if (this.IsVirtualizedDeploymentScope(requestContext) && memberDescriptor.IsTeamFoundationType())
        return this.m_virtualizedGroupStore.IsMember(requestContext, groupDescriptor, memberDescriptor);
      groupDescriptor = this.Domain.MapFromWellKnownIdentifier(groupDescriptor);
      memberDescriptor = this.Domain.MapFromWellKnownIdentifier(memberDescriptor);
      return this.IdentityStore.IsMember(requestContext.Elevate(), this.Domain, groupDescriptor, memberDescriptor);
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      GraphSecurityHelper.CheckPermissionToReadIdentity(requestContext, 1);
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      return socialDescriptors == null ? (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>() : this.IdentityStore.ReadIdentities(requestContext, this.Domain, socialDescriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      return this.ReadIdentitiesBySubjectDescriptor(requestContext, subjectDescriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesBySubjectDescriptor(
      IVssRequestContext requestContext,
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      GraphSecurityHelper.CheckPermissionToReadIdentity(requestContext, 1);
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      if (subjectDescriptors == null)
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList1 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      if (this.IsVirtualizedDeploymentScope(requestContext))
      {
        IdentityDescriptor[] descriptors = new IdentityDescriptor[subjectDescriptors.Count];
        for (int index = 0; index < subjectDescriptors.Count; ++index)
        {
          SubjectDescriptor subjectDescriptor = subjectDescriptors[index];
          if (subjectDescriptor != new SubjectDescriptor() && subjectDescriptor.IsVstsGroupType())
          {
            IdentityDescriptor identityDescriptor = subjectDescriptor.ToIdentityDescriptor(requestContext);
            IdentityValidation.CheckDescriptor(identityDescriptor, "descriptors element");
            descriptors[index] = identityDescriptor;
          }
        }
        identityList1 = this.m_virtualizedGroupStore.ReadIdentities(requestContext, (IList<IdentityDescriptor>) descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
      }
      SubjectDescriptor[] subjectDescriptors1 = new SubjectDescriptor[subjectDescriptors.Count];
      for (int index = 0; index < subjectDescriptors.Count; ++index)
      {
        if (identityList1 == null || identityList1[index] == null)
          subjectDescriptors1[index] = this.Domain.MapFromWellKnownIdentifier(requestContext, subjectDescriptors[index]);
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList2 = this.IdentityStore.ReadIdentities(requestContext, this.Domain, (IList<SubjectDescriptor>) subjectDescriptors1, queryMembership, propertyNameFilters, includeRestrictedVisibility);
      if (identityList1 != null)
      {
        for (int index = 0; index < identityList2.Count; ++index)
        {
          if (identityList1[index] != null)
            identityList2[index] = identityList1[index];
        }
      }
      if (requestContext.IsTracing(348908, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityService)))
      {
        try
        {
          List<KeyValuePair<SubjectDescriptor, SubjectDescriptor>> keyValuePairList = new List<KeyValuePair<SubjectDescriptor, SubjectDescriptor>>(identityList2.Count);
          for (int index = 0; index < identityList2.Count; ++index)
          {
            if (identityList2[index] != null && subjectDescriptors1[index] != new SubjectDescriptor() && SubjectDescriptorComparer.Instance.Compare(subjectDescriptors1[index], identityList2[index].SubjectDescriptor) != 0)
              keyValuePairList.Add(new KeyValuePair<SubjectDescriptor, SubjectDescriptor>(subjectDescriptors[index], identityList2[index].SubjectDescriptor));
          }
          if (keyValuePairList.Count > 0)
            requestContext.TraceAlways(202027, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityService), "Request for reading identities by descriptor ended up hitting translated identities. {0}. {1}", (object) keyValuePairList.Serialize<List<KeyValuePair<SubjectDescriptor, SubjectDescriptor>>>(), (object) EnvironmentWrapper.ToReadableStackTrace());
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(401711, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityService), "Exception occur when try to checking translated identity. {0}", (object) ex);
        }
      }
      return identityList2;
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      if (descriptors == null)
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      if (!requestContext.RootContext.Items.GetCastedValueOrDefault<string, bool>(RequestContextItemsKeys.IsFrameworkIdentityReadPermissionCheckComplete))
        GraphSecurityHelper.CheckPermissionToReadIdentity(requestContext, 2);
      IdentityDescriptor[] descriptors1 = new IdentityDescriptor[descriptors.Count];
      for (int index = 0; index < descriptors.Count; ++index)
      {
        IdentityValidation.CheckDescriptor(descriptors[index], "descriptors element");
        descriptors1[index] = this.Domain.MapFromWellKnownIdentifier(descriptors[index]);
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList1 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      if (this.IsVirtualizedDeploymentScope(requestContext))
      {
        identityList1 = this.m_virtualizedGroupStore.ReadIdentities(requestContext, descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
        for (int index = 0; index < descriptors1.Length; ++index)
        {
          if (descriptors1[index].IsTeamFoundationType())
            descriptors1[index] = (IdentityDescriptor) null;
        }
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList2 = this.IdentityStore.ReadIdentities(requestContext, this.Domain, (IList<IdentityDescriptor>) descriptors1, queryMembership, propertyNameFilters, includeRestrictedVisibility);
      if (identityList1 != null)
      {
        for (int index = 0; index < identityList2.Count; ++index)
        {
          if (identityList1[index] != null)
            identityList2[index] = identityList1[index];
        }
      }
      if (requestContext.IsTracing(1000201, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityService)))
      {
        try
        {
          List<KeyValuePair<IdentityDescriptor, IdentityDescriptor>> keyValuePairList = new List<KeyValuePair<IdentityDescriptor, IdentityDescriptor>>(identityList2.Count);
          for (int index = 0; index < identityList2.Count; ++index)
          {
            if (identityList2[index] != null && descriptors[index].CompareTo(identityList2[index].Descriptor) != 0)
              keyValuePairList.Add(new KeyValuePair<IdentityDescriptor, IdentityDescriptor>(descriptors[index], identityList2[index].Descriptor));
          }
          if (keyValuePairList.Count > 0)
            requestContext.TraceAlways(1000201, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityService), "Request for reading identities by descriptor ended up hitting translated identities. {0}. {1}", (object) keyValuePairList.Serialize<List<KeyValuePair<IdentityDescriptor, IdentityDescriptor>>>(), (object) EnvironmentWrapper.ToReadableStackTrace());
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(1000201, TraceLevel.Info, "IdentityService", nameof (FrameworkIdentityService), "Exception occur when try to checking translated identity. {0}", (object) ex);
        }
      }
      return identityList2;
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      if (identityIds == null)
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      GraphSecurityHelper.CheckPermissionToReadIdentity(requestContext, 1);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList1 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      IList<Guid> identityIds1 = identityIds;
      if (this.IsVirtualizedDeploymentScope(requestContext))
      {
        identityList1 = this.m_virtualizedGroupStore.ReadIdentities(requestContext, identityIds1, queryMembership, propertyNameFilters, includeRestrictedVisibility);
        if (identityList1 != null)
        {
          identityIds1 = (IList<Guid>) new List<Guid>((IEnumerable<Guid>) identityIds);
          for (int index = 0; index < identityList1.Count; ++index)
          {
            if (identityList1[index] != null)
              identityIds1[index] = Guid.Empty;
          }
        }
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList2 = this.IdentityStore.ReadIdentities(requestContext, this.Domain, identityIds1, queryMembership, propertyNameFilters, includeRestrictedVisibility);
      if (identityList1 != null)
      {
        for (int index = 0; index < identityList2.Count; ++index)
        {
          if (identityList1[index] != null)
            identityList2[index] = identityList1[index];
        }
      }
      return identityList2;
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      return this.ReadIdentities(requestContext, searchFactor, factorValue, queryMembership, propertyNameFilters, ReadIdentitiesOptions.None);
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      ReadIdentitiesOptions options)
    {
      if (searchFactor == IdentitySearchFilter.TeamGroupName)
        searchFactor = IdentitySearchFilter.LocalGroupName;
      IdentityValidation.CheckFactorAndValue(searchFactor, ref factorValue);
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      int permission = searchFactor == IdentitySearchFilter.LocalGroupName ? 1 : 2;
      GraphSecurityHelper.CheckPermissionToReadIdentity(requestContext, permission);
      if (searchFactor == IdentitySearchFilter.Identifier)
        factorValue = this.Domain.MapFromWellKnownIdentifier(factorValue);
      else if (searchFactor == IdentitySearchFilter.DisplayName || searchFactor == IdentitySearchFilter.General)
      {
        IdentityDisplayName disambiguatedSearchTerm = IdentityDisplayName.GetDisambiguatedSearchTerm(factorValue);
        if (disambiguatedSearchTerm.Type != SearchTermType.Unknown)
        {
          if (disambiguatedSearchTerm.Type == SearchTermType.Vsid)
            return this.IdentityStore.ReadIdentities(requestContext, this.Domain, (IList<Guid>) new Guid[1]
            {
              disambiguatedSearchTerm.Vsid
            }, queryMembership, propertyNameFilters, false);
          if (disambiguatedSearchTerm.Type == SearchTermType.DomainAndAccountName)
          {
            searchFactor = IdentitySearchFilter.AccountName;
            factorValue = disambiguatedSearchTerm.Domain + "\\" + disambiguatedSearchTerm.AccountName;
          }
          else if (disambiguatedSearchTerm.Type == SearchTermType.AccoutName)
          {
            searchFactor = IdentitySearchFilter.AccountName;
            factorValue = disambiguatedSearchTerm.AccountName;
          }
        }
      }
      if (searchFactor == IdentitySearchFilter.AccountName)
        factorValue = FrameworkIdentityService.NormalizeTfsGroupName(this.Domain.DomainId, factorValue);
      return this.IdentityStore.ReadIdentities(requestContext, this.Domain, searchFactor, factorValue, queryMembership, propertyNameFilters, options);
    }

    public override IdentitySearchResult SearchIdentities(
      IVssRequestContext requestContext,
      IdentitySearchParameters searchParameters)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IdentitySearchParameters>(searchParameters, nameof (searchParameters));
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new InvalidRequestContextHostException(FrameworkResources.ApplicationHostRequired());
      return this.IdentityStore.SearchIdentities(requestContext, this.Domain, searchParameters);
    }

    public override void RefreshSearchIdentitiesCache(
      IVssRequestContext requestContext,
      Guid scopeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.IdentityStore.RefreshSearchIdentitiesCache(requestContext, this.Domain, scopeId);
    }

    public override FilteredIdentitiesList ReadFilteredIdentities(
      IVssRequestContext requestContext,
      Guid scopeId,
      IList<IdentityDescriptor> descriptors,
      IEnumerable<IdentityFilter> filters,
      int suggestedPageSize,
      string lastSearchResult,
      bool lookForward,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      return this.IdentityStore.ReadFilteredIdentities(requestContext, this.Domain, scopeId, descriptors, filters, suggestedPageSize, lastSearchResult, lookForward, queryMembership, propertyNameFilters);
    }

    protected override ChangedIdentities GetIdentityChanges(
      IVssRequestContext requestContext,
      ChangedIdentitiesContext sequenceContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ChangedIdentitiesContext>(sequenceContext, nameof (sequenceContext));
      return this.IdentityStore.GetIdentityChanges(requestContext, this.Domain, sequenceContext);
    }

    public override bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetadataUpdates)
    {
      if (allowMetadataUpdates)
        throw new InvalidOperationException("Metadata updates are not permitted from the framework.");
      return this.UpdateIdentities(requestContext, identities);
    }

    public override bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      ArgumentUtility.CheckForNull<IList<Microsoft.VisualStudio.Services.Identity.Identity>>(identities, nameof (identities));
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, "identity");
        string property = identity.GetProperty<string>("Alias", (string) null);
        if (!string.IsNullOrEmpty(property))
          IdentityValidation.CheckAlias(ref property);
      }
      if (identities.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => !identity.IsContainer)))
        requestContext.TraceSerializedConditionally(10002013, TraceLevel.Warning, "IdentityService", nameof (FrameworkIdentityService), "Update identities called in framework for identities: {0}", (object) identities);
      return this.IdentityStore.UpdateIdentities(requestContext, this.Domain, identities);
    }

    protected override IdentityChanges GetIdentityChanges(
      IVssRequestContext requestContext,
      int sequenceId,
      long identitySequenceId,
      long groupSequenceId,
      long organizationIdentitySequenceId)
    {
      throw new NotSupportedException();
    }

    protected override int GetCurrentSequenceId(IVssRequestContext requestContext) => throw new NotSupportedException();

    public override string GetSignoutToken(IVssRequestContext requestContext)
    {
      try
      {
        AccessTokenResult signoutToken = this.IdentityStore.GetSignoutToken(requestContext);
        if (!signoutToken.HasError)
          return signoutToken.AccessToken.EncodedToken;
        requestContext.Trace(10002010, TraceLevel.Error, "IdentityService", nameof (FrameworkIdentityService), string.Format("Error getting signout token - {0}", (object) signoutToken.AccessTokenError));
        return (string) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10002011, "IdentityService", nameof (FrameworkIdentityService), ex);
        return (string) null;
      }
    }

    protected override bool RefreshIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      return true;
    }

    protected override int GetCurrentChangeId() => this.IdentityStore.GetCurrentChangeId();

    internal static string NormalizeTfsGroupName(Guid scopeId, string factorValue) => FrameworkIdentityService.m_globalScopeRegex.Replace(factorValue, string.Format("[{0}]\\", (object) scopeId.ToString()));

    public override void ClearIdentityCache(IVssRequestContext requestContext) => this.IdentityStore.ClearCache(requestContext);

    public override void InvalidateIdentities(
      IVssRequestContext requestContext,
      ICollection<Guid> identityIds)
    {
      this.IdentityStore.InvalidateIdentities(requestContext, this.Domain, identityIds);
    }

    public override void SweepIdentityCache(IVssRequestContext requestContext) => this.IdentityStore.SweepCache(requestContext);

    private bool IsVirtualizedDeploymentScope(IVssRequestContext requestContext) => requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.VirtualizedDeploymentGroupStore");

    internal FrameworkIdentityStore IdentityStore { get; set; }
  }
}
