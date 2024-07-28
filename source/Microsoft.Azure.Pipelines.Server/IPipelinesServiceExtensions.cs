// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.IPipelinesServiceExtensions
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Server
{
  public static class IPipelinesServiceExtensions
  {
    public static Pipeline GetPipeline(
      this IPipelinesService pipelinesService,
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId)
    {
      return pipelinesService.GetPipelines(requestContext, projectId, (ICollection<int>) new int[1]
      {
        pipelineId
      }).FirstOrDefault<Pipeline>();
    }

    public static async Task<Pipeline> GetPipelineAsync(
      this IPipelinesService pipelinesService,
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId)
    {
      return (await pipelinesService.GetPipelinesAsync(requestContext, projectId, (ICollection<int>) new int[1]
      {
        pipelineId
      })).FirstOrDefault<Pipeline>();
    }

    public static IList<Pipeline> GetPipelines(
      this IPipelinesService pipelinesService,
      IVssRequestContext requestContext,
      Guid projectId,
      ICollection<int> pipelineIds)
    {
      return requestContext.RunSynchronously<IList<Pipeline>>((Func<Task<IList<Pipeline>>>) (() => pipelinesService.GetPipelinesAsync(requestContext, projectId, pipelineIds)));
    }
  }
}
