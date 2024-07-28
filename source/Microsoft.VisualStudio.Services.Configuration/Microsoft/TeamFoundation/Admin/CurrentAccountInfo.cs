// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.CurrentAccountInfo
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Globalization;
using System.Security;

namespace Microsoft.TeamFoundation.Admin
{
  internal sealed class CurrentAccountInfo : AccountInfo
  {
    private CurrentAccountInfo()
      : base(AccountType.User, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) Environment.UserDomainName, (object) Environment.UserName), (SecureString) null, false)
    {
    }

    public static CurrentAccountInfo Instance => CurrentAccountInfo.Nested.Instance;

    private class Nested
    {
      internal static readonly CurrentAccountInfo Instance = new CurrentAccountInfo();

      private Nested()
      {
      }
    }
  }
}
