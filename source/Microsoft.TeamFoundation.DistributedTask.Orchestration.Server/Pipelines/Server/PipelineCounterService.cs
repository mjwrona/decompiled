// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.PipelineCounterService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal class PipelineCounterService : IPipelineCounterService, IVssFrameworkService
  {
    private const string c_layer = "PipelineCounterService";

    public async Task<List<CounterVariable>> GetAllCountersAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int definitionId)
    {
      List<CounterVariable> allCountersAsync;
      using (new MethodScope(requestContext, nameof (PipelineCounterService), nameof (GetAllCountersAsync)))
      {
        using (Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterComponent component = requestContext.CreateComponent<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterComponent>())
          allCountersAsync = await component.GetAllCountersAsync(projectId, planType, definitionId);
      }
      return allCountersAsync;
    }

    public async Task<CounterVariable> GetCounterAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int definitionId,
      string prefix)
    {
      CounterVariable counterAsync;
      using (new MethodScope(requestContext, nameof (PipelineCounterService), nameof (GetCounterAsync)))
      {
        using (Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterComponent component = requestContext.CreateComponent<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterComponent>())
          counterAsync = await component.GetCounterAsync(projectId, planType, definitionId, prefix);
      }
      return counterAsync;
    }

    public async Task<int> IncrementCounterAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int definitionId,
      Guid planId,
      string prefix,
      int seed)
    {
      int num;
      using (new MethodScope(requestContext, nameof (PipelineCounterService), nameof (IncrementCounterAsync)))
      {
        using (Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterComponent component = requestContext.CreateComponent<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterComponent>())
          num = await component.IncrementCounterAsync(projectId, planType, definitionId, planId, prefix, seed);
      }
      return num;
    }

    public async Task RemoveAllCountersAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int definitionId)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (PipelineCounterService), nameof (RemoveAllCountersAsync));
      try
      {
        using (Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterComponent component = requestContext.CreateComponent<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterComponent>())
          await component.RemoveAllCountersAsync(projectId, planType, definitionId);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public async Task RemoveCounterAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int definitionId,
      string prefix)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (PipelineCounterService), nameof (RemoveCounterAsync));
      try
      {
        using (Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterComponent component = requestContext.CreateComponent<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterComponent>())
          await component.RemoveAllCountersAsync(projectId, planType, definitionId);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
