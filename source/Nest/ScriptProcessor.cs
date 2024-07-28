// Decompiled with JetBrains decompiler
// Type: Nest.ScriptProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class ScriptProcessor : ProcessorBase, IScriptProcessor, IProcessor
  {
    public string Id { get; set; }

    public string Lang { get; set; }

    public Dictionary<string, object> Params { get; set; }

    public string Source { get; set; }

    protected override string Name => "script";
  }
}
