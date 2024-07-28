// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.ConsumerImplementation
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public abstract class ConsumerImplementation
  {
    private static readonly string s_layer = typeof (ConsumerImplementation).Name;
    private static readonly string s_area = typeof (ConsumerImplementation).Namespace;

    public IList<ConsumerActionImplementation> Actions { get; set; }

    public virtual SupportedConsumerDeploymentTypesEnum SupportedDeploymentTypes => this.ExternalConfiguration != null ? SupportedConsumerDeploymentTypesEnum.HostedOnly : SupportedConsumerDeploymentTypesEnum.All;

    public string FeatureName => "ServiceHooks.Consumers." + this.Id;

    public abstract string Id { get; }

    public abstract string Name { get; }

    public abstract string Description { get; }

    public abstract string ImageUrl { get; }

    public abstract string InformationUrl { get; }

    public abstract AuthenticationType AuthenticationType { get; }

    public abstract ExternalConfigurationDescriptor ExternalConfiguration { get; }

    public abstract IList<InputDescriptor> InputDescriptors { get; }

    public virtual bool IsFeatureAvailable(IVssRequestContext requestContext) => true;

    public virtual InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues)
    {
      return (InputValues) null;
    }

    public virtual string GetActionDescription(
      IVssRequestContext requestContext,
      string consumerActionId,
      IDictionary<string, string> consumerInputValues)
    {
      ConsumerActionImplementation actionImplementation = this.Actions.First<ConsumerActionImplementation>((Func<ConsumerActionImplementation, bool>) (a => a.Id == consumerActionId));
      if (actionImplementation == null)
        throw new ConsumerActionNotFoundException(this.Id, consumerActionId);
      return actionImplementation.GetActionDescription(requestContext, consumerInputValues) ?? this.CreateDefaultActionDescription(this.InputDescriptors.Concat<InputDescriptor>((IEnumerable<InputDescriptor>) actionImplementation.InputDescriptors).Where<InputDescriptor>((Func<InputDescriptor, bool>) (d => d.UseInDefaultDescription)), consumerInputValues);
    }

    private string CreateDefaultActionDescription(
      IEnumerable<InputDescriptor> inputDescriptors,
      IDictionary<string, string> consumerInputValues)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (InputDescriptor inputDescriptor in inputDescriptors)
      {
        if (consumerInputValues.ContainsKey(inputDescriptor.Id))
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(", ");
          stringBuilder.Append(inputDescriptor.Name);
          stringBuilder.Append(": ");
          stringBuilder.Append(consumerInputValues[inputDescriptor.Id]);
        }
      }
      return stringBuilder.Length != 0 ? stringBuilder.ToString() : CommonResources.NoCustomActionDescription;
    }
  }
}
