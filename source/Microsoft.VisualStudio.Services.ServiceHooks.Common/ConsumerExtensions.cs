// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.ConsumerExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public static class ConsumerExtensions
  {
    public static Consumer ToConsumer(this ConsumerImplementation consumerImpl)
    {
      List<ConsumerAction> consumerActionList = new List<ConsumerAction>();
      if (consumerImpl.Actions != null)
        consumerActionList.AddRange((IEnumerable<ConsumerAction>) consumerImpl.Actions.Select<ConsumerActionImplementation, ConsumerAction>((Func<ConsumerActionImplementation, ConsumerAction>) (actionImpl => actionImpl.ToConsumerAction())).OrderBy<ConsumerAction, string>((Func<ConsumerAction, string>) (action => action.Name)));
      return new Consumer()
      {
        Actions = (IList<ConsumerAction>) consumerActionList,
        AuthenticationType = consumerImpl.AuthenticationType,
        Description = consumerImpl.Description,
        Id = consumerImpl.Id,
        ImageUrl = consumerImpl.ImageUrl,
        InformationUrl = consumerImpl.InformationUrl,
        Name = consumerImpl.Name,
        InputDescriptors = consumerImpl.InputDescriptors,
        ExternalConfiguration = consumerImpl.ExternalConfiguration
      };
    }

    public static List<Consumer> ToConsumers(
      this IEnumerable<ConsumerImplementation> consumerImpls,
      bool isOnPremises,
      bool includeAllConsumers = false)
    {
      return (!includeAllConsumers ? (!isOnPremises ? consumerImpls.Where<ConsumerImplementation>((Func<ConsumerImplementation, bool>) (consumer => consumer.SupportedDeploymentTypes == SupportedConsumerDeploymentTypesEnum.All || consumer.SupportedDeploymentTypes == SupportedConsumerDeploymentTypesEnum.HostedOnly)) : consumerImpls.Where<ConsumerImplementation>((Func<ConsumerImplementation, bool>) (consumer => consumer.SupportedDeploymentTypes == SupportedConsumerDeploymentTypesEnum.All || consumer.SupportedDeploymentTypes == SupportedConsumerDeploymentTypesEnum.OnPremisesOnly))) : consumerImpls.Where<ConsumerImplementation>((Func<ConsumerImplementation, bool>) (consumer => consumer.SupportedDeploymentTypes == SupportedConsumerDeploymentTypesEnum.All || consumer.SupportedDeploymentTypes == SupportedConsumerDeploymentTypesEnum.OnPremisesOnly || consumer.SupportedDeploymentTypes == SupportedConsumerDeploymentTypesEnum.HostedOnly))).Select<ConsumerImplementation, Consumer>((Func<ConsumerImplementation, Consumer>) (consumerImpl => consumerImpl.ToConsumer())).ToList<Consumer>();
    }

    public static ConsumerAction ToConsumerAction(
      this ConsumerActionImplementation consumerActionImpl)
    {
      return new ConsumerAction()
      {
        AllowResourceVersionOverride = consumerActionImpl.AllowResourceVersionOverride,
        ConsumerId = consumerActionImpl.ConsumerId,
        Description = consumerActionImpl.Description,
        Id = consumerActionImpl.Id,
        Name = consumerActionImpl.Name,
        InputDescriptors = consumerActionImpl.InputDescriptors,
        SupportedEventTypes = consumerActionImpl.SupportedEventTypes,
        SupportedResourceVersions = consumerActionImpl.SupportedResourceVersions
      };
    }
  }
}
