// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Slack.SlackConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Slack
{
  [Export(typeof (ConsumerImplementation))]
  public class SlackConsumer : ConsumerImplementation
  {
    private const string c_consumerId = "slack";
    private const string c_infoUrl = "https://go.microsoft.com/fwlink/?LinkId=521779";
    private const string c_logoUrl = "http://static.tumblr.com/wvuzcz9/LlKncfhmp/slack_logo_240.png";

    public static string ConsumerId => "slack";

    public override string Id => "slack";

    public override string Name => SlackConsumerResources.ConsumerName;

    public override string Description => SlackConsumerResources.ConsumerDescription;

    public override string ImageUrl => "http://static.tumblr.com/wvuzcz9/LlKncfhmp/slack_logo_240.png";

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?LinkId=521779";

    public override AuthenticationType AuthenticationType => AuthenticationType.None;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>(0);
  }
}
