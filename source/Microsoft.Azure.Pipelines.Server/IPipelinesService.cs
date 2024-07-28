// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.IPipelinesService
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Server
{
  [DefaultServiceImplementation(typeof (PipelinesService))]
  public interface IPipelinesService : IVssFrameworkService
  {
    Task<IList<Pipeline>> GetPipelinesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      ICollection<int> pipelineIds);

    Pipeline CreatePipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      CreatePipelineParameters parameters);
  }
}
