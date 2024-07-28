// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.ExternalProviderHttpRequesterExtensions
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  public static class ExternalProviderHttpRequesterExtensions
  {
    public static HttpResponseMessage SendRequest(
      this IExternalProviderHttpRequester requester,
      HttpRequestMessage message)
    {
      ArgumentUtility.CheckForNull<IExternalProviderHttpRequester>(requester, nameof (requester));
      HttpResponseMessage response;
      HttpStatusCode code;
      return !requester.SendRequest(message, HttpCompletionOption.ResponseContentRead, out response, out code, out string _) ? new HttpResponseMessage(code) : response ?? new HttpResponseMessage(code);
    }
  }
}
