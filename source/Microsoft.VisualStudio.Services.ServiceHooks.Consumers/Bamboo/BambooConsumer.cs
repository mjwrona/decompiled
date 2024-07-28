// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Bamboo.BambooConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Bamboo
{
  [Export(typeof (ConsumerImplementation))]
  public sealed class BambooConsumer : ConsumerImplementation
  {
    private const string c_infoUrl = "https://go.microsoft.com/fwlink/?linkid=825695";
    private const string c_logoUrl = "https://www.atlassian.com/wac/software/bamboo/productLogo/imageBinary/bamboo_logo_landing.png";
    public const string ConsumerId = "bamboo";
    public const string UsernameInputId = "username";
    public const string PasswordInputId = "password";
    public const string ServerBaseUrlInputId = "serverBaseUrl";

    public override string Id => "bamboo";

    public override string Name => BambooConsumerResources.ConsumerName;

    public override string Description => BambooConsumerResources.ConsumerDescription;

    public override string ImageUrl => "https://www.atlassian.com/wac/software/bamboo/productLogo/imageBinary/bamboo_logo_landing.png";

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?linkid=825695";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = BambooConsumerResources.BambooConsumer_ServerBaseUrlInputName,
        Description = BambooConsumerResources.BambooConsumer_ServerBaseUrlInputDescription,
        InputMode = InputMode.TextBox,
        Id = "serverBaseUrl",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.Uri
        }
      },
      new InputDescriptor()
      {
        Name = BambooConsumerResources.BambooConsumer_UsernameInputName,
        Description = BambooConsumerResources.BambooConsumer_UsernameInputDescription,
        InputMode = InputMode.TextBox,
        Id = "username",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        }
      },
      new InputDescriptor()
      {
        Name = BambooConsumerResources.BambooConsumer_PasswordInputName,
        Description = BambooConsumerResources.BambooConsumer_PasswordInputDescription,
        InputMode = InputMode.PasswordBox,
        Id = "password",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "serverBaseUrl"
        }
      }
    };

    public static bool TryGetConsumerInputs(
      IDictionary<string, string> inputValues,
      out string serverBaseUrl,
      out string username,
      out string password)
    {
      username = (string) null;
      password = (string) null;
      serverBaseUrl = (string) null;
      bool consumerInputs = inputValues.TryGetValue(nameof (serverBaseUrl), out serverBaseUrl) && inputValues.TryGetValue(nameof (username), out username) && inputValues.TryGetValue(nameof (password), out password);
      if (consumerInputs)
        consumerInputs = !string.IsNullOrEmpty(serverBaseUrl) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
      return consumerInputs;
    }
  }
}
