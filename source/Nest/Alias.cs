// Decompiled with JetBrains decompiler
// Type: Nest.Alias
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class Alias : IAlias
  {
    public QueryContainer Filter { get; set; }

    public Routing IndexRouting { get; set; }

    public bool? IsWriteIndex { get; set; }

    public bool? IsHidden { get; set; }

    public Routing Routing { get; set; }

    public Routing SearchRouting { get; set; }
  }
}
