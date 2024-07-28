// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.LegacyVariableGroupController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "variablegroups", ResourceVersion = 1)]
  public class LegacyVariableGroupController : DistributedTaskProjectApiController
  {
    public override string ActivityLogArea => "VariableGroup";

    private IVariableGroupService VariableGroupService => this.TfsRequestContext.GetService<IVariableGroupService>();

    public static HttpResponseMessage CreateResponseForGetVariableGroups(
      HttpRequestMessage request,
      int top,
      VariableGroupQueryOrder queryOrder,
      IList<VariableGroup> variableGroups)
    {
      IEnumerable<VariableGroup> variableGroups1 = top < 0 ? (IEnumerable<VariableGroup>) variableGroups : variableGroups.Take<VariableGroup>(top);
      HttpResponseMessage responseMessage = (HttpResponseMessage) null;
      try
      {
        responseMessage = request.CreateResponse<IEnumerable<VariableGroup>>(HttpStatusCode.OK, variableGroups1);
        if (top >= 0)
        {
          if (variableGroups.Count > top)
          {
            if (variableGroups[top] != null)
            {
              string tokenValue = (string) null;
              switch (queryOrder)
              {
                case VariableGroupQueryOrder.IdAscending:
                case VariableGroupQueryOrder.IdDescending:
                  tokenValue = variableGroups[top].Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                  break;
              }
              DistributedTaskProjectApiController.SetContinuationToken(responseMessage, tokenValue);
            }
          }
        }
      }
      catch
      {
        responseMessage?.Dispose();
        throw;
      }
      return responseMessage;
    }

    [HttpPost]
    [ClientLocationId("F5B09DD5-9D54-45A1-8B5A-1C8287D634CC")]
    [ClientExample("POST_CreateVariableGroup.json", "Add a variable group", null, null)]
    public VariableGroup AddVariableGroup(VariableGroupParameters group)
    {
      if (group == null)
        throw new ArgumentNullException(nameof (group));
      return this.VariableGroupService.AddVariableGroup(this.TfsRequestContext, this.ProjectId, group);
    }

    [HttpPut]
    [ClientLocationId("F5B09DD5-9D54-45A1-8B5A-1C8287D634CC")]
    [ClientExample("PUT_UpdateVariableGroup.json", "Update a variable group", null, null)]
    public VariableGroup UpdateVariableGroup(int groupId, VariableGroupParameters group)
    {
      if (group == null)
        throw new ArgumentNullException(nameof (group));
      return this.VariableGroupService.UpdateVariableGroup(this.TfsRequestContext, this.ProjectId, groupId, group);
    }

    [HttpGet]
    [ClientLocationId("F5B09DD5-9D54-45A1-8B5A-1C8287D634CC")]
    [ClientExample("GET_VariableGroup.json", "Get a variable group", null, null)]
    public VariableGroup GetVariableGroup(int groupId) => this.VariableGroupService.GetVariableGroup(this.TfsRequestContext, this.ProjectId, groupId);

    [HttpGet]
    [ClientLocationId("F5B09DD5-9D54-45A1-8B5A-1C8287D634CC")]
    [ClientExample("GET_VariableGroupsByIds.json", "Get variable groups by ids", null, null)]
    public IList<VariableGroup> GetVariableGroupsById([ClientParameterAsIEnumerable(typeof (int), ',')] string groupIds) => this.VariableGroupService.GetVariableGroups(this.TfsRequestContext, this.ProjectId, (IList<int>) groupIds.ParseArray().ToList<int>());

    [HttpGet]
    [ClientLocationId("F5B09DD5-9D54-45A1-8B5A-1C8287D634CC")]
    [ClientResponseType(typeof (IEnumerable<VariableGroup>), null, null)]
    [ClientExample("GET_VariableGroupsByGroupName.json", "Get variable groups using group name filter", null, null)]
    [ClientExample("GET_VariableGroupsByActionFilter.json", "Get variable groups using action filter", null, null)]
    public HttpResponseMessage GetVariableGroups(
      [ClientQueryParameter] string groupName = null,
      VariableGroupActionFilter actionFilter = VariableGroupActionFilter.None,
      [FromUri(Name = "$top")] int top = -1,
      int? continuationToken = null,
      VariableGroupQueryOrder queryOrder = VariableGroupQueryOrder.IdDescending)
    {
      IList<VariableGroup> variableGroups = this.VariableGroupService.GetVariableGroups(this.TfsRequestContext, this.ProjectId, groupName, actionFilter, continuationToken, top + 1, queryOrder);
      return LegacyVariableGroupController.CreateResponseForGetVariableGroups(this.Request, top, queryOrder, variableGroups);
    }

    [HttpDelete]
    [ClientLocationId("F5B09DD5-9D54-45A1-8B5A-1C8287D634CC")]
    [ClientExample("DELETE_VariableGroup.json", "Delete a variable group", null, null)]
    public void DeleteVariableGroup(int groupId) => this.VariableGroupService.DeleteVariableGroup(this.TfsRequestContext, this.ProjectId, groupId);
  }
}
