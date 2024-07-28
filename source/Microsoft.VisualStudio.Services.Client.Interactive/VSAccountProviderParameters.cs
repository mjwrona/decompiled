// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VSAccountProviderParameters
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Client.AccountManagement;

namespace Microsoft.VisualStudio.Services.Client
{
  public sealed class VSAccountProviderParameters
  {
    public VSAccountProvider KeychainAccountProvider { get; set; }

    public string VSTSEndpointResource { get; set; }

    public string TenantId { get; set; }

    public string UserUniqueId { get; set; }
  }
}
