// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.Server.Controllers.VirtualMachinesResourceController
// Assembly: Microsoft.Azure.Pipelines.Environments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A0C9A0D-816B-442F-8D21-CE0F4EA438AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.Converters;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Environments.Server.Controllers
{
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "environments", ResourceName = "vmresource")]
  public sealed class VirtualMachinesResourceController : EnvironmentsProjectApiController
  {
    [HttpPost]
    [FeatureEnabled("DistributedTask.Environments.EnvironmentVirtualMachineResource")]
    public async Task<VirtualMachineResource> AddVirtualMachineResource(
      int environmentId,
      [FromBody] VirtualMachineResourceCreateParameters createParameters)
    {
      VirtualMachinesResourceController resourceController = this;
      ArgumentUtility.CheckForNull<VirtualMachineResourceCreateParameters>(createParameters, nameof (createParameters));
      return await resourceController.TfsRequestContext.GetService<IVirtualMachineResourceService>().AddEnvironmentResourceAsync(resourceController.TfsRequestContext, resourceController.ProjectId, createParameters.ToResource(environmentId));
    }

    [HttpGet]
    [FeatureEnabled("DistributedTask.Environments.EnvironmentVirtualMachineResource")]
    [ClientResponseType(typeof (IEnumerable<VirtualMachineResource>), null, null)]
    public async Task<HttpResponseMessage> GetVirtualMachineResources(
      int environmentId,
      [ClientQueryParameter] string name = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string tags = null,
      [ClientQueryParameter] string continuationToken = null,
      [FromUri(Name = "$top")] int top = 1000)
    {
      VirtualMachinesResourceController resourceController = this;
      if (top > 1000)
        throw new ArgumentOutOfRangeException();
      IVirtualMachineResourceService service = resourceController.TfsRequestContext.GetService<IVirtualMachineResourceService>();
      IVssRequestContext tfsRequestContext = resourceController.TfsRequestContext;
      Guid projectId = resourceController.ProjectId;
      int environmentId1 = environmentId;
      string continuationToken1 = continuationToken;
      int top1 = top;
      string name1 = name;
      string str = tags;
      string[] tagFilters;
      if (str == null)
        tagFilters = (string[]) null;
      else
        tagFilters = str.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
      IPagedList<VirtualMachineResource> machinesPagedAsync = await service.GetVirtualMachinesPagedAsync(tfsRequestContext, projectId, environmentId1, continuationToken1, top1, name1, (IList<string>) tagFilters);
      HttpResponseMessage response = resourceController.Request.CreateResponse<IPagedList<VirtualMachineResource>>(HttpStatusCode.OK, machinesPagedAsync);
      if (!string.IsNullOrWhiteSpace(machinesPagedAsync.ContinuationToken))
        EnvironmentsProjectApiController.SetContinuationToken(response, machinesPagedAsync.ContinuationToken);
      return response;
    }

    [HttpGet]
    [ClientInternalUseOnly(false)]
    public VirtualMachineResource GetVirtualMachineResource(int environmentId, int resourceId) => throw new NotImplementedException();

    [HttpPatch]
    [FeatureEnabled("DistributedTask.Environments.EnvironmentVirtualMachineResource")]
    public VirtualMachineResource UpdateVirtualMachineResource(
      int environmentId,
      [FromBody] VirtualMachineResource resource)
    {
      return this.TfsRequestContext.GetService<IVirtualMachineResourceService>().UpdateVirtualMachineResource(this.TfsRequestContext, this.ProjectId, environmentId, resource, TaskAgentCapabilityType.None);
    }

    [HttpPut]
    [FeatureEnabled("DistributedTask.Environments.EnvironmentVirtualMachineResource")]
    public VirtualMachineResource ReplaceVirtualMachineResource(
      int environmentId,
      [FromBody] VirtualMachineResource resource)
    {
      return this.TfsRequestContext.GetService<IVirtualMachineResourceService>().UpdateVirtualMachineResource(this.TfsRequestContext, this.ProjectId, environmentId, resource);
    }

    [HttpDelete]
    [FeatureEnabled("DistributedTask.Environments.EnvironmentVirtualMachineResource")]
    public void DeleteVirtualMachineResource(int environmentId, int resourceId) => this.TfsRequestContext.GetService<IVirtualMachineResourceService>().DeleteVirtualMachineResource(this.TfsRequestContext, this.ProjectId, environmentId, resourceId);
  }
}
