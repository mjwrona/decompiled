// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.TempQueryController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "tempQueries")]
  public class TempQueryController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5924000;

    public override string TraceArea => "tempQueries";

    [TraceFilter(5924000, 5924010)]
    [HttpPost]
    [ClientLocationId("9f614388-a9f0-4952-ad6c-89756bd8e388")]
    [ClientResponseType(typeof (TemporaryQueryResponseModel), null, null)]
    [ClientExample("POST__wit_tempQueries_create.json", "Create a temporary query", null, null)]
    public HttpResponseMessage CreateTempQuery([FromBody] TemporaryQueryRequestModel postedQuery)
    {
      if (postedQuery == null)
        throw new VssPropertyValidationException(nameof (postedQuery), ResourceStrings.NullQueryParameter());
      if (string.IsNullOrEmpty(postedQuery.Wiql))
        throw new VssPropertyValidationException("Wiql", ResourceStrings.MissingQueryParameter((object) "Wiql"));
      QueryUtils.ValidateQuery(postedQuery.Wiql, this.TfsRequestContext, this.ProjectId, this.Team?.Name, out string _);
      JObject jobject = JObject.FromObject((object) new TemporaryQueryModel()
      {
        QueryText = postedQuery.Wiql
      }, new JsonSerializer()
      {
        ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
      });
      return this.Request.CreateResponse<TemporaryQueryResponseModel>(HttpStatusCode.OK, new TemporaryQueryResponseModel()
      {
        Id = this.ProcessTemporyQuery(jobject.ToString())
      });
    }

    private Guid ProcessTemporyQuery(string data)
    {
      try
      {
        return this.TfsRequestContext.GetService<ITeamFoundationTemporaryDataService>().CreateTemporaryDataWithPropertyService(this.TfsRequestContext, data, "Wit").Id;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5924001, "wit", nameof (TempQueryController), ex);
        throw;
      }
    }
  }
}
