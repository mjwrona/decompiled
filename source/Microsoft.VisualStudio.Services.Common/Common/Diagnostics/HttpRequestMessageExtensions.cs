// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Diagnostics.HttpRequestMessageExtensions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Common.Diagnostics
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class HttpRequestMessageExtensions
  {
    public static VssHttpMethod GetHttpMethod(this HttpRequestMessage message)
    {
      string method = message.Method.Method;
      VssHttpMethod httpMethod = VssHttpMethod.UNKNOWN;
      ref VssHttpMethod local = ref httpMethod;
      if (!Enum.TryParse<VssHttpMethod>(method, true, out local))
        httpMethod = VssHttpMethod.UNKNOWN;
      return httpMethod;
    }

    public static VssTraceActivity GetActivity(this HttpRequestMessage message)
    {
      object obj;
      return !message.Properties.TryGetValue("MS.VSS.Diagnostics.TraceActivity", out obj) ? VssTraceActivity.Empty : (VssTraceActivity) obj;
    }
  }
}
