// Decompiled with JetBrains decompiler
// Type: Nest.InlineScriptDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class InlineScriptDescriptor : 
    ScriptDescriptorBase<InlineScriptDescriptor, IInlineScript>,
    IInlineScript,
    IScript
  {
    public InlineScriptDescriptor()
    {
    }

    public InlineScriptDescriptor(string script) => this.Self.Source = script;

    string IInlineScript.Source { get; set; }

    public InlineScriptDescriptor Source(string script) => this.Assign<string>(script, (Action<IInlineScript, string>) ((a, v) => a.Source = v));
  }
}
