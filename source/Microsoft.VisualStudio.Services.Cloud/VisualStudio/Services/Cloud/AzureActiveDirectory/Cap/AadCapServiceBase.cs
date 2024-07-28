// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Cap.AadCapServiceBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Cap
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal abstract class AadCapServiceBase : IAadCapService, IVssFrameworkService
  {
    protected const string TraceArea = "AzureActiveDirectory";
    private const string TraceLayer = "AadCapServiceBase";

    public virtual void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckOrganizationOnlyRequestContext();

    public virtual void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public AadCapResult IsUserConditionAllowed(
      IVssRequestContext context,
      Guid tenantId,
      SubjectDescriptor subjectDescriptor,
      string clientIp)
    {
      context.TraceEnter(9003000, "AzureActiveDirectory", nameof (AadCapServiceBase), nameof (IsUserConditionAllowed));
      context.CheckOrganizationOnlyRequestContext();
      try
      {
        IAadCapCacheService service = context.GetService<IAadCapCacheService>();
        AadCapResult cacheResult = service.CheckIsUserConditionAllowed(context, tenantId, subjectDescriptor, clientIp);
        if (cacheResult != null)
        {
          context.TraceDataConditionally(9003001, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapServiceBase), "Cached CAP result", (Func<object>) (() => (object) new
          {
            tenantId = tenantId,
            subjectDescriptor = subjectDescriptor,
            clientIp = clientIp,
            cacheResult = cacheResult
          }), nameof (IsUserConditionAllowed));
          return cacheResult;
        }
        AadCapResult result = this.CheckIsUserConditionAllowed(context, tenantId, subjectDescriptor, clientIp);
        if (result.Allowed)
          service.Set(context, subjectDescriptor, clientIp, result);
        context.TraceDataConditionally(9003002, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapServiceBase), "Return CAP result", (Func<object>) (() => (object) new
        {
          tenantId = tenantId,
          subjectDescriptor = subjectDescriptor,
          clientIp = clientIp,
          result = result
        }), nameof (IsUserConditionAllowed));
        return result;
      }
      finally
      {
        context.TraceLeave(9003000, "AzureActiveDirectory", nameof (AadCapServiceBase), nameof (IsUserConditionAllowed));
      }
    }

    protected abstract AadCapResult CheckIsUserConditionAllowed(
      IVssRequestContext context,
      Guid tenantId,
      SubjectDescriptor subjectDescriptor,
      string ip);
  }
}
