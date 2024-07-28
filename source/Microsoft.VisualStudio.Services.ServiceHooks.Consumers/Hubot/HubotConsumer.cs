// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Hubot.HubotConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Hubot
{
  [Export(typeof (ConsumerImplementation))]
  public sealed class HubotConsumer : ConsumerImplementation
  {
    private const string c_infoUrl = "https://go.microsoft.com/fwlink/?LinkID=402677";
    private const string c_logoUrl = "";
    public const string ConsumerId = "hubot";

    public override string Id => "hubot";

    public override string Name => HubotConsumerResources.ConsumerName;

    public override string Description => HubotConsumerResources.ConsumerDescription;

    public override string ImageUrl => "";

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?LinkID=402677";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>(0);

    public override SupportedConsumerDeploymentTypesEnum SupportedDeploymentTypes => SupportedConsumerDeploymentTypesEnum.HostedOnly;

    internal static string BuildMessagePayloadToSend(Event raisedEvent) => EventTransformer.TransformEvent(raisedEvent, messagesToSend: EventMessages.None, detailedMessagesToSend: EventMessages.None).GetStringRepresentation();
  }
}
