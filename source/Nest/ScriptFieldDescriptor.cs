// Decompiled with JetBrains decompiler
// Type: Nest.ScriptFieldDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ScriptFieldDescriptor : 
    DescriptorBase<ScriptFieldDescriptor, IScriptField>,
    IScriptField
  {
    IScript IScriptField.Script { get; set; }

    public ScriptFieldDescriptor Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IScriptField, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
  }
}
