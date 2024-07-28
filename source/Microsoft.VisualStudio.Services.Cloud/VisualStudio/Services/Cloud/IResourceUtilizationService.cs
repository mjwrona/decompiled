// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.IResourceUtilizationService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DefaultServiceImplementation(typeof (ResourceUtilization2Service), typeof (VirtualResourceUtilizationService))]
  internal interface IResourceUtilizationService : IVssFrameworkService
  {
    Task ThrottleRequestAsync(IVssRequestContext requestContext, RUStage stage);

    void ThrottleRequest(IVssRequestContext requestContext);

    void QueuePreMethodResourcesIncrementData(IVssRequestContext requestContext);

    void QueuePostMethodResourcesIncrementData(IVssRequestContext requestContext);

    long GetAccumulatedUsage(
      IVssRequestContext requestContext,
      string resourceName,
      Guid namespaceId,
      int windowSeconds,
      string key);

    int SetRUMacro(IVssRequestContext requestContext, string macroName, string macroDefinition);

    int SetRURule(IVssRequestContext requestContext, string ruleName, string definition);

    int SetRUThreshold(
      IVssRequestContext requestContext,
      string ruleName,
      string entity,
      string flag,
      string tarpit,
      string block,
      string dpMagniture,
      string note,
      string expirationTime);

    int DeleteRUMacro(IVssRequestContext requestContext, string macroName);

    int DeleteRURule(IVssRequestContext requestContext, string ruleName);

    int DeleteRUThreshold(IVssRequestContext requestContext, string ruleName, string entity);
  }
}
