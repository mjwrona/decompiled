// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ServiceHooksChannelMetadata
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public class ServiceHooksChannelMetadata
  {
    public string PublisherId { get; set; }

    public string EventType { get; set; }

    public string ConsumerId { get; set; }

    public string ConsumerActionId { get; set; }

    public string ResourceVersion { get; set; }

    public string Scope { get; set; }

    public string ResourceDetailsToSend { get; set; }

    public string MessagesToSend { get; set; }

    public string DetailedMessagesToSend { get; set; }

    public bool IsAnyMessageSet() => this.MessagesToSend == null || !this.MessagesToSend.Equals("None", StringComparison.OrdinalIgnoreCase);

    public bool IsTextMessageSet() => this.MessagesToSend == null || this.MessagesToSend.Equals("All", StringComparison.OrdinalIgnoreCase) || this.MessagesToSend.Equals("Text", StringComparison.OrdinalIgnoreCase);

    public bool IsHtmlMessageSet() => this.MessagesToSend == null || this.MessagesToSend.Equals("All", StringComparison.OrdinalIgnoreCase) || this.MessagesToSend.Equals("Html", StringComparison.OrdinalIgnoreCase);

    public bool IsMarkdownMessageSet() => this.MessagesToSend == null || this.MessagesToSend.Equals("All", StringComparison.OrdinalIgnoreCase) || this.MessagesToSend.Equals("Markdown", StringComparison.OrdinalIgnoreCase);

    public bool IsAnyDetailedMessageSet() => this.DetailedMessagesToSend == null || !this.DetailedMessagesToSend.Equals("None", StringComparison.InvariantCultureIgnoreCase);

    public bool IsTextDetailedMessageSet() => this.DetailedMessagesToSend == null || this.DetailedMessagesToSend.Equals("All", StringComparison.InvariantCultureIgnoreCase) || this.DetailedMessagesToSend.Equals("Text", StringComparison.InvariantCultureIgnoreCase);

    public bool IsHtmlDetailedMessageSet() => this.DetailedMessagesToSend == null || this.DetailedMessagesToSend.Equals("All", StringComparison.OrdinalIgnoreCase) || this.DetailedMessagesToSend.Equals("Html", StringComparison.OrdinalIgnoreCase);

    public bool IsMarkdownDetailedMessageSet() => this.DetailedMessagesToSend == null || this.DetailedMessagesToSend.Equals("All", StringComparison.OrdinalIgnoreCase) || this.DetailedMessagesToSend.Equals("Markdown", StringComparison.OrdinalIgnoreCase);
  }
}
