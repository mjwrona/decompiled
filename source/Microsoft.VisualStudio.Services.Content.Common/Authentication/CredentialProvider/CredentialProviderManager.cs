// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Authentication.CredentialProvider.CredentialProviderManager
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common.Authentication.CredentialProvider
{
  internal class CredentialProviderManager
  {
    private const int DefaultTimeoutInSeconds = 300;
    private readonly Action<string> loggingFunc;
    private readonly TimeSpan timeout;
    private readonly ICredentialProviderLoader _providerLoader;

    public CredentialProviderManager(
      Action<string> loggingFunc,
      int timeoutInSeconds = 300,
      ICredentialProviderLoader providerLoader = null)
    {
      this.loggingFunc = loggingFunc != null ? loggingFunc : throw new ArgumentNullException(nameof (loggingFunc));
      this._providerLoader = providerLoader ?? (ICredentialProviderLoader) new CredentialProviderLoader();
      this.timeout = TimeSpan.FromSeconds((double) timeoutInSeconds);
    }

    public VssCredentials GetCredentials(Uri targetUri, bool isRetry, bool nonInteractive) => TaskSafety.SyncResultOnThreadPool<VssCredentials>((Func<Task<VssCredentials>>) (() => this.GetCredentialsAsync(targetUri, isRetry, nonInteractive)));

    public async Task<VssCredentials> GetCredentialsAsync(
      Uri targetUri,
      bool isRetry,
      bool nonInteractive)
    {
      IEnumerable<ICredentialProvider> credentialProviders = this._providerLoader.FindCredentialProviders();
      CancellationTokenSource cts = new CancellationTokenSource();
      foreach (ICredentialProvider credentialProvider in credentialProviders)
      {
        ICredentialProvider provider = credentialProvider;
        CredentialResponse credentialResponse = await provider.GetCredentialsAsync(targetUri, isRetry, nonInteractive, this.timeout, cts.Token).ConfigureAwait(false);
        if (credentialResponse.Status == RequestStatus.Success)
        {
          this.loggingFunc("Credentials were retrieved from provider: " + provider.Description);
          return (VssCredentials) (FederatedCredential) new VssBasicCredential(new VssBasicToken(credentialResponse.Credentials));
        }
        this.loggingFunc("$Credential provider {provider.Name} did not return credentials");
        provider = (ICredentialProvider) null;
      }
      return (VssCredentials) null;
    }
  }
}
