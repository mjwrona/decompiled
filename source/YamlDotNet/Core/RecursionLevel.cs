// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.RecursionLevel
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Core
{
  internal class RecursionLevel
  {
    private int current;

    public int Maximum { get; private set; }

    public RecursionLevel(int maximum) => this.Maximum = maximum;

    public void Increment()
    {
      if (!this.TryIncrement())
        throw new MaximumRecursionLevelReachedException();
    }

    public bool TryIncrement()
    {
      if (this.current >= this.Maximum)
        return false;
      ++this.current;
      return true;
    }

    public void Decrement()
    {
      if (this.current == 0)
        throw new InvalidOperationException("Attempted to decrement RecursionLevel to a negative value");
      --this.current;
    }
  }
}
