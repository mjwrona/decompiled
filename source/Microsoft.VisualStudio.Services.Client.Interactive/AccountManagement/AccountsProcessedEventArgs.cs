// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountsProcessedEventArgs
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public class AccountsProcessedEventArgs
  {
    public AccountsProcessedEventArgs(IList<Exception> exceptions)
      : this(false, exceptions)
    {
    }

    public AccountsProcessedEventArgs(bool skipped, IList<Exception> exceptions)
    {
      this.Skipped = skipped;
      this.Exceptions = exceptions;
    }

    public bool Skipped { get; set; }

    public IList<Exception> Exceptions { get; set; }
  }
}
