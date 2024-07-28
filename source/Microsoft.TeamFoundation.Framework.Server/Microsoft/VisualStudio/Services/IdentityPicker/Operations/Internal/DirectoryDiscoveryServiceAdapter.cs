// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.DirectoryDiscoveryServiceAdapter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DiscoveryService;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.SearchFilter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  internal sealed class DirectoryDiscoveryServiceAdapter : IdentityProvider
  {
    private static readonly IEnumerable<string> s_DefaultIdentityProperties = (IEnumerable<string>) new List<string>()
    {
      "Domain",
      "http://schemas.microsoft.com/identity/claims/objectidentifier"
    };
    private static readonly IEnumerable<string> s_NonPublicIdentityProperties = (IEnumerable<string>) new List<string>()
    {
      "SignInAddress",
      "SamAccountName",
      "Department",
      "JobTitle",
      "Mail",
      "MailNickname",
      "PhysicalDeliveryOfficeName",
      "Surname",
      "TelephoneNumber",
      "Description"
    };

    internal override IDictionary<string, QueryTokenResult> GetIdentities(
      IdentityProviderAdapterGetRequest ipdGetRequest)
    {
      if (!(ipdGetRequest is IdentityProviderAdapterDdsGetRequest getRequest))
        throw new NullReferenceException("DirectoryDiscoveryServiceAdapter.GetIdentities::IdentityProviderAdapterGetRequest");
      try
      {
        Tracing.TraceEnter(ipdGetRequest.RequestContext, 801, "DirectoryDiscoveryServiceAdapter.GetIdentities");
        if (!string.IsNullOrEmpty(getRequest.Prefix))
          return DirectoryDiscoveryServiceAdapter.GetIdentitiesByPrefix(getRequest);
        if (getRequest.Emails != null && getRequest.Emails.Count > 0)
          return DirectoryDiscoveryServiceAdapter.GetIdentitiesByEmail(getRequest);
        if (getRequest.DomainSamAccountNames != null && getRequest.DomainSamAccountNames.Count > 0)
          return DirectoryDiscoveryServiceAdapter.GetIdentitiesByDomainSamAccountName(getRequest);
        if (getRequest.EntityIds != null && getRequest.EntityIds.Count > 0)
          return DirectoryDiscoveryServiceAdapter.GetIdentitiesByEntityIds(getRequest);
        if (getRequest.SubjectDescriptors != null && getRequest.SubjectDescriptors.Count > 0)
          return this.GetIdentitiesBySubjectDescriptors(getRequest);
        if (getRequest.DirectoryIds != null && getRequest.DirectoryIds.Count > 0)
          return DirectoryDiscoveryServiceAdapter.GetIdentitiesByDirectoryIds(getRequest);
        if (getRequest.AccountNames != null)
        {
          if (getRequest.AccountNames.Count > 0)
            return this.GetIdentitiesByAccountNames(getRequest);
        }
      }
      catch (AggregateException ex)
      {
        foreach (Exception innerException in ex.InnerExceptions)
          Tracing.TraceException(ipdGetRequest.RequestContext, 807, innerException);
        throw;
      }
      catch (CircuitBreakerException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        if (!(ex is IdentityPickerException))
        {
          DirectoryDiscoveryServiceAdapter.HandleDirectoryDiscoveryServiceException("DirectoryDiscoveryServiceAdapter.GetIdentities", ex);
          throw new IdentityPickerAdapterException("DirectoryDiscoveryServiceAdapter:" + ex.Message, ex);
        }
        throw;
      }
      finally
      {
        Tracing.TraceLeave(ipdGetRequest.RequestContext, 809, "DirectoryDiscoveryServiceAdapter.GetIdentities");
      }
      throw new IdentityPickerArgumentException("Insufficient data for DirectoryDiscoveryServiceAdapter.GetIdentities");
    }

    internal static IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity> ResolveVsidsOrSubjectDescriptorsToIdentities(
      IVssRequestContext requestContext,
      IList<Guid> vsIds,
      List<SubjectDescriptor> subjectDescriptors,
      IEnumerable<string> requestProperties,
      IDictionary<string, object> presetProperties = null,
      IEnumerable<string> filterByAncestorEntityIds = null,
      IEnumerable<string> filterByEntityIds = null,
      int? maxItemsCount = null)
    {
      bool flag = vsIds != null && vsIds.Count != 0;
      // ISSUE: explicit non-virtual call
      if (!flag && (subjectDescriptors == null || __nonvirtual (subjectDescriptors.Count) == 0))
        return (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) new List<Microsoft.VisualStudio.Services.IdentityPicker.Identity>();
      IdentityService service = requestContext.GetService<IdentityService>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> imsIdentities = !flag ? service.ReadIdentities(requestContext, (IList<SubjectDescriptor>) subjectDescriptors, QueryMembership.None, DirectoryDiscoveryServiceAdapter.s_DefaultIdentityProperties) : service.ReadIdentities(requestContext, vsIds, QueryMembership.None, DirectoryDiscoveryServiceAdapter.s_DefaultIdentityProperties);
      return DirectoryDiscoveryServiceAdapter.TransformAndFilterIdentities(requestContext, imsIdentities, requestProperties, presetProperties, filterByAncestorEntityIds, filterByEntityIds, maxItemsCount);
    }

    private static IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity> TransformAndFilterIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> imsIdentities,
      IEnumerable<string> requestProperties,
      IDictionary<string, object> presetProperties,
      IEnumerable<string> filterByAncestorEntityIds,
      IEnumerable<string> filterByEntityIds,
      int? maxItemsCount = null)
    {
      if (imsIdentities.IsNullOrEmpty<Microsoft.VisualStudio.Services.Identity.Identity>())
        return (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) new List<Microsoft.VisualStudio.Services.IdentityPicker.Identity>();
      if (filterByAncestorEntityIds.IsNullOrEmpty<string>() && filterByEntityIds.IsNullOrEmpty<string>() || requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableIdentitySearchFilters"))
        return DirectoryDiscoveryServiceAdapter.TransformImsToIpsIdentities(requestContext, imsIdentities, requestProperties, presetProperties, maxItemsCount);
      IEnumerable<IdentityDescriptor> identityDescriptors1 = VisualStudioDirectoryHelper.GetIdentityDescriptors(requestContext, filterByAncestorEntityIds, nameof (filterByAncestorEntityIds));
      IEnumerable<IdentityDescriptor> identityDescriptors2 = VisualStudioDirectoryHelper.GetIdentityDescriptors(requestContext, filterByEntityIds, nameof (filterByEntityIds));
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = new AncestorAndIdentitySearchFilter(requestContext, (Microsoft.VisualStudio.Services.Identity.SearchFilter.IIdentityProvider) IdentityServiceProvider.Instance, identityDescriptors1, identityDescriptors2).FilterIdentities<Microsoft.VisualStudio.Services.Identity.Identity>(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) imsIdentities, (Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (identity => identity.Descriptor)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      return DirectoryDiscoveryServiceAdapter.TransformImsToIpsIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) list, requestProperties, presetProperties, maxItemsCount);
    }

    private static IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity> TransformImsToIpsIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> imsIdentities,
      IEnumerable<string> requestProperties,
      IDictionary<string, object> presetProperties = null,
      int? maxItemsCount = null)
    {
      IEnumerable<Microsoft.VisualStudio.Services.IdentityPicker.Identity> source = imsIdentities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, IDirectoryEntity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IDirectoryEntity>) (x => VisualStudioDirectoryEntityConverter.ConvertIdentity(requestContext, x, requestProperties))).Select<IDirectoryEntity, Microsoft.VisualStudio.Services.IdentityPicker.Identity>((Func<IDirectoryEntity, Microsoft.VisualStudio.Services.IdentityPicker.Identity>) (x => IdentityFactory.Create((DirectoryEntity) x, requestProperties, requestContext, (IDictionary<string, object>) null, presetProperties)));
      if (maxItemsCount.HasValue)
      {
        int count = maxItemsCount.Value;
        source = count > 0 ? source.Take<Microsoft.VisualStudio.Services.IdentityPicker.Identity>(count) : throw new IdentityPickerAdapterException(string.Format("maxItemsCount value must be more than zero. Received value: {0}", (object) count));
      }
      return (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) source.ToList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>();
    }

    private static IDictionary<string, QueryTokenResult> GetIdentitiesByEmail(
      IdentityProviderAdapterDdsGetRequest getRequest)
    {
      IVssRequestContext requestContext = getRequest.RequestContext;
      IdentityOperationHelper.CheckRequestByAuthorizedMember(requestContext);
      IdentityTypeEnum identityType = getRequest.IdentityType;
      OperationScopeEnum operationScope = getRequest.OperationScope;
      IList<string> requestProperties = getRequest.RequestProperties;
      Dictionary<string, object> options = getRequest.Options;
      IList<string> emails = getRequest.Emails;
      string ddsPagingToken = getRequest.DdsPagingToken;
      if (emails == null || emails.Count == 0)
        throw new IdentityPickerArgumentException("Search emails cannot be null or empty (GetIdentitiesByEmail)");
      Dictionary<string, QueryTokenResult> resultBuilder1 = new Dictionary<string, QueryTokenResult>();
      string[] other1 = new string[2]
      {
        "SignInAddress",
        "Mail"
      };
      string[] other2 = new string[1]{ "Mail" };
      HashSet<string> stringSet = new HashSet<string>();
      List<string> directoryEntityTypes = DirectoryDiscoveryServiceAdapter.GetDirectoryEntityTypes(identityType);
      List<string> directories = DirectoryDiscoveryServiceAdapter.GetDirectories(operationScope, requestContext);
      if (directoryEntityTypes.Contains("User"))
        stringSet.UnionWith((IEnumerable<string>) other1);
      if (directoryEntityTypes.Contains("Group"))
        stringSet.UnionWith((IEnumerable<string>) other2);
      IEnumerable<string> ancestorEntityIds = DirectoryDiscoveryServiceAdapter.GetFilterByAncestorEntityIds(getRequest.FilterByAncestorEntityIds);
      IEnumerable<string> filterByEntityIds = DirectoryDiscoveryServiceAdapter.GetFilterByEntityIds(getRequest.FilterByEntityIds);
      DirectoryDiscoveryService service = requestContext.GetService<DirectoryDiscoveryService>();
      bool flag1 = emails.Count == 1;
      if (directories.Count > 0)
      {
        foreach (string y in (IEnumerable<string>) emails)
        {
          if (!string.IsNullOrEmpty(y))
          {
            DirectoryDiscoveryService discoveryService = service;
            IVssRequestContext context = requestContext;
            DirectorySearchRequest request = new DirectorySearchRequest();
            request.Directories = (IEnumerable<string>) directories;
            request.Query = y;
            request.TypesToSearch = (IEnumerable<string>) directoryEntityTypes;
            request.FilterByAncestorEntityIds = ancestorEntityIds;
            request.FilterByEntityIds = filterByEntityIds;
            request.PropertiesToSearch = (IEnumerable<string>) stringSet;
            request.PropertiesToReturn = (IEnumerable<string>) requestProperties;
            request.PagingToken = flag1 ? ddsPagingToken : string.Empty;
            request.MaxResults = new int?(options.ContainsKey("MaxResults") ? (int) options["MaxResults"] : 20);
            request.MinResults = new int?(options.ContainsKey("MinResults") ? (int) options["MinResults"] : 20);
            request.QueryType = QueryType.LookUp;
            request.ScopeId = DirectoryDiscoveryServiceAdapter.extractScopeId((IDictionary<string, object>) options);
            DirectorySearchResponse directorySearchResponse = discoveryService.Search(context, request);
            flag1 = false;
            IList<IDirectoryEntity> source = directorySearchResponse != null ? directorySearchResponse.Entities : throw new IdentityPickerAdapterException("Directory Discovery Service returned null objects (GetIdentitiesByEmail)");
            string str = directorySearchResponse.HasMoreResults ? directorySearchResponse.PagingToken : string.Empty;
            bool guestUserAccessEnabled = IdentityOperationHelper.IsExternalGuestAccessPolicyEnabled(requestContext);
            if (source != null && source.Count > 0)
            {
              foreach (Microsoft.VisualStudio.Services.IdentityPicker.Identity identity in source.Where<IDirectoryEntity>((Func<IDirectoryEntity, bool>) (identity =>
              {
                if (!(identity is DirectoryUser))
                  return true;
                bool? guest = ((DirectoryUser) identity).Guest;
                bool flag2 = true;
                return !(guest.GetValueOrDefault() == flag2 & guest.HasValue) | guestUserAccessEnabled;
              })).Select<IDirectoryEntity, Microsoft.VisualStudio.Services.IdentityPicker.Identity>((Func<IDirectoryEntity, Microsoft.VisualStudio.Services.IdentityPicker.Identity>) (identity => IdentityFactory.Create((DirectoryEntity) identity, (IEnumerable<string>) requestProperties, requestContext, (IDictionary<string, object>) options))).Where<Microsoft.VisualStudio.Services.IdentityPicker.Identity>((Func<Microsoft.VisualStudio.Services.IdentityPicker.Identity, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>())
              {
                if (VssStringComparer.MailAddress.Equals(identity.SignInAddress, y))
                {
                  Dictionary<string, QueryTokenResult> resultBuilder2 = resultBuilder1;
                  string query = y;
                  List<Microsoft.VisualStudio.Services.IdentityPicker.Identity> identities = new List<Microsoft.VisualStudio.Services.IdentityPicker.Identity>();
                  identities.Add(identity);
                  Dictionary<string, object> optionalProperties;
                  if (string.IsNullOrEmpty(str))
                  {
                    optionalProperties = (Dictionary<string, object>) null;
                  }
                  else
                  {
                    optionalProperties = new Dictionary<string, object>();
                    optionalProperties.Add(QueryTokenResultProperties.PagingToken, (object) str);
                  }
                  QueryTokenResult newQtr = new QueryTokenResult(query, (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) identities, (IDictionary<string, object>) optionalProperties);
                  IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) resultBuilder2, newQtr, true);
                  break;
                }
                if (VssStringComparer.MailAddress.Equals(identity.Mail, y))
                {
                  Dictionary<string, QueryTokenResult> resultBuilder3 = resultBuilder1;
                  string query = y;
                  List<Microsoft.VisualStudio.Services.IdentityPicker.Identity> identities = new List<Microsoft.VisualStudio.Services.IdentityPicker.Identity>();
                  identities.Add(identity);
                  Dictionary<string, object> optionalProperties;
                  if (string.IsNullOrEmpty(str))
                  {
                    optionalProperties = (Dictionary<string, object>) null;
                  }
                  else
                  {
                    optionalProperties = new Dictionary<string, object>();
                    optionalProperties.Add(QueryTokenResultProperties.PagingToken, (object) str);
                  }
                  QueryTokenResult newQtr = new QueryTokenResult(query, (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) identities, (IDictionary<string, object>) optionalProperties);
                  IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) resultBuilder3, newQtr, true);
                }
              }
            }
          }
        }
      }
      foreach (string str in (IEnumerable<string>) emails)
      {
        if (!resultBuilder1.ContainsKey(str))
          IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) resultBuilder1, new QueryTokenResult(str, (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) new List<Microsoft.VisualStudio.Services.IdentityPicker.Identity>()));
      }
      return (IDictionary<string, QueryTokenResult>) resultBuilder1;
    }

    private static Guid extractScopeId(IDictionary<string, object> options)
    {
      object obj;
      return options.TryGetValue("ScopeId", out obj) && obj is Guid guid ? guid : new Guid();
    }

    private static IDictionary<string, QueryTokenResult> GetIdentitiesByDomainSamAccountName(
      IdentityProviderAdapterDdsGetRequest getRequest)
    {
      IVssRequestContext requestContext = getRequest.RequestContext;
      IdentityTypeEnum identityType = getRequest.IdentityType;
      OperationScopeEnum operationScope = getRequest.OperationScope;
      IList<string> requestProperties = getRequest.RequestProperties;
      Dictionary<string, object> options = getRequest.Options;
      IList<string> domainSamAccountNames = getRequest.DomainSamAccountNames;
      string ddsPagingToken = getRequest.DdsPagingToken;
      if (domainSamAccountNames == null || domainSamAccountNames.Count == 0)
        throw new IdentityPickerArgumentException("Search Domain\\SamAccountName list cannot be null or empty (GetIdentitiesByDomainSamAccountName)");
      Dictionary<string, QueryTokenResult> resultBuilder1 = new Dictionary<string, QueryTokenResult>();
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) new string[1]
      {
        "SamAccountName"
      });
      List<string> directoryEntityTypes = DirectoryDiscoveryServiceAdapter.GetDirectoryEntityTypes(identityType);
      List<string> directories = DirectoryDiscoveryServiceAdapter.GetDirectories(operationScope, requestContext);
      IEnumerable<string> ancestorEntityIds = DirectoryDiscoveryServiceAdapter.GetFilterByAncestorEntityIds(getRequest.FilterByAncestorEntityIds);
      IEnumerable<string> filterByEntityIds = DirectoryDiscoveryServiceAdapter.GetFilterByEntityIds(getRequest.FilterByEntityIds);
      DirectoryDiscoveryService service = requestContext.GetService<DirectoryDiscoveryService>();
      bool flag = domainSamAccountNames.Count == 1;
      if (directories.Count > 0)
      {
        foreach (string input in (IEnumerable<string>) domainSamAccountNames)
        {
          if (!string.IsNullOrEmpty(input))
          {
            Match match = IdentityOperationHelper.DomainSamAccountNameRegex.Match(input);
            if (match.Success)
            {
              string y1 = match.Groups["domain"].Value;
              string y2 = match.Groups["samAccountNameGroup"].Value;
              DirectoryDiscoveryService discoveryService = service;
              IVssRequestContext context = requestContext;
              DirectorySearchRequest request = new DirectorySearchRequest();
              request.Directories = (IEnumerable<string>) directories;
              request.Query = input;
              request.TypesToSearch = (IEnumerable<string>) directoryEntityTypes;
              request.FilterByAncestorEntityIds = ancestorEntityIds;
              request.FilterByEntityIds = filterByEntityIds;
              request.PropertiesToSearch = (IEnumerable<string>) stringSet;
              request.PropertiesToReturn = (IEnumerable<string>) requestProperties;
              request.PagingToken = flag ? ddsPagingToken : string.Empty;
              request.MaxResults = new int?(options.ContainsKey("MaxResults") ? (int) options["MaxResults"] : 20);
              request.MinResults = new int?(options.ContainsKey("MinResults") ? (int) options["MinResults"] : 20);
              request.QueryType = QueryType.LookUp;
              request.ScopeId = DirectoryDiscoveryServiceAdapter.extractScopeId((IDictionary<string, object>) options);
              DirectorySearchResponse directorySearchResponse = discoveryService.Search(context, request);
              flag = false;
              IList<IDirectoryEntity> source = directorySearchResponse != null ? directorySearchResponse.Entities : throw new IdentityPickerAdapterException("Directory Discovery Service returned null objects (GetIdentitiesByDomainSamAccountName)");
              string str = directorySearchResponse.HasMoreResults ? directorySearchResponse.PagingToken : string.Empty;
              if (source != null && source.Count > 0)
              {
                foreach (Microsoft.VisualStudio.Services.IdentityPicker.Identity identity in source.Select<IDirectoryEntity, Microsoft.VisualStudio.Services.IdentityPicker.Identity>((Func<IDirectoryEntity, Microsoft.VisualStudio.Services.IdentityPicker.Identity>) (identity => IdentityFactory.Create((DirectoryEntity) identity, (IEnumerable<string>) requestProperties, requestContext, (IDictionary<string, object>) options))).Where<Microsoft.VisualStudio.Services.IdentityPicker.Identity>((Func<Microsoft.VisualStudio.Services.IdentityPicker.Identity, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>())
                {
                  if (identity.SamAccountName != null && VssStringComparer.SamAccountName.Equals(identity.SamAccountName, y2) && identity.ScopeName != null && VssStringComparer.DomainName.Equals(identity.ScopeName, y1))
                  {
                    Dictionary<string, QueryTokenResult> resultBuilder2 = resultBuilder1;
                    string query = input;
                    List<Microsoft.VisualStudio.Services.IdentityPicker.Identity> identities = new List<Microsoft.VisualStudio.Services.IdentityPicker.Identity>();
                    identities.Add(identity);
                    Dictionary<string, object> optionalProperties;
                    if (string.IsNullOrEmpty(str))
                    {
                      optionalProperties = (Dictionary<string, object>) null;
                    }
                    else
                    {
                      optionalProperties = new Dictionary<string, object>();
                      optionalProperties.Add(QueryTokenResultProperties.PagingToken, (object) str);
                    }
                    QueryTokenResult newQtr = new QueryTokenResult(query, (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) identities, (IDictionary<string, object>) optionalProperties);
                    IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) resultBuilder2, newQtr, true);
                    break;
                  }
                }
              }
            }
          }
        }
      }
      foreach (string str in (IEnumerable<string>) domainSamAccountNames)
      {
        if (!resultBuilder1.ContainsKey(str))
          IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) resultBuilder1, new QueryTokenResult(str, (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) new List<Microsoft.VisualStudio.Services.IdentityPicker.Identity>()));
      }
      return (IDictionary<string, QueryTokenResult>) resultBuilder1;
    }

    private static IDictionary<string, QueryTokenResult> GetIdentitiesByPrefix(
      IdentityProviderAdapterDdsGetRequest getRequest)
    {
      IVssRequestContext requestContext = getRequest.RequestContext;
      IdentityOperationHelper.CheckRequestByAuthorizedMember(requestContext);
      IdentityTypeEnum identityType = getRequest.IdentityType;
      OperationScopeEnum operationScope = getRequest.OperationScope;
      IList<string> requestProperties = getRequest.RequestProperties;
      Dictionary<string, object> options = getRequest.Options;
      string prefix = getRequest.Prefix;
      string ddsPagingToken = getRequest.DdsPagingToken;
      Dictionary<string, QueryTokenResult> identitiesByPrefix = new Dictionary<string, QueryTokenResult>();
      string[] first = new string[4]
      {
        "DisplayName",
        "Surname",
        "Mail",
        "MailNickname"
      };
      string[] second1 = new string[1]{ "SignInAddress" };
      string[] second2 = new string[1]{ "SamAccountName" };
      string[] strArray = new string[2]
      {
        "DisplayName",
        "MailNickname"
      };
      string[] other = new string[6]
      {
        "DisplayName",
        "AppId",
        "Mail",
        "SamAccountName",
        "SignInAddress",
        "PrincipalName"
      };
      string[] array;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        array = ((IEnumerable<string>) first).Union<string>((IEnumerable<string>) second1).ToArray<string>();
      }
      else
      {
        array = ((IEnumerable<string>) first).Union<string>((IEnumerable<string>) second2).ToArray<string>();
        strArray = ((IEnumerable<string>) strArray).Union<string>((IEnumerable<string>) second2).ToArray<string>();
      }
      List<string> directoryEntityTypes = DirectoryDiscoveryServiceAdapter.GetDirectoryEntityTypes(identityType);
      List<string> directories = DirectoryDiscoveryServiceAdapter.GetDirectories(operationScope, requestContext);
      IEnumerable<string> ancestorEntityIds = DirectoryDiscoveryServiceAdapter.GetFilterByAncestorEntityIds(getRequest.FilterByAncestorEntityIds);
      IEnumerable<string> filterByEntityIds = DirectoryDiscoveryServiceAdapter.GetFilterByEntityIds(getRequest.FilterByEntityIds);
      HashSet<string> stringSet = new HashSet<string>();
      if (directoryEntityTypes.Contains("User"))
        stringSet.UnionWith((IEnumerable<string>) array);
      if (directoryEntityTypes.Contains("Group"))
        stringSet.UnionWith((IEnumerable<string>) strArray);
      if (directoryEntityTypes.Contains("ServicePrincipal"))
        stringSet.UnionWith((IEnumerable<string>) other);
      DirectoryDiscoveryService service = requestContext.GetService<DirectoryDiscoveryService>();
      IVssRequestContext context = requestContext;
      DirectorySearchRequest request = new DirectorySearchRequest();
      request.Directories = (IEnumerable<string>) directories;
      request.Query = prefix;
      request.TypesToSearch = (IEnumerable<string>) directoryEntityTypes;
      request.FilterByAncestorEntityIds = ancestorEntityIds;
      request.FilterByEntityIds = filterByEntityIds;
      request.PropertiesToSearch = (IEnumerable<string>) stringSet;
      request.PagingToken = ddsPagingToken;
      request.PropertiesToReturn = (IEnumerable<string>) requestProperties;
      request.MaxResults = new int?(options.ContainsKey("MaxResults") ? (int) options["MaxResults"] : 20);
      request.MinResults = new int?(options.ContainsKey("MinResults") ? (int) options["MinResults"] : 20);
      request.ScopeId = DirectoryDiscoveryServiceAdapter.extractScopeId((IDictionary<string, object>) options);
      DirectorySearchResponse response = service.Search(context, request);
      if (response == null)
        throw new IdentityPickerAdapterException("Directory Discovery Service returned null objects (GetIdentitiesByPrefix)");
      List<Microsoft.VisualStudio.Services.IdentityPicker.Identity> directoryIdentities = DirectoryDiscoveryServiceAdapter.ConvertToDirectoryIdentities(response, requestContext, requestProperties, options);
      Dictionary<string, QueryTokenResult> resultBuilder = identitiesByPrefix;
      string query = prefix;
      List<Microsoft.VisualStudio.Services.IdentityPicker.Identity> identities = directoryIdentities;
      Dictionary<string, object> optionalProperties;
      if (!response.HasMoreResults)
      {
        optionalProperties = (Dictionary<string, object>) null;
      }
      else
      {
        optionalProperties = new Dictionary<string, object>();
        optionalProperties.Add(QueryTokenResultProperties.PagingToken, (object) response.PagingToken);
      }
      QueryTokenResult newQtr = new QueryTokenResult(query, (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) identities, (IDictionary<string, object>) optionalProperties);
      IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) resultBuilder, newQtr);
      return (IDictionary<string, QueryTokenResult>) identitiesByPrefix;
    }

    private static List<Microsoft.VisualStudio.Services.IdentityPicker.Identity> ConvertToDirectoryIdentities(
      DirectorySearchResponse response,
      IVssRequestContext requestContext,
      IList<string> requestProperties,
      Dictionary<string, object> options)
    {
      return response.Entities.Where<IDirectoryEntity>((Func<IDirectoryEntity, bool>) (identity => !DirectoryDiscoveryServiceAdapter.IsGuestUserIfExternalGuestAccessPolicyIsDisabled(requestContext, identity))).Select<IDirectoryEntity, Microsoft.VisualStudio.Services.IdentityPicker.Identity>((Func<IDirectoryEntity, Microsoft.VisualStudio.Services.IdentityPicker.Identity>) (identity => IdentityFactory.Create((DirectoryEntity) identity, (IEnumerable<string>) requestProperties, requestContext, (IDictionary<string, object>) options))).Where<Microsoft.VisualStudio.Services.IdentityPicker.Identity>((Func<Microsoft.VisualStudio.Services.IdentityPicker.Identity, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>();
    }

    private static bool IsGuestUserIfExternalGuestAccessPolicyIsDisabled(
      IVssRequestContext requestContext,
      IDirectoryEntity identity)
    {
      if (identity is DirectoryUser directoryUser)
      {
        bool? guest = directoryUser.Guest;
        bool flag = true;
        if (guest.GetValueOrDefault() == flag & guest.HasValue)
          return !IdentityOperationHelper.IsExternalGuestAccessPolicyEnabled(requestContext);
      }
      return false;
    }

    private static void RemovePrivateProperties(Microsoft.VisualStudio.Services.IdentityPicker.Identity identity)
    {
      if (identity == null)
        return;
      foreach (string identityProperty in DirectoryDiscoveryServiceAdapter.s_NonPublicIdentityProperties)
        identity[identityProperty] = (object) null;
    }

    private static IDictionary<string, QueryTokenResult> GetIdentitiesByEntityIds(
      IdentityProviderAdapterDdsGetRequest getRequest)
    {
      IVssRequestContext requestContext1 = getRequest.RequestContext;
      int operationScope = (int) getRequest.OperationScope;
      IList<string> requestProperties = getRequest.RequestProperties;
      Dictionary<string, object> options = getRequest.Options;
      Dictionary<string, object> presetProperties = getRequest.PresetProperties;
      IList<string> entityIds = getRequest.EntityIds;
      Dictionary<string, QueryTokenResult> dictionary1 = new Dictionary<string, QueryTokenResult>();
      IVssRequestContext requestContext2 = requestContext1;
      List<string> directories = DirectoryDiscoveryServiceAdapter.GetDirectories((OperationScopeEnum) operationScope, requestContext2);
      DirectoryDiscoveryService service = requestContext1.GetService<DirectoryDiscoveryService>();
      IVssRequestContext context = requestContext1;
      DirectoryGetEntitiesRequest request = new DirectoryGetEntitiesRequest();
      request.Directories = (IEnumerable<string>) directories;
      request.PropertiesToReturn = (IEnumerable<string>) requestProperties;
      request.EntityIds = (IEnumerable<string>) entityIds;
      DirectoryGetEntitiesResponse entities = service.GetEntities(context, request);
      if (entities == null)
        throw new IdentityPickerAdapterException("Directory Discovery Service returned null objects (GetIdentitiesByEntityIds)");
      bool guestUserAccessEnabled = IdentityOperationHelper.IsExternalGuestAccessPolicyEnabled(requestContext1);
      Dictionary<string, DirectoryGetEntityResult> dictionary2 = entities.Results.Where<KeyValuePair<string, DirectoryGetEntityResult>>((Func<KeyValuePair<string, DirectoryGetEntityResult>, bool>) (kvp =>
      {
        if (kvp.Value == null || kvp.Value.Entity == null || kvp.Value.Exception != null)
          return false;
        if (!(kvp.Value.Entity is DirectoryUser))
          return true;
        bool? guest = ((DirectoryUser) kvp.Value.Entity).Guest;
        bool flag = true;
        return !(guest.GetValueOrDefault() == flag & guest.HasValue) | guestUserAccessEnabled;
      })).ToDictionary<KeyValuePair<string, DirectoryGetEntityResult>, string, DirectoryGetEntityResult>((Func<KeyValuePair<string, DirectoryGetEntityResult>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, DirectoryGetEntityResult>, DirectoryGetEntityResult>) (kvp => kvp.Value));
      Dictionary<string, QueryTokenResult> identitiesByEntityIds = new Dictionary<string, QueryTokenResult>();
      bool flag1 = DirectoryDiscoveryServiceAdapter.RequestedByAuthorizedMember(requestContext1);
      foreach (string str in (IEnumerable<string>) entityIds)
      {
        DirectoryGetEntityResult directoryGetEntityResult;
        if (dictionary2.TryGetValue(str, out directoryGetEntityResult) && directoryGetEntityResult != null && directoryGetEntityResult.Exception == null)
        {
          Microsoft.VisualStudio.Services.IdentityPicker.Identity identity = IdentityFactory.Create((DirectoryEntity) directoryGetEntityResult.Entity, (IEnumerable<string>) requestProperties, requestContext1, (IDictionary<string, object>) options, (IDictionary<string, object>) presetProperties);
          if (identity != null)
          {
            if (!flag1)
              DirectoryDiscoveryServiceAdapter.RemovePrivateProperties(identity);
            identitiesByEntityIds.Add(str, new QueryTokenResult(str, (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) new List<Microsoft.VisualStudio.Services.IdentityPicker.Identity>()
            {
              identity
            }));
          }
        }
      }
      return (IDictionary<string, QueryTokenResult>) identitiesByEntityIds;
    }

    private IDictionary<string, QueryTokenResult> GetIdentitiesBySubjectDescriptors(
      IdentityProviderAdapterDdsGetRequest getRequest)
    {
      IVssRequestContext requestContext = getRequest.RequestContext;
      List<string> directories = DirectoryDiscoveryServiceAdapter.GetDirectories(getRequest.OperationScope, requestContext);
      if (DirectoryDiscoveryServiceAdapter.IsImsOnlySearchRequested(requestContext, (IList<string>) directories))
        return DirectoryDiscoveryServiceAdapter.GetIdentitiesBySubjectDescriptorsInIms(getRequest);
      DirectoryInternalConvertKeysRequest convertKeysRequest = new DirectoryInternalConvertKeysRequest();
      convertKeysRequest.ConvertFrom = "SubjectDescriptor";
      convertKeysRequest.ConvertTo = "DirectoryEntityIdentifier";
      convertKeysRequest.Directories = (IEnumerable<string>) new string[1]
      {
        "vsd"
      };
      convertKeysRequest.Keys = getRequest.SubjectDescriptors.ToHashSet<SubjectDescriptor>().Select<SubjectDescriptor, string>((Func<SubjectDescriptor, string>) (x => x.ToString()));
      DirectoryInternalConvertKeysRequest request = convertKeysRequest;
      IDictionary<string, DirectoryInternalConvertKeyResult> results = VisualStudioDirectoryConvertKeysHelper.ConvertKeys(requestContext, request).Results;
      IEnumerable<string> enumerable = results != null ? results.Where<KeyValuePair<string, DirectoryInternalConvertKeyResult>>((Func<KeyValuePair<string, DirectoryInternalConvertKeyResult>, bool>) (x => string.IsNullOrWhiteSpace(x.Value?.Key) || x.Value.Exception != null)).Select<KeyValuePair<string, DirectoryInternalConvertKeyResult>, string>((Func<KeyValuePair<string, DirectoryInternalConvertKeyResult>, string>) (x => x.Key)) : (IEnumerable<string>) null;
      if (!enumerable.IsNullOrEmpty<string>())
        throw new IdentityPickerAdapterException("SubjectDescriptors: " + enumerable.Serialize<IEnumerable<string>>() + " are either invalid or not found");
      List<string> list = results != null ? results.Values.Select<DirectoryInternalConvertKeyResult, string>((Func<DirectoryInternalConvertKeyResult, string>) (x => x.Key)).ToList<string>() : (List<string>) null;
      if (list.IsNullOrEmpty<string>())
        return (IDictionary<string, QueryTokenResult>) new Dictionary<string, QueryTokenResult>();
      if (!getRequest.RequestProperties.Contains("SubjectDescriptor"))
        getRequest.RequestProperties.Add("SubjectDescriptor");
      IdentityProviderAdapterDdsGetRequest getRequest1 = new IdentityProviderAdapterDdsGetRequest();
      getRequest1.RequestContext = requestContext;
      getRequest1.OperationScope = getRequest.OperationScope;
      getRequest1.RequestProperties = (IList<string>) getRequest.RequestProperties.ToList<string>();
      getRequest1.Options = getRequest.Options;
      getRequest1.EntityIds = (IList<string>) list;
      getRequest1.DdsPagingToken = getRequest.DdsPagingToken;
      Dictionary<string, QueryTokenResult> subjectDescriptors = new Dictionary<string, QueryTokenResult>();
      foreach (KeyValuePair<string, QueryTokenResult> identitiesByEntityId in (IEnumerable<KeyValuePair<string, QueryTokenResult>>) DirectoryDiscoveryServiceAdapter.GetIdentitiesByEntityIds(getRequest1))
      {
        QueryTokenResult queryTokenResult = identitiesByEntityId.Value;
        Microsoft.VisualStudio.Services.IdentityPicker.Identity identity1;
        if (queryTokenResult == null)
        {
          identity1 = (Microsoft.VisualStudio.Services.IdentityPicker.Identity) null;
        }
        else
        {
          IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity> identities = queryTokenResult.Identities;
          identity1 = identities != null ? identities.FirstOrDefault<Microsoft.VisualStudio.Services.IdentityPicker.Identity>() : (Microsoft.VisualStudio.Services.IdentityPicker.Identity) null;
        }
        Microsoft.VisualStudio.Services.IdentityPicker.Identity identity2 = identity1;
        if (identity2 != null)
        {
          SubjectDescriptor? subjectDescriptor = identity2.SubjectDescriptor;
          if (subjectDescriptor.HasValue)
          {
            subjectDescriptor = identity2.SubjectDescriptor;
            string str = subjectDescriptor.ToString();
            subjectDescriptors[str] = new QueryTokenResult(str, (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) new Microsoft.VisualStudio.Services.IdentityPicker.Identity[1]
            {
              identity2
            });
          }
        }
      }
      return (IDictionary<string, QueryTokenResult>) subjectDescriptors;
    }

    private static IDictionary<string, QueryTokenResult> GetIdentitiesBySubjectDescriptorsInIms(
      IdentityProviderAdapterDdsGetRequest getRequest)
    {
      IVssRequestContext requestContext = getRequest.RequestContext;
      IList<string> requestProperties = getRequest.RequestProperties;
      Dictionary<string, QueryTokenResult> descriptorsInIms = new Dictionary<string, QueryTokenResult>();
      HashSet<SubjectDescriptor> source = new HashSet<SubjectDescriptor>((IEnumerable<SubjectDescriptor>) getRequest.SubjectDescriptors);
      IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity> identities = DirectoryDiscoveryServiceAdapter.ResolveVsidsOrSubjectDescriptorsToIdentities(requestContext, (IList<Guid>) null, source.ToList<SubjectDescriptor>(), (IEnumerable<string>) requestProperties);
      if (identities == null || identities.Count == 0)
        return (IDictionary<string, QueryTokenResult>) descriptorsInIms;
      foreach (Microsoft.VisualStudio.Services.IdentityPicker.Identity identity1 in (IEnumerable<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) identities)
      {
        if (identity1.SubjectDescriptor.HasValue && source.Contains(identity1.SubjectDescriptor.Value))
        {
          Microsoft.VisualStudio.Services.IdentityPicker.Identity identity2 = identity1;
          if (IdentityOperationHelper.IsRequestByNonMember(requestContext))
            DirectoryDiscoveryServiceAdapter.RemovePrivateProperties(identity2);
          descriptorsInIms.Add(identity2.SubjectDescriptor.ToString(), new QueryTokenResult(identity2.SubjectDescriptor.ToString(), (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) new List<Microsoft.VisualStudio.Services.IdentityPicker.Identity>()
          {
            identity2
          }));
        }
      }
      return (IDictionary<string, QueryTokenResult>) descriptorsInIms;
    }

    private static IDictionary<string, QueryTokenResult> GetIdentitiesByDirectoryIds(
      IdentityProviderAdapterDdsGetRequest getRequest)
    {
      IVssRequestContext requestContext1 = getRequest.RequestContext;
      int identityType = (int) getRequest.IdentityType;
      int operationScope = (int) getRequest.OperationScope;
      IList<string> requestProperties = getRequest.RequestProperties;
      Dictionary<string, object> options = getRequest.Options;
      Dictionary<string, object> presetProperties = getRequest.PresetProperties;
      Dictionary<string, QueryTokenResult> identitiesByDirectoryIds = new Dictionary<string, QueryTokenResult>();
      IVssRequestContext requestContext2 = requestContext1;
      List<string> directories = DirectoryDiscoveryServiceAdapter.GetDirectories((OperationScopeEnum) operationScope, requestContext2);
      if (DirectoryDiscoveryServiceAdapter.IsImsSearchRequested(requestContext1, (IList<string>) directories))
      {
        HashSet<Guid> source = new HashSet<Guid>((IEnumerable<Guid>) getRequest.DirectoryIds);
        IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity> identities = DirectoryDiscoveryServiceAdapter.ResolveVsidsOrSubjectDescriptorsToIdentities(requestContext1, (IList<Guid>) source.ToList<Guid>(), (List<SubjectDescriptor>) null, (IEnumerable<string>) requestProperties);
        if (identities == null || identities.Count == 0)
          return (IDictionary<string, QueryTokenResult>) identitiesByDirectoryIds;
        bool flag = DirectoryDiscoveryServiceAdapter.RequestedByAuthorizedMember(requestContext1);
        foreach (Microsoft.VisualStudio.Services.IdentityPicker.Identity identity1 in (IEnumerable<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) identities)
        {
          Guid result;
          if (!string.IsNullOrWhiteSpace(identity1.LocalId) && Guid.TryParse(identity1.LocalId, out result) && source.Contains(result))
          {
            Microsoft.VisualStudio.Services.IdentityPicker.Identity identity2 = identity1;
            if (!flag)
              DirectoryDiscoveryServiceAdapter.RemovePrivateProperties(identity2);
            identitiesByDirectoryIds.Add(result.ToString(), new QueryTokenResult(result.ToString(), (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) new List<Microsoft.VisualStudio.Services.IdentityPicker.Identity>()
            {
              identity2
            }));
          }
        }
      }
      return (IDictionary<string, QueryTokenResult>) identitiesByDirectoryIds;
    }

    private IDictionary<string, QueryTokenResult> GetIdentitiesByAccountNames(
      IdentityProviderAdapterDdsGetRequest getRequest)
    {
      IVssRequestContext requestContext1 = getRequest.RequestContext;
      int operationScope = (int) getRequest.OperationScope;
      IList<string> requestProperties = getRequest.RequestProperties;
      Dictionary<string, QueryTokenResult> identitiesByAccountNames = new Dictionary<string, QueryTokenResult>();
      IVssRequestContext requestContext2 = requestContext1;
      List<string> directories = DirectoryDiscoveryServiceAdapter.GetDirectories((OperationScopeEnum) operationScope, requestContext2);
      if (DirectoryDiscoveryServiceAdapter.IsImsSearchRequested(requestContext1, (IList<string>) directories))
      {
        HashSet<string> source = new HashSet<string>((IEnumerable<string>) getRequest.AccountNames, (IEqualityComparer<string>) VssStringComparer.AccountName);
        IDictionary<string, Microsoft.VisualStudio.Services.IdentityPicker.Identity> enumerable = DirectoryDiscoveryServiceAdapter.ResolveIdentitiesByAccountNames(requestContext1, source.ToList<string>(), (IEnumerable<string>) requestProperties);
        if (enumerable.IsNullOrEmpty<KeyValuePair<string, Microsoft.VisualStudio.Services.IdentityPicker.Identity>>())
          return (IDictionary<string, QueryTokenResult>) identitiesByAccountNames;
        bool flag = DirectoryDiscoveryServiceAdapter.RequestedByAuthorizedMember(requestContext1);
        foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.IdentityPicker.Identity> keyValuePair in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.IdentityPicker.Identity>>) enumerable)
        {
          string key = keyValuePair.Key;
          Microsoft.VisualStudio.Services.IdentityPicker.Identity identity = keyValuePair.Value;
          if (!flag)
            DirectoryDiscoveryServiceAdapter.RemovePrivateProperties(identity);
          identitiesByAccountNames.Add(key, new QueryTokenResult(key, (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) new Microsoft.VisualStudio.Services.IdentityPicker.Identity[1]
          {
            identity
          }));
        }
      }
      return (IDictionary<string, QueryTokenResult>) identitiesByAccountNames;
    }

    private static IDictionary<string, Microsoft.VisualStudio.Services.IdentityPicker.Identity> ResolveIdentitiesByAccountNames(
      IVssRequestContext context,
      List<string> accountNames,
      IEnumerable<string> requestProperties,
      IDictionary<string, object> presetProperties = null,
      IEnumerable<string> filterByAncestorEntityIds = null,
      IEnumerable<string> filterByEntityIds = null)
    {
      if (accountNames.IsNullOrEmpty<string>())
        return (IDictionary<string, Microsoft.VisualStudio.Services.IdentityPicker.Identity>) new Dictionary<string, Microsoft.VisualStudio.Services.IdentityPicker.Identity>();
      context.TraceConditionally(10030701, TraceLevel.Info, "IdentityPicker", nameof (DirectoryDiscoveryServiceAdapter), (Func<string>) (() => string.Format("Resolving identities by {0} account names: {1}.", (object) accountNames.Count, (object) accountNames.Serialize<List<string>>())));
      IdentityService service = context.GetService<IdentityService>();
      Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> accountNameToImsIdentitiesMap = new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>((IEqualityComparer<string>) VssStringComparer.AccountName);
      Dictionary<string, Microsoft.VisualStudio.Services.IdentityPicker.Identity> accountNameToIpsIdentitiesMap = new Dictionary<string, Microsoft.VisualStudio.Services.IdentityPicker.Identity>((IEqualityComparer<string>) VssStringComparer.AccountName);
      foreach (string accountName in accountNames)
      {
        if (!accountName.IsNullOrEmpty<char>())
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ReadIdentities(context, IdentitySearchFilter.AccountName, accountName, QueryMembership.None, DirectoryDiscoveryServiceAdapter.s_DefaultIdentityProperties);
          if (source != null && source.Count > 1)
            context.Trace(10030704, TraceLevel.Error, "IdentityPicker", nameof (DirectoryDiscoveryServiceAdapter), "Read identity by account name " + accountName + " returned multiple results: " + source.Serialize<IList<Microsoft.VisualStudio.Services.Identity.Identity>>() + ".");
          else if ((source != null ? source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() : (Microsoft.VisualStudio.Services.Identity.Identity) null) != null)
            accountNameToImsIdentitiesMap.Add(accountName, source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>());
        }
      }
      context.TraceConditionally(10030702, TraceLevel.Info, "IdentityPicker", nameof (DirectoryDiscoveryServiceAdapter), (Func<string>) (() => "Read identities by account names result: " + accountNameToImsIdentitiesMap.Serialize<Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>() + "."));
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity> keyValuePair in accountNameToImsIdentitiesMap)
      {
        IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity> source = DirectoryDiscoveryServiceAdapter.TransformAndFilterIdentities(context, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          keyValuePair.Value
        }, requestProperties, presetProperties, filterByAncestorEntityIds, filterByEntityIds);
        Microsoft.VisualStudio.Services.IdentityPicker.Identity identity = source != null ? source.FirstOrDefault<Microsoft.VisualStudio.Services.IdentityPicker.Identity>() : (Microsoft.VisualStudio.Services.IdentityPicker.Identity) null;
        if (identity != null)
          accountNameToIpsIdentitiesMap.Add(keyValuePair.Key, identity);
      }
      context.TraceConditionally(10030703, TraceLevel.Info, "IdentityPicker", nameof (DirectoryDiscoveryServiceAdapter), (Func<string>) (() => "Transformed result: " + accountNameToIpsIdentitiesMap.Serialize<Dictionary<string, Microsoft.VisualStudio.Services.IdentityPicker.Identity>>() + "."));
      return (IDictionary<string, Microsoft.VisualStudio.Services.IdentityPicker.Identity>) accountNameToIpsIdentitiesMap;
    }

    internal override byte[] GetIdentityImage(
      IVssRequestContext requestContext,
      string objectId,
      Dictionary<string, object> options)
    {
      Tracing.TraceEnter(requestContext, 521, "DDS.GetIdentityImage");
      try
      {
        DirectoryDiscoveryService service = requestContext.GetService<DirectoryDiscoveryService>();
        IVssRequestContext context = requestContext;
        foreach (KeyValuePair<string, byte[]> result in (IEnumerable<KeyValuePair<string, byte[]>>) service.GetAvatars(context, new DirectoryGetAvatarsRequest()
        {
          ObjectIds = (IEnumerable<string>) new List<string>()
          {
            objectId
          }
        }).Results)
        {
          if (result.Value != null && result.Value.Length != 0)
          {
            byte[] imageByteArray = DirectoryDiscoveryServiceAdapter.GetImageByteArray(result.Value, options == null || !options.ContainsKey("ImageWidth") ? 0 : (int) options["ImageWidth"], options == null || !options.ContainsKey("ImageHeight") ? 0 : (int) options["ImageHeight"]);
            return imageByteArray != null && imageByteArray.Length != 0 ? imageByteArray : Array.Empty<byte>();
          }
        }
        return Array.Empty<byte>();
      }
      catch (Exception ex)
      {
        DirectoryDiscoveryServiceAdapter.HandleDirectoryDiscoveryServiceException("DDS.GetIdentityImage", ex);
        throw new IdentityPickerImageRetrievalException("Identity image could not be retrieved", ex);
      }
      finally
      {
        Tracing.TraceLeave(requestContext, 529, "DDS.GetIdentityImage");
      }
    }

    internal static IEnumerable<string> GetFilterByAncestorEntityIds(
      IEnumerable<string> filterByAncestorEntityIds)
    {
      return filterByAncestorEntityIds ?? (IEnumerable<string>) Array.Empty<string>();
    }

    internal static IEnumerable<string> GetFilterByEntityIds(IEnumerable<string> filterByEntityIds) => filterByEntityIds ?? (IEnumerable<string>) Array.Empty<string>();

    private static void HandleDirectoryDiscoveryServiceException(string message, Exception ex)
    {
      switch (ex)
      {
        case DirectoryDiscoveryServiceGuestAccessException _:
          throw new IdentityPickerAuthorizationException(message, ex)
          {
            IdentityPickerAuthorizationExceptionType = 1
          };
        case DirectoryDiscoveryServiceAccessException _:
          throw new IdentityPickerAuthorizationException(message, ex)
          {
            IdentityPickerAuthorizationExceptionType = 0
          };
        case DirectoryDiscoveryAvatarNotFoundException _:
          throw new IdentityPickerImageNotAvailableException(message, ex);
      }
    }

    internal override IList<Guid> GetIdentities(IVssRequestContext requestContext) => throw new NotImplementedException();

    internal override bool RemoveIdentities(IVssRequestContext requestContext, IList<string> vsids) => throw new NotImplementedException();

    internal override bool AddIdentities(IVssRequestContext requestContext, IList<string> vsids) => throw new NotImplementedException();

    private static byte[] GetImageByteArray(
      byte[] imageArray,
      int width = 0,
      int height = 0,
      ImageFormat format = null)
    {
      try
      {
        if (imageArray == null || imageArray.Length == 0)
          return Array.Empty<byte>();
        using (Stream stream = (Stream) new MemoryStream(imageArray))
        {
          using (Image image1 = Image.FromStream(stream))
          {
            if (image1 == null)
              return Array.Empty<byte>();
            if (width == 0 || height == 0)
            {
              if (image1.Width == 0 || image1.Height == 0)
                return Array.Empty<byte>();
              width = image1.Width;
              height = image1.Height;
            }
            using (Image image2 = (Image) new Bitmap(width, height))
            {
              using (Graphics graphics = Graphics.FromImage(image2))
              {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(image1, new Rectangle(0, 0, width, height));
              }
              using (MemoryStream memoryStream = new MemoryStream())
              {
                image2.Save((Stream) memoryStream, image1.RawFormat);
                return memoryStream.ToArray();
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        return Array.Empty<byte>();
      }
    }

    internal static List<string> GetDirectoryEntityTypes(IdentityTypeEnum type)
    {
      List<string> directoryEntityTypes = new List<string>();
      if (type.HasFlag((Enum) IdentityTypeEnum.User))
        directoryEntityTypes.Add("User");
      if (type.HasFlag((Enum) IdentityTypeEnum.Group))
        directoryEntityTypes.Add("Group");
      if (type.HasFlag((Enum) IdentityTypeEnum.ServicePrincipal))
        directoryEntityTypes.Add("ServicePrincipal");
      return directoryEntityTypes;
    }

    internal static List<string> GetDirectories(
      OperationScopeEnum scope,
      IVssRequestContext requestContext)
    {
      List<string> directories = new List<string>();
      if (scope.HasFlag((Enum) OperationScopeEnum.IMS))
        directories.Add("vsd");
      if (scope.HasFlag((Enum) OperationScopeEnum.AAD))
      {
        if (!IdentityOperationHelper.IsAadBackedOrg(requestContext))
          throw new IdentityPickerArgumentException("Operation scope AAD can only be used for AAD-backed accounts in hosted environments");
        directories.Add("aad");
      }
      TeamFoundationExecutionEnvironment executionEnvironment;
      if (scope.HasFlag((Enum) OperationScopeEnum.AD))
      {
        executionEnvironment = requestContext.ExecutionEnvironment;
        if (!executionEnvironment.IsOnPremisesDeployment)
          throw new IdentityPickerArgumentException("Operation scope AD can only be used for OnPrem environments");
        directories.Add("ad");
      }
      if (scope.HasFlag((Enum) OperationScopeEnum.GHB))
      {
        if (IdentityOperationHelper.IsAadBackedOrg(requestContext))
          throw new IdentityPickerArgumentException("Operation scope GHB can only be used for non AAD-backed accounts in hosted environments");
        directories.Add("ghb");
      }
      if (scope.HasFlag((Enum) OperationScopeEnum.WMD))
      {
        executionEnvironment = requestContext.ExecutionEnvironment;
        if (!executionEnvironment.IsOnPremisesDeployment)
          throw new IdentityPickerArgumentException("Operation scope WMD can only be used for OnPrem environments");
        directories.Add("wmd");
      }
      if (scope.HasFlag((Enum) OperationScopeEnum.Source))
        directories.Add("src");
      return directories;
    }

    private static bool RequestedByAuthorizedMember(IVssRequestContext requestContext) => (!requestContext.IsFeatureEnabled("Microsoft.AzureDevOps.Identity.CheckGuestAccessForIdentityPicker.M202") || IdentityOperationHelper.IsExternalGuestAccessPolicyEnabled(requestContext) || !IdentityOperationHelper.IsRequestByGuest(requestContext)) && !IdentityOperationHelper.IsRequestByNonMember(requestContext);

    internal static bool IsImsSearchRequested(
      IVssRequestContext requestContext,
      IList<string> directories)
    {
      if (directories.Contains("vsd"))
        return true;
      if (!directories.Contains("src"))
        return false;
      return requestContext.ExecutionEnvironment.IsOnPremisesDeployment || !IdentityOperationHelper.IsAadBackedOrg(requestContext);
    }

    internal static bool IsImsOnlySearchRequested(
      IVssRequestContext requestContext,
      IList<string> directories)
    {
      if (!directories.Contains("vsd") && !directories.Contains("src"))
        return false;
      return requestContext.ExecutionEnvironment.IsOnPremisesDeployment || !IdentityOperationHelper.IsAadBackedOrg(requestContext);
    }
  }
}
