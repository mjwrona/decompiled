// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven.DataDrivenConsumerActionConfig
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven
{
  public class DataDrivenConsumerActionConfig
  {
    public string ConsumerId { get; set; }

    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string[] SupportedEventTypes { get; set; }

    public IDictionary<string, string[]> SupportedResourceVersions { get; set; }

    public bool AllowResourceVersionOverride { get; set; }

    public InputDescriptor[] ConsumerInputDescriptors { get; set; }

    public InputDescriptor[] InputDescriptors { get; set; }

    public PublishEventHttpActionConfig PublishEvent { get; set; }

    public DataDrivenHttpActionConfig QueryInputValues { get; set; }

    public DataDrivenConsumerActionConfig.QueryInputValuesConfig[] OverrideQueryInputValues { get; set; }

    public static DataDrivenConsumerActionConfig CreateFromConsumerType(
      Type consumerType,
      string actionId)
    {
      return ((IEnumerable<DataDrivenConsumerActionConfig>) DataDrivenConsumerConfig.CreateFromConsumerType(consumerType).Actions).Single<DataDrivenConsumerActionConfig>((Func<DataDrivenConsumerActionConfig, bool>) (x => x.Id == actionId));
    }

    public InputDescriptor GetInputDescritor(string inputId)
    {
      InputDescriptor inputDescritor = (InputDescriptor) null;
      if (this.InputDescriptors != null)
        inputDescritor = ((IEnumerable<InputDescriptor>) this.InputDescriptors).SingleOrDefault<InputDescriptor>((Func<InputDescriptor, bool>) (x => x.Id == inputId));
      if (inputDescritor == null && this.ConsumerInputDescriptors != null)
        inputDescritor = ((IEnumerable<InputDescriptor>) this.ConsumerInputDescriptors).SingleOrDefault<InputDescriptor>((Func<InputDescriptor, bool>) (x => x.Id == inputId));
      return inputDescritor;
    }

    public class QueryInputValuesConfig
    {
      public string InputId { get; set; }

      public DataDrivenHttpActionConfig Query { get; set; }
    }
  }
}
