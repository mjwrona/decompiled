// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityBatchController_BackCompatController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
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
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "IdentityBatch")]
  public class IdentityBatchController_BackCompatController : IdentitiesControllerBase
  {
    private const string s_identityIds = "identityIds";
    private const string s_identityDescriptors = "identityDescriptors";
    private const string s_properties = "properties";
    private const string s_area = "Identity";
    private const string s_layer = "IdentityBatchController";
    private const string s_methodReadIdentityBatch = "ReadIdentityBatch";

    [HttpPost]
    public HttpResponseMessage ReadIdentityBatch(JObject container)
    {
      this.TfsRequestContext.TraceEnter(850550, "Identity", "IdentityBatchController", nameof (ReadIdentityBatch));
      try
      {
        JToken jtoken1 = container["identityIds"];
        JToken jtoken2 = container["identityDescriptors"];
        if (jtoken1 == null && jtoken2 == null)
          throw new ArgumentException(HostingResources.OneOfTwoRequiredArgumentsIsMissing((object) "identityIds", (object) "identityDescriptors"));
        JToken jtoken3 = container["properties"];
        string properties = jtoken3 == null ? string.Empty : jtoken3.ToObject<string>();
        return this.Request.CreateResponse<IQueryable<Identity_BackCompat>>(HttpStatusCode.OK, jtoken1 != null ? this.ReadIdentitiesByIds(jtoken1.ToObject<string>(), properties) : this.ReadIdentitiesByDescriptors(jtoken2.ToObject<string>(), properties));
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(850558, "Identity", "IdentityBatchController", ex);
        TeamFoundationEventLog.Default.LogException(ex.Message, ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(850559, "Identity", "IdentityBatchController", nameof (ReadIdentityBatch));
      }
    }

    private IQueryable<Identity_BackCompat> ReadIdentitiesByIds(
      string identityIds,
      string properties)
    {
      IList<Guid> ids = !string.IsNullOrEmpty(identityIds) ? IdentityParser.GetIdentityIdsFromString(identityIds) : throw new ArgumentNullException(nameof (identityIds));
      if (ids == null || ids.Count == 0)
        throw new ArgumentNullException(nameof (identityIds));
      IEnumerable<string> filtersFromString = IdentityParser.GetPropertyFiltersFromString(properties);
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      this.TfsRequestContext.GetService<IPlatformIdentityServiceInternal>().CheckForLeakedMasterIds(this.TfsRequestContext, (IEnumerable<Guid>) ids);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      IList<Guid> identityIds1 = ids;
      IEnumerable<string> propertyNameFilters = filtersFromString;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(tfsRequestContext, identityIds1, QueryMembership.None, propertyNameFilters);
      this.ScrubMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList);
      return Identity_BackCompat.Convert(identityList).AsQueryable<Identity_BackCompat>();
    }

    private IQueryable<Identity_BackCompat> ReadIdentitiesByDescriptors(
      string identityDescriptors,
      string properties)
    {
      IList<IdentityDescriptor> descriptors = !string.IsNullOrEmpty(identityDescriptors) ? IdentityParser.GetDescriptorsFromString(identityDescriptors) : throw new ArgumentNullException(nameof (identityDescriptors));
      if (descriptors == null || descriptors.Count == 0)
        throw new ArgumentNullException(nameof (identityDescriptors));
      IEnumerable<string> filtersFromString = IdentityParser.GetPropertyFiltersFromString(properties);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, descriptors, QueryMembership.None, filtersFromString);
      this.ScrubMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList);
      return Identity_BackCompat.Convert(identityList).AsQueryable<Identity_BackCompat>();
    }
  }
}
