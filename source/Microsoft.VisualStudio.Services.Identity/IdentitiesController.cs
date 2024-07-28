// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitiesController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Identities")]
  public class IdentitiesController : IdentitiesControllerBase, IOverrideLoggingMethodNames
  {
    internal static readonly Dictionary<Type, HttpStatusCode> s_httpIdentityServiceExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (IdentityDomainMismatchException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (AddMemberCyclicMembershipException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IdentityPropertyRequiredException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IdentityExpressionException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidDisplayNameException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (GroupNameNotRecognizedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IdentityMapReadOnlyException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IdentityNotServiceIdentityException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidServiceIdentityNameException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IllegalIdentityException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MissingRequiredParameterException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IncompatibleScopeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IdentityInvalidTypeIdException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidOperationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (GroupNameIsReservedBySystemException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (RemoveAccountOwnerFromAdminGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (RemoveSelfFromAdminGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (AddGroupMemberIllegalMemberException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (AddGroupMemberIllegalWindowsIdentityException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (AddGroupMemberIllegalInternetIdentityException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (RemoveSpecialGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (NotApplicationGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (ModifyEveryoneGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (NotASecurityGroupException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (RemoveMemberServiceAccountException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (AccountPreferencesAlreadyExistException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (InvalidAccessException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (RemoveGroupMemberNotMemberException),
        HttpStatusCode.NotFound
      },
      {
        typeof (RemoveNonexistentGroupException),
        HttpStatusCode.NotFound
      },
      {
        typeof (FindGroupSidDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (GroupScopeDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (IdentityNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (GroupCreationException),
        HttpStatusCode.Conflict
      },
      {
        typeof (GroupScopeCreationException),
        HttpStatusCode.Conflict
      },
      {
        typeof (AddMemberIdentityAlreadyMemberException),
        HttpStatusCode.Conflict
      },
      {
        typeof (GroupRenameException),
        HttpStatusCode.Conflict
      },
      {
        typeof (IdentityAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (IdentityAccountNameAlreadyInUseException),
        HttpStatusCode.Conflict
      },
      {
        typeof (IdentityAliasAlreadyInUseException),
        HttpStatusCode.Conflict
      },
      {
        typeof (AddProjectGroupProjectMismatchException),
        HttpStatusCode.Conflict
      },
      {
        typeof (IdentitySyncException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (InvalidChangedIdentityException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (IdentityProviderUnavailableException),
        HttpStatusCode.ServiceUnavailable
      }
    };
    private const string Area = "Identity";
    private const string Layer = "IdentitiesController";
    private const string ExtractMinSequenceIdRequestHeaders = "VisualStudio.Services.Identity.HttpRequests.ExtractMinSequenceIdRequestHeaders";
    private const string IgnoreCacheOnStrongInvalidations = "VisualStudio.Services.Identity.IgnoreCacheOnStrongInvalidations";

    [HttpPut]
    [ClientResponseType(typeof (IEnumerable<IdentityUpdateData>), null, null)]
    public HttpResponseMessage UpdateIdentities(
      VssJsonCollectionWrapper<List<Microsoft.VisualStudio.Services.Identity.Identity>> identities)
    {
      return this.UpdateIdentitiesInternal(identities, new bool?());
    }

    [HttpPut]
    [ClientResponseType(typeof (IEnumerable<IdentityUpdateData>), null, null)]
    public HttpResponseMessage UpdateIdentities(
      VssJsonCollectionWrapper<List<Microsoft.VisualStudio.Services.Identity.Identity>> identities,
      bool allowMetaDataUpdate)
    {
      return this.UpdateIdentitiesInternal(identities, new bool?(allowMetaDataUpdate));
    }

    private HttpResponseMessage UpdateIdentitiesInternal(
      VssJsonCollectionWrapper<List<Microsoft.VisualStudio.Services.Identity.Identity>> identities,
      bool? allowMetaDataUpdate)
    {
      if (identities?.Value == null)
        throw new MissingRequiredParameterException(WebApiResources.MissingRequiredParameterMessage((object) "identity"));
      this.ScrubIdentityPropertiesAndMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities.Value, true);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities.Value)
      {
        try
        {
          IdentityValidation.CheckDescriptor(identity.Descriptor, "identity");
        }
        catch (ArgumentException ex)
        {
          throw new IllegalIdentityException("identity", (Exception) ex);
        }
        identity.SetAllModifiedProperties();
      }
      bool flag = this.TfsRequestContext.GetService<IdentityService>().UpdateIdentities(this.TfsRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities.Value, allowMetaDataUpdate.GetValueOrDefault());
      List<IdentityUpdateData> identityUpdateDataList = new List<IdentityUpdateData>();
      for (int index = 0; index < identities.Value.Count; ++index)
        identityUpdateDataList.Add(new IdentityUpdateData()
        {
          Index = index,
          Id = identities.Value[index].Id,
          Updated = flag
        });
      return this.Request.CreateResponse<IEnumerable<IdentityUpdateData>>(HttpStatusCode.OK, (IEnumerable<IdentityUpdateData>) identityUpdateDataList);
    }

    [HttpGet]
    [PublicCollectionRequestRestrictions(true, true, null)]
    [ClientInclude(RestClientLanguages.All)]
    [ClientExample("SearchPCVUByName.json", "By Name", null, null)]
    [ClientExample("SearchIdentityByEmail.json", "By Email", null, null)]
    [ClientExample("SearchByIds.json", "By Ids", null, null)]
    [ClientExample("SearchByIdentityDescriptors.json", "By IdentityDescriptors", null, null)]
    [ClientExample("SearchBySubjectDescriptors.json", "By Subject Descriptors", null, null)]
    public IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      string descriptors = "",
      string identityIds = "",
      string subjectDescriptors = "",
      [ClientInclude(~RestClientLanguages.Swagger2)] string socialDescriptors = "",
      string searchFilter = "",
      string filterValue = "",
      QueryMembership queryMembership = QueryMembership.None,
      [ClientInclude(~RestClientLanguages.Swagger2)] string properties = "",
      [ClientInclude(~RestClientLanguages.Swagger2)] bool includeRestrictedVisibility = false,
      [ClientInclude(~RestClientLanguages.Swagger2)] ReadIdentitiesOptions options = ReadIdentitiesOptions.None)
    {
      IList<IdentityDescriptor> descriptorList = IdentityParser.GetDescriptorsFromString(descriptors);
      IList<SubjectDescriptor> subjectDescriptorList = (IList<SubjectDescriptor>) SubjectDescriptor.FromCommaSeperatedStrings(subjectDescriptors).ToList<SubjectDescriptor>();
      IList<SocialDescriptor> socialDescriptorList = (IList<SocialDescriptor>) SocialDescriptor.FromCommaSeperatedStrings(socialDescriptors).ToList<SocialDescriptor>();
      IList<Guid> identityIdList = IdentityParser.GetIdentityIdsFromString(identityIds);
      IEnumerable<string> propertyNameFilters = IdentityParser.GetPropertyFiltersFromString(properties);
      bool ignoreCache = false;
      SequenceContext minSequenceContext = (SequenceContext) null;
      RequestHeadersContext requestHeadersContext;
      if (RequestHeadersContext.HeadersUtils.TryExtractRequestHeaderContext(this.Request.Headers, out requestHeadersContext) && requestHeadersContext != null)
      {
        if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.IgnoreCacheOnStrongInvalidations"))
        {
          ignoreCache = requestHeadersContext.IgnoreCache && this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.IgnoreCacheOnStrongInvalidations");
          this.TfsRequestContext.RootContext.Items[RequestContextItemsKeys.IgnoreMembershipCache] = (object) ignoreCache;
        }
        if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.HttpRequests.ExtractMinSequenceIdRequestHeaders") && requestHeadersContext.SequenceContext != null)
        {
          minSequenceContext = requestHeadersContext.SequenceContext;
          this.TfsRequestContext.RootContext.Items[RequestContextItemsKeys.MinSequenceContext] = (object) minSequenceContext;
        }
      }
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      this.TfsRequestContext.GetService<IPlatformIdentityServiceInternal>().CheckForLeakedMasterIds(this.TfsRequestContext, (IEnumerable<Guid>) identityIdList);
      IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> queryable;
      using (this.TfsRequestContext.SetDisposableContextItem(RequestContextItemsKeys.ReadOnlyReplicaReadEnabled, (object) true))
      {
        if (descriptorList != null && descriptorList.Count > 0)
        {
          this.TfsRequestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ByDescriptor, queryMembership, propertyNameFilters.Count<string>(), descriptorList.Count), TraceLevel.Verbose, "Identity", nameof (IdentitiesController), (Func<string>) (() => string.Format("IdentitiesController.ReadIdentities where descriptors : {0}, queryMembership : {1}, propertyNames : {2}, includeRestrictedVisibility : {3}, minSequenceContext : {4}, byPassCacheUponPopulation : {5}", (object) descriptorList.Serialize<IList<IdentityDescriptor>>(), (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>(), (object) includeRestrictedVisibility, (object) (minSequenceContext?.ToString() ?? "null"), (object) ignoreCache.ToString())));
          queryable = service.ReadIdentities(this.TfsRequestContext, descriptorList, queryMembership, propertyNameFilters, includeRestrictedVisibility).AsQueryable<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        else if (subjectDescriptorList != null && subjectDescriptorList.Count > 0)
        {
          this.TfsRequestContext.TraceConditionally(745266, TraceLevel.Verbose, "Identity", nameof (IdentitiesController), (Func<string>) (() => string.Format("IdentitiesController.ReadIdentities where subjectDescriptors : {0}, queryMembership : {1}, propertyNames : {2}, includeRestrictedVisibility : {3}, minSequenceContext : {4}, identityCount: {5}", (object) subjectDescriptorList.Serialize<IList<SubjectDescriptor>>(), (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>(), (object) includeRestrictedVisibility, (object) (minSequenceContext?.ToString() ?? "null"), (object) subjectDescriptorList.Count)));
          queryable = service.ReadIdentities(this.TfsRequestContext, subjectDescriptorList, queryMembership, propertyNameFilters, includeRestrictedVisibility).AsQueryable<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        else if (socialDescriptorList != null && socialDescriptorList.Count > 0)
        {
          this.TfsRequestContext.TraceConditionally(745266, TraceLevel.Verbose, "Identity", nameof (IdentitiesController), (Func<string>) (() => string.Format("IdentitiesController.ReadIdentities where socialDescriptors : {0}, queryMembership : {1}, propertyNames : {2}, includeRestrictedVisibility : {3}, minSequenceContext : {4}, identityCount: {5}", (object) socialDescriptorList.Serialize<IList<SocialDescriptor>>(), (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>(), (object) includeRestrictedVisibility, (object) (minSequenceContext?.ToString() ?? "null"), (object) socialDescriptorList.Count)));
          queryable = service.ReadIdentities(this.TfsRequestContext, socialDescriptorList, queryMembership, propertyNameFilters, includeRestrictedVisibility).AsQueryable<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        else if (identityIdList != null && identityIdList.Count > 0)
        {
          this.TfsRequestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ById, queryMembership, propertyNameFilters.Count<string>(), identityIdList.Count), TraceLevel.Verbose, "Identity", nameof (IdentitiesController), (Func<string>) (() => string.Format("IdentitiesController.ReadIdentities where identityIds : {0}, queryMembership : {1}, propertyNames : {2}, includeRestrictedVisibility : {3}, minSequenceContext : {4}", (object) identityIdList.Serialize<IList<Guid>>(), (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>(), (object) includeRestrictedVisibility, (object) (minSequenceContext?.ToString() ?? "null"))));
          queryable = service.ReadIdentities(this.TfsRequestContext, identityIdList, queryMembership, propertyNameFilters, includeRestrictedVisibility).AsQueryable<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
        else
        {
          IdentitySearchFilter parsedSearchFactor = !string.IsNullOrEmpty(searchFilter) ? (IdentitySearchFilter) System.Enum.Parse(typeof (IdentitySearchFilter), searchFilter) : throw new ArgumentException("Either descriptors, subject descriptors or social descriptors or identityIds or searchFactor/factorValue must be specified");
          IdentityValidation.CheckFactorAndValue(parsedSearchFactor, ref filterValue);
          this.TfsRequestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.CovertToReadIdentityTraceKind(parsedSearchFactor), queryMembership, propertyNameFilters.Count<string>(), 0), TraceLevel.Verbose, "Identity", nameof (IdentitiesController), (Func<string>) (() => string.Format("IdentitiesController.ReadIdentities where searchFactor : {0}, filterValue : {1}, queryMembership : {2}, propertyNames : {3}", (object) parsedSearchFactor, (object) filterValue, (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>())));
          queryable = service.ReadIdentities(this.TfsRequestContext, parsedSearchFactor, filterValue, queryMembership, propertyNameFilters, options).AsQueryable<Microsoft.VisualStudio.Services.Identity.Identity>();
        }
      }
      this.ScrubIdentityPropertiesAndMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) queryable);
      return queryable;
    }

    [HttpGet]
    public IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByScope(
      Guid scopeId,
      QueryMembership queryMembership = QueryMembership.None,
      string properties = "")
    {
      IPlatformIdentityServiceInternal service = this.TfsRequestContext.GetService<IPlatformIdentityServiceInternal>();
      IEnumerable<string> propertyNameFilters = IdentityParser.GetPropertyFiltersFromString(properties);
      this.TfsRequestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ByScope, queryMembership, propertyNameFilters.Count<string>(), 0), TraceLevel.Verbose, "Identity", nameof (IdentitiesController), (Func<string>) (() => string.Format("IdentitiesController.ReadIdentitiesByScope where scopeId : {0}, queryMembership : {1}, propertyNames : {2}", (object) scopeId, (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>())));
      IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> queryable = (IQueryable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      using (this.TfsRequestContext.SetDisposableContextItem(RequestContextItemsKeys.ReadOnlyReplicaReadEnabled, (object) true))
        queryable = service.ReadIdentitiesByScope(this.TfsRequestContext, scopeId, queryMembership, propertyNameFilters).AsQueryable<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.ScrubIdentityPropertiesAndMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) queryable);
      return queryable;
    }

    [HttpGet]
    public IQueryable<Guid> GetUserIdentityIdsByDomainId(Guid domainId)
    {
      IPlatformIdentityServiceInternal service = this.TfsRequestContext.GetService<IPlatformIdentityServiceInternal>();
      this.TfsRequestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ByDomainId, QueryMembership.None, 0, 0), TraceLevel.Verbose, "Identity", nameof (IdentitiesController), (Func<string>) (() => string.Format("IdentitiesController.GetIdentityIdsByDomainId where domainId : {0}", (object) domainId)));
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid domainId1 = domainId;
      return service.GetIdentityIdsByDomainId(tfsRequestContext, domainId1).AsQueryable<Guid>();
    }

    [HttpPut]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage UpdateIdentity(Guid identityId, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity == null)
        throw new MissingRequiredParameterException(WebApiResources.MissingRequiredParameterMessage((object) nameof (identity)));
      try
      {
        IdentityValidation.CheckDescriptor(identity.Descriptor, nameof (identity));
      }
      catch (ArgumentException ex)
      {
        throw new IllegalIdentityException(nameof (identity), (Exception) ex);
      }
      if (identityId != identity.Id)
        throw new ArgumentException("Identity to update does not have the right Id");
      this.ScrubIdentityPropertiesAndMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      }, true);
      identity.SetAllModifiedProperties();
      this.TfsRequestContext.GetService<IdentityService>().UpdateIdentities(this.TfsRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      });
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        RequestMessage = this.Request
      };
    }

    [HttpGet]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Identity.Identity), null, null)]
    public HttpResponseMessage ReadIdentity(
      string identityId,
      QueryMembership queryMembership = QueryMembership.None,
      string properties = "")
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      IEnumerable<string> propertyNameFilters = IdentityParser.GetPropertyFiltersFromString(properties);
      IdentityService service1 = this.TfsRequestContext.GetService<IdentityService>();
      IPlatformIdentityServiceInternal service2 = this.TfsRequestContext.GetService<IPlatformIdentityServiceInternal>();
      using (this.TfsRequestContext.SetDisposableContextItem(RequestContextItemsKeys.ReadOnlyReplicaReadEnabled, (object) true))
      {
        Guid identityGuid;
        if (Guid.TryParse(identityId, out identityGuid))
        {
          service2.CheckForLeakedMasterIds(this.TfsRequestContext, (IEnumerable<Guid>) new Guid[1]
          {
            identityGuid
          });
          this.TfsRequestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ById, queryMembership, propertyNameFilters.Count<string>(), 1), TraceLevel.Verbose, "Identity", nameof (IdentitiesController), (Func<string>) (() => string.Format("IdentitiesController.ReadIdentity where identityId : {0}, queryMembership : {1}, propertyNames : {2}", (object) identityGuid, (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>())));
          identity = service1.ReadIdentities(this.TfsRequestContext, (IList<Guid>) new Guid[1]
          {
            identityGuid
          }, queryMembership, propertyNameFilters)[0];
        }
        else
        {
          IdentityDescriptor descriptor = IdentityParser.GetDescriptorFromString(identityId);
          this.TfsRequestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ByDescriptor, queryMembership, propertyNameFilters.Count<string>(), 1), TraceLevel.Verbose, "Identity", nameof (IdentitiesController), (Func<string>) (() => string.Format("IdentitiesController.ReadIdentity where descriptor : {0}, queryMembership : {1}, propertyNames : {2}", (object) descriptor, (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>())));
          identity = service1.ReadIdentities(this.TfsRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            descriptor
          }, queryMembership, propertyNameFilters)[0];
        }
      }
      this.ScrubIdentityPropertiesAndMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      });
      return this.Request.CreateResponse<Microsoft.VisualStudio.Services.Identity.Identity>(HttpStatusCode.OK, identity);
    }

    [HttpGet]
    [ClientResponseType(typeof (ChangedIdentities), null, null)]
    public HttpResponseMessage GetIdentityChanges(
      int identitySequenceId,
      int groupSequenceId,
      Guid? scopeId = null)
    {
      int organizationIdentitySequenceId = -1;
      return this.GetIdentityChanges(identitySequenceId, groupSequenceId, organizationIdentitySequenceId, scopeId);
    }

    [HttpGet]
    [ClientResponseType(typeof (ChangedIdentities), null, null)]
    public HttpResponseMessage GetIdentityChanges(
      int identitySequenceId,
      int groupSequenceId,
      int organizationIdentitySequenceId,
      Guid? scopeId = null)
    {
      return this.GetIdentityChanges(identitySequenceId, groupSequenceId, organizationIdentitySequenceId, 0, scopeId);
    }

    [HttpGet]
    [ClientResponseType(typeof (ChangedIdentities), null, null)]
    public HttpResponseMessage GetIdentityChanges(
      int identitySequenceId,
      int groupSequenceId,
      int organizationIdentitySequenceId,
      int pageSize,
      Guid? scopeId = null)
    {
      this.TfsRequestContext.TraceEnter(850000, "Identity", nameof (IdentitiesController), nameof (GetIdentityChanges));
      try
      {
        IdentityService service1 = this.TfsRequestContext.GetService<IdentityService>();
        IPlatformIdentityServiceInternal service2 = this.TfsRequestContext.GetService<IPlatformIdentityServiceInternal>();
        ChangedIdentitiesContext sequenceContext = new ChangedIdentitiesContext(identitySequenceId, groupSequenceId, organizationIdentitySequenceId, pageSize);
        ChangedIdentities changedIdentities = scopeId.HasValue ? service2.GetIdentityChanges(this.TfsRequestContext, sequenceContext, scopeId.Value) : service1.IdentityServiceInternal().GetIdentityChanges(this.TfsRequestContext, sequenceContext);
        this.ScrubIdentityPropertiesAndMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) changedIdentities.Identities);
        return this.Request.CreateResponse<ChangedIdentities>(HttpStatusCode.OK, changedIdentities);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(850008, "Identity", nameof (IdentitiesController), ex);
        TeamFoundationEventLog.Default.LogException(ex.Message, ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(850009, "Identity", nameof (IdentitiesController), nameof (GetIdentityChanges));
      }
    }

    string IOverrideLoggingMethodNames.GetLoggingMethodName(
      string methodName,
      HttpActionContext actionContext)
    {
      if (methodName.EndsWith("ReadIdentities", StringComparison.Ordinal))
        methodName = IdentitiesControllerLoggingNameGenerator.GetReadIdentitiesLoggingName(methodName, (IReadOnlyDictionary<string, object>) actionContext.ActionArguments);
      else if (methodName.EndsWith("ReadIdentity", StringComparison.Ordinal))
        methodName = IdentitiesControllerLoggingNameGenerator.GetReadIdentityLoggingName(methodName, (IReadOnlyDictionary<string, object>) actionContext.ActionArguments);
      IdentitiesControllerLoggingNameGenerator.RenameCurrentServiceIfRequestIsMembershipRelated(this.TfsRequestContext.RootContext, methodName);
      return methodName;
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) IdentitiesController.s_httpIdentityServiceExceptions;

    public override string TraceArea => "IdentityService";
  }
}
