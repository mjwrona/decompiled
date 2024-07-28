// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupsController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Groups")]
  public class GroupsController : IdentitiesControllerBase
  {
    [HttpGet]
    public IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      string scopeIds = "",
      bool recurse = false,
      bool deleted = false,
      string properties = "")
    {
      Guid[] scopeIdsFromString = this.GetScopeIdsFromString(scopeIds);
      IEnumerable<string> filtersFromString = IdentityParser.GetPropertyFiltersFromString(properties);
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      return !deleted ? service.ListGroups(this.TfsRequestContext, scopeIdsFromString, recurse, filtersFromString).AsQueryable<Microsoft.VisualStudio.Services.Identity.Identity>() : service.ListDeletedGroups(this.TfsRequestContext, scopeIdsFromString, recurse, filtersFromString).AsQueryable<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteGroup(string groupId)
    {
      IdentityDescriptor descriptorFromString = IdentityParser.GetDescriptorFromString(groupId);
      this.TfsRequestContext.GetService<IdentityService>().DeleteGroup(this.TfsRequestContext, descriptorFromString);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        RequestMessage = this.Request
      };
    }

    [HttpPost]
    [ClientResponseType(typeof (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>), null, null)]
    public HttpResponseMessage CreateGroups(JObject container)
    {
      Guid scopeId = container["scopeId"].ToObject<Guid>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = container["groups"].ToObject<List<Microsoft.VisualStudio.Services.Identity.Identity>>();
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      for (int index = 0; index < identityList.Count; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
        if (!IdentityValidation.IsNonEmptyDescriptor(identity.Descriptor))
          throw new MissingRequiredParameterException(TFCommonResources.InvalidGroupDescriptor((object) identity.DisplayName));
        if (IdentityValidation.IsTeamFoundationType(identity.Descriptor))
        {
          string property = identity.GetProperty<string>("Description", string.Empty);
          SpecialGroupType specialGroupType = IdentityHelper.GetSpecialGroupType((IReadOnlyVssIdentity) identity);
          bool hasRestrictedVisibility = false;
          bool scopeLocal = true;
          object obj;
          if (identity.TryGetProperty("RestrictedVisible", out obj))
            hasRestrictedVisibility = true;
          if (identity.TryGetProperty("CrossProject", out obj))
            scopeLocal = false;
          identityList[index] = service.CreateGroup(this.TfsRequestContext, scopeId, identity.Descriptor.Identifier, identity.DisplayName, property, specialGroupType, scopeLocal, hasRestrictedVisibility);
        }
        else if (string.Equals(identity.Descriptor.IdentityType, "Microsoft.TeamFoundation.UnauthenticatedIdentity", StringComparison.OrdinalIgnoreCase))
        {
          string property1 = identity.GetProperty<string>("Domain", string.Empty);
          string property2 = identity.GetProperty<string>("Account", string.Empty);
          string property3 = identity.GetProperty<string>("Description", string.Empty);
          identityList[index] = service.IdentityServiceInternal().CreateUser(this.TfsRequestContext, scopeId, identity.Descriptor.Identifier, property1, property2, property3);
        }
      }
      return this.Request.CreateResponse<IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>>(HttpStatusCode.OK, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList);
    }

    private Guid[] GetScopeIdsFromString(string scopeIds)
    {
      if (string.IsNullOrWhiteSpace(scopeIds))
        return (Guid[]) null;
      return ((IEnumerable<string>) scopeIds.Split(',')).Where<string>((Func<string, bool>) (scopeId => !string.IsNullOrEmpty(scopeId))).Select<string, Guid>((Func<string, Guid>) (scopeId => new Guid(scopeId))).ToArray<Guid>();
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) IdentitiesController.s_httpIdentityServiceExceptions;

    public override string TraceArea => "IdentityService";
  }
}
