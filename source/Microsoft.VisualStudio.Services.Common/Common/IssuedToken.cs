// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.IssuedToken
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Common
{
  [Serializable]
  public abstract class IssuedToken
  {
    private int m_authenticated;

    internal IssuedToken()
    {
    }

    public bool IsAuthenticated => this.m_authenticated == 1;

    protected internal abstract VssCredentialsType CredentialType { get; }

    internal bool FromStorage { get; set; }

    public IDictionary<string, string> Properties { get; set; }

    internal Guid UserId { get; set; }

    internal string UserName { get; set; }

    internal bool Authenticated() => Interlocked.CompareExchange(ref this.m_authenticated, 1, 0) == 0;

    internal void GetUserData(IHttpResponse response)
    {
      IEnumerable<string> values;
      if (!response.Headers.TryGetValues("X-VSS-UserData", out values))
        return;
      string str = values.FirstOrDefault<string>();
      if (string.IsNullOrWhiteSpace(str))
        return;
      string[] strArray = str.Split(':');
      if (strArray.Length < 2)
        return;
      this.UserId = Guid.Parse(strArray[0]);
      this.UserName = strArray[1];
    }

    internal abstract void ApplyTo(IHttpRequest request);
  }
}
