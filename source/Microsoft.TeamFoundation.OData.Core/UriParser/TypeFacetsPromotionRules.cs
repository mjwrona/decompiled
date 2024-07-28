// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.TypeFacetsPromotionRules
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData.UriParser
{
  public class TypeFacetsPromotionRules
  {
    public virtual int? GetPromotedPrecision(int? left, int? right)
    {
      if (!left.HasValue)
        return right;
      return right.HasValue ? new int?(Math.Max(left.Value, right.Value)) : left;
    }

    public virtual int? GetPromotedScale(int? left, int? right)
    {
      if (!left.HasValue)
        return right;
      return right.HasValue ? new int?(Math.Max(left.Value, right.Value)) : left;
    }
  }
}
