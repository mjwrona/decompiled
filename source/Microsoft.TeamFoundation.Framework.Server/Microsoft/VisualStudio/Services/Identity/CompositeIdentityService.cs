// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.CompositeIdentityService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class CompositeIdentityService : 
    IdentityService,
    IUserIdentityService,
    ISystemIdentityService,
    IVssFrameworkService,
    IIdentityServiceInternalRestricted,
    IIdentityServiceInternal,
    IIdentityServiceInternalRequired,
    IInstallableIdentityService
  {
    private EventHandler<DescriptorChangeEventArgs> m_descriptorChanged;
    private EventHandler<DescriptorChangeNotificationEventArgs> m_descriptorChangedNotification;
    private IdentityService m_userIdentityService;
    private ISystemIdentityService m_systemIdentityService;
    private ServicePrincipalIsMemberReporter m_isMemberReporter;
    private Dictionary<string, IIdentityProvider> m_compositeSyncAgents;
    private const string c_area = "IdentityService";
    private const string c_layer = "CompositeIdentityService";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(10004001, "IdentityService", nameof (CompositeIdentityService), "ServiceStart");
      try
      {
        this.m_userIdentityService = (IdentityService) systemRequestContext.GetService<IUserIdentityService>();
        IVssRequestContext context = systemRequestContext.To(TeamFoundationHostType.Deployment);
        this.m_systemIdentityService = context.GetService<ISystemIdentityService>();
        this.m_isMemberReporter = context.GetService<ServicePrincipalIsMemberReporter>();
        this.m_compositeSyncAgents = new Dictionary<string, IIdentityProvider>(this.m_userIdentityService.SyncAgents);
        foreach (KeyValuePair<string, IIdentityProvider> syncAgent in (IEnumerable<KeyValuePair<string, IIdentityProvider>>) this.m_systemIdentityService.SyncAgents)
          this.m_compositeSyncAgents[syncAgent.Key] = syncAgent.Value;
        if (!(this.m_userIdentityService is IIdentityServiceInternal))
          return;
        ((IIdentityServiceInternal) this.m_userIdentityService).DescriptorsChanged += new EventHandler<DescriptorChangeEventArgs>(this.CompositeIdentityService_DescriptorsChanged);
        ((IIdentityServiceInternal) this.m_userIdentityService).DescriptorsChangedNotification += new EventHandler<DescriptorChangeNotificationEventArgs>(this.CompositeIdentityService_DescriptorsChangedNotification);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(10004002, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(10004003, "IdentityService", nameof (CompositeIdentityService), "ServiceStart");
      }
    }

    private void CompositeIdentityService_DescriptorsChangedNotification(
      object sender,
      DescriptorChangeNotificationEventArgs e)
    {
      EventHandler<DescriptorChangeNotificationEventArgs> changedNotification = this.m_descriptorChangedNotification;
      if (changedNotification == null)
        return;
      changedNotification(sender, e);
    }

    private void CompositeIdentityService_DescriptorsChanged(
      object sender,
      DescriptorChangeEventArgs e)
    {
      EventHandler<DescriptorChangeEventArgs> descriptorChanged = this.m_descriptorChanged;
      if (descriptorChanged == null)
        return;
      descriptorChanged(sender, e);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(10004004, "IdentityService", nameof (CompositeIdentityService), "ServiceEnd");
      try
      {
        if (this.m_userIdentityService is IIdentityServiceInternal)
        {
          ((IIdentityServiceInternal) this.m_userIdentityService).DescriptorsChanged -= new EventHandler<DescriptorChangeEventArgs>(this.CompositeIdentityService_DescriptorsChanged);
          ((IIdentityServiceInternal) this.m_userIdentityService).DescriptorsChangedNotification -= new EventHandler<DescriptorChangeNotificationEventArgs>(this.CompositeIdentityService_DescriptorsChangedNotification);
        }
        this.m_userIdentityService = (IdentityService) null;
        this.m_systemIdentityService = (ISystemIdentityService) null;
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(10004005, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(10004006, "IdentityService", nameof (CompositeIdentityService), "ServiceEnd");
      }
    }

    public Guid DomainId => this.m_userIdentityService.DomainId;

    public IdentityMapper IdentityMapper => this.m_userIdentityService.IdentityMapper;

    public IDictionary<string, IIdentityProvider> SyncAgents => (IDictionary<string, IIdentityProvider>) this.m_compositeSyncAgents;

    public bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      requestContext.TraceEnter(10004007, "IdentityService", nameof (CompositeIdentityService), nameof (AddMemberToGroup));
      try
      {
        ArgumentUtility.CheckForNull<IdentityDescriptor>(memberDescriptor, nameof (memberDescriptor));
        if (memberDescriptor.IsSystemServicePrincipalType())
          throw new AddGroupMemberIllegalMemberException("System descriptors cannot be added to groups.");
        return this.m_userIdentityService.AddMemberToGroup(requestContext, groupDescriptor, memberDescriptor);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004008, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004009, "IdentityService", nameof (CompositeIdentityService), nameof (AddMemberToGroup));
      }
    }

    public bool AddMemberToGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity member)
    {
      requestContext.TraceEnter(10004010, "IdentityService", nameof (CompositeIdentityService), nameof (AddMemberToGroup));
      try
      {
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(member, nameof (member));
        ArgumentUtility.CheckForNull<IdentityDescriptor>(member.Descriptor, "Descriptor");
        if (member.Descriptor.IsSystemServicePrincipalType())
          throw new AddGroupMemberIllegalMemberException("System descriptors cannot be added to groups.");
        return this.m_userIdentityService.AddMemberToGroup(requestContext, groupDescriptor, member);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004011, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004012, "IdentityService", nameof (CompositeIdentityService), nameof (AddMemberToGroup));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity CreateFrameworkIdentity(
      IVssRequestContext requestContext,
      FrameworkIdentityType identityType,
      string role,
      string identifier,
      string displayName,
      string mailAddress = null)
    {
      requestContext.TraceEnter(10004013, "IdentityService", nameof (CompositeIdentityService), nameof (CreateFrameworkIdentity));
      try
      {
        return this.m_userIdentityService.CreateFrameworkIdentity(requestContext, identityType, role, identifier, displayName, mailAddress);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004014, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004015, "IdentityService", nameof (CompositeIdentityService), nameof (CreateFrameworkIdentity));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity CreateGroup(
      IVssRequestContext requestContext,
      Guid scopeId,
      string groupSid,
      string groupName,
      string groupDescription,
      SpecialGroupType specialType = SpecialGroupType.Generic,
      bool scopeLocal = true,
      bool hasRestrictedVisibility = false)
    {
      requestContext.TraceEnter(10004016, "IdentityService", nameof (CompositeIdentityService), nameof (CreateGroup));
      try
      {
        return this.m_userIdentityService.CreateGroup(requestContext, scopeId, groupSid, groupName, groupDescription, specialType, scopeLocal, hasRestrictedVisibility);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004017, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004018, "IdentityService", nameof (CompositeIdentityService), nameof (CreateGroup));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity CreateGroup(
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
      requestContext.TraceEnter(10004019, "IdentityService", nameof (CompositeIdentityService), nameof (CreateGroup));
      try
      {
        return this.m_userIdentityService.CreateGroup(requestContext, scopeId, groupId, groupSid, groupName, groupDescription, specialType, scopeLocal, hasRestrictedVisibility);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004020, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004021, "IdentityService", nameof (CompositeIdentityService), nameof (CreateGroup));
      }
    }

    public IdentityScope CreateScope(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid parentScopeId,
      GroupScopeType scopeType,
      string scopeName,
      string adminGroupName,
      string adminGroupDescription,
      Guid creatorId)
    {
      requestContext.TraceEnter(10004022, "IdentityService", nameof (CompositeIdentityService), nameof (CreateScope));
      try
      {
        return this.m_userIdentityService.CreateScope(requestContext, scopeId, parentScopeId, scopeType, scopeName, adminGroupName, adminGroupDescription, creatorId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004023, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004024, "IdentityService", nameof (CompositeIdentityService), nameof (CreateScope));
      }
    }

    public void DeleteGroup(IVssRequestContext requestContext, IdentityDescriptor groupDescriptor)
    {
      requestContext.TraceEnter(10004025, "IdentityService", nameof (CompositeIdentityService), nameof (DeleteGroup));
      try
      {
        this.m_userIdentityService.DeleteGroup(requestContext, groupDescriptor);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004026, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004027, "IdentityService", nameof (CompositeIdentityService), nameof (DeleteGroup));
      }
    }

    public void DeleteScope(IVssRequestContext requestContext, Guid scopeId)
    {
      requestContext.TraceEnter(10004034, "IdentityService", nameof (CompositeIdentityService), nameof (DeleteScope));
      try
      {
        this.m_userIdentityService.DeleteScope(requestContext, scopeId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004035, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004036, "IdentityService", nameof (CompositeIdentityService), nameof (DeleteScope));
      }
    }

    public IdentityScope GetScope(IVssRequestContext requestContext, string scopeName)
    {
      requestContext.TraceEnter(10004043, "IdentityService", nameof (CompositeIdentityService), nameof (GetScope));
      try
      {
        return this.m_userIdentityService.GetScope(requestContext, scopeName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004044, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004045, "IdentityService", nameof (CompositeIdentityService), nameof (GetScope));
      }
    }

    public IdentityScope GetScope(IVssRequestContext requestContext, Guid scopeId)
    {
      requestContext.TraceEnter(10004046, "IdentityService", nameof (CompositeIdentityService), nameof (GetScope));
      try
      {
        return this.m_userIdentityService.GetScope(requestContext, scopeId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004047, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004048, "IdentityService", nameof (CompositeIdentityService), nameof (GetScope));
      }
    }

    public string GetSignoutToken(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(10004049, "IdentityService", nameof (CompositeIdentityService), nameof (GetSignoutToken));
      try
      {
        return this.m_userIdentityService.GetSignoutToken(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004050, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004051, "IdentityService", nameof (CompositeIdentityService), nameof (GetSignoutToken));
      }
    }

    public bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      bool flag = requestContext.IsTracing(10004055, TraceLevel.Verbose, "IdentityService", nameof (CompositeIdentityService));
      if (flag)
        requestContext.TraceEnter(10004055, "IdentityService", nameof (CompositeIdentityService), nameof (IsMember));
      try
      {
        ArgumentUtility.CheckForNull<IdentityDescriptor>(groupDescriptor, nameof (groupDescriptor));
        ArgumentUtility.CheckForNull<IdentityDescriptor>(memberDescriptor, nameof (memberDescriptor));
        if (memberDescriptor.IsSystemServicePrincipalType() || IdentityDescriptorComparer.Instance.Equals(memberDescriptor, UserWellKnownIdentityDescriptors.AnonymousPrincipal))
          return this.m_systemIdentityService.IsMember(requestContext, groupDescriptor, memberDescriptor);
        int num = this.m_userIdentityService.IsMember(requestContext, groupDescriptor, memberDescriptor) ? 1 : 0;
        if (num != 0 && this.m_isMemberReporter != null)
          this.m_isMemberReporter.AddRecord(requestContext, memberDescriptor, groupDescriptor);
        return num != 0;
      }
      catch (Exception ex)
      {
        if (flag)
          requestContext.TraceException(10004056, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        if (flag)
          requestContext.TraceLeave(10004057, "IdentityService", nameof (CompositeIdentityService), nameof (IsMember));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ListDeletedGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      requestContext.TraceEnter(10004058, "IdentityService", nameof (CompositeIdentityService), nameof (ListDeletedGroups));
      try
      {
        return this.m_userIdentityService.ListDeletedGroups(requestContext, scopeIds, recurse, propertyNameFilters);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004059, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004060, "IdentityService", nameof (CompositeIdentityService), nameof (ListDeletedGroups));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      Guid[] scopeIds,
      bool recurse,
      IEnumerable<string> propertyNameFilters)
    {
      requestContext.TraceEnter(10004061, "IdentityService", nameof (CompositeIdentityService), nameof (ListGroups));
      try
      {
        List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.m_userIdentityService.ListGroups(requestContext, scopeIds, recurse, propertyNameFilters));
        IList<Microsoft.VisualStudio.Services.Identity.Identity> collection = this.m_systemIdentityService.ListGroups(requestContext, scopeIds, recurse, propertyNameFilters);
        if (collection != null && collection.Count > 0)
          identityList.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) collection);
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityList;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004062, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004063, "IdentityService", nameof (CompositeIdentityService), nameof (ListGroups));
      }
    }

    public FilteredIdentitiesList ReadFilteredIdentities(
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
      requestContext.TraceEnter(10004064, "IdentityService", nameof (CompositeIdentityService), nameof (ReadFilteredIdentities));
      try
      {
        return this.m_userIdentityService.ReadFilteredIdentities(requestContext, scopeId, descriptors, filters, suggestedPageSize, lastSearchResult, lookForward, queryMembership, propertyNameFilters);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004065, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004066, "IdentityService", nameof (CompositeIdentityService), nameof (ReadFilteredIdentities));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters)
    {
      requestContext.TraceEnter(10004067, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentities));
      try
      {
        if (!string.IsNullOrEmpty(factorValue) && (searchFactor == IdentitySearchFilter.General || searchFactor == IdentitySearchFilter.AccountName || searchFactor == IdentitySearchFilter.Identifier))
        {
          string identifier = factorValue;
          int num = factorValue.IndexOf('\\');
          if (num >= 0)
            identifier = factorValue.Substring(num + 1);
          Guid spGuid;
          if (ServicePrincipals.TryParse(identifier, out spGuid, out Guid _))
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = this.m_systemIdentityService.ReadIdentities(requestContext.To(TeamFoundationHostType.Deployment), (IList<Guid>) new Guid[1]
            {
              spGuid
            }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
            if (identity != null && identity.Descriptor.Identifier.Equals(identifier, StringComparison.OrdinalIgnoreCase))
              return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
              {
                identity
              };
          }
        }
        return this.m_userIdentityService.ReadIdentities(requestContext, searchFactor, factorValue, queryMembership, propertyNameFilters);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004068, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004069, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentities));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      requestContext.TraceEnter(10004070, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentities));
      try
      {
        if (identityIds == null)
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source1 = this.m_systemIdentityService.ReadIdentities(requestContext, identityIds, queryMembership, propertyNameFilters, includeRestrictedVisibility);
        if (source1.All<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)))
          return source1;
        if (source1.All<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x == null)))
          return this.m_userIdentityService.ReadIdentities(requestContext, identityIds, queryMembership, propertyNameFilters, includeRestrictedVisibility);
        List<Tuple<Guid, int>> source2 = new List<Tuple<Guid, int>>(identityIds.Count);
        for (int index = 0; index < identityIds.Count; ++index)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = source1[index];
          Guid identityId = identityIds[index];
          if (identity == null)
            source2.Add(new Tuple<Guid, int>(identityId, index));
        }
        List<Guid> list = source2.Select<Tuple<Guid, int>, Guid>((Func<Tuple<Guid, int>, Guid>) (x => x.Item1)).ToList<Guid>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.m_userIdentityService.ReadIdentities(requestContext, (IList<Guid>) list, queryMembership, propertyNameFilters, includeRestrictedVisibility);
        for (int index = 0; index < identityList.Count; ++index)
        {
          if (identityList[index] != null)
          {
            source1[source2[index].Item2] = identityList[index];
            identityIds[source2[index].Item2] = list[index];
          }
        }
        return source1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004071, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004072, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentities));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      requestContext.TraceEnter(10004073, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentities));
      try
      {
        if (descriptors == null)
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source1 = this.m_systemIdentityService.ReadIdentities(requestContext, descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
        if (source1.All<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)))
          return source1;
        if (source1.All<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x == null)))
          return this.m_userIdentityService.ReadIdentities(requestContext, descriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
        List<Tuple<IdentityDescriptor, int>> source2 = new List<Tuple<IdentityDescriptor, int>>(descriptors.Count);
        for (int index = 0; index < descriptors.Count; ++index)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = source1[index];
          IdentityDescriptor descriptor = descriptors[index];
          if (identity == null)
            source2.Add(new Tuple<IdentityDescriptor, int>(descriptor, index));
        }
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.m_userIdentityService.ReadIdentities(requestContext, (IList<IdentityDescriptor>) source2.Select<Tuple<IdentityDescriptor, int>, IdentityDescriptor>((Func<Tuple<IdentityDescriptor, int>, IdentityDescriptor>) (x => x.Item1)).ToList<IdentityDescriptor>(), queryMembership, propertyNameFilters, includeRestrictedVisibility);
        for (int index = 0; index < identityList.Count; ++index)
        {
          if (identityList[index] != null)
            source1[source2[index].Item2] = identityList[index];
        }
        return source1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004074, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004075, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentities));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      requestContext.TraceEnter(10004073, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentities));
      try
      {
        return socialDescriptors == null ? (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>() : this.m_userIdentityService.ReadIdentities(requestContext, socialDescriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004074, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004075, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentities));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility = false)
    {
      requestContext.TraceEnter(10004073, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentities));
      try
      {
        if (subjectDescriptors == null)
          return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source1 = this.m_systemIdentityService.ReadIdentities(requestContext, subjectDescriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
        if (source1.All<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)))
          return source1;
        if (source1.All<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x == null)))
          return this.m_userIdentityService.ReadIdentities(requestContext, subjectDescriptors, queryMembership, propertyNameFilters, includeRestrictedVisibility);
        List<Tuple<SubjectDescriptor, int>> source2 = new List<Tuple<SubjectDescriptor, int>>(subjectDescriptors.Count);
        for (int index = 0; index < subjectDescriptors.Count; ++index)
        {
          if (source1[index] == null)
            source2.Add(new Tuple<SubjectDescriptor, int>(subjectDescriptors[index], index));
        }
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.m_userIdentityService.ReadIdentities(requestContext, (IList<SubjectDescriptor>) source2.Select<Tuple<SubjectDescriptor, int>, SubjectDescriptor>((Func<Tuple<SubjectDescriptor, int>, SubjectDescriptor>) (x => x.Item1)).ToList<SubjectDescriptor>(), queryMembership, propertyNameFilters, includeRestrictedVisibility);
        for (int index = 0; index < identityList.Count; ++index)
        {
          if (identityList[index] != null)
            source1[source2[index].Item2] = identityList[index];
        }
        return source1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004074, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004075, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentities));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      ReadIdentitiesOptions options)
    {
      requestContext.TraceEnter(10004076, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentities));
      try
      {
        return this.m_userIdentityService.ReadIdentities(requestContext, searchFactor, factorValue, queryMembership, propertyNameFilters, options);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004077, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004078, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentities));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership)
    {
      requestContext.TraceEnter(10004079, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentitiesFromSource));
      try
      {
        return this.m_userIdentityService.ReadIdentitiesFromSource(requestContext, descriptors, queryMembership);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004080, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004081, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentitiesFromSource));
      }
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IVssRequestContext requestContext,
      IList<string> accountNames,
      QueryMembership queryMembership)
    {
      requestContext.TraceEnter(10004082, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentitiesFromSource));
      try
      {
        return this.m_userIdentityService.ReadIdentitiesFromSource(requestContext, accountNames, queryMembership);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004083, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004084, "IdentityService", nameof (CompositeIdentityService), nameof (ReadIdentitiesFromSource));
      }
    }

    [Obsolete("This method should not be used. Use IVssRequestContext members or extension methods instead, such as GetUserId, UserContext, or GetUserIdentity.")]
    public Microsoft.VisualStudio.Services.Identity.Identity ReadRequestIdentity(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(10004085, "IdentityService", nameof (CompositeIdentityService), nameof (ReadRequestIdentity));
      try
      {
        throw new NotImplementedException();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004086, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004087, "IdentityService", nameof (CompositeIdentityService), nameof (ReadRequestIdentity));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadRequestIdentity(
      IVssRequestContext requestContext,
      IEnumerable<string> propertyNameFilters)
    {
      requestContext.TraceEnter(10004088, "IdentityService", nameof (CompositeIdentityService), nameof (ReadRequestIdentity));
      try
      {
        if (requestContext.UserContext.IsSystemServicePrincipalType())
          throw new NotImplementedException();
        return this.m_userIdentityService.ReadRequestIdentity(requestContext, propertyNameFilters);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004089, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004090, "IdentityService", nameof (CompositeIdentityService), nameof (ReadRequestIdentity));
      }
    }

    public bool RemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      requestContext.TraceEnter(10004091, "IdentityService", nameof (CompositeIdentityService), nameof (RemoveMemberFromGroup));
      try
      {
        return this.m_userIdentityService.RemoveMemberFromGroup(requestContext, groupDescriptor, memberDescriptor);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004092, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004093, "IdentityService", nameof (CompositeIdentityService), nameof (RemoveMemberFromGroup));
      }
    }

    public bool ForceRemoveMemberFromGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      requestContext.TraceEnter(10004148, "IdentityService", nameof (CompositeIdentityService), nameof (ForceRemoveMemberFromGroup));
      try
      {
        return this.m_userIdentityService.ForceRemoveMemberFromGroup(requestContext, groupDescriptor, memberDescriptor);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004149, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004150, "IdentityService", nameof (CompositeIdentityService), nameof (ForceRemoveMemberFromGroup));
      }
    }

    public void RenameScope(IVssRequestContext requestContext, Guid scopeId, string newName)
    {
      requestContext.TraceEnter(10004094, "IdentityService", nameof (CompositeIdentityService), nameof (RenameScope));
      try
      {
        this.m_userIdentityService.RenameScope(requestContext, scopeId, newName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004095, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004096, "IdentityService", nameof (CompositeIdentityService), nameof (RenameScope));
      }
    }

    public IdentitySearchResult SearchIdentities(
      IVssRequestContext requestContext,
      IdentitySearchParameters searchParameters)
    {
      requestContext.TraceEnter(10004097, "IdentityService", nameof (CompositeIdentityService), nameof (SearchIdentities));
      try
      {
        return this.m_userIdentityService.SearchIdentities(requestContext, searchParameters);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004098, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004099, "IdentityService", nameof (CompositeIdentityService), nameof (SearchIdentities));
      }
    }

    public bool UpdateIdentities(IVssRequestContext requestContext, IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      requestContext.TraceEnter(10004106, "IdentityService", nameof (CompositeIdentityService), nameof (UpdateIdentities));
      try
      {
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities1 = identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => !x.Descriptor.IsSystemServicePrincipalType()));
        return identities1.Any<Microsoft.VisualStudio.Services.Identity.Identity>() && this.m_userIdentityService.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>(identities1));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004107, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004108, "IdentityService", nameof (CompositeIdentityService), nameof (UpdateIdentities));
      }
    }

    public bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetadataUpdates)
    {
      requestContext.TraceEnter(10004109, "IdentityService", nameof (CompositeIdentityService), nameof (UpdateIdentities));
      try
      {
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> collection = identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => !x.Descriptor.IsSystemServicePrincipalType()));
        return this.m_userIdentityService.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>(collection), allowMetadataUpdates);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004110, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004111, "IdentityService", nameof (CompositeIdentityService), nameof (UpdateIdentities));
      }
    }

    public void RestoreScope(IVssRequestContext requestContext, Guid scopeId)
    {
      requestContext.TraceEnter(10004136, "IdentityService", nameof (CompositeIdentityService), nameof (RestoreScope));
      try
      {
        this.m_userIdentityService.RestoreScope(requestContext, scopeId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004137, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004138, "IdentityService", nameof (CompositeIdentityService), nameof (RestoreScope));
      }
    }

    public int UpgradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      throw new NotImplementedException();
    }

    public int DowngradeIdentitiesToTargetResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      throw new NotImplementedException();
    }

    Guid IIdentityServiceInternalRequired.GetScopeParentId(
      IVssRequestContext requestContext,
      Guid scopeId)
    {
      requestContext.TraceEnter(10004112, "IdentityService", nameof (CompositeIdentityService), "GetScopeParentId");
      try
      {
        return ((IIdentityServiceInternalRequired) this.m_userIdentityService).GetScopeParentId(requestContext, scopeId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004113, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004114, "IdentityService", nameof (CompositeIdentityService), "GetScopeParentId");
      }
    }

    Microsoft.VisualStudio.Services.Identity.Identity IIdentityServiceInternal.CreateUser(
      IVssRequestContext requestContext,
      Guid scopeId,
      string userSid,
      string domainName,
      string accountName,
      string description)
    {
      requestContext.TraceEnter(10004115, "IdentityService", nameof (CompositeIdentityService), "CreateUser");
      try
      {
        return this.CheckIdentityServiceInternal().CreateUser(requestContext, scopeId, userSid, domainName, accountName, description);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004116, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004117, "IdentityService", nameof (CompositeIdentityService), "CreateUser");
      }
    }

    ChangedIdentities IIdentityServiceInternal.GetIdentityChanges(
      IVssRequestContext requestContext,
      ChangedIdentitiesContext sequenceContext)
    {
      requestContext.TraceEnter(10004118, "IdentityService", nameof (CompositeIdentityService), "GetIdentityChanges");
      try
      {
        return this.CheckIdentityServiceInternal().GetIdentityChanges(requestContext, sequenceContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004119, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004120, "IdentityService", nameof (CompositeIdentityService), "GetIdentityChanges");
      }
    }

    IdentityChanges IIdentityServiceInternal.GetIdentityChanges(
      IVssRequestContext requestContext,
      int sequenceId,
      long identitySequenceId,
      long groupSequenceId,
      long organizationIdentitySequenceId)
    {
      requestContext.TraceEnter(10004121, "IdentityService", nameof (CompositeIdentityService), "GetIdentityChanges");
      try
      {
        return this.CheckIdentityServiceInternal().GetIdentityChanges(requestContext, sequenceId, identitySequenceId, groupSequenceId, organizationIdentitySequenceId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004122, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004123, "IdentityService", nameof (CompositeIdentityService), "GetIdentityChanges");
      }
    }

    int IIdentityServiceInternalRequired.GetCurrentSequenceId(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(10004124, "IdentityService", nameof (CompositeIdentityService), "GetCurrentSequenceId");
      try
      {
        return ((IIdentityServiceInternalRequired) this.m_userIdentityService).GetCurrentSequenceId(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004125, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004126, "IdentityService", nameof (CompositeIdentityService), "GetCurrentSequenceId");
      }
    }

    public IEnumerable<Guid> ReadIdentityIdsByResourceVersion(
      IVssRequestContext requestContext,
      int targetResourceVersion,
      int resultLimit = 10000,
      bool readOnlyIdsLessThanTargetVersion = true)
    {
      throw new NotImplementedException();
    }

    bool IIdentityServiceInternal.RefreshIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      requestContext.TraceEnter(10004127, "IdentityService", nameof (CompositeIdentityService), "RefreshIdentity");
      try
      {
        return this.CheckIdentityServiceInternal().RefreshIdentity(requestContext, descriptor);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004128, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004129, "IdentityService", nameof (CompositeIdentityService), "RefreshIdentity");
      }
    }

    int IIdentityServiceInternalRequired.GetCurrentChangeId() => ((IIdentityServiceInternalRequired) this.m_userIdentityService).GetCurrentChangeId();

    bool IIdentityServiceInternal.LastUserAccessUpdateScheduled(
      IVssRequestContext requestContext,
      Guid identityId)
    {
      requestContext.TraceEnter(10004130, "IdentityService", nameof (CompositeIdentityService), "LastUserAccessUpdateScheduled");
      try
      {
        return this.CheckIdentityServiceInternal().LastUserAccessUpdateScheduled(requestContext, identityId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004131, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004132, "IdentityService", nameof (CompositeIdentityService), "LastUserAccessUpdateScheduled");
      }
    }

    void IIdentityServiceInternal.ScheduleLastUserAccessUpdate(
      IVssRequestContext requestContext,
      Guid identityId)
    {
      requestContext.TraceEnter(10004133, "IdentityService", nameof (CompositeIdentityService), "ScheduleLastUserAccessUpdate");
      try
      {
        this.CheckIdentityServiceInternal().ScheduleLastUserAccessUpdate(requestContext, identityId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004134, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004135, "IdentityService", nameof (CompositeIdentityService), "ScheduleLastUserAccessUpdate");
      }
    }

    void IIdentityServiceInternal.RefreshSearchIdentitiesCache(
      IVssRequestContext requestContext,
      Guid scopeId)
    {
      requestContext.TraceEnter(10004145, "IdentityService", nameof (CompositeIdentityService), "RefreshSearchIdentitiesCache");
      try
      {
        this.CheckIdentityServiceInternal().RefreshSearchIdentitiesCache(requestContext, scopeId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004146, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004147, "IdentityService", nameof (CompositeIdentityService), "RefreshSearchIdentitiesCache");
      }
    }

    IdentityDomain IIdentityServiceInternal.Domain => this.CheckIdentityServiceInternal().Domain;

    event EventHandler<DescriptorChangeEventArgs> IIdentityServiceInternal.DescriptorsChanged
    {
      add => this.m_descriptorChanged += value;
      remove => this.m_descriptorChanged -= value;
    }

    event EventHandler<DescriptorChangeNotificationEventArgs> IIdentityServiceInternal.DescriptorsChangedNotification
    {
      add => this.m_descriptorChangedNotification += value;
      remove => this.m_descriptorChangedNotification -= value;
    }

    void IIdentityServiceInternalRestricted.ClearIdentityCache(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(10004154, "IdentityService", nameof (CompositeIdentityService), "ClearIdentityCache");
      try
      {
        this.m_userIdentityService.IdentityServiceInternalRestricted()?.ClearIdentityCache(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10004155, "IdentityService", nameof (CompositeIdentityService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(10004156, "IdentityService", nameof (CompositeIdentityService), "ClearIdentityCache");
      }
    }

    void IIdentityServiceInternalRestricted.InvalidateIdentities(
      IVssRequestContext requestContext,
      ICollection<Guid> identityIds)
    {
      this.m_userIdentityService.IdentityServiceInternalRestricted()?.InvalidateIdentities(requestContext, identityIds);
    }

    void IInstallableIdentityService.Install(IVssRequestContext requestContext)
    {
      if (this.m_userIdentityService is IInstallableIdentityService userIdentityService)
        userIdentityService.Install(requestContext);
      if (!(this.m_systemIdentityService is IInstallableIdentityService systemIdentityService))
        return;
      systemIdentityService.Install(requestContext);
    }

    void IInstallableIdentityService.Uninstall(
      IVssRequestContext requestContext,
      IdentityDomain domain)
    {
      if (this.m_userIdentityService is IInstallableIdentityService userIdentityService)
        userIdentityService.Uninstall(requestContext, domain);
      if (!(this.m_systemIdentityService is IInstallableIdentityService systemIdentityService))
        return;
      systemIdentityService.Uninstall(requestContext, domain);
    }

    private IIdentityServiceInternal CheckIdentityServiceInternal() => this.m_userIdentityService is IIdentityServiceInternal userIdentityService ? userIdentityService : throw new NotImplementedException();
  }
}
