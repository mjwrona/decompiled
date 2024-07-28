// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationIdentityService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Notifications.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationIdentityService : IVssFrameworkService, ITeamFoundationIdentityService
  {
    private IIdentityServiceInternal m_identityService;
    private int m_readBatchSizeLimit;
    private IdentityDomain m_hostDomain;
    private TeamFoundationIdentityService.IdentityPropertyHelper m_propertyHelper = new TeamFoundationIdentityService.IdentityPropertyHelper();

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_identityService = systemRequestContext.GetService<IdentityService>().IdentityServiceInternal();
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        this.m_readBatchSizeLimit = systemRequestContext.GetService<CachedRegistryService>().GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Integration/Settings/ReadBatchSizeLimit", 100000);
        this.m_hostDomain = new IdentityDomain(systemRequestContext, (IdentityDomain) null);
      }
      else
      {
        TeamFoundationIdentityService service = systemRequestContext.To(TeamFoundationHostType.Parent).GetService<TeamFoundationIdentityService>();
        this.m_readBatchSizeLimit = service.ReadBatchSizeLimit;
        this.m_hostDomain = new IdentityDomain(systemRequestContext, service.Domain);
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddGroupAdministrator(
      IVssRequestContext requestContext,
      IdentityDescriptor groupIdentity,
      IdentityDescriptor descriptor)
    {
      IdentityValidation.CheckDescriptor(groupIdentity, nameof (groupIdentity));
      IdentityValidation.CheckDescriptor(descriptor, nameof (descriptor));
      TeamFoundationIdentity applicationGroup = this.GetApplicationGroup(requestContext, groupIdentity);
      string securityToken = IdentityUtil.CreateSecurityToken(applicationGroup);
      requestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId).SetAccessControlEntries(requestContext, securityToken, (IEnumerable<IAccessControlEntry>) new AccessControlEntry[1]
      {
        new AccessControlEntry(descriptor, 31, 0)
      }, true);
      requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) new TeamFoundationIdentityGroupAdminAddedEvent(applicationGroup, descriptor));
    }

    public void RemoveGroupAdministrator(
      IVssRequestContext requestContext,
      IdentityDescriptor groupIdentity,
      IdentityDescriptor descriptor)
    {
      IdentityValidation.CheckDescriptor(groupIdentity, nameof (groupIdentity));
      IdentityValidation.CheckDescriptor(descriptor, nameof (descriptor));
      TeamFoundationIdentity applicationGroup = this.GetApplicationGroup(requestContext, groupIdentity);
      string securityToken = IdentityUtil.CreateSecurityToken(applicationGroup);
      requestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId).RemovePermissions(requestContext, securityToken, descriptor, 31);
      requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) new TeamFoundationIdentityGroupAdminRemovedEvent(applicationGroup, descriptor));
    }

    public void AddRecentUser(IVssRequestContext requestContext, Guid recentUser)
    {
      this.CheckGenericReadPermission(requestContext);
      TeamFoundationIdentity foundationIdentity = this.ReadRequestIdentity(requestContext);
      HashSet<Guid> recentUsers = this.GetRecentUsers(requestContext, foundationIdentity.TeamFoundationId);
      if (recentUsers.Contains(recentUser))
        return;
      recentUsers.Add(recentUser);
      string str = string.Join<Guid>(",", (IEnumerable<Guid>) recentUsers);
      requestContext.GetService<CachedRegistryService>().SetValue(requestContext, foundationIdentity.TeamFoundationId, "/Settings/RecentUsers", str);
    }

    public TeamFoundationIdentity[] GetMostRecentlyUsedUsers(IVssRequestContext requestContext)
    {
      TeamFoundationIdentity foundationIdentity = this.ReadRequestIdentity(requestContext);
      HashSet<Guid> recentUsers = this.GetRecentUsers(requestContext, foundationIdentity.TeamFoundationId);
      Guid[] guidArray = new Guid[recentUsers.Count];
      recentUsers.CopyTo(guidArray);
      return this.ReadIdentities(requestContext, guidArray);
    }

    internal HashSet<Guid> GetRecentUsers(IVssRequestContext requestContext, Guid teamFoundationId)
    {
      string[] strArray = requestContext.GetService<CachedRegistryService>().GetValue<string>(requestContext, teamFoundationId, "/Settings/RecentUsers", string.Empty).Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries);
      HashSet<Guid> recentUsers = new HashSet<Guid>();
      recentUsers.Add(teamFoundationId);
      foreach (string input in strArray)
      {
        Guid result;
        if (Guid.TryParse(input, out result) && !recentUsers.Contains(result))
          recentUsers.Add(result);
      }
      return recentUsers;
    }

    public virtual TeamFoundationDataReader GetIdentityChanges(
      IVssRequestContext requestContext,
      Tuple<int, int> sequenceId)
    {
      ChangedIdentities identityChanges = this.m_identityService.GetIdentityChanges(requestContext, new ChangedIdentitiesContext(sequenceId.Item1, sequenceId.Item2, -1));
      TeamFoundationIdentity[] items = IdentityUtil.Convert(identityChanges.Identities);
      foreach (TeamFoundationIdentity identity in items)
        this.MapToWellKnownIdentifiers(identity);
      return new TeamFoundationDataReader(new object[2]
      {
        (object) new Tuple<int, int>(identityChanges.SequenceContext.IdentitySequenceId, identityChanges.SequenceContext.GroupSequenceId),
        (object) new StreamingCollection<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) items)
      });
    }

    public virtual TeamFoundationDataReader GetIdentityChanges(
      IVssRequestContext requestContext,
      Tuple<int, int, int> sequenceId)
    {
      ChangedIdentities identityChanges = this.m_identityService.GetIdentityChanges(requestContext, new ChangedIdentitiesContext(sequenceId.Item1, sequenceId.Item2, sequenceId.Item3, 0));
      TeamFoundationIdentity[] items = IdentityUtil.Convert(identityChanges.Identities);
      foreach (TeamFoundationIdentity identity in items)
        this.MapToWellKnownIdentifiers(identity);
      return new TeamFoundationDataReader(new object[2]
      {
        (object) new Tuple<int, int, int>(identityChanges.SequenceContext.IdentitySequenceId, identityChanges.SequenceContext.GroupSequenceId, identityChanges.SequenceContext.OrganizationIdentitySequenceId),
        (object) new StreamingCollection<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) items)
      });
    }

    public virtual TeamFoundationDataReader GetIdentityChanges(
      IVssRequestContext requestContext,
      Tuple<int, int, int> sequenceId,
      int pageSize)
    {
      ChangedIdentities identityChanges = this.m_identityService.GetIdentityChanges(requestContext, new ChangedIdentitiesContext(sequenceId.Item1, sequenceId.Item2, sequenceId.Item3, pageSize));
      TeamFoundationIdentity[] items = IdentityUtil.Convert(identityChanges.Identities);
      foreach (TeamFoundationIdentity identity in items)
        this.MapToWellKnownIdentifiers(identity);
      return new TeamFoundationDataReader(new object[2]
      {
        (object) new Tuple<int, int, int, bool>(identityChanges.SequenceContext.IdentitySequenceId, identityChanges.SequenceContext.GroupSequenceId, identityChanges.SequenceContext.OrganizationIdentitySequenceId, identityChanges.MoreData),
        (object) new StreamingCollection<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) items)
      });
    }

    public IdentityDescriptor CreateDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      IIdentityProvider identityProvider;
      if (!this.m_identityService.SyncAgents.TryGetValue(identity.GetType().FullName, out identityProvider))
        throw new NotSupportedException(FrameworkResources.IdentityProviderNotFoundMessage((object) identity.GetType().FullName));
      return identityProvider.CreateDescriptor(requestContext, identity);
    }

    public TeamFoundationIdentity ReadIdentityFromSource(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool withDirectMembership)
    {
      return IdentityUtil.Convert(this.m_identityService.ReadIdentitiesFromSource(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, (QueryMembership) (withDirectMembership ? 1 : 0))[0]);
    }

    internal TeamFoundationFilteredIdentitiesList ReadFilteredIdentities(
      IVssRequestContext requestContext,
      string scope,
      int suggestedPageSize,
      IEnumerable<IdentityFilter> filters,
      string lastSearchResult,
      bool lookForward,
      MembershipQuery membershipQuery,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      TFCommonUtil.CheckProjectUri(ref scope, true);
      ArgumentUtility.CheckForOutOfRange((int) membershipQuery, "queryMembership", 0, 4);
      Guid scopeId;
      if (!IdentityHelper.TryParseScopeId(scope, this.Domain.DomainId, out scopeId, out GroupScopeType _))
        scopeId = this.m_identityService.GetScope(requestContext.Elevate(), scope).Id;
      IEnumerable<string> globalPropertyNameFilters = (IEnumerable<string>) null;
      IEnumerable<string> localPropertyNameFilters = (IEnumerable<string>) null;
      TeamFoundationIdentityService.ExtractGlobalAndLocalPropertyNames(propertyScope, ref propertyNameFilters, out globalPropertyNameFilters, out localPropertyNameFilters);
      TeamFoundationFilteredIdentitiesList filteredIdentitiesList = TeamFoundationFilteredIdentitiesList.Convert(this.m_identityService.ReadFilteredIdentities(requestContext, scopeId, (IList<IdentityDescriptor>) null, filters, suggestedPageSize, lastSearchResult, lookForward, (QueryMembership) membershipQuery, globalPropertyNameFilters));
      if (localPropertyNameFilters != null)
        this.m_propertyHelper.ReadExtendedProperties(requestContext, (IEnumerable<TeamFoundationIdentity>) filteredIdentitiesList.Items, localPropertyNameFilters, IdentityPropertyScope.Local);
      return filteredIdentitiesList;
    }

    internal TeamFoundationFilteredIdentitiesList ReadFilteredIdentitiesById(
      IVssRequestContext requestContext,
      Guid[] tfids,
      int suggestedPageSize,
      IEnumerable<IdentityFilter> filters,
      string lastSearchResult,
      bool lookForward,
      MembershipQuery membershipQueryForTfids,
      MembershipQuery membershipQuery,
      bool readMembers,
      IEnumerable<string> propertyNameFilters)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Guid[]>(tfids, nameof (tfids));
      ArgumentUtility.CheckForOutOfRange((int) membershipQuery, "queryMembership", 0, 4);
      TeamFoundationIdentity[] foundationIdentityArray = this.ReadIdentities(requestContext, tfids, membershipQueryForTfids, ReadIdentityOptions.None, (IEnumerable<string>) null);
      HashSet<IdentityDescriptor> collection = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      foreach (TeamFoundationIdentity foundationIdentity in foundationIdentityArray)
      {
        if (foundationIdentity != null)
        {
          foreach (IdentityDescriptor descriptor in readMembers ? (IEnumerable<IdentityDescriptor>) foundationIdentity.Members : (IEnumerable<IdentityDescriptor>) foundationIdentity.MemberOf)
          {
            IdentityDescriptor identityDescriptor = this.m_hostDomain.MapFromWellKnownIdentifier(descriptor);
            collection.Add(identityDescriptor);
          }
        }
      }
      TeamFoundationFilteredIdentitiesList filteredIdentitiesList = TeamFoundationFilteredIdentitiesList.Convert(this.m_identityService.ReadFilteredIdentities(requestContext, this.m_hostDomain.DomainId, (IList<IdentityDescriptor>) new List<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) collection), filters, suggestedPageSize, lastSearchResult, lookForward, (QueryMembership) membershipQuery, propertyNameFilters));
      if (propertyNameFilters != null)
        this.m_propertyHelper.ReadExtendedProperties(requestContext, (IEnumerable<TeamFoundationIdentity>) filteredIdentitiesList.Items, propertyNameFilters, IdentityPropertyScope.Local);
      return filteredIdentitiesList;
    }

    public TeamFoundationFilteredIdentitiesList ReadFilteredIdentitiesByDescriptor(
      IVssRequestContext requestContext,
      IEnumerable<IdentityDescriptor> identityDescriptors,
      int suggestedPageSize,
      IEnumerable<IdentityFilter> filters,
      string lastSearchResult,
      bool lookForward,
      MembershipQuery membershipQueryForTfids,
      MembershipQuery membershipQuery,
      IEnumerable<string> propertyNameFilters)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<IdentityDescriptor>>(identityDescriptors, nameof (identityDescriptors));
      ArgumentUtility.CheckForOutOfRange((int) membershipQuery, "queryMembership", 0, 4);
      HashSet<IdentityDescriptor> collection = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      foreach (IdentityDescriptor identityDescriptor1 in identityDescriptors)
      {
        IdentityDescriptor identityDescriptor2 = this.m_hostDomain.MapFromWellKnownIdentifier(identityDescriptor1);
        collection.Add(identityDescriptor2);
      }
      TeamFoundationFilteredIdentitiesList filteredIdentitiesList = TeamFoundationFilteredIdentitiesList.Convert(this.m_identityService.ReadFilteredIdentities(requestContext, this.m_hostDomain.DomainId, (IList<IdentityDescriptor>) new List<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) collection), filters, suggestedPageSize, lastSearchResult, lookForward, (QueryMembership) membershipQuery, propertyNameFilters));
      if (propertyNameFilters != null)
        this.m_propertyHelper.ReadExtendedProperties(requestContext, (IEnumerable<TeamFoundationIdentity>) filteredIdentitiesList.Items, propertyNameFilters, IdentityPropertyScope.Local);
      return filteredIdentitiesList;
    }

    public TeamFoundationFilteredIdentitiesList ReadFilteredIdentities(
      IVssRequestContext requestContext,
      string expression,
      int suggestedPageSize,
      string lastSearchResult,
      bool lookForward,
      MembershipQuery membershipQuery)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(expression, nameof (expression));
      ArgumentUtility.CheckForOutOfRange((int) membershipQuery, "queryMembership", 0, 4);
      QueryExpression queryExpression = new QueryExpression(expression, (IEnumerable<string>) new string[5]
      {
        "In",
        "Under",
        "Near",
        "StartsWith",
        "Contains"
      });
      List<IdentityFilter> filters = new List<IdentityFilter>();
      string projectUri = (string) null;
      foreach (ExpressionToken token in queryExpression.Tokens)
      {
        if (token.TokenType == Microsoft.TeamFoundation.Framework.Server.TokenType.OpenParen || token.TokenType == Microsoft.TeamFoundation.Framework.Server.TokenType.CloseParen)
          throw new IdentityExpressionException(FrameworkResources.QueryExpression_Malformed());
      }
      IEnumerator<ExpressionToken> enumerator = queryExpression.Tokens.GetEnumerator();
      while (enumerator.MoveNext())
      {
        IdentityFilter identityFilter;
        if (enumerator.Current.Value.Equals("Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
        {
          enumerator.MoveNext();
          ExpressionToken current = enumerator.Current;
          enumerator.MoveNext();
          identityFilter = (IdentityFilter) new RelationshipIdentityFilter(requestContext, current, enumerator.Current.Value);
        }
        else if (enumerator.Current.Value.Equals("Microsoft.TeamFoundation.Identity.DisplayName", StringComparison.OrdinalIgnoreCase))
        {
          enumerator.MoveNext();
          ExpressionToken current = enumerator.Current;
          enumerator.MoveNext();
          identityFilter = (IdentityFilter) new DisplayNameIdentityFilter(current, enumerator.Current.Value);
        }
        else if (enumerator.Current.Value.Equals("Microsoft.TeamFoundation.Identity.Type", StringComparison.OrdinalIgnoreCase))
        {
          enumerator.MoveNext();
          ExpressionToken current = enumerator.Current;
          enumerator.MoveNext();
          identityFilter = (IdentityFilter) new MembershipTypeIdentityFilter(current, enumerator.Current.Value);
        }
        else if (enumerator.Current.Value.Equals("Microsoft.TeamFoundation.Identity.Scope", StringComparison.OrdinalIgnoreCase))
        {
          enumerator.MoveNext();
          ExpressionToken current = enumerator.Current;
          enumerator.MoveNext();
          if (current.TokenType != Microsoft.TeamFoundation.Framework.Server.TokenType.Equal && projectUri == null)
            throw new IdentityExpressionException(FrameworkResources.QueryExpression_Malformed());
          projectUri = enumerator.Current.Value;
          TFCommonUtil.CheckProjectUri(ref projectUri, true);
          identityFilter = (IdentityFilter) null;
        }
        else
          break;
        if (identityFilter != null)
          filters.Add(identityFilter);
        if (enumerator.MoveNext())
        {
          if (enumerator.Current.TokenType != Microsoft.TeamFoundation.Framework.Server.TokenType.And && enumerator.Current.TokenType != Microsoft.TeamFoundation.Framework.Server.TokenType.EndOfExpression)
            throw new IdentityExpressionException(FrameworkResources.QueryExpression_Malformed());
        }
        else
          break;
      }
      return this.ReadFilteredIdentities(requestContext, projectUri, suggestedPageSize, (IEnumerable<IdentityFilter>) filters, lastSearchResult, lookForward, membershipQuery, (IEnumerable<string>) null, IdentityPropertyScope.None);
    }

    public TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDescriptor[] descriptors)
    {
      return this.ReadIdentities(requestContext, descriptors, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
    }

    public TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDescriptor[] descriptors,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters)
    {
      return this.ReadIdentities(requestContext, descriptors, queryMembership, readOptions, propertyNameFilters, IdentityPropertyScope.Both);
    }

    public TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDescriptor[] descriptors,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      int num1 = (readOptions & ReadIdentityOptions.IncludeReadFromSource) != 0 ? 1 : 0;
      bool flag = (readOptions & ReadIdentityOptions.TrueSid) != 0;
      int num2 = (readOptions & ReadIdentityOptions.ExtendedProperties) != 0 ? 1 : 0;
      IEnumerable<string> globalPropertyNameFilters = (IEnumerable<string>) null;
      IEnumerable<string> localPropertyNameFilters = (IEnumerable<string>) null;
      if (num2 != 0)
        TeamFoundationIdentityService.ExtractGlobalAndLocalPropertyNames(propertyScope, ref propertyNameFilters, out globalPropertyNameFilters, out localPropertyNameFilters);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = this.m_identityService.ReadIdentities(requestContext, (IList<IdentityDescriptor>) descriptors, (QueryMembership) queryMembership, globalPropertyNameFilters);
      TeamFoundationIdentity[] source = IdentityUtil.Convert(identities);
      if (num1 != 0)
      {
        for (int index = 0; index < source.Length; ++index)
        {
          if (source[index] == null)
            source[index] = this.ReadIdentityFromRequestContext(requestContext, descriptors[index]);
          if (source[index] == null)
            source[index] = this.ReadIdentityFromSource(requestContext, descriptors[index], queryMembership != 0);
          else if (!identities[index].IsActive)
          {
            TeamFoundationIdentity foundationIdentity = this.ReadIdentityFromSource(requestContext, descriptors[index], false);
            if (foundationIdentity != null)
              source[index] = foundationIdentity;
          }
        }
      }
      if (!flag)
      {
        foreach (TeamFoundationIdentity identity in source)
          this.MapToWellKnownIdentifiers(identity);
      }
      if (localPropertyNameFilters != null)
        this.m_propertyHelper.ReadExtendedProperties(requestContext, ((IEnumerable<TeamFoundationIdentity>) source).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (result => result != null && result.TeamFoundationId != Guid.Empty)), localPropertyNameFilters, IdentityPropertyScope.Local);
      return source;
    }

    public virtual TeamFoundationIdentity ReadRequestIdentity(IVssRequestContext requestContext) => IdentityUtil.Convert(requestContext.GetUserIdentity());

    public TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions)
    {
      return this.ReadIdentity(requestContext, descriptor, queryMembership, readOptions, (IEnumerable<string>) null, IdentityPropertyScope.None);
    }

    public TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      return this.ReadIdentities(requestContext, new IdentityDescriptor[1]
      {
        descriptor
      }, queryMembership, readOptions, propertyNameFilters, propertyScope)[0];
    }

    public virtual TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      Guid[] teamFoundationIds)
    {
      return this.ReadIdentities(requestContext, teamFoundationIds, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
    }

    public TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      Guid[] teamFoundationIds,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters)
    {
      return this.ReadIdentities(requestContext, teamFoundationIds, queryMembership, readOptions, propertyNameFilters, IdentityPropertyScope.Both);
    }

    public TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      Guid[] teamFoundationIds,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "TeamFoundationIdentityService.ReadIdentities"))
      {
        if (teamFoundationIds == null)
          return Array.Empty<TeamFoundationIdentity>();
        if (teamFoundationIds.Length > this.ReadBatchSizeLimit)
          throw new TooManyItemsException(this.ReadBatchSizeLimit);
        int num = (readOptions & ReadIdentityOptions.ExtendedProperties) != 0 ? 1 : 0;
        bool flag = (readOptions & ReadIdentityOptions.TrueSid) != 0;
        IEnumerable<string> globalPropertyNameFilters = (IEnumerable<string>) null;
        IEnumerable<string> localPropertyNameFilters = (IEnumerable<string>) null;
        if (num != 0)
          TeamFoundationIdentityService.ExtractGlobalAndLocalPropertyNames(propertyScope, ref propertyNameFilters, out globalPropertyNameFilters, out localPropertyNameFilters);
        TeamFoundationIdentity[] identities = IdentityUtil.Convert(this.m_identityService.ReadIdentities(requestContext, (IList<Guid>) teamFoundationIds, (QueryMembership) queryMembership, globalPropertyNameFilters));
        if (!flag)
        {
          foreach (TeamFoundationIdentity identity in identities)
            this.MapToWellKnownIdentifiers(identity);
        }
        if (localPropertyNameFilters != null)
          this.m_propertyHelper.ReadExtendedProperties(requestContext, (IEnumerable<TeamFoundationIdentity>) identities, localPropertyNameFilters, IdentityPropertyScope.Local);
        return identities;
      }
    }

    public TeamFoundationIdentity[][] ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string[] factorValues)
    {
      return this.ReadIdentities(requestContext, searchFactor, factorValues, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
    }

    public TeamFoundationIdentity[][] ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string[] factorValues,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters)
    {
      return this.ReadIdentities(requestContext, searchFactor, factorValues, queryMembership, readOptions, propertyNameFilters, IdentityPropertyScope.Both);
    }

    public TeamFoundationIdentity[][] ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string[] factorValues,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      if (factorValues == null)
        return Array.Empty<TeamFoundationIdentity[]>();
      if (factorValues.Length > this.ReadBatchSizeLimit)
        throw new TooManyItemsException(this.ReadBatchSizeLimit);
      ArgumentUtility.CheckForOutOfRange((int) queryMembership, nameof (queryMembership), 0, 4);
      TeamFoundationIdentity[][] foundationIdentityArray = new TeamFoundationIdentity[factorValues.Length][];
      for (int index = 0; index < factorValues.Length; ++index)
        foundationIdentityArray[index] = this.ReadIdentities(requestContext, searchFactor, factorValues[index], queryMembership, readOptions, propertyNameFilters, propertyScope);
      return foundationIdentityArray;
    }

    public TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      string generalSearchValue)
    {
      return this.ReadIdentity(requestContext, IdentitySearchFactor.General, generalSearchValue);
    }

    public virtual TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string factorValue)
    {
      return this.ReadIdentity(requestContext, searchFactor, factorValue, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
    }

    public TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string factorValue,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters)
    {
      return this.ReadIdentity(requestContext, searchFactor, factorValue, queryMembership, readOptions, propertyNameFilters, IdentityPropertyScope.Both);
    }

    public TeamFoundationIdentity ReadIdentity(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string factorValue,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      TeamFoundationIdentity[] matchingIdentities = this.ReadIdentities(requestContext, searchFactor, factorValue, queryMembership, readOptions, propertyNameFilters, propertyScope);
      int length = matchingIdentities.Length;
      TeamFoundationIdentity foundationIdentity1;
      if (length > 1)
      {
        int num = 0;
        TeamFoundationIdentity foundationIdentity2 = (TeamFoundationIdentity) null;
        foreach (TeamFoundationIdentity foundationIdentity3 in matchingIdentities)
        {
          if (foundationIdentity3.IsActive)
          {
            foundationIdentity2 = foundationIdentity3;
            ++num;
          }
        }
        if (num != 1)
          throw new MultipleIdentitiesFoundException(factorValue, matchingIdentities);
        foundationIdentity1 = foundationIdentity2;
      }
      else
        foundationIdentity1 = length != 1 ? (TeamFoundationIdentity) null : matchingIdentities[0];
      return foundationIdentity1;
    }

    public TeamFoundationIdentitySearchResult SearchIdentities(
      IVssRequestContext requestContext,
      IdentitySearchParameters searchParameters)
    {
      return new TeamFoundationIdentitySearchResult(this.m_identityService.SearchIdentities(requestContext, searchParameters));
    }

    internal TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string factorValue,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters)
    {
      return this.ReadIdentities(requestContext, searchFactor, factorValue, queryMembership, readOptions, propertyNameFilters, IdentityPropertyScope.Both);
    }

    internal TeamFoundationIdentity[] ReadIdentities(
      IVssRequestContext requestContext,
      IdentitySearchFactor searchFactor,
      string factorValue,
      MembershipQuery queryMembership,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      int num1 = (readOptions & ReadIdentityOptions.IncludeReadFromSource) != 0 ? 1 : 0;
      bool flag1 = (readOptions & ReadIdentityOptions.TrueSid) != 0;
      int num2 = (readOptions & ReadIdentityOptions.ExtendedProperties) != 0 ? 1 : 0;
      ReadIdentitiesOptions options = ReadIdentitiesOptions.None;
      if (!readOptions.HasFlag((Enum) ReadIdentityOptions.DoNotQualifyAccountNames))
        options |= ReadIdentitiesOptions.FilterIllegalMemberships;
      IEnumerable<string> globalPropertyNameFilters = (IEnumerable<string>) null;
      IEnumerable<string> localPropertyNameFilters = (IEnumerable<string>) null;
      if (num2 != 0)
        TeamFoundationIdentityService.ExtractGlobalAndLocalPropertyNames(propertyScope, ref propertyNameFilters, out globalPropertyNameFilters, out localPropertyNameFilters);
      TeamFoundationIdentity[] source = IdentityUtil.Convert(this.m_identityService.ReadIdentities(requestContext, (IdentitySearchFilter) searchFactor, factorValue, (QueryMembership) queryMembership, globalPropertyNameFilters, options));
      if (num1 != 0 && searchFactor == IdentitySearchFactor.AccountName)
      {
        IdentityDescriptor descriptorByAccountName = this.GetDescriptorByAccountName(requestContext, factorValue);
        if (descriptorByAccountName != (IdentityDescriptor) null && !string.Equals(descriptorByAccountName.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
        {
          bool flag2 = false;
          foreach (TeamFoundationIdentity foundationIdentity in source)
          {
            if (foundationIdentity != null && IdentityDescriptorComparer.Instance.Equals(descriptorByAccountName, foundationIdentity.Descriptor))
            {
              foundationIdentity.IsActive = true;
              flag2 = true;
              break;
            }
          }
          if (!flag2)
          {
            if (string.Equals(descriptorByAccountName.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase))
            {
              bool flag3 = false;
              foreach (TeamFoundationIdentity foundationIdentity in source)
              {
                if (foundationIdentity != null && foundationIdentity.IsActive)
                {
                  this.m_identityService.RefreshIdentity(requestContext, foundationIdentity.Descriptor);
                  flag3 = true;
                }
              }
              if (flag3)
                source = IdentityUtil.Convert(this.m_identityService.ReadIdentities(requestContext, (IdentitySearchFilter) searchFactor, factorValue, (QueryMembership) queryMembership, globalPropertyNameFilters, options));
            }
            TeamFoundationIdentity foundationIdentity1 = this.ReadIdentityFromSource(requestContext, descriptorByAccountName, queryMembership != 0);
            if (foundationIdentity1 != null)
            {
              TeamFoundationIdentity[] foundationIdentityArray = new TeamFoundationIdentity[source.Length + 1];
              source.CopyTo((Array) foundationIdentityArray, 0);
              foundationIdentityArray[source.Length] = foundationIdentity1;
              source = foundationIdentityArray;
            }
            else
              TeamFoundationTrace.Warning("Couldn't read identity of descriptor: {0}", (object) descriptorByAccountName);
          }
        }
      }
      if (!flag1)
      {
        foreach (TeamFoundationIdentity identity in source)
          this.MapToWellKnownIdentifiers(identity);
      }
      if (localPropertyNameFilters != null)
        this.m_propertyHelper.ReadExtendedProperties(requestContext, ((IEnumerable<TeamFoundationIdentity>) source).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (result => result != null && result.TeamFoundationId != Guid.Empty)), localPropertyNameFilters, IdentityPropertyScope.Local);
      return source;
    }

    public IdentityDescriptor CreateScope(
      IVssRequestContext requestContext,
      string scopeUri,
      string scopeName,
      string adminGroupName,
      string adminGroupDescription)
    {
      ArgumentUtility.CheckForNull<string>(scopeUri, nameof (scopeUri));
      GroupScopeType scopeType;
      Guid scopeId = IdentityHelper.ParseScopeId(scopeUri, Guid.Empty, out scopeType);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return this.m_identityService.CreateScope(requestContext, scopeId, this.Domain.DomainId, scopeType, scopeName, adminGroupName, adminGroupDescription, userIdentity != null ? userIdentity.Id : Guid.Empty).Administrators;
    }

    public void GetScopeInfo(
      IVssRequestContext requestContext,
      string scopeId,
      out string scopeName,
      out IdentityDescriptor administrators,
      out bool isGlobal)
    {
      TFCommonUtil.CheckProjectUri(ref scopeId, false);
      Guid scopeId1 = IdentityHelper.ParseScopeId(scopeId, this.Domain.DomainId, out GroupScopeType _);
      IdentityScope scope = this.m_identityService.GetScope(requestContext, scopeId1);
      scopeName = scope.Name;
      isGlobal = scope.IsGlobal;
      administrators = this.m_hostDomain.MapToWellKnownIdentifier(scope.Administrators);
    }

    public void DeleteScope(IVssRequestContext requestContext, string scopeId)
    {
      TFCommonUtil.CheckProjectUri(ref scopeId, false);
      Guid scopeId1 = IdentityHelper.ParseScopeId(scopeId, this.Domain.DomainId, out GroupScopeType _);
      this.m_identityService.DeleteScope(requestContext, scopeId1);
    }

    public void RenameScope(IVssRequestContext requestContext, string scopeId, string newName)
    {
      TFCommonUtil.CheckProjectUri(ref scopeId, false);
      TFCommonUtil.CheckGroupName(ref newName);
      Guid scopeId1 = IdentityHelper.ParseScopeId(scopeId, this.Domain.DomainId, out GroupScopeType _);
      this.m_identityService.RenameScope(requestContext, scopeId1, newName);
    }

    public void RestoreScope(IVssRequestContext requestContext, string scopeId)
    {
      TFCommonUtil.CheckProjectUri(ref scopeId, false);
      Guid scopeId1 = IdentityHelper.ParseScopeId(scopeId, this.Domain.DomainId, out GroupScopeType _);
      this.m_identityService.RestoreScope(requestContext, scopeId1);
    }

    public TeamFoundationIdentity CreateApplicationGroup(
      IVssRequestContext requestContext,
      string scopeId,
      string groupName,
      string groupDescription)
    {
      return this.CreateApplicationGroup(requestContext, scopeId, groupName, groupDescription, true, false);
    }

    public TeamFoundationIdentity CreateApplicationGroup(
      IVssRequestContext requestContext,
      string projectUri,
      string groupName,
      string groupDescription,
      bool scopeLocal,
      bool hasRestrictedVisibility)
    {
      Guid scopeId = IdentityHelper.ParseScopeId(projectUri, this.Domain.DomainId, out GroupScopeType _);
      return IdentityUtil.Convert(this.m_identityService.CreateGroup(requestContext, scopeId, (string) null, groupName, groupDescription, scopeLocal: scopeLocal, hasRestrictedVisibility: hasRestrictedVisibility));
    }

    internal TeamFoundationIdentity CreateApplicationGroup(
      string groupSid,
      string groupName,
      string groupDescription,
      SpecialGroupType groupType,
      IVssRequestContext requestContext)
    {
      Guid scopeId = IdentityHelper.ParseScopeId((string) null, this.Domain.DomainId, out GroupScopeType _);
      return IdentityUtil.Convert(this.m_identityService.CreateGroup(requestContext, scopeId, groupSid, groupName, groupDescription, groupType));
    }

    public void EnsureWellKnownGroupExists(
      IVssRequestContext requestContext,
      string groupSid,
      string groupName,
      string groupDescription)
    {
      this.EnsureWellKnownGroupExists(requestContext, groupSid, groupName, groupDescription, GroupSpecialType.Generic);
    }

    internal TeamFoundationIdentity EnsureWellKnownGroupExists(
      IVssRequestContext requestContext,
      string groupSid,
      string groupName,
      string groupDescription,
      GroupSpecialType specialType)
    {
      return this.EnsureWellKnownGroupExists(requestContext, groupSid, groupName, groupDescription, specialType, true, false);
    }

    internal TeamFoundationIdentity EnsureWellKnownGroupExists(
      IVssRequestContext requestContext,
      string groupSid,
      string groupName,
      string groupDescription,
      GroupSpecialType specialType,
      bool scopeLocal,
      bool hasRestrictedVisibility)
    {
      return this.EnsureWellKnownGroupExists(requestContext, (string) null, groupSid, groupName, groupDescription, specialType, scopeLocal, hasRestrictedVisibility);
    }

    internal TeamFoundationIdentity EnsureWellKnownGroupExists(
      IVssRequestContext requestContext,
      string scopeId,
      string groupSid,
      string groupName,
      string groupDescription,
      GroupSpecialType specialType,
      bool scopeLocal,
      bool hasRestrictedVisibility)
    {
      TFCommonUtil.CheckSid(groupSid, nameof (groupSid));
      if (!groupSid.StartsWith(SidIdentityHelper.WellKnownSidPrefix, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(FrameworkResources.ArgumentMustBeWellKnownGroup(), nameof (groupSid));
      Guid scopeId1 = IdentityHelper.ParseScopeId(scopeId, this.Domain.DomainId, out GroupScopeType _);
      return IdentityUtil.Convert(this.m_identityService.CreateGroup(requestContext, scopeId1, groupSid, groupName, groupDescription, (SpecialGroupType) specialType, scopeLocal, hasRestrictedVisibility));
    }

    public IdentityDescriptor CreateUser(
      IVssRequestContext requestContext,
      string userDomain,
      string account,
      string description)
    {
      return this.m_identityService.CreateUser(requestContext, this.Domain.DomainId, (string) null, userDomain, account, description).Descriptor;
    }

    public TeamFoundationIdentity[] ListApplicationGroups(
      IVssRequestContext requestContext,
      string scope,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters)
    {
      return this.ListApplicationGroups(requestContext, scope, readOptions, propertyNameFilters, IdentityPropertyScope.Both);
    }

    public TeamFoundationIdentity[] ListApplicationGroups(
      IVssRequestContext requestContext,
      string scope,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      IdentityPropertyScope propertyScope)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "TeamFoundationIdentityService.ListApplicationGroups"))
      {
        TFCommonUtil.CheckProjectUri(ref scope, true);
        Guid scopeId;
        if (!IdentityHelper.TryParseScopeId(scope, this.Domain.DomainId, out scopeId, out GroupScopeType _))
          scopeId = this.m_identityService.GetScope(requestContext.Elevate(), scope).Id;
        int num = (readOptions & ReadIdentityOptions.ExtendedProperties) != 0 ? 1 : 0;
        bool flag = (readOptions & ReadIdentityOptions.TrueSid) != 0;
        IEnumerable<string> globalPropertyNameFilters = (IEnumerable<string>) null;
        IEnumerable<string> localPropertyNameFilters = (IEnumerable<string>) null;
        if (num != 0)
          TeamFoundationIdentityService.ExtractGlobalAndLocalPropertyNames(propertyScope, ref propertyNameFilters, out globalPropertyNameFilters, out localPropertyNameFilters);
        TeamFoundationIdentity[] identities = IdentityUtil.Convert(this.m_identityService.ListGroups(requestContext, new Guid[1]
        {
          scopeId
        }, false, globalPropertyNameFilters));
        if (!flag)
        {
          foreach (TeamFoundationIdentity identity in identities)
            this.MapToWellKnownIdentifiers(identity);
        }
        if (localPropertyNameFilters != null)
          this.m_propertyHelper.ReadExtendedProperties(requestContext, (IEnumerable<TeamFoundationIdentity>) identities, localPropertyNameFilters, IdentityPropertyScope.Local);
        return identities;
      }
    }

    private static void ExtractGlobalAndLocalPropertyNames(
      IdentityPropertyScope propertyScope,
      ref IEnumerable<string> propertyNameFilters,
      out IEnumerable<string> globalPropertyNameFilters,
      out IEnumerable<string> localPropertyNameFilters)
    {
      globalPropertyNameFilters = (IEnumerable<string>) null;
      localPropertyNameFilters = (IEnumerable<string>) null;
      if (propertyNameFilters == null)
        propertyNameFilters = (IEnumerable<string>) new string[1]
        {
          "*"
        };
      if (propertyScope == IdentityPropertyScope.Both || propertyScope == IdentityPropertyScope.Global)
        globalPropertyNameFilters = propertyNameFilters;
      if (propertyScope != IdentityPropertyScope.Both && propertyScope != IdentityPropertyScope.Local)
        return;
      localPropertyNameFilters = propertyNameFilters;
    }

    internal IList<TeamFoundationIdentity> ListAllApplicationGroups(
      IVssRequestContext systemRequestContext,
      Guid[] hostIds,
      ReadIdentityOptions readOptions,
      IEnumerable<string> propertyNameFilters,
      bool deleted = false)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) hostIds, nameof (hostIds));
      if (!systemRequestContext.IsSystemContext)
        return (IList<TeamFoundationIdentity>) null;
      if (hostIds == null)
        throw new ArgumentNullException(nameof (hostIds));
      int num = (readOptions & ReadIdentityOptions.ExtendedProperties) != 0 ? 1 : 0;
      bool flag = (readOptions & ReadIdentityOptions.TrueSid) != 0;
      if (num != 0 && propertyNameFilters == null)
        propertyNameFilters = (IEnumerable<string>) new string[1]
        {
          "*"
        };
      TeamFoundationIdentity[] foundationIdentityArray = IdentityUtil.Convert(deleted ? this.m_identityService.ListDeletedGroups(systemRequestContext, hostIds, true, propertyNameFilters) : this.m_identityService.ListGroups(systemRequestContext, hostIds, true, propertyNameFilters));
      if (!flag)
      {
        foreach (TeamFoundationIdentity identity in foundationIdentityArray)
          this.MapToWellKnownIdentifiers(identity);
      }
      return (IList<TeamFoundationIdentity>) foundationIdentityArray;
    }

    public void UpdateApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      GroupProperty groupProperty,
      string newValue)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      TFCommonUtil.CheckSid(groupDescriptor.Identifier, "groupSid");
      TFCommonUtil.CheckApplicationGroupPropertyAndValue(groupProperty, ref newValue);
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = this.m_identityService.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        groupDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (readIdentity == null)
        throw new FindGroupSidDoesNotExistException(groupDescriptor.Identifier);
      switch (groupProperty)
      {
        case GroupProperty.Name:
          readIdentity.ProviderDisplayName = newValue;
          readIdentity.SetProperty("Account", (object) newValue);
          break;
        case GroupProperty.Description:
          readIdentity.SetProperty("Description", (object) newValue);
          break;
      }
      this.m_identityService.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        readIdentity
      });
    }

    public void DeleteApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor)
    {
      IdentityValidation.CheckDescriptor(groupDescriptor, nameof (groupDescriptor));
      requestContext.GetService<TeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) new PreGroupDeletionNotification(this.GetApplicationGroup(requestContext, groupDescriptor) ?? throw new FindGroupSidDoesNotExistException(groupDescriptor.Identifier)));
      this.m_identityService.DeleteGroup(requestContext, groupDescriptor);
    }

    public ServicingJobDetail DeleteUser(
      IVssRequestContext requestContext,
      IdentityDescriptor userDescriptor,
      bool deleteArtifacts = true)
    {
      IdentityValidation.CheckDescriptor(userDescriptor, nameof (userDescriptor));
      TeamFoundationIdentity identity1 = this.ReadIdentity(requestContext, userDescriptor, MembershipQuery.Direct, ReadIdentityOptions.None);
      if (identity1 == null)
        throw new IdentityNotFoundException(userDescriptor);
      if (!identity1.Descriptor.IdentityType.Equals("Microsoft.IdentityModel.Claims.ClaimsIdentity", StringComparison.OrdinalIgnoreCase) && !identity1.Descriptor.IdentityType.Equals("Microsoft.TeamFoundation.BindPendingIdentity", StringComparison.OrdinalIgnoreCase))
        throw new NotSupportedException();
      if (IdentityDescriptorComparer.Instance.Equals(identity1.Descriptor, requestContext.UserContext))
        throw new DeleteSelfException();
      TeamFoundationSecurityService foundationSecurityService = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.GetService<TeamFoundationSecurityService>() : throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      foundationSecurityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 2, false);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = IdentityUtil.Convert(identity1);
      foundationSecurityService.RemoveIdentityACEs(requestContext1, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity2
      });
      this.EnsureNotMember(requestContext1, GroupWellKnownIdentityDescriptors.SecurityServiceGroup, userDescriptor);
      foreach (IdentityDescriptor groupDescriptor in (IEnumerable<IdentityDescriptor>) identity1.MemberOf)
        this.EnsureNotMember(requestContext1, groupDescriptor, userDescriptor);
      ILicensingEntitlementService service = requestContext.GetService<ILicensingEntitlementService>();
      try
      {
        service.DeleteAccountEntitlement(requestContext, identity1.StorageKey(requestContext, TeamFoundationHostType.Application));
      }
      catch (Exception ex1)
      {
        TeamFoundationTrace.Info(ex1.Message);
        try
        {
          this.EnsureNotMember(requestContext1, GroupWellKnownIdentityDescriptors.LicensedUsersGroup, userDescriptor);
        }
        catch (Exception ex2)
        {
          TeamFoundationTrace.Info(ex2.Message);
        }
      }
      return deleteArtifacts ? this.QueueDeletePrivateArtifacts(requestContext1, requestContext1.ServiceHost.InstanceId, identity1.TeamFoundationId) : (ServicingJobDetail) null;
    }

    private ServicingJobDetail QueueDeletePrivateArtifacts(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid teamFoundationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      ArgumentUtility.CheckForEmptyGuid(teamFoundationId, "identityId");
      TeamFoundationLockInfo[] foundationLockInfoArray = new TeamFoundationLockInfo[2]
      {
        new TeamFoundationLockInfo()
        {
          LockMode = TeamFoundationLockMode.Shared,
          LockName = "Servicing-" + hostId.ToString(),
          LockTimeout = -1
        },
        new TeamFoundationLockInfo()
        {
          LockMode = TeamFoundationLockMode.Exclusive,
          LockName = "DeletePrivateArtifacts-" + teamFoundationId.ToString("D"),
          LockTimeout = -1
        }
      };
      ServicingJobData servicingJobData = new ServicingJobData(new string[1]
      {
        ServicingOperationConstants.DeletePrivateArtifacts
      })
      {
        JobTitle = FrameworkResources.DeletePrivateArtifactsJobTitle((object) teamFoundationId),
        OperationClass = "DeletePrivateArtifacts",
        ServicingHostId = hostId,
        ServicingOptions = ServicingFlags.HostMustExist | ServicingFlags.UseSystemTargetRequestContext,
        ServicingLocks = foundationLockInfoArray
      };
      servicingJobData.ServicingTokens.Add(ServicingTokenConstants.TeamFoundationId, teamFoundationId.ToString("D"));
      return requestContext.GetService<TeamFoundationServicingService>().QueueServicingJob(requestContext, servicingJobData, JobPriorityClass.Normal, JobPriorityLevel.Lowest, new Guid?());
    }

    public TeamFoundationIdentity AddMemberToApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      TeamFoundationIdentity member)
    {
      return this.AddMemberToApplicationGroup(requestContext, groupDescriptor, member, true);
    }

    private TeamFoundationIdentity AddMemberToApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      TeamFoundationIdentity member,
      bool errorOnDuplicate)
    {
      Microsoft.VisualStudio.Services.Identity.Identity member1 = IdentityUtil.Convert(requestContext, member);
      bool group = this.m_identityService.AddMemberToGroup(requestContext, groupDescriptor, member1);
      if (errorOnDuplicate && !group)
        throw new AddMemberIdentityAlreadyMemberException(groupDescriptor.Identifier, member.Descriptor.Identifier);
      member.MasterId = member1.Id;
      member.TeamFoundationId = member1.Id;
      return member;
    }

    public TeamFoundationIdentity AddMemberToApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor)
    {
      return this.AddMemberToApplicationGroup(requestContext, groupDescriptor, descriptor, true);
    }

    private TeamFoundationIdentity AddMemberToApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor,
      bool errorOnDuplicate)
    {
      bool group = this.m_identityService.AddMemberToGroup(requestContext, groupDescriptor, descriptor);
      if (errorOnDuplicate && !group)
        throw new AddMemberIdentityAlreadyMemberException(groupDescriptor.Identifier, descriptor.Identifier);
      return this.ReadIdentities(requestContext, new IdentityDescriptor[1]
      {
        descriptor
      }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null, IdentityPropertyScope.None)[0];
    }

    public TeamFoundationIdentity EnsureIsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor)
    {
      return this.AddMemberToApplicationGroup(requestContext, groupDescriptor, descriptor, false);
    }

    public TeamFoundationIdentity EnsureIsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      TeamFoundationIdentity member)
    {
      return this.AddMemberToApplicationGroup(requestContext, groupDescriptor, member, false);
    }

    public void RemoveMemberFromApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor)
    {
      this.RemoveMemberFromApplicationGroup(requestContext, groupDescriptor, descriptor, true);
    }

    public void RemoveMemberFromApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor,
      bool errorOnNotMember)
    {
      bool flag = this.m_identityService.RemoveMemberFromGroup(requestContext, groupDescriptor, memberDescriptor);
      if (errorOnNotMember && !flag)
        throw new RemoveGroupMemberNotMemberException(memberDescriptor.Identifier);
    }

    public void EnsureNotMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor)
    {
      this.RemoveMemberFromApplicationGroup(requestContext, groupDescriptor, descriptor, false);
    }

    public virtual bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor)
    {
      return this.IsMember(requestContext, groupDescriptor, descriptor, false);
    }

    public bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor descriptor,
      bool forceCacheReload)
    {
      return this.m_identityService.IsMember(requestContext, groupDescriptor, descriptor);
    }

    public bool RefreshIdentity(IVssRequestContext requestContext, IdentityDescriptor descriptor) => this.m_identityService.RefreshIdentity(requestContext, descriptor);

    public void SetCustomDisplayName(IVssRequestContext requestContext, string customDisplayName)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<TeamFoundationIdentityService>().SetCustomDisplayName(vssRequestContext, customDisplayName);
      }
      else
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        this.CheckGenericReadPermission(requestContext);
        TeamFoundationIdentity foundationIdentity = this.ReadRequestIdentity(requestContext);
        foundationIdentity.CustomDisplayName = customDisplayName ?? string.Empty;
        this.UpdateIdentities(requestContext, (IEnumerable<TeamFoundationIdentity>) new TeamFoundationIdentity[1]
        {
          foundationIdentity
        }, IdentityPropertyScope.Both);
      }
    }

    public void SetPreferredEmailAddress(
      IVssRequestContext requestContext,
      string preferredEmailAddress)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<TeamFoundationIdentityService>().SetPreferredEmailAddress(vssRequestContext, preferredEmailAddress);
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        requestContext.GetService<INotificationSubscriberService>().InvalidateSubscriber(requestContext, requestContext.GetUserId());
      }
      else
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        this.CheckGenericReadPermission(requestContext);
        TeamFoundationIdentity foundationIdentity = this.ReadRequestIdentity(requestContext);
        List<PropertyValue> properties = new List<PropertyValue>()
        {
          new PropertyValue("CustomNotificationAddresses", (object) preferredEmailAddress)
        };
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          if (string.IsNullOrWhiteSpace(preferredEmailAddress))
            return;
          requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) new IdentityEmailChangedNotification(foundationIdentity.TeamFoundationId, preferredEmailAddress));
        }
        else
          properties.Add(new PropertyValue("ConfirmedNotificationAddress", (object) preferredEmailAddress));
        this.UpdateExtendedProperties(requestContext, IdentityPropertyScope.Global, foundationIdentity.Descriptor, (IEnumerable<PropertyValue>) properties);
      }
    }

    public string GetPreferredEmailAddress(IVssRequestContext requestContext, Guid teamFoundationId) => this.GetPreferredEmailAddress(requestContext, teamFoundationId, true);

    public string GetPreferredEmailAddress(
      IVssRequestContext requestContext,
      Guid teamFoundationId,
      bool confirmed)
    {
      return IdentityHelper.GetPreferredEmailAddress(requestContext, teamFoundationId, confirmed);
    }

    public bool IsEmailConfirmationPending(IVssRequestContext requestContext, Guid teamFoundationId) => IdentityHelper.IsEmailConfirmationPending(requestContext, teamFoundationId);

    public void UpdateIdentity(
      IVssRequestContext requestContext,
      TeamFoundationIdentity identity,
      IdentityPropertyScope scope = IdentityPropertyScope.Both)
    {
      this.UpdateIdentities(requestContext, (IEnumerable<TeamFoundationIdentity>) new TeamFoundationIdentity[1]
      {
        identity
      }, scope);
    }

    public void UpdateIdentities(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationIdentity> identities,
      IdentityPropertyScope scope = IdentityPropertyScope.Both)
    {
      this.TeamUpdateTracer(requestContext, identities);
      if (scope == IdentityPropertyScope.Both || scope == IdentityPropertyScope.Global)
        this.UpdateIdentitiesInternal(requestContext, identities);
      if (scope != IdentityPropertyScope.Both && scope != IdentityPropertyScope.Local)
        return;
      this.m_propertyHelper.UpdateExtendedProperties(requestContext, IdentityPropertyScope.Local, identities);
    }

    private void TeamUpdateTracer(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationIdentity> identities)
    {
      if (requestContext.Items.ContainsKey(TeamConstants.CalledFromTeamPlatformService) || requestContext.ServiceHost.IsProduction && !requestContext.IsTracing(1509041423, TraceLevel.Warning, "TeamFoundationEntityService", "Team"))
        return;
      List<TeamFoundationIdentity> list = identities.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (i => i.TryGetProperty(TeamConstants.TeamPropertyName, out object _))).ToList<TeamFoundationIdentity>();
      if (list.Count <= 0)
        return;
      string str = new StackTrace(true).ToString();
      foreach (TeamFoundationIdentity foundationIdentity in list)
        requestContext.TraceAlways(1509041423, TraceLevel.Warning, "TeamFoundationEntityService", "Team", "Updating team {0} called from {1}", (object) foundationIdentity.DisplayName, (object) str);
    }

    private void UpdateIdentitiesInternal(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationIdentity> identities)
    {
      List<TeamFoundationIdentity> identities1 = new List<TeamFoundationIdentity>(identities);
      Microsoft.VisualStudio.Services.Identity.Identity[] identities2 = IdentityUtil.Convert((IList<TeamFoundationIdentity>) identities1);
      this.m_identityService.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities2);
      for (int index = 0; index < identities2.Length; ++index)
        identities1[index].TeamFoundationId = identities2[index].Id;
    }

    public void UpdateExtendedProperties(
      IVssRequestContext requestContext,
      IdentityPropertyScope propertyScope,
      IdentityDescriptor descriptor,
      IEnumerable<PropertyValue> properties)
    {
      IdentityValidation.CheckDescriptor(descriptor, nameof (descriptor));
      ArgumentUtility.CheckForNull<IEnumerable<PropertyValue>>(properties, nameof (properties));
      TeamFoundationIdentity owner = this.ReadIdentity(requestContext, descriptor, MembershipQuery.None, ReadIdentityOptions.None);
      if (owner == null)
        throw new IdentityNotFoundException(descriptor);
      if (propertyScope == IdentityPropertyScope.Both)
        propertyScope = IdentityPropertyScope.Global;
      List<PropertyValue> propertyValues = new List<PropertyValue>(properties);
      if (FavoritesServiceShim.IsFavoritesWriteShimNeeded(requestContext, propertyValues, propertyScope))
      {
        FavoritesServiceShim.UpdateFavoriteItems(requestContext, owner, propertyValues);
      }
      else
      {
        foreach (PropertyValue propertyValue in propertyValues)
        {
          if (IdentityAttributeTags.ReadOnlyProperties.Contains(propertyValue.PropertyName))
            throw new NotSupportedException(TFCommonResources.IdentityPropertyReadOnly((object) propertyValue.PropertyName));
          owner.SetProperty(propertyScope, propertyValue.PropertyName, propertyValue.Value);
        }
        if (propertyScope == IdentityPropertyScope.Global)
          this.UpdateIdentities(requestContext, (IEnumerable<TeamFoundationIdentity>) new TeamFoundationIdentity[1]
          {
            owner
          }, IdentityPropertyScope.Both);
        else
          this.m_propertyHelper.UpdateExtendedProperties(requestContext, IdentityPropertyScope.Local, (IEnumerable<TeamFoundationIdentity>) new TeamFoundationIdentity[1]
          {
            owner
          });
      }
    }

    private TeamFoundationIdentity GetApplicationGroup(
      IVssRequestContext requestContext,
      IdentityDescriptor groupSid)
    {
      return this.ReadIdentities(requestContext, new IdentityDescriptor[1]
      {
        groupSid
      }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null, IdentityPropertyScope.None)[0] ?? throw new FindGroupSidDoesNotExistException(groupSid.Identifier);
    }

    private IdentityDescriptor GetDescriptorByAccountName(
      IVssRequestContext requestContext,
      string accountName)
    {
      IdentityDescriptor descriptorByAccountName = (IdentityDescriptor) null;
      if (string.IsNullOrEmpty(accountName))
        return (IdentityDescriptor) null;
      string userName;
      int uniqueUserId;
      if (UserNameUtil.TryParseUniqueUserName(accountName, out userName, out uniqueUserId) && uniqueUserId != 0)
        return (IdentityDescriptor) null;
      foreach (IIdentityProvider identityProvider in (IEnumerable<IIdentityProvider>) this.m_identityService.SyncAgents.Values)
      {
        descriptorByAccountName = identityProvider.CreateDescriptor(requestContext, userName);
        if (descriptorByAccountName != (IdentityDescriptor) null)
          break;
      }
      return descriptorByAccountName;
    }

    public string GetProjectAdminSid(IVssRequestContext requestContext, string projectUri)
    {
      Guid scopeId = IdentityHelper.ParseScopeId(projectUri, this.Domain.DomainId, out GroupScopeType _);
      ProjectAdminCacheService service = requestContext.GetService<ProjectAdminCacheService>();
      string identifier;
      if (!service.TryGetValue(requestContext, scopeId, out identifier))
      {
        identifier = this.m_identityService.GetScope(requestContext, scopeId).Administrators.Identifier;
        service.Set(requestContext, scopeId, identifier);
      }
      return identifier;
    }

    public bool IsIdentityCached(IVssRequestContext requestContext, SecurityIdentifier securityId) => true;

    [Obsolete]
    public bool IsIdentityCached(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor)
    {
      TeamFoundationIdentity foundationIdentity1 = this.ReadIdentityFromSource(requestContext, identityDescriptor, true);
      if (foundationIdentity1 == null)
        return false;
      TeamFoundationIdentity foundationIdentity2 = this.ReadIdentity(requestContext, identityDescriptor, MembershipQuery.Direct, ReadIdentityOptions.None);
      return foundationIdentity2 != null && new HashSet<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) foundationIdentity1.Members, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance).SetEquals((IEnumerable<IdentityDescriptor>) new HashSet<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) foundationIdentity2.Members, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance));
    }

    internal virtual IdentityDomain Domain => this.m_hostDomain;

    internal void Install(IVssRequestContext requestContext, bool idempotent) => ((IInstallableIdentityService) requestContext.GetService<IdentityService>()).Install(requestContext);

    internal int GetCurrentChangeId() => this.m_identityService.GetCurrentChangeId();

    internal static void Trace(TraceLevel traceLevel, string format, params object[] args)
    {
      string[] authentication = TraceKeywordSets.Authentication;
      if (!TeamFoundationTrace.IsTracing(authentication, traceLevel))
        return;
      switch (traceLevel)
      {
        case TraceLevel.Off:
          break;
        case TraceLevel.Error:
          TeamFoundationTrace.Error(authentication, format, args);
          break;
        case TraceLevel.Warning:
          TeamFoundationTrace.Warning(authentication, format, args);
          break;
        case TraceLevel.Info:
          TeamFoundationTrace.Info(authentication, format, args);
          break;
        default:
          TeamFoundationTrace.Verbose(authentication, format, args);
          break;
      }
    }

    public int ReadBatchSizeLimit => this.m_readBatchSizeLimit;

    private void MapToWellKnownIdentifiers(TeamFoundationIdentity identity)
    {
      if (identity == null)
        return;
      identity.Descriptor = this.m_hostDomain.MapToWellKnownIdentifier(identity.Descriptor);
      IdentityDescriptor[] identityDescriptorArray1 = new IdentityDescriptor[identity.Members.Count];
      int index1 = 0;
      foreach (IdentityDescriptor member in (IEnumerable<IdentityDescriptor>) identity.Members)
      {
        identityDescriptorArray1[index1] = this.m_hostDomain.MapToWellKnownIdentifier(member);
        ++index1;
      }
      identity.Members = (ICollection<IdentityDescriptor>) identityDescriptorArray1;
      IdentityDescriptor[] identityDescriptorArray2 = new IdentityDescriptor[identity.MemberOf.Count];
      int index2 = 0;
      foreach (IdentityDescriptor descriptor in (IEnumerable<IdentityDescriptor>) identity.MemberOf)
      {
        identityDescriptorArray2[index2] = this.m_hostDomain.MapToWellKnownIdentifier(descriptor);
        ++index2;
      }
      identity.MemberOf = (ICollection<IdentityDescriptor>) identityDescriptorArray2;
    }

    private TeamFoundationIdentity ReadIdentityFromRequestContext(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity != null)
      {
        if (IdentityDescriptorComparer.Instance.Equals(descriptor, userIdentity.Descriptor))
          return IdentityUtil.Convert(userIdentity);
        TeamFoundationTrace.Info("ReadIdentityFromRequestContext: requestContext.UserIdentity did not match the requested descriptor. An IncludeReadFromSource call we can get rid of?");
      }
      else
        TeamFoundationTrace.Info("ReadIdentityFromRequestContext: requestContext.UserIdentity is not set. Returning null.");
      return (TeamFoundationIdentity) null;
    }

    private void CheckGenericReadPermission(IVssRequestContext requestContext) => requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);

    internal string GetSecurableToken(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      out TeamFoundationIdentity group)
    {
      string securableToken;
      try
      {
        group = this.GetApplicationGroup(requestContext, groupDescriptor);
        return this.GetSecurableToken(requestContext, group);
      }
      catch (FindGroupSidDoesNotExistException ex)
      {
        TeamFoundationTrace.TraceException((Exception) ex);
        securableToken = (string) null;
        group = (TeamFoundationIdentity) null;
      }
      return securableToken;
    }

    internal string GetSecurableToken(
      IVssRequestContext requestContext,
      TeamFoundationIdentity group)
    {
      if (requestContext.IsSystemContext || group == null || !(this.GetSecuringHostId(requestContext, group) != requestContext.ServiceHost.InstanceId))
        return IdentityUtil.CreateSecurityToken(group);
      if (IdentityValidation.IsTeamFoundationType(group.Descriptor))
        throw new IdentityDomainMismatchException(this.Domain.Name, string.Empty);
      throw new NotApplicationGroupException();
    }

    private Guid GetSecuringHostId(IVssRequestContext requestContext, TeamFoundationIdentity group)
    {
      if (this.m_hostDomain.IsOwner(group.Descriptor))
        return requestContext.ServiceHost.InstanceId;
      return !string.Equals(group.GetAttribute("ScopeType", "Generic"), "ServiceHost", StringComparison.OrdinalIgnoreCase) ? requestContext.ServiceHost.InstanceId : Guid.Empty;
    }

    private sealed class IdentityPropertyHelper : Microsoft.VisualStudio.Services.Identity.IdentityPropertyHelper<TeamFoundationIdentity>
    {
      protected override Guid GetArtifactId(
        IdentityPropertyScope propertyScope,
        TeamFoundationIdentity identity)
      {
        return propertyScope == IdentityPropertyScope.Global ? identity.MasterId : identity.TeamFoundationId;
      }

      protected override void SetProperty(
        IdentityPropertyScope propertyScope,
        TeamFoundationIdentity identity,
        string propertyName,
        object propertyValue)
      {
        identity.InitializeProperty(propertyScope, propertyName, propertyValue);
      }

      protected override object GetProperty(
        IdentityPropertyScope propertyScope,
        TeamFoundationIdentity identity,
        string propertyName)
      {
        return identity.GetProperty(propertyScope, propertyName);
      }

      protected override HashSet<string> GetModifiedProperties(
        IdentityPropertyScope propertyScope,
        TeamFoundationIdentity identity)
      {
        return identity.GetModifiedPropertiesLog(propertyScope);
      }

      protected override void ResetModifiedProperties(
        IdentityPropertyScope propertyScope,
        TeamFoundationIdentity identity)
      {
        identity.ResetModifiedProperties();
      }

      internal override IEnumerable<TeamFoundationIdentity> UpdateHostSpecificExtendedProperties(
        IVssRequestContext requestContext,
        IdentityPropertyScope propertyScope,
        IEnumerable<TeamFoundationIdentity> identities,
        Func<IVssRequestContext, TeamFoundationIdentity, IdentityPropertyScope, string, bool> skipPropertyUpdate = null)
      {
        IEnumerable<TeamFoundationIdentity> foundationIdentities = base.UpdateHostSpecificExtendedProperties(requestContext, propertyScope, identities);
        List<TeamFoundationIdentity> list = identities.Select<TeamFoundationIdentity, TeamFoundationIdentity>((Func<TeamFoundationIdentity, TeamFoundationIdentity>) (x => new TeamFoundationIdentity(x))).ToList<TeamFoundationIdentity>();
        requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) new TeamFoundationIdentityPropertiesUpdateEvent((IEnumerable<TeamFoundationIdentity>) list, propertyScope));
        return foundationIdentities;
      }
    }
  }
}
