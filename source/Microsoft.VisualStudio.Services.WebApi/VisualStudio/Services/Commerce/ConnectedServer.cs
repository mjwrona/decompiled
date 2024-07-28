// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ConnectedServer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ConnectedServer
  {
    public Guid SubscriptionId { get; set; }

    public string AccountName { get; set; }

    public Guid AccountId { get; set; }

    public Guid ServerId { get; set; }

    public string ServerName { get; set; }

    public Guid TargetId { get; set; }

    public string TargetName { get; set; }

    public string SpsUrl { get; set; }

    public ConnectedServerAuthorization Authorization { get; set; }
  }
}
