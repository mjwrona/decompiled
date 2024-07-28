// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthClientCredential
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.OAuth
{
  public abstract class VssOAuthClientCredential : IVssOAuthTokenParameterProvider, IDisposable
  {
    private bool m_disposed;
    private readonly string m_clientId;
    private readonly VssOAuthClientCredentialType m_type;

    protected VssOAuthClientCredential(VssOAuthClientCredentialType type, string clientId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(clientId, nameof (clientId));
      this.m_type = type;
      this.m_clientId = clientId;
    }

    public string ClientId => this.m_clientId;

    public VssOAuthClientCredentialType CredentialType => this.m_type;

    public void Dispose()
    {
      if (this.m_disposed)
        return;
      this.m_disposed = true;
      this.Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    protected abstract void SetParameters(IDictionary<string, string> parameters);

    void IVssOAuthTokenParameterProvider.SetParameters(IDictionary<string, string> parameters) => this.SetParameters(parameters);
  }
}
