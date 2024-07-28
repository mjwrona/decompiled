// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Authentication.ITestableAuthenticationContext
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common.Authentication
{
  [CLSCompliant(false)]
  public interface ITestableAuthenticationContext
  {
    Task<AuthenticationResult> AcquireTokenAsync(
      string[] scopes,
      string username,
      string password,
      Guid? correlationId);

    Task<AuthenticationResult> AcquireTokenAsync(
      string[] scopes,
      Prompt promptBehavior,
      Guid? correlationId);

    Task<AuthenticationResult> AcquireTokenSilentAsync(string[] scopes, Guid? correlationId);

    PublicClientApplicationBuilder ApplicationBuilder { get; }

    ITokenCache TokenCache { get; }
  }
}
