// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HockeyApp.HockeyAppConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HockeyApp
{
  [Export(typeof (ConsumerImplementation))]
  public class HockeyAppConsumer : ConsumerImplementation
  {
    public const string ConsumerId = "hockeyApp";
    private const string c_infoUrl = "https://aka.ms/vsts-hockeyapp-integration";
    private const string c_createSubscriptionUrl = "https://aka.ms/vsts-hockeyapp-integration";

    public override string Id => "hockeyApp";

    public override string Name => HockeyAppConsumerResources.ConsumerName;

    public override string Description => HockeyAppConsumerResources.ConsumerDescription;

    public override string ImageUrl => (string) null;

    public override string InformationUrl => "https://aka.ms/vsts-hockeyapp-integration";

    public override AuthenticationType AuthenticationType => AuthenticationType.None;

    public override ExternalConfigurationDescriptor ExternalConfiguration => new ExternalConfigurationDescriptor()
    {
      CreateSubscriptionUrl = "https://aka.ms/vsts-hockeyapp-integration"
    };

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>(0);

    public override bool IsFeatureAvailable(IVssRequestContext requestContext) => false;
  }
}
