// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.VariableGroupController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "variablegroups", ResourceVersion = 2)]
  public class VariableGroupController : DistributedTaskProjectApiController
  {
    public override string ActivityLogArea => "VariableGroup";

    private IVariableGroupService VariableGroupService => this.TfsRequestContext.GetService<IVariableGroupService>();

    [HttpPost]
    [ClientLocationId("EF5B7057-FFC3-4C77-BBAD-C10B4A4ABCC7")]
    [ClientExample("POST_CreateVariableGroup.json", "Add a variable group", null, null)]
    public VariableGroup AddVariableGroup(VariableGroupParameters variableGroupParameters)
    {
      if (variableGroupParameters == null)
        throw new ArgumentNullException(nameof (variableGroupParameters));
      return this.VariableGroupService.AddVariableGroupForCollection(this.TfsRequestContext, variableGroupParameters);
    }

    [HttpPut]
    [ClientLocationId("EF5B7057-FFC3-4C77-BBAD-C10B4A4ABCC7")]
    public VariableGroup UpdateVariableGroup(
      int groupId,
      VariableGroupParameters variableGroupParameters)
    {
      if (variableGroupParameters == null)
        throw new ArgumentNullException(nameof (variableGroupParameters));
      return this.VariableGroupService.UpdateVariableGroupForCollection(this.TfsRequestContext, groupId, variableGroupParameters);
    }

    [HttpPatch]
    [ClientLocationId("EF5B7057-FFC3-4C77-BBAD-C10B4A4ABCC7")]
    public void ShareVariableGroup(
      int variableGroupId,
      IList<VariableGroupProjectReference> variableGroupProjectReferences)
    {
      if (variableGroupProjectReferences == null)
        throw new ArgumentNullException(nameof (variableGroupProjectReferences));
      this.VariableGroupService.ShareVariableGroupForCollection(this.TfsRequestContext, variableGroupId, variableGroupProjectReferences);
    }

    [HttpDelete]
    [ClientLocationId("EF5B7057-FFC3-4C77-BBAD-C10B4A4ABCC7")]
    public void DeleteVariableGroup(int groupId, [ClientParameterAsIEnumerable(typeof (string), ',')] string projectIds)
    {
      IList<Guid> projectIdsAsGuid = VariableGroupController.GetProjectIdsAsGuid(projectIds);
      this.VariableGroupService.DeleteVariableGroupForCollection(this.TfsRequestContext, groupId, projectIdsAsGuid);
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
      top = top > 1000 ? 1000 : top;
      IList<VariableGroup> variableGroups = this.VariableGroupService.GetVariableGroups(this.TfsRequestContext, this.ProjectId, groupName, actionFilter, continuationToken, top + 1, queryOrder);
      return LegacyVariableGroupController.CreateResponseForGetVariableGroups(this.Request, top, queryOrder, variableGroups);
    }

    private static IList<Guid> GetProjectIdsAsGuid(string projectIds)
    {
      if (string.IsNullOrWhiteSpace(projectIds))
        return (IList<Guid>) null;
      IList<Guid> projectIdsAsGuid = (IList<Guid>) new List<Guid>();
      List<string> list = ((IEnumerable<string>) projectIds.Split(',')).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).ToList<string>();
      if (list != null)
      {
        List<string> stringList = new List<string>();
        foreach (string input in list)
        {
          Guid result;
          if (Guid.TryParse(input, out result))
            projectIdsAsGuid.Add(result);
          else
            stringList.Add(input);
        }
        if (stringList.Any<string>())
          throw new ArgumentException(Microsoft.TeamFoundation.DistributedTask.Server.TaskResources.InvalidProjectIds((object) string.Join(", ", (IEnumerable<string>) stringList)));
      }
      return projectIdsAsGuid;
    }
  }
}
