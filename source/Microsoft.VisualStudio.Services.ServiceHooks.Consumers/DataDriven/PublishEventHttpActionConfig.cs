// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven.PublishEventHttpActionConfig
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven
{
  public class PublishEventHttpActionConfig : DataDrivenHttpActionConfig
  {
    public string ResourceDetailsToSend { get; set; }

    public string MessagesToSend { get; set; }

    public string DetailedMessagesToSend { get; set; }

    public PublishEventTransformConfig[] Transforms { get; set; }
  }
}
