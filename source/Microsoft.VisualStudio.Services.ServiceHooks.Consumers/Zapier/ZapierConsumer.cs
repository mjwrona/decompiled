// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zapier.ZapierConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zapier
{
  [Export(typeof (ConsumerImplementation))]
  public class ZapierConsumer : ConsumerImplementation
  {
    public const string ConsumerId = "zapier";
    private const string c_infoUrl = "https://go.microsoft.com/fwlink/?LinkID=390581";
    private const string c_editSubscriptionPropertyName = "editSubscriptionUrl";
    private const string c_createSubscriptionUrl = "http://zapier.com";

    public override string Id => "zapier";

    public override string Name => ZapierConsumerResources.ConsumerName;

    public override string Description => ZapierConsumerResources.ConsumerDescription;

    public override string ImageUrl => (string) null;

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?LinkID=390581";

    public override AuthenticationType AuthenticationType => AuthenticationType.None;

    public override ExternalConfigurationDescriptor ExternalConfiguration => new ExternalConfigurationDescriptor()
    {
      CreateSubscriptionUrl = "http://zapier.com",
      EditSubscriptionPropertyName = "editSubscriptionUrl"
    };

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>(0);
  }
}
