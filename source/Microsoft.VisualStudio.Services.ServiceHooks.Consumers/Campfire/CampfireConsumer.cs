// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Campfire.CampfireConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Campfire
{
  [Export(typeof (ConsumerImplementation))]
  public sealed class CampfireConsumer : ConsumerImplementation
  {
    private const string c_infoUrl = "https://go.microsoft.com/fwlink/?LinkID=393613";
    private const string c_logoUrl = "https://campfirenow.com/images/logo_campfire-full.png";
    private const int c_accountNameInputMaxLength = 63;
    private const string c_accountNameInputPattern = "^([A-Za-z0-9][A-Za-z0-9\\-]{0,61}[A-Za-z0-9]|[A-Za-z0-9]{1,63})$";
    public const string ConsumerId = "campfire";
    public const string AccountNameInputId = "accountName";
    public const string AuthTokenInputId = "authToken";

    public override string Id => "campfire";

    public override string Name => CampfireConsumerResources.ConsumerName;

    public override string Description => CampfireConsumerResources.ConsumerDescription;

    public override string ImageUrl => "https://campfirenow.com/images/logo_campfire-full.png";

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?LinkID=393613";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = CampfireConsumerResources.CampfireConsumer_AccountNameInputName,
        Description = CampfireConsumerResources.CampfireConsumer_AccountNameInputDescription,
        InputMode = InputMode.TextBox,
        Id = "accountName",
        IsConfidential = false,
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
        Name = CampfireConsumerResources.CampfireConsumer_AuthTokenInputName,
        Description = CampfireConsumerResources.CampfireConsumer_AuthTokenInputDescription,
        InputMode = InputMode.TextBox,
        Id = "authToken",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String
        },
        DependencyInputIds = (IList<string>) new List<string>()
        {
          "accountName"
        }
      }
    };

    public static bool TryGetConsumerInputs(
      IDictionary<string, string> inputValues,
      out string accountName,
      out string authToken)
    {
      accountName = (string) null;
      authToken = (string) null;
      bool consumerInputs = inputValues.TryGetValue(nameof (accountName), out accountName) && inputValues.TryGetValue(nameof (authToken), out authToken);
      if (consumerInputs)
        consumerInputs = !string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(authToken);
      return consumerInputs;
    }

    public override bool IsFeatureAvailable(IVssRequestContext requestContext) => false;
  }
}
