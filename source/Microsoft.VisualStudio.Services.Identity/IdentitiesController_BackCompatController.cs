// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitiesController_BackCompatController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [BackCompatJsonFormatter]
  [ControllerApiVersion(0.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Identities")]
  public class IdentitiesController_BackCompatController : IdentitiesControllerBase
  {
    private const string s_area = "Identity";
    private const string s_layer = "IdentitiesController_BackCompatController";
    private const string s_methodGetIdentityChanges = "GetIdentityChanges";

    [HttpPost]
    public HttpResponseMessage UpdateIdentities(JObject container)
    {
      List<Identity_BackCompat> identityBackCompatList = container["identities"].ToObject<List<Identity_BackCompat>>();
      this.ScrubIdentityProperties((IEnumerable<Identity_BackCompat>) identityBackCompatList, true);
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = Identity_BackCompat.Convert((IList<Identity_BackCompat>) identityBackCompatList);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = identityList;
      service.UpdateIdentities(tfsRequestContext, identities);
      List<IdentitiesController_BackCompatController.IdentityUpdateData> identityUpdateDataList = new List<IdentitiesController_BackCompatController.IdentityUpdateData>();
      for (int index = 0; index < identityList.Count; ++index)
        identityUpdateDataList.Add(new IdentitiesController_BackCompatController.IdentityUpdateData()
        {
          Index = index,
          Id = identityList[index].Id
        });
      return this.Request.CreateResponse<IEnumerable<IdentitiesController_BackCompatController.IdentityUpdateData>>(HttpStatusCode.OK, (IEnumerable<IdentitiesController_BackCompatController.IdentityUpdateData>) identityUpdateDataList);
    }

    [HttpGet]
    public IQueryable<Identity_BackCompat> ReadIdentities(
      string descriptors = "",
      string identityIds = "",
      string searchFactor = "",
      string factorValue = "",
      QueryMembership queryMembership = QueryMembership.None,
      string properties = "")
    {
      IList<IdentityDescriptor> descriptorList = IdentityParser.GetDescriptorsFromString(descriptors);
      IList<Guid> identityIdList = IdentityParser.GetIdentityIdsFromString(identityIds);
      IEnumerable<string> propertyNameFilters = IdentityParser.GetPropertyFiltersFromString(properties);
      IdentityService service1 = this.TfsRequestContext.GetService<IdentityService>();
      IPlatformIdentityServiceInternal service2 = this.TfsRequestContext.GetService<IPlatformIdentityServiceInternal>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList;
      if (descriptorList != null && descriptorList.Count > 0)
      {
        this.TfsRequestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ByDescriptor, queryMembership, propertyNameFilters.Count<string>(), descriptorList.Count), TraceLevel.Verbose, "Identity", nameof (IdentitiesController_BackCompatController), (Func<string>) (() => string.Format("IdentitiesController_BackCompatController.ReadIdentities where descriptors : {0}, queryMembership : {1}, propertyNames : {2}", (object) descriptorList.Serialize<IList<IdentityDescriptor>>(), (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>())));
        identityList = service1.ReadIdentities(this.TfsRequestContext, descriptorList, queryMembership, propertyNameFilters);
      }
      else if (identityIdList != null && identityIdList.Count > 0)
      {
        service2.CheckForLeakedMasterIds(this.TfsRequestContext, (IEnumerable<Guid>) identityIdList);
        this.TfsRequestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ById, queryMembership, propertyNameFilters.Count<string>(), identityIdList.Count), TraceLevel.Verbose, "Identity", nameof (IdentitiesController_BackCompatController), (Func<string>) (() => string.Format("IdentitiesController_BackCompatController.ReadIdentities where identityIds : {0}, queryMembership : {1}, propertyNames : {2}", (object) identityIdList.Serialize<IList<Guid>>(), (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>())));
        identityList = service1.ReadIdentities(this.TfsRequestContext, identityIdList, queryMembership, propertyNameFilters);
      }
      else
      {
        IdentitySearchFilter parsedSearchFilter = !string.IsNullOrEmpty(searchFactor) ? (IdentitySearchFilter) System.Enum.Parse(typeof (IdentitySearchFilter), searchFactor) : throw new ArgumentException("Either descriptors or identityIds or searchFactor/factorValue must be specified");
        this.TfsRequestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.CovertToReadIdentityTraceKind(parsedSearchFilter), queryMembership, propertyNameFilters.Count<string>(), 0), TraceLevel.Verbose, "Identity", nameof (IdentitiesController_BackCompatController), (Func<string>) (() => string.Format("IdentitiesController_BackCompatController.ReadIdentities where searchFactor : {0}, filterValue : {1}, queryMembership : {2}, propertyNames : {3}", (object) parsedSearchFilter, (object) factorValue, (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>())));
        identityList = service1.ReadIdentities(this.TfsRequestContext, parsedSearchFilter, factorValue, queryMembership, propertyNameFilters);
      }
      this.ScrubIdentityPropertiesAndMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList);
      return Identity_BackCompat.Convert(identityList).AsQueryable<Identity_BackCompat>();
    }

    [HttpPut]
    public HttpResponseMessage UpdateIdentity(Guid identityId, Identity_BackCompat identity)
    {
      ArgumentUtility.CheckForNull<Identity_BackCompat>(identity, nameof (identity));
      if (identityId != identity.Id)
        throw new ArgumentException("Identity to update does not have the right Id");
      this.ScrubIdentityProperties((IEnumerable<Identity_BackCompat>) new Identity_BackCompat[1]
      {
        identity
      }, true);
      identity.SetAllModifiedProperties();
      this.TfsRequestContext.GetService<IdentityService>().UpdateIdentities(this.TfsRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity.ToIdentity()
      });
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        RequestMessage = this.Request
      };
    }

    [HttpGet]
    public HttpResponseMessage ReadIdentity(
      string identityId,
      QueryMembership queryMembership = QueryMembership.None,
      string properties = "")
    {
      IEnumerable<string> propertyNameFilters = IdentityParser.GetPropertyFiltersFromString(properties);
      IdentityService service1 = this.TfsRequestContext.GetService<IdentityService>();
      IPlatformIdentityServiceInternal service2 = this.TfsRequestContext.GetService<IPlatformIdentityServiceInternal>();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity;
      Guid identityGuid;
      if (Guid.TryParse(identityId, out identityGuid))
      {
        service2.CheckForLeakedMasterIds(this.TfsRequestContext, (IEnumerable<Guid>) new Guid[1]
        {
          identityGuid
        });
        this.TfsRequestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ById, queryMembership, propertyNameFilters.Count<string>(), 1), TraceLevel.Verbose, "Identity", nameof (IdentitiesController_BackCompatController), (Func<string>) (() => string.Format("IdentitiesController_BackCompatController.ReadIdentity where identityId : {0}, queryMembership : {1}, propertyNames : {2}", (object) identityGuid, (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>())));
        readIdentity = service1.ReadIdentities(this.TfsRequestContext, (IList<Guid>) new Guid[1]
        {
          identityGuid
        }, queryMembership, propertyNameFilters)[0];
        if (readIdentity == null)
          throw new IdentityNotFoundException(identityGuid);
      }
      else
      {
        IdentityDescriptor descriptor = IdentityParser.GetDescriptorFromString(identityId);
        this.TfsRequestContext.TraceConditionally(IdentityTracing.ConvertToReadIdentityTracePoint(IdentityTracing.ReadIdentityTraceKind.ByDescriptor, queryMembership, propertyNameFilters.Count<string>(), 1), TraceLevel.Verbose, "Identity", nameof (IdentitiesController_BackCompatController), (Func<string>) (() => string.Format("IdentitiesController_BackCompatController.ReadIdentity where descriptor : {0}, queryMembership : {1}, propertyNames : {2}", (object) descriptor, (object) queryMembership, (object) propertyNameFilters.Serialize<IEnumerable<string>>())));
        readIdentity = service1.ReadIdentities(this.TfsRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          descriptor
        }, queryMembership, propertyNameFilters)[0];
        if (readIdentity == null)
          throw new IdentityNotFoundException(descriptor);
      }
      this.ScrubIdentityPropertiesAndMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        readIdentity
      });
      return this.Request.CreateResponse<Identity_BackCompat>(HttpStatusCode.OK, new Identity_BackCompat(readIdentity));
    }

    [HttpGet]
    public HttpResponseMessage GetIdentityChanges(int identitySequenceId, int groupSequenceId)
    {
      this.TfsRequestContext.TraceEnter(850000, "Identity", nameof (IdentitiesController_BackCompatController), nameof (GetIdentityChanges));
      try
      {
        ChangedIdentities identityChanges = this.TfsRequestContext.GetService<IdentityService>().IdentityServiceInternal().GetIdentityChanges(this.TfsRequestContext, new ChangedIdentitiesContext(identitySequenceId, groupSequenceId));
        this.ScrubIdentityPropertiesAndMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityChanges.Identities);
        return this.Request.CreateResponse<ChangedIdentities_BackCompat>(HttpStatusCode.OK, new ChangedIdentities_BackCompat(identityChanges));
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(850008, "Identity", nameof (IdentitiesController_BackCompatController), ex);
        TeamFoundationEventLog.Default.LogException(ex.Message, ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(850009, "Identity", nameof (IdentitiesController_BackCompatController), nameof (GetIdentityChanges));
      }
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) IdentitiesController.s_httpIdentityServiceExceptions;

    public override string TraceArea => "IdentityService";

    protected void ScrubIdentityProperties(
      IEnumerable<Identity_BackCompat> identities,
      bool forUpdate = false)
    {
      if (identities == null || !identities.Any<Identity_BackCompat>())
        return;
      bool flag = this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Application) && IdentitiesControllerBase.DeploymentAccessChecker.HasDeploymentAccess(this.TfsRequestContext) && !IdentityHelper.IsShardedFrameworkIdentity(this.TfsRequestContext, this.TfsRequestContext.UserContext);
      foreach (Identity_BackCompat identity in identities)
      {
        if (identity != null)
          IdentitiesControllerBase.ScrubIdentityProperties((IDictionary<string, object>) identity.Properties);
        if (forUpdate & flag)
          identity.SetAllModifiedProperties();
        else
          identity.Properties.Clear();
      }
    }

    internal class IdentityUpdateData
    {
      public int Index;
      public Guid Id;
    }
  }
}
