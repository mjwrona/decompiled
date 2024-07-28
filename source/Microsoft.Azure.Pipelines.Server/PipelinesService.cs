// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.PipelinesService
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Server.Migration;
using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Server
{
  internal class PipelinesService : IPipelinesService, IVssFrameworkService
  {
    private const string TraceLayer = "PipelinesService";

    public Task<IList<Pipeline>> GetPipelinesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      ICollection<int> pipelineIds)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<ICollection<int>>(pipelineIds, nameof (pipelineIds));
      using (requestContext.TraceScope(nameof (PipelinesService), nameof (GetPipelinesAsync)))
        return Task.FromResult<IList<Pipeline>>((IList<Pipeline>) Array.Empty<Pipeline>());
    }

    public Pipeline CreatePipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      CreatePipelineParameters parameters)
    {
      return requestContext.GetService<IExternalPipelinesService>().CreatePipeline(requestContext, projectId, parameters);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
