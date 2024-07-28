// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HipChat.HipChatConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HipChat
{
  [Export(typeof (ConsumerImplementation))]
  public class HipChatConsumer : ConsumerImplementation
  {
    private const string c_consumerId = "hipChat";
    private const string c_logoUrl = "http://www.hipchat.com/img/logo.png";
    private const string c_infoUrl = "https://go.microsoft.com/fwlink/?LinkId=392098";
    private const string c_authTokenInputId = "authToken";

    public static string ConsumerId => "hipChat";

    public static string AuthTokenInputId => "authToken";

    public override string Id => "hipChat";

    public override string Name => HipChatConsumerResources.ConsumerName;

    public override string Description => HipChatConsumerResources.ConsumerDescription;

    public override string ImageUrl => "http://www.hipchat.com/img/logo.png";

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?LinkId=392098";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = HipChatConsumerResources.HipChatConsumer_AuthTokenInputName,
        Description = HipChatConsumerResources.HipChatConsumer_AuthTokenInputDescription,
        InputMode = InputMode.TextBox,
        Id = "authToken",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        }
      }
    };

    public override bool IsFeatureAvailable(IVssRequestContext requestContext) => false;
  }
}
