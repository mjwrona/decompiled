// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityChangeInfo
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityChangeInfo
  {
    internal string Account;
    internal string OldSid;
    internal string NewSid;
    internal bool TargetExists;
    internal bool Matched;
    internal bool Changed;
    internal Microsoft.VisualStudio.Services.Identity.Identity Identity;

    internal IdentityChangeInfo(string identityAccount, string identitySid)
    {
      this.Account = identityAccount;
      this.OldSid = identitySid;
      this.Changed = false;
    }
  }
}
