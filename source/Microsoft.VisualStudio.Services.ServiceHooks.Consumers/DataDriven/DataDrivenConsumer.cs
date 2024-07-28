// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven.DataDrivenConsumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven
{
  public abstract class DataDrivenConsumer : ConsumerImplementation
  {
    protected DataDrivenConsumerConfig m_consumerDataConfig;

    public DataDrivenConsumer() => this.m_consumerDataConfig = this.BuildConsumerDataConfig();

    public DataDrivenConsumer(DataDrivenConsumerConfig consumerConfig) => this.m_consumerDataConfig = consumerConfig;

    protected virtual DataDrivenConsumerConfig BuildConsumerDataConfig() => DataDrivenConsumerConfig.CreateFromConsumerType(this.GetType());

    public override string Id => this.m_consumerDataConfig.Id;

    public override string Name => this.m_consumerDataConfig.Name;

    public override string Description => this.m_consumerDataConfig.Description;

    public override AuthenticationType AuthenticationType => AuthenticationType.External;

    public override ExternalConfigurationDescriptor ExternalConfiguration => this.m_consumerDataConfig.ExternalConfiguration;

    public override string ImageUrl => this.m_consumerDataConfig.ImageUrl;

    public override string InformationUrl => this.m_consumerDataConfig.InformationUrl;

    public override IList<InputDescriptor> InputDescriptors => this.m_consumerDataConfig.InputDescriptors == null ? (IList<InputDescriptor>) new List<InputDescriptor>() : (IList<InputDescriptor>) this.m_consumerDataConfig.InputDescriptors;
  }
}
