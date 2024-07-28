// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssAadVSAccountProviderTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Client.AccountManagement;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client
{
  public sealed class VssAadVSAccountProviderTokenProvider : IssuedTokenProvider
  {
    private Action<string, string, Guid> m_raiseSilentFailureEvent;
    private Func<VSAccountProvider, string, string, string, IntPtr, CancellationToken, AuthenticationResult> m_acquireTokenDelegate;
    private VSAccountProviderParameters m_parameters;

    public VssAadVSAccountProviderTokenProvider(
      FederatedCredential credential,
      VSAccountProviderParameters parameters,
      Action<string, string, Guid> raiseSilentFailureEventAction = null,
      Func<VSAccountProvider, string, string, string, IntPtr, CancellationToken, AuthenticationResult> acquireTokenDelegate = null)
      : base((IssuedTokenCredential) credential, (Uri) null, (Uri) null)
    {
      this.m_parameters = parameters;
      this.m_raiseSilentFailureEvent = raiseSilentFailureEventAction;
      this.m_acquireTokenDelegate = acquireTokenDelegate;
    }

    public override bool GetTokenIsInteractive => false;

    protected override Task<IssuedToken> OnGetTokenAsync(
      IssuedToken failedToken,
      CancellationToken cancellationToken)
    {
      IssuedToken result1 = (IssuedToken) null;
      if (this.m_parameters?.KeychainAccountProvider != null)
      {
        try
        {
          AuthenticationResult result2 = (AuthenticationResult) null;
          if (this.m_acquireTokenDelegate != null)
            result2 = this.m_acquireTokenDelegate(this.m_parameters.KeychainAccountProvider, this.m_parameters.VSTSEndpointResource, this.m_parameters.TenantId, this.m_parameters.UserUniqueId, IntPtr.Zero, cancellationToken);
          if (result2 != null)
            result1 = (IssuedToken) new VssAadVSAccountProviderToken(result2);
        }
        catch (MsalUiRequiredException ex)
        {
          if (this.m_raiseSilentFailureEvent != null)
            this.m_raiseSilentFailureEvent(this.m_parameters.TenantId, this.m_parameters.UserUniqueId, this.m_parameters.KeychainAccountProvider.AccountProviderId);
          throw;
        }
      }
      return Task.FromResult<IssuedToken>(result1);
    }
  }
}
