// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzerBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class AnalyzerBase : IAnalyzer
  {
    internal AnalyzerBase()
    {
    }

    protected AnalyzerBase(string type) => this.Type = type;

    public virtual string Type { get; protected set; }

    [Obsolete("Setting a version on analysis component has no effect and is deprecated.")]
    public string Version { get; set; }
  }
}
