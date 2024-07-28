// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.AzureServiceBusConsumer
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
  public class AzureServiceBusConsumer : ConsumerImplementation
  {
    private const string c_consumerId = "azureServiceBus";
    private const string c_infoUrl = "https://go.microsoft.com/fwlink/?LinkID=392636";
    private const string c_logoUrl = "http://www.windowsazure.com/css/images/logo.png";
    private const string c_connectionStringInputId = "connectionString";
    private const int c_connectionStringInputLengthMin = 1;
    private const int c_connectionStringInputLengthMax = 500;
    public const string ServiceBusEndpointFormat = "sb://{0}.servicebus.windows.net/";

    public static string ConsumerId => "azureServiceBus";

    public static string ConnectionStringInputId => "connectionString";

    public override string Id => "azureServiceBus";

    public override string Name => AzureConsumerResources.ServiceBusConsumerName;

    public override string Description => AzureConsumerResources.ServiceBusConsumerDescription;

    public override string ImageUrl => "http://www.windowsazure.com/css/images/logo.png";

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?LinkID=392636";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = AzureConsumerResources.ServiceBusConsumer_ConnectionStringInputName,
        Description = AzureConsumerResources.ServiceBusConsumer_ConnectionStringInputDescription,
        InputMode = InputMode.TextBox,
        Id = "connectionString",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          MaxLength = new int?(500),
          MinLength = new int?(1)
        }
      }
    };

    public static string BuildConnectionStringFromInputValues(
      IDictionary<string, string> currentConsumerInputValues)
    {
      string str = (string) null;
      return !currentConsumerInputValues.TryGetValue("connectionString", out str) ? (string) null : str;
    }

    public static string BuildConnectionStringFromNotification(Notification notification) => notification.GetConsumerInput("connectionString", true);
  }
}
