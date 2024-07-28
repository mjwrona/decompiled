// Decompiled with JetBrains decompiler
// Type: Nest.RareFunctionsExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public static class RareFunctionsExtensions
  {
    public static string GetStringValue(this RareFunction rareFunction)
    {
      if (rareFunction == RareFunction.Rare)
        return "rare";
      if (rareFunction == RareFunction.FreqRare)
        return "freq_rare";
      throw new ArgumentOutOfRangeException(nameof (rareFunction), (object) rareFunction, (string) null);
    }
  }
}
