// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.IExternalProviderHttpRequester
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IExternalProviderHttpRequester : IDisposable
  {
    bool SendRequest(
      HttpRequestMessage message,
      HttpCompletionOption option,
      out HttpResponseMessage response,
      out HttpStatusCode code,
      out string errorMessage);

    Task<HttpRequestResult> SendRequestAsync(
      HttpRequestMessage message,
      HttpCompletionOption option);
  }
}
