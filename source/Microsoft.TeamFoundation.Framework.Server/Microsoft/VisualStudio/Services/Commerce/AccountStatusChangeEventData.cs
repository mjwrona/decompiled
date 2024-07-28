// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AccountStatusChangeEventData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AccountStatusChangeEventData
  {
    public Guid SubscriptionId { get; set; }

    public Guid HostId { get; set; }

    public AccountStatusChangeEventType ChangeType { get; set; }

    public string Source { get; set; }

    public AccountProviderNamespace ProviderNamespace { get; set; }

    public AccountStatusChangeEventData(
      Guid subscriptionId,
      Guid hostId,
      AccountProviderNamespace providerNamespace,
      AccountStatusChangeEventType changeType,
      string source)
    {
      this.SubscriptionId = subscriptionId;
      this.HostId = hostId;
      this.ProviderNamespace = providerNamespace;
      this.ChangeType = changeType;
      this.Source = source;
    }
  }
}
