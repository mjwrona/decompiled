// Decompiled with JetBrains decompiler
// Type: Nest.GeoIpProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class GeoIpProcessorDescriptor<T> : 
    ProcessorDescriptorBase<GeoIpProcessorDescriptor<T>, IGeoIpProcessor>,
    IGeoIpProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "geoip";

    string IGeoIpProcessor.DatabaseFile { get; set; }

    Nest.Field IGeoIpProcessor.Field { get; set; }

    bool? IGeoIpProcessor.IgnoreMissing { get; set; }

    IEnumerable<string> IGeoIpProcessor.Properties { get; set; }

    Nest.Field IGeoIpProcessor.TargetField { get; set; }

    bool? IGeoIpProcessor.FirstOnly { get; set; }

    public GeoIpProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IGeoIpProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public GeoIpProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IGeoIpProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public GeoIpProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IGeoIpProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public GeoIpProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IGeoIpProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public GeoIpProcessorDescriptor<T> TargetField<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IGeoIpProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public GeoIpProcessorDescriptor<T> DatabaseFile(string file) => this.Assign<string>(file, (Action<IGeoIpProcessor, string>) ((a, v) => a.DatabaseFile = v));

    public GeoIpProcessorDescriptor<T> Properties(IEnumerable<string> properties) => this.Assign<IEnumerable<string>>(properties, (Action<IGeoIpProcessor, IEnumerable<string>>) ((a, v) => a.Properties = v));

    public GeoIpProcessorDescriptor<T> Properties(params string[] properties) => this.Assign<string[]>(properties, (Action<IGeoIpProcessor, string[]>) ((a, v) => a.Properties = (IEnumerable<string>) v));

    public GeoIpProcessorDescriptor<T> FirstOnly(bool? firstOnly = true) => this.Assign<bool?>(firstOnly, (Action<IGeoIpProcessor, bool?>) ((a, v) => a.FirstOnly = v));
  }
}
