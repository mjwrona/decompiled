// Decompiled with JetBrains decompiler
// Type: Nest.TimeOfMonth
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class TimeOfMonth : ITimeOfMonth
  {
    public TimeOfMonth()
    {
    }

    public TimeOfMonth(int on, string at)
    {
      this.On = (IEnumerable<int>) new int[1]{ on };
      this.At = (IEnumerable<string>) new string[1]{ at };
    }

    public IEnumerable<string> At { get; set; }

    public IEnumerable<int> On { get; set; }
  }
}
