// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusTestReceiver
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ServiceBusTestReceiver : IMessageBusSubscriberJobExtensionReceiver
  {
    private const string s_Area = "ServiceBus";
    private string s_Layer = nameof (ServiceBusTestReceiver);

    public TeamFoundationHostType AcceptedHostTypes => TeamFoundationHostType.Deployment;

    public void Receive(IVssRequestContext requestContext, IMessage message) => requestContext.TraceAlways(1005246, TraceLevel.Info, "ServiceBus", this.s_Layer, message.GetBody<string>());
  }
}
