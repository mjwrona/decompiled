// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupsController_BackCompatController
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
  [BackCompatJsonFormatter]
  [ControllerApiVersion(0.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Groups")]
  public class GroupsController_BackCompatController : IdentitiesControllerBase
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    [HttpGet]
    public IQueryable<Identity_BackCompat> ListGroups(
      string scopeIds = "",
      bool recurse = false,
      string properties = "")
    {
      Guid[] scopeIdsFromString = this.GetScopeIdsFromString(scopeIds);
      IEnumerable<string> filtersFromString = IdentityParser.GetPropertyFiltersFromString(properties);
      return Identity_BackCompat.Convert(this.TfsRequestContext.GetService<IdentityService>().ListGroups(this.TfsRequestContext, scopeIdsFromString, recurse, filtersFromString)).AsQueryable<Identity_BackCompat>();
    }

    [HttpDelete]
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
    public HttpResponseMessage CreateGroups(JObject container)
    {
      Guid scopeId = container["scopeId"].ToObject<Guid>();
      List<Identity_BackCompat> identityBackCompatList = container["groups"].ToObject<List<Identity_BackCompat>>();
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      List<IdentitiesController_BackCompatController.IdentityUpdateData> identityUpdateDataList = new List<IdentitiesController_BackCompatController.IdentityUpdateData>();
      for (int index = 0; index < identityBackCompatList.Count; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityBackCompatList[index].ToIdentity();
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
          Microsoft.VisualStudio.Services.Identity.Identity group = service.CreateGroup(this.TfsRequestContext, scopeId, identity.Descriptor.Identifier, identity.DisplayName, property, specialGroupType, scopeLocal, hasRestrictedVisibility);
          identityBackCompatList[index] = new Identity_BackCompat(group);
        }
        else if (string.Equals(identity.Descriptor.IdentityType, "Microsoft.TeamFoundation.UnauthenticatedIdentity", StringComparison.OrdinalIgnoreCase))
        {
          string property1 = identity.GetProperty<string>("Domain", string.Empty);
          string property2 = identity.GetProperty<string>("Account", string.Empty);
          string property3 = identity.GetProperty<string>("Description", string.Empty);
          Microsoft.VisualStudio.Services.Identity.Identity user = service.IdentityServiceInternal().CreateUser(this.TfsRequestContext, scopeId, identity.Descriptor.Identifier, property1, property2, property3);
          identityBackCompatList[index] = new Identity_BackCompat(user);
        }
      }
      return this.Request.CreateResponse<IEnumerable<Identity_BackCompat>>(HttpStatusCode.OK, (IEnumerable<Identity_BackCompat>) identityBackCompatList);
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
