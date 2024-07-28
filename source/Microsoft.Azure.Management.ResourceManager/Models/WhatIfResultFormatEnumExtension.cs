// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.WhatIfResultFormatEnumExtension
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  internal static class WhatIfResultFormatEnumExtension
  {
    internal static string ToSerializedValue(this WhatIfResultFormat? value) => !value.HasValue ? (string) null : value.Value.ToSerializedValue();

    internal static string ToSerializedValue(this WhatIfResultFormat value)
    {
      switch (value)
      {
        case WhatIfResultFormat.ResourceIdOnly:
          return "ResourceIdOnly";
        case WhatIfResultFormat.FullResourcePayloads:
          return "FullResourcePayloads";
        default:
          return (string) null;
      }
    }

    internal static WhatIfResultFormat? ParseWhatIfResultFormat(this string value)
    {
      switch (value)
      {
        case "ResourceIdOnly":
          return new WhatIfResultFormat?(WhatIfResultFormat.ResourceIdOnly);
        case "FullResourcePayloads":
          return new WhatIfResultFormat?(WhatIfResultFormat.FullResourcePayloads);
        default:
          return new WhatIfResultFormat?();
      }
    }
  }
}
