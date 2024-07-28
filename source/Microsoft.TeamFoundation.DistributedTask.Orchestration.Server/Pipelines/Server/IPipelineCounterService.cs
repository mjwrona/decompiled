// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.IPipelineCounterService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  [DefaultServiceImplementation(typeof (PipelineCounterService))]
  public interface IPipelineCounterService : IVssFrameworkService
  {
    Task<List<CounterVariable>> GetAllCountersAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int definitionId);

    Task<CounterVariable> GetCounterAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int definitionId,
      string prefix);

    Task<int> IncrementCounterAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int definitionId,
      Guid planId,
      string prefix,
      int seed);

    Task RemoveAllCountersAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int definitionId);

    Task RemoveCounterAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int definitionId,
      string prefix);
  }
}
