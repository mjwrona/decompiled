// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.IcM.IcMConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.IcM
{
  [Export(typeof (ConsumerImplementation))]
  public class IcMConsumer : ConsumerImplementation
  {
    public const string ConsumerId = "icm";
    public const string IcMEnvironmentInputId = "url";
    public const string IcMConnectorIdInputId = "connectorId";
    public const string IcMCertificateInputId = "icmCert";
    public const string IcMPrivateKeyInputId = "icmPvtKey";
    private const string c_infoUrl = "https://icmdocs.azurewebsites.net/index.html";

    public override string Id => "icm";

    public override string Name => IcMConsumerResources.ConsumerName;

    public override string Description => IcMConsumerResources.ConsumerDescription;

    public override string ImageUrl => (string) null;

    public override string InformationUrl => "https://icmdocs.azurewebsites.net/index.html";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = IcMConsumerResources.AddIncidentAction_IcMEnvironmentName,
        Description = IcMConsumerResources.AddIncidentAction_IcMEnvironmentDescription,
        InputMode = InputMode.Combo,
        Id = "url",
        Values = new InputValues()
        {
          PossibleValues = (IList<InputValue>) new List<InputValue>()
          {
            new InputValue()
            {
              Value = "https://icm.ad.msft.net",
              DisplayValue = "PROD"
            },
            new InputValue()
            {
              Value = "https://icm.ad.msoppe.msft.net",
              DisplayValue = "PPE"
            },
            new InputValue()
            {
              Value = "https://icmtest.test.icm.msftcloudes.com",
              DisplayValue = "Test"
            }
          },
          IsLimitedToPossibleValues = true
        },
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.Uri,
          IsRequired = true
        }
      },
      new InputDescriptor()
      {
        Name = IcMConsumerResources.AddIncidentAction_IcMConnectorIdInputName,
        Description = IcMConsumerResources.AddIncidentAction_IcMConnectorIdDescription,
        InputMode = InputMode.TextBox,
        Id = "connectorId",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.Guid,
          IsRequired = true
        }
      },
      new InputDescriptor()
      {
        Name = IcMConsumerResources.AddIncidentAction_IcMCertificateInputName,
        Description = IcMConsumerResources.AddIncidentAction_IcMCertificateDescription,
        InputMode = InputMode.TextArea,
        Id = "icmCert",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = true
        }
      },
      new InputDescriptor()
      {
        Name = IcMConsumerResources.AddIncidentAction_IcMPrivateKeyInputName,
        Description = IcMConsumerResources.AddIncidentAction_IcMPrivateKeyDescription,
        InputMode = InputMode.TextArea,
        Id = "icmPvtKey",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = true
        }
      }
    };

    public override bool IsFeatureAvailable(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("ServiceHooks.Consumers.icm");
  }
}
