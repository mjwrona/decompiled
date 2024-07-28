// Decompiled with JetBrains decompiler
// Type: Nest.SetSecurityUserProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class SetSecurityUserProcessorDescriptor<T> : 
    ProcessorDescriptorBase<SetSecurityUserProcessorDescriptor<T>, ISetSecurityUserProcessor>,
    ISetSecurityUserProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "set_security_user";

    Nest.Field ISetSecurityUserProcessor.Field { get; set; }

    IEnumerable<string> ISetSecurityUserProcessor.Properties { get; set; }

    public SetSecurityUserProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ISetSecurityUserProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public SetSecurityUserProcessorDescriptor<T> Field<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ISetSecurityUserProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));
    }

    public SetSecurityUserProcessorDescriptor<T> Properties(IEnumerable<string> properties) => this.Assign<IEnumerable<string>>(properties, (Action<ISetSecurityUserProcessor, IEnumerable<string>>) ((a, v) => a.Properties = v));

    public SetSecurityUserProcessorDescriptor<T> Properties(params string[] properties) => this.Assign<string[]>(properties, (Action<ISetSecurityUserProcessor, string[]>) ((a, v) => a.Properties = (IEnumerable<string>) v));
  }
}
