// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.UserVoice.UserVoiceConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.UserVoice
{
  [Export(typeof (ConsumerImplementation))]
  public sealed class UserVoiceConsumer : ConsumerImplementation
  {
    private const string c_infoUrl = "https://go.microsoft.com/fwlink/?LinkID=393617";
    private const string c_logoUrl = "";
    private const string c_createSubscriptionUrl = "http://uservoice.com";
    public const string ConsumerId = "userVoice";

    public override string Id => "userVoice";

    public override string Name => UserVoiceConsumerResources.ConsumerName;

    public override string Description => UserVoiceConsumerResources.ConsumerDescription;

    public override string ImageUrl => "";

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?LinkID=393617";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => new ExternalConfigurationDescriptor()
    {
      CreateSubscriptionUrl = "http://uservoice.com"
    };

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>(0);
  }
}
