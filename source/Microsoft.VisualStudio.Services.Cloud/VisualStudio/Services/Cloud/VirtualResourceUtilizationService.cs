// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.VirtualResourceUtilizationService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class VirtualResourceUtilizationService : 
    IResourceUtilizationService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Task ThrottleRequestAsync(IVssRequestContext requestContext, RUStage stage) => throw new VirtualServiceHostException();

    public void ThrottleRequest(IVssRequestContext requestContext) => throw new VirtualServiceHostException();

    public void QueuePreMethodResourcesIncrementData(IVssRequestContext requestContext) => throw new VirtualServiceHostException();

    public void QueuePostMethodResourcesIncrementData(IVssRequestContext requestContext) => throw new VirtualServiceHostException();

    public long GetAccumulatedUsage(
      IVssRequestContext requestContext,
      string resourceName,
      Guid namespaceId,
      int windowSeconds,
      string key)
    {
      throw new VirtualServiceHostException();
    }

    public int SetRUMacro(
      IVssRequestContext requestContext,
      string macroName,
      string macroDefinition)
    {
      throw new NotImplementedException();
    }

    public int SetRURule(IVssRequestContext requestContext, string ruleName, string definition) => throw new NotImplementedException();

    public int SetRUThreshold(
      IVssRequestContext requestContext,
      string ruleName,
      string entity,
      string flag,
      string tarpit,
      string block,
      string dpMagniture,
      string note,
      string expirationTime)
    {
      throw new NotImplementedException();
    }

    public int DeleteRUMacro(IVssRequestContext requestContext, string macroName) => throw new NotImplementedException();

    public int DeleteRURule(IVssRequestContext requestContext, string ruleName) => throw new NotImplementedException();

    public int DeleteRUThreshold(IVssRequestContext requestContext, string ruleName, string entity) => throw new NotImplementedException();
  }
}
