// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Authentication.TestableAuthenticationContext
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Client;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common.Authentication
{
  internal class TestableAuthenticationContext : ITestableAuthenticationContext
  {
    private IPublicClientApplication application;

    public TestableAuthenticationContext(PublicClientApplicationBuilder applicationBuilder)
    {
      this.ApplicationBuilder = applicationBuilder ?? throw new ArgumentNullException(nameof (applicationBuilder));
      this.application = applicationBuilder.Build();
    }

    public Task<AuthenticationResult> AcquireTokenAsync(
      string[] scopes,
      string username,
      string password,
      Guid? correlationId)
    {
      return this.application.AcquireTokenByUsernamePassword((IEnumerable<string>) scopes, username, password).WithOptionalCorrelationId<AcquireTokenByUsernamePasswordParameterBuilder>(correlationId).ExecuteAsync();
    }

    public Task<AuthenticationResult> AcquireTokenAsync(
      string[] scopes,
      Prompt promptBehavior,
      Guid? correlationId)
    {
      return this.ApplicationBuilder.WithRedirectUri(VssAadSettings.NativeClientRedirectUri.AbsoluteUri).Build().AcquireTokenInteractive((IEnumerable<string>) scopes).WithPrompt(promptBehavior).WithOptionalCorrelationId<AcquireTokenInteractiveParameterBuilder>(correlationId).ExecuteAsync();
    }

    public async Task<AuthenticationResult> AcquireTokenSilentAsync(
      string[] scopes,
      Guid? correlationId)
    {
      List<Exception> exceptions = new List<Exception>(3);
      try
      {
        string windowsUpn = TestableAuthenticationContext.GetWindowsUpn();
        return await this.application.AcquireTokenSilent((IEnumerable<string>) scopes, windowsUpn).WithOptionalCorrelationId<AcquireTokenSilentParameterBuilder>(correlationId).ExecuteAsync();
      }
      catch (Exception ex)
      {
        exceptions.Add(ex);
      }
      try
      {
        return await this.application.AcquireTokenSilent((IEnumerable<string>) scopes, PublicClientApplication.OperatingSystemAccount).WithOptionalCorrelationId<AcquireTokenSilentParameterBuilder>(correlationId).ExecuteAsync();
      }
      catch (Exception ex)
      {
        exceptions.Add(ex);
      }
      try
      {
        return await this.application.AcquireTokenByIntegratedWindowsAuth((IEnumerable<string>) scopes).WithOptionalCorrelationId<AcquireTokenByIntegratedWindowsAuthParameterBuilder>(correlationId).ExecuteAsync();
      }
      catch (Exception ex)
      {
        exceptions.Add(ex);
      }
      throw new AggregateException((IEnumerable<Exception>) exceptions);
    }

    [DllImport("secur32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool GetUserNameEx(
      int nameFormat,
      StringBuilder userName,
      ref uint userNameSize);

    private static string GetWindowsUpn()
    {
      uint userNameSize = 0;
      TestableAuthenticationContext.GetUserNameEx(8, (StringBuilder) null, ref userNameSize);
      if (userNameSize <= 0U)
        return (string) null;
      StringBuilder userName = new StringBuilder((int) userNameSize);
      TestableAuthenticationContext.GetUserNameEx(8, userName, ref userNameSize);
      return userName.ToString();
    }

    public PublicClientApplicationBuilder ApplicationBuilder { get; }

    public ITokenCache TokenCache => this.application.UserTokenCache;
  }
}
