// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountExtensions
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal static class AccountExtensions
  {
    public static bool IsDisabled(this AccountUserStatus value) => value == AccountUserStatus.Disabled || value == AccountUserStatus.PendingDisabled;

    public static bool IsDisabled(this AccountUser user)
    {
      ArgumentUtility.CheckForNull<AccountUser>(user, nameof (user));
      return user.UserStatus.IsDisabled();
    }

    public static bool IsLive(this AccountUserStatus value) => value == AccountUserStatus.Active || value == AccountUserStatus.Pending;

    public static bool IsLive(this AccountUser user)
    {
      ArgumentUtility.CheckForNull<AccountUser>(user, nameof (user));
      return user.UserStatus.IsLive();
    }

    public static bool IsLicensable(this AccountUserStatus value) => value.IsDisabled() || value.IsLive();

    public static bool IsLicensable(this AccountUser user)
    {
      ArgumentUtility.CheckForNull<AccountUser>(user, nameof (user));
      return user.UserStatus.IsLicensable();
    }
  }
}
