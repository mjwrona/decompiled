// Decompiled with JetBrains decompiler
// Type: Nest.RuntimeFieldDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RuntimeFieldDescriptor : 
    DescriptorBase<RuntimeFieldDescriptor, IRuntimeField>,
    IRuntimeField
  {
    public RuntimeFieldDescriptor(FieldType fieldType) => this.Self.Type = fieldType;

    string IRuntimeField.Format { get; set; }

    IInlineScript IRuntimeField.Script { get; set; }

    FieldType IRuntimeField.Type { get; set; }

    public RuntimeFieldDescriptor Format(string format) => this.Assign<string>(format, (Action<IRuntimeField, string>) ((a, v) => a.Format = v));

    public RuntimeFieldDescriptor Script(IInlineScript script) => this.Assign<IInlineScript>(script, (Action<IRuntimeField, IInlineScript>) ((a, v) => a.Script = v));

    public RuntimeFieldDescriptor Script(string source) => this.Assign<string>(source, (Action<IRuntimeField, string>) ((a, v) => a.Script = (IInlineScript) new InlineScript(source)));

    public RuntimeFieldDescriptor Script(
      Func<InlineScriptDescriptor, IInlineScript> selector)
    {
      return this.Assign<Func<InlineScriptDescriptor, IInlineScript>>(selector, (Action<IRuntimeField, Func<InlineScriptDescriptor, IInlineScript>>) ((a, v) => a.Script = v != null ? v(new InlineScriptDescriptor()) : (IInlineScript) null));
    }
  }
}
