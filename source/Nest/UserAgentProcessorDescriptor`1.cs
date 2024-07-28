// Decompiled with JetBrains decompiler
// Type: Nest.UserAgentProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class UserAgentProcessorDescriptor<T> : 
    ProcessorDescriptorBase<UserAgentProcessorDescriptor<T>, IUserAgentProcessor>,
    IUserAgentProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "user_agent";

    Nest.Field IUserAgentProcessor.Field { get; set; }

    bool? IUserAgentProcessor.IgnoreMissing { get; set; }

    IEnumerable<UserAgentProperty> IUserAgentProcessor.Properties { get; set; }

    string IUserAgentProcessor.RegexFile { get; set; }

    Nest.Field IUserAgentProcessor.TargetField { get; set; }

    public UserAgentProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IUserAgentProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public UserAgentProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IUserAgentProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public UserAgentProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IUserAgentProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public UserAgentProcessorDescriptor<T> TargetField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IUserAgentProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));
    }

    public UserAgentProcessorDescriptor<T> RegexFile(string file) => this.Assign<string>(file, (Action<IUserAgentProcessor, string>) ((a, v) => a.RegexFile = v));

    public UserAgentProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IUserAgentProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public UserAgentProcessorDescriptor<T> Properties(IEnumerable<UserAgentProperty> properties) => this.Assign<IEnumerable<UserAgentProperty>>(properties, (Action<IUserAgentProcessor, IEnumerable<UserAgentProperty>>) ((a, v) => a.Properties = v));

    public UserAgentProcessorDescriptor<T> Properties(params UserAgentProperty[] properties) => this.Assign<UserAgentProperty[]>(properties, (Action<IUserAgentProcessor, UserAgentProperty[]>) ((a, v) => a.Properties = (IEnumerable<UserAgentProperty>) v));
  }
}
