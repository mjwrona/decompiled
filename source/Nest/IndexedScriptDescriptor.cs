// Decompiled with JetBrains decompiler
// Type: Nest.IndexedScriptDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IndexedScriptDescriptor : 
    ScriptDescriptorBase<IndexedScriptDescriptor, IIndexedScript>,
    IIndexedScript,
    IScript
  {
    public IndexedScriptDescriptor()
    {
    }

    public IndexedScriptDescriptor(string id) => this.Self.Id = id;

    string IIndexedScript.Id { get; set; }

    public IndexedScriptDescriptor Id(string id) => this.Assign<string>(id, (Action<IIndexedScript, string>) ((a, v) => a.Id = v));
  }
}
