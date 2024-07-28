// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.AccountPicker.CachedAccount
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Client.AccountManagement;

namespace Microsoft.VisualStudio.Services.Client.Controls.AccountPicker
{
  public class CachedAccount : IAccountPickerItem
  {
    public string DisplayableId => this.CachedToken.Username;

    public string AccountName => this.CachedToken.GivenName + " " + this.CachedToken.FamilyName;

    public string DisplayName => this.AccountName + " (" + this.DisplayableId + ")";

    internal IAccountCacheItem CachedToken { get; }

    internal CachedAccount(IAccountCacheItem token) => this.CachedToken = token;
  }
}
