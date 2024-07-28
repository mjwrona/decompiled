// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zendesk.ZendeskConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zendesk
{
  [Export(typeof (ConsumerImplementation))]
  public sealed class ZendeskConsumer : ConsumerImplementation
  {
    private const string c_infoUrl = "https://go.microsoft.com/fwlink/?LinkID=396756";
    private const string c_logoUrl = "";
    private const int c_accountNameInputMaxLength = 63;
    private const string c_accountNameInputPattern = "^([A-Za-z0-9][A-Za-z0-9\\-]{0,61}[A-Za-z0-9]|[A-Za-z0-9]{1,63})$";
    private const int c_userNameInputMaxLength = 254;
    private const string c_userNameInputPattern = "^.+\\@.+\\..+$";
    private const int c_apiTokenInputMaxLength = 100;
    public const string ConsumerId = "zendesk";
    public const string AccountNameInputId = "accountName";
    public const string UsernameInputId = "username";
    public const string ApiTokenInputId = "apiToken";

    public override string Id => "zendesk";

    public override string Name => ZendeskConsumerResources.ConsumerName;

    public override string Description => ZendeskConsumerResources.ConsumerDescription;

    public override string ImageUrl => "";

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?LinkID=396756";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = ZendeskConsumerResources.ZendeskConsumer_AccountNameInputName,
        Description = ZendeskConsumerResources.ZendeskConsumer_AccountNameInputDescription,
        InputMode = InputMode.TextBox,
        Id = "accountName",
        IsConfidential = false,
        UseInDefaultDescription = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(63),
          Pattern = "^([A-Za-z0-9][A-Za-z0-9\\-]{0,61}[A-Za-z0-9]|[A-Za-z0-9]{1,63})$"
        }
      },
      new InputDescriptor()
      {
        Name = ZendeskConsumerResources.ZendeskConsumer_UsernameInputName,
        Description = ZendeskConsumerResources.ZendeskConsumer_UsernameInputDescription,
        InputMode = InputMode.TextBox,
        Id = "username",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(254),
          Pattern = "^.+\\@.+\\..+$"
        }
      },
      new InputDescriptor()
      {
        Name = ZendeskConsumerResources.ZendeskConsumer_ApiTokenInputName,
        Description = ZendeskConsumerResources.ZendeskConsumer_ApiTokenInputDescription,
        InputMode = InputMode.PasswordBox,
        Id = "apiToken",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(100)
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "accountName"
        }
      }
    };
  }
}
