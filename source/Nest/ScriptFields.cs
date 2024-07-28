// Decompiled with JetBrains decompiler
// Type: Nest.ScriptFields
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class ScriptFields : 
    IsADictionaryBase<string, IScriptField>,
    IScriptFields,
    IIsADictionary<string, IScriptField>,
    IDictionary<string, IScriptField>,
    ICollection<KeyValuePair<string, IScriptField>>,
    IEnumerable<KeyValuePair<string, IScriptField>>,
    IEnumerable,
    IIsADictionary
  {
    public ScriptFields()
    {
    }

    public ScriptFields(IDictionary<string, IScriptField> container)
      : base(container)
    {
    }

    public ScriptFields(Dictionary<string, IScriptField> container)
      : base((IDictionary<string, IScriptField>) container)
    {
    }

    public void Add(string name, IScriptField script) => this.BackingDictionary.Add(name, script);

    public void Add(string name, IScript script) => this.BackingDictionary.Add(name, (IScriptField) new ScriptField()
    {
      Script = script
    });
  }
}
