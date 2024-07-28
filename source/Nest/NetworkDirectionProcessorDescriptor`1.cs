// Decompiled with JetBrains decompiler
// Type: Nest.NetworkDirectionProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class NetworkDirectionProcessorDescriptor<T> : 
    ProcessorDescriptorBase<NetworkDirectionProcessorDescriptor<T>, INetworkDirectionProcessor>,
    INetworkDirectionProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "network_direction";

    Field INetworkDirectionProcessor.DestinationIp { get; set; }

    bool? INetworkDirectionProcessor.IgnoreMissing { get; set; }

    IEnumerable<string> INetworkDirectionProcessor.InternalNetworks { get; set; }

    Field INetworkDirectionProcessor.InternalNetworksField { get; set; }

    Field INetworkDirectionProcessor.SourceIp { get; set; }

    Field INetworkDirectionProcessor.TargetField { get; set; }

    public NetworkDirectionProcessorDescriptor<T> DestinationIp(Field field) => this.Assign<Field>(field, (Action<INetworkDirectionProcessor, Field>) ((a, v) => a.DestinationIp = v));

    public NetworkDirectionProcessorDescriptor<T> DestinationIp<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkDirectionProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.DestinationIp = (Field) (Expression) v));
    }

    public NetworkDirectionProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<INetworkDirectionProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public NetworkDirectionProcessorDescriptor<T> InternalNetworks(
      IEnumerable<string> internalNetworks)
    {
      return this.Assign<IEnumerable<string>>(internalNetworks, (Action<INetworkDirectionProcessor, IEnumerable<string>>) ((a, v) => a.InternalNetworks = v));
    }

    public NetworkDirectionProcessorDescriptor<T> InternalNetworks(params string[] internalNetworks) => this.Assign<string[]>(internalNetworks, (Action<INetworkDirectionProcessor, string[]>) ((a, v) => a.InternalNetworks = (IEnumerable<string>) v));

    public NetworkDirectionProcessorDescriptor<T> InternalNetworksField(Field internalNetworksField) => this.Assign<Field>(internalNetworksField, (Action<INetworkDirectionProcessor, Field>) ((a, v) => a.InternalNetworksField = v));

    public NetworkDirectionProcessorDescriptor<T> InternalNetworksField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkDirectionProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.InternalNetworksField = (Field) (Expression) v));
    }

    public NetworkDirectionProcessorDescriptor<T> SourceIp(Field field) => this.Assign<Field>(field, (Action<INetworkDirectionProcessor, Field>) ((a, v) => a.SourceIp = v));

    public NetworkDirectionProcessorDescriptor<T> SourceIp<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkDirectionProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.SourceIp = (Field) (Expression) v));
    }

    public NetworkDirectionProcessorDescriptor<T> TargetField(Field field) => this.Assign<Field>(field, (Action<INetworkDirectionProcessor, Field>) ((a, v) => a.TargetField = v));

    public NetworkDirectionProcessorDescriptor<T> TargetField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkDirectionProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Field) (Expression) v));
    }
  }
}
