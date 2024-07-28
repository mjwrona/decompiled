// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentPoolMetadataController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientInternalUseOnly(true, OmitFromTypeScriptDeclareFile = false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "poolmetadata")]
  [RequestContentTypeRestriction(AllowStream = true)]
  public sealed class TaskAgentPoolMetadataController : DistributedTaskApiController
  {
    private static int MaxPutRequestSize = 1048576;

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetAgentPoolMetadata", "text/plain")]
    public async Task<HttpResponseMessage> GetAgentPoolMetadata(int poolId)
    {
      TaskAgentPoolMetadataController metadataController = this;
      List<RequestMediaType> supportedTypes = new List<RequestMediaType>()
      {
        RequestMediaType.Text
      };
      int num = (int) MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(metadataController.Request, supportedTypes).FirstOrDefault<RequestMediaType>();
      Stream poolMetadataAsync = await metadataController.ResourceService.GetAgentPoolMetadataAsync(metadataController.TfsRequestContext, poolId);
      if (poolMetadataAsync == null)
        return metadataController.Request.CreateResponse(HttpStatusCode.NotFound);
      HttpResponseMessage response = metadataController.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(poolMetadataAsync);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
      return response;
    }

    [HttpPut]
    [ClientRequestContentMediaType("application/octet-stream")]
    [ClientRequestBodyType(typeof (Stream), "agentPoolMetadata")]
    [ClientRequestContentIsRawData]
    public async Task SetAgentPoolMetadata(int poolId)
    {
      TaskAgentPoolMetadataController metadataController = this;
      List<RequestMediaType> supportedTypes = new List<RequestMediaType>()
      {
        RequestMediaType.OctetStream
      };
      int num = (int) MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(metadataController.Request, supportedTypes).FirstOrDefault<RequestMediaType>();
      TaskAgentPool agentPoolAsync = await metadataController.ResourceService.GetAgentPoolAsync(metadataController.TfsRequestContext, poolId);
      if (agentPoolAsync == null)
        throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
      TaskAgentPool taskAgentPool = new TaskAgentPool();
      taskAgentPool.Id = agentPoolAsync.Id;
      taskAgentPool.Name = agentPoolAsync.Name;
      TaskAgentPool updatePool = taskAgentPool;
      using (Stream poolMetadataStream = await metadataController.Request.Content.ReadAsStreamAsync())
      {
        if (poolMetadataStream.Length > (long) TaskAgentPoolMetadataController.MaxPutRequestSize)
          throw new ArgumentException(TaskResources.RequestMaxSizeExceeded((object) TaskAgentPoolMetadataController.MaxPutRequestSize), "contentStream");
        if (poolMetadataStream.Length == 0L)
        {
          metadataController.ResourceService.UpdateAgentPool(metadataController.TfsRequestContext, poolId, updatePool, true);
          updatePool = (TaskAgentPool) null;
        }
        else
        {
          metadataController.ResourceService.UpdateAgentPool(metadataController.TfsRequestContext, poolId, updatePool, poolMetadataStream: poolMetadataStream);
          updatePool = (TaskAgentPool) null;
        }
      }
    }
  }
}
