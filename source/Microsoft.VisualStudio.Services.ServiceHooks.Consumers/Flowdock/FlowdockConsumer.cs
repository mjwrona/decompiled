// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Flowdock.FlowdockConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Flowdock
{
  [Export(typeof (ConsumerImplementation))]
  public sealed class FlowdockConsumer : ConsumerImplementation
  {
    private const string c_infoUrl = "https://go.microsoft.com/fwlink/?LinkID=393615";
    private const string c_logoUrl = null;
    private const string c_flowAPITokenInputPattern = "^[\\w0-9\\s]*$";
    private const int c_flowNameInputMaxLength = 100;
    public const string ConsumerId = "flowdock";
    public const string FlowApiTokenInputId = "flowAPIToken";
    public const string FlowNameInputId = "flowName";
    public const string ShowDetailsInputId = "showDetails";

    public override string Id => "flowdock";

    public override string Name => FlowdockConsumerResources.ConsumerName;

    public override string Description => FlowdockConsumerResources.ConsumerDescription;

    public override string ImageUrl => (string) null;

    public override string InformationUrl => "https://go.microsoft.com/fwlink/?LinkID=393615";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = FlowdockConsumerResources.FlowdockConsumer_FlowAPITokenInputName,
        Description = FlowdockConsumerResources.FlowdockConsumer_FlowAPITokenDescription,
        InputMode = InputMode.TextBox,
        Id = "flowAPIToken",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          IsRequired = true,
          DataType = InputDataType.String,
          Pattern = "^[\\w0-9\\s]*$"
        }
      },
      new InputDescriptor()
      {
        Name = FlowdockConsumerResources.FlowdockConsumer_FlowNameInputName,
        Description = FlowdockConsumerResources.FlowdockConsumer_FlowNameInputDescription,
        InputMode = InputMode.TextBox,
        Id = "flowName",
        IsConfidential = false,
        UseInDefaultDescription = true,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.String,
          MaxLength = new int?(100)
        }
      },
      new InputDescriptor()
      {
        Name = FlowdockConsumerResources.FlowdockConsumer_ShowDetailsInputName,
        Description = FlowdockConsumerResources.FlowdockConsumer_ShowDetailsInputDescription,
        InputMode = InputMode.CheckBox,
        Id = "showDetails",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          IsRequired = false,
          DataType = InputDataType.Boolean
        }
      }
    };

    public override bool IsFeatureAvailable(IVssRequestContext requestContext) => false;
  }
}
