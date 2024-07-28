// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountInitializationData
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public struct AccountInitializationData
  {
    public AccountDisplayInfo DisplayInfo { get; set; }

    public string UniqueId { get; set; }

    public string Authenticator { get; set; }

    public Guid ParentProviderId { get; set; }

    public IReadOnlyDictionary<string, string> Properties { get; set; }

    public IReadOnlyList<Guid> SupportedAccountProviders { get; set; }

    public bool NeedsReauthentication { get; set; }
  }
}
