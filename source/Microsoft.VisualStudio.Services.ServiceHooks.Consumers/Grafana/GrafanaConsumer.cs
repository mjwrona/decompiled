// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Grafana.GrafanaConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Grafana
{
  [Export(typeof (ConsumerImplementation))]
  public class GrafanaConsumer : ConsumerImplementation
  {
    public const string ConsumerId = "grafana";
    public const string UrlInputId = "url";
    public const string ApiTokenInputId = "apiToken";
    private const string c_infoUrl = "https://grafana.com/";

    public override string Id => "grafana";

    public override string Name => GrafanaConsumerResources.ConsumerName;

    public override string Description => GrafanaConsumerResources.ConsumerDescription;

    public override string ImageUrl => (string) null;

    public override string InformationUrl => "https://grafana.com/";

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => (ExternalConfigurationDescriptor) null;

    public override IList<InputDescriptor> InputDescriptors => (IList<InputDescriptor>) new List<InputDescriptor>()
    {
      new InputDescriptor()
      {
        Name = GrafanaConsumerResources.AddAnnotationAction_UrlInputName,
        Description = GrafanaConsumerResources.AddAnnotationAction_UrlInputDescription,
        InputMode = InputMode.TextBox,
        Id = "url",
        IsConfidential = false,
        Validation = new InputValidation()
        {
          DataType = InputDataType.Uri,
          IsRequired = true
        }
      },
      new InputDescriptor()
      {
        Name = GrafanaConsumerResources.AddAnnotationAction_ApiTokenInputName,
        Description = GrafanaConsumerResources.AddAnnotationAction_ApiTokenInputDescription,
        InputMode = InputMode.TextBox,
        Id = "apiToken",
        IsConfidential = true,
        Validation = new InputValidation()
        {
          DataType = InputDataType.String,
          IsRequired = true
        }
      }
    };
  }
}
