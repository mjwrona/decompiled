// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Keychain.SessionToken
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client.Keychain
{
  public class SessionToken
  {
    public DateTime DateToRefresh { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public DateTime RefreshedOn { get; set; }

    public string UserId { get; set; }

    public string Scope { get; set; }

    public bool HasCompact { get; set; }

    public IEnumerable<Guid> TargetAccounts { get; set; }
  }
}
