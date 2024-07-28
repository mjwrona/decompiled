// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.Tracing.IAzureClientTracingInterceptor
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk.Tracing
{
  public interface IAzureClientTracingInterceptor
  {
    void Configuration(Guid activityId, string source, string name, string value);

    void EnterMethod(
      Guid activityId,
      string invocationId,
      object instance,
      string method,
      IDictionary<string, object> parameters);

    void ExitMethod(
      Guid activityId,
      string invocationId,
      object instance,
      string method,
      object returnValue);

    void Information(Guid activityId, string invocationId, string message);

    void ReceiveResponse(Guid activityId, string invocationId, HttpResponseMessage response);

    void SendRequest(Guid activityId, string invocationId, HttpRequestMessage request);

    void TraceError(Guid activityId, string invocationId, Exception exception);
  }
}
