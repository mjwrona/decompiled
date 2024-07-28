// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Trello.TrelloConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Trello
{
  [Export(typeof (ConsumerImplementation))]
  public class TrelloConsumer : ConsumerImplementation
  {
    private const string c_trelloUrl = "https://go.microsoft.com/fwlink/?LinkID=390530";
    private const string c_trelloApplicationKey = "7d6630fd03ac2b6fc9fde2f2ef0c4096";
    private const string c_trelloLogoUrl = "https://d2k1ftgv7pobq7.cloudfront.net/images/bd87ee916375920ae72dffadbb10d412/logo-blue-lg.png";
    public const string ConsumerId = "trello";
    public const string UserTokenInputId = "userToken";

    public static string ApplicationKey => "7d6630fd03ac2b6fc9fde2f2ef0c4096";

    public override string Id => "trello";

    public override string Name => TrelloConsumerResources.ConsumerName;

    public override string Description => TrelloConsumerResources.ConsumerDescription;

    public override string ImageUrl => "https://d2k1ftgv7pobq7.cloudfront.net/images/bd87ee916375920ae72dffadbb10d412/logo-blue-lg.png";

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?LinkID=390530";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = TrelloConsumerResources.Consumer_InputUserTokenName,
        Description = TrelloConsumerResources.Consumer_InputUserTokenDescription,
        InputMode = InputMode.TextBox,
        Id = "userToken",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        }
      }
    };
  }
}
