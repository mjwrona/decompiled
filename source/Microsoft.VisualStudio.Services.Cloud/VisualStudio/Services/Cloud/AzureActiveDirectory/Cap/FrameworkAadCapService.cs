// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Cap.FrameworkAadCapService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.AadCap.WebApi;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Cap
{
  internal class FrameworkAadCapService : AadCapServiceBase
  {
    private const string TraceLayer = "FrameworkAadCapService";

    protected override AadCapResult CheckIsUserConditionAllowed(
      IVssRequestContext context,
      Guid tenantId,
      SubjectDescriptor subjectDescriptor,
      string clientIp)
    {
      context.TraceEnter(9003100, "AzureActiveDirectory", nameof (FrameworkAadCapService), nameof (CheckIsUserConditionAllowed));
      try
      {
        context.TraceDataConditionally(9003102, TraceLevel.Verbose, "AzureActiveDirectory", nameof (FrameworkAadCapService), "Client CAP Call with", (Func<object>) (() => (object) new
        {
          tenantId = tenantId,
          subjectDescriptor = subjectDescriptor,
          clientIp = clientIp
        }), nameof (CheckIsUserConditionAllowed));
        AadCapResult result = context.GetClient<AadConditionalAccessPolicyHttpClient>().ValidateUserAccessConditionAsync(tenantId, (string) subjectDescriptor, clientIp).SyncResult<AadCapResult>();
        context.TraceDataConditionally(9003102, TraceLevel.Verbose, "AzureActiveDirectory", nameof (FrameworkAadCapService), "Client CAP result", (Func<object>) (() => (object) new
        {
          tenantId = tenantId,
          subjectDescriptor = subjectDescriptor,
          clientIp = clientIp,
          result = result
        }), nameof (CheckIsUserConditionAllowed));
        return result;
      }
      finally
      {
        context.TraceLeave(9003100, "AzureActiveDirectory", nameof (FrameworkAadCapService), nameof (CheckIsUserConditionAllowed));
      }
    }
  }
}
