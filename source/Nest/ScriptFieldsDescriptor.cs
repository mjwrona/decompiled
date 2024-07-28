// Decompiled with JetBrains decompiler
// Type: Nest.ScriptFieldsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ScriptFieldsDescriptor : 
    IsADictionaryDescriptorBase<ScriptFieldsDescriptor, IScriptFields, string, IScriptField>
  {
    public ScriptFieldsDescriptor()
      : base((IScriptFields) new ScriptFields())
    {
    }

    public ScriptFieldsDescriptor ScriptField(string name, Func<ScriptDescriptor, IScript> selector) => this.Assign(name, ScriptFieldsDescriptor.ToScript(selector != null ? selector(new ScriptDescriptor()) : (IScript) null));

    private static IScriptField ToScript(IScript script)
    {
      if (script == null)
        return (IScriptField) null;
      return (IScriptField) new Nest.ScriptField()
      {
        Script = script
      };
    }
  }
}
