// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.AzureStorageConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  [Export(typeof (ConsumerImplementation))]
  public class AzureStorageConsumer : ConsumerImplementation
  {
    private const string c_consumerId = "azureStorageQueue";
    private const string c_infoUrl = "https://go.microsoft.com/fwlink/?LinkID=390532";
    private const string c_logoUrl = "http://www.windowsazure.com/css/images/logo.png";
    private const int c_accountNameInputLengthMin = 1;
    private const int c_accountNameInputLengthMax = 100;
    private const int c_accountKeyInputLengthMin = 64;
    private const int c_accountKeyInputLengthMax = 100;
    public const string AccountKeyInputId = "accountKey";
    public const string AccountNameInputId = "accountName";

    public static string ConsumerId => "azureStorageQueue";

    public override string Id => "azureStorageQueue";

    public override string Name => AzureConsumerResources.StorageConsumerName;

    public override string Description => AzureConsumerResources.StorageConsumerDescription;

    public override string ImageUrl => "http://www.windowsazure.com/css/images/logo.png";

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?LinkID=390532";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = AzureConsumerResources.StorageQueueEnqueueAction_AccountNameInputName,
        Description = AzureConsumerResources.StorageQueueEnqueueAction_AccountNameInputDescription,
        InputMode = InputMode.TextBox,
        Id = "accountName",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(100),
          MinLength = new int?(1)
        }
      },
      new InputDescriptor()
      {
        Name = AzureConsumerResources.StorageQueueEnqueueAction_AccountKeyInputName,
        Description = AzureConsumerResources.StorageQueueEnqueueAction_AccountKeyInputDescription,
        InputMode = InputMode.TextBox,
        Id = "accountKey",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(100),
          MinLength = new int?(64)
        }
      }
    };
  }
}
