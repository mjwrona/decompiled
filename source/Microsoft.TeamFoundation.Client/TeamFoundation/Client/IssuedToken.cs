// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IssuedToken
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
using System.Threading;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.IssuedToken instead.", false)]
  [Serializable]
  public abstract class IssuedToken
  {
    private int m_authenticated;

    internal IssuedToken()
    {
    }

    public bool IsAuthenticated => this.m_authenticated == 1;

    protected internal abstract VssCredentialsType CredentialType { get; }

    internal Guid UserId { get; set; }

    internal string UserName { get; set; }

    internal bool FromStorage { get; set; }

    internal bool Authenticated() => Interlocked.CompareExchange(ref this.m_authenticated, 1, 0) == 0;

    internal virtual void RequestUserData(HttpWebRequest webRequest)
    {
    }

    internal virtual void GetUserData(HttpWebResponse webResponse)
    {
    }

    internal abstract void ApplyTo(HttpWebRequest webRequest);
  }
}
