// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.Server.Controllers.VirtualMachines2Controller
// Assembly: Microsoft.Azure.Pipelines.Environments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A0C9A0D-816B-442F-8D21-CE0F4EA438AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Environments.Server.Controllers
{
  [ControllerApiVersion(5.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "environments", ResourceName = "virtualmachines")]
  public sealed class VirtualMachines2Controller : EnvironmentsProjectApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<VirtualMachine>), null, null)]
    public async Task<HttpResponseMessage> GetVirtualMachines(
      int environmentId,
      int resourceId,
      [ClientQueryParameter] string continuationToken = null,
      [ClientQueryParameter] string name = null,
      [ClientQueryParameter] bool partialNameMatch = false,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string tags = null,
      [FromUri(Name = "$top")] int top = 1000)
    {
      VirtualMachines2Controller machines2Controller = this;
      if (top > 1000)
        throw new ArgumentOutOfRangeException(EnvironmentResources.RequestedMoreThanMaxSupport());
      IVirtualMachineGroupService service = machines2Controller.TfsRequestContext.GetService<IVirtualMachineGroupService>();
      IVssRequestContext tfsRequestContext = machines2Controller.TfsRequestContext;
      Guid projectId = machines2Controller.ProjectId;
      int environmentId1 = environmentId;
      int resourceId1 = resourceId;
      string continuationToken1 = continuationToken;
      int top1 = top;
      string name1 = name;
      int num = partialNameMatch ? 1 : 0;
      string str = tags;
      List<string> tagFilters;
      if (str == null)
        tagFilters = (List<string>) null;
      else
        tagFilters = ((IEnumerable<string>) str.Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
      IPagedList<VirtualMachine> virtualMachines = await service.GetVirtualMachines(tfsRequestContext, projectId, environmentId1, resourceId1, continuationToken1, top1, name1, num != 0, (IList<string>) tagFilters);
      HttpResponseMessage response = machines2Controller.Request.CreateResponse<IPagedList<VirtualMachine>>(HttpStatusCode.OK, virtualMachines);
      if (!string.IsNullOrWhiteSpace(virtualMachines.ContinuationToken))
        EnvironmentsProjectApiController.SetContinuationToken(response, virtualMachines.ContinuationToken);
      return response;
    }

    [HttpPatch]
    public IList<VirtualMachine> UpdateVirtualMachines(
      int environmentId,
      int resourceId,
      IList<VirtualMachine> machines)
    {
      return this.TfsRequestContext.GetService<IVirtualMachineGroupService>().UpdateVirtualMachines(this.TfsRequestContext, this.ProjectId, environmentId, resourceId, machines);
    }
  }
}
