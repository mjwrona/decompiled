// Decompiled with JetBrains decompiler
// Type: Nest.NetworkCommunityIdProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class NetworkCommunityIdProcessorDescriptor<T> : 
    ProcessorDescriptorBase<NetworkCommunityIdProcessorDescriptor<T>, INetworkCommunityIdProcessor>,
    INetworkCommunityIdProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "community_id";

    Field INetworkCommunityIdProcessor.DestinationIp { get; set; }

    Field INetworkCommunityIdProcessor.DestinationPort { get; set; }

    Field INetworkCommunityIdProcessor.IanaNumber { get; set; }

    Field INetworkCommunityIdProcessor.IcmpType { get; set; }

    Field INetworkCommunityIdProcessor.IcmpCode { get; set; }

    bool? INetworkCommunityIdProcessor.IgnoreMissing { get; set; }

    int? INetworkCommunityIdProcessor.Seed { get; set; }

    Field INetworkCommunityIdProcessor.SourceIp { get; set; }

    Field INetworkCommunityIdProcessor.SourcePort { get; set; }

    Field INetworkCommunityIdProcessor.TargetField { get; set; }

    Field INetworkCommunityIdProcessor.Transport { get; set; }

    public NetworkCommunityIdProcessorDescriptor<T> DestinationIp(Field field) => this.Assign<Field>(field, (Action<INetworkCommunityIdProcessor, Field>) ((a, v) => a.DestinationIp = v));

    public NetworkCommunityIdProcessorDescriptor<T> DestinationIp<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkCommunityIdProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.DestinationIp = (Field) (Expression) v));
    }

    public NetworkCommunityIdProcessorDescriptor<T> DestinationPort(Field field) => this.Assign<Field>(field, (Action<INetworkCommunityIdProcessor, Field>) ((a, v) => a.DestinationPort = v));

    public NetworkCommunityIdProcessorDescriptor<T> DestinationPort<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkCommunityIdProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.DestinationPort = (Field) (Expression) v));
    }

    public NetworkCommunityIdProcessorDescriptor<T> IanaNumber(Field field) => this.Assign<Field>(field, (Action<INetworkCommunityIdProcessor, Field>) ((a, v) => a.IanaNumber = v));

    public NetworkCommunityIdProcessorDescriptor<T> IanaNumber<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkCommunityIdProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.IanaNumber = (Field) (Expression) v));
    }

    public NetworkCommunityIdProcessorDescriptor<T> IcmpType(Field field) => this.Assign<Field>(field, (Action<INetworkCommunityIdProcessor, Field>) ((a, v) => a.IcmpType = v));

    public NetworkCommunityIdProcessorDescriptor<T> IcmpType<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkCommunityIdProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.IcmpType = (Field) (Expression) v));
    }

    public NetworkCommunityIdProcessorDescriptor<T> IcmpCode(Field field) => this.Assign<Field>(field, (Action<INetworkCommunityIdProcessor, Field>) ((a, v) => a.IcmpCode = v));

    public NetworkCommunityIdProcessorDescriptor<T> IcmpCode<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkCommunityIdProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.IcmpCode = (Field) (Expression) v));
    }

    public NetworkCommunityIdProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<INetworkCommunityIdProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public NetworkCommunityIdProcessorDescriptor<T> Seed(int? seed = null) => this.Assign<int?>(seed, (Action<INetworkCommunityIdProcessor, int?>) ((a, v) => a.Seed = v));

    public NetworkCommunityIdProcessorDescriptor<T> SourceIp(Field field) => this.Assign<Field>(field, (Action<INetworkCommunityIdProcessor, Field>) ((a, v) => a.SourceIp = v));

    public NetworkCommunityIdProcessorDescriptor<T> SourceIp<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkCommunityIdProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.SourceIp = (Field) (Expression) v));
    }

    public NetworkCommunityIdProcessorDescriptor<T> SourcePort(Field field) => this.Assign<Field>(field, (Action<INetworkCommunityIdProcessor, Field>) ((a, v) => a.SourcePort = v));

    public NetworkCommunityIdProcessorDescriptor<T> SourcePort<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkCommunityIdProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.SourcePort = (Field) (Expression) v));
    }

    public NetworkCommunityIdProcessorDescriptor<T> TargetField(Field field) => this.Assign<Field>(field, (Action<INetworkCommunityIdProcessor, Field>) ((a, v) => a.TargetField = v));

    public NetworkCommunityIdProcessorDescriptor<T> TargetField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkCommunityIdProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Field) (Expression) v));
    }

    public NetworkCommunityIdProcessorDescriptor<T> Transport(Field field) => this.Assign<Field>(field, (Action<INetworkCommunityIdProcessor, Field>) ((a, v) => a.Transport = v));

    public NetworkCommunityIdProcessorDescriptor<T> Transport<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INetworkCommunityIdProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Transport = (Field) (Expression) v));
    }
  }
}
