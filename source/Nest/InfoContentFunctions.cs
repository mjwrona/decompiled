// Decompiled with JetBrains decompiler
// Type: Nest.InfoContentFunctions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public static class InfoContentFunctions
  {
    public static string GetStringValue(this InfoContentFunction infoContentFunction)
    {
      switch (infoContentFunction)
      {
        case InfoContentFunction.InfoContent:
          return "info_content";
        case InfoContentFunction.HighInfoContent:
          return "high_info_content";
        case InfoContentFunction.LowInfoContent:
          return "low_info_content";
        default:
          throw new ArgumentOutOfRangeException(nameof (infoContentFunction), (object) infoContentFunction, (string) null);
      }
    }
  }
}
