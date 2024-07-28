// Decompiled with JetBrains decompiler
// Type: Nest.RelationNameExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  internal static class RelationNameExtensions
  {
    internal static bool IsConditionless(this RelationName marker)
    {
      if (marker == (RelationName) null)
        return true;
      return marker.Name.IsNullOrEmpty() && marker.Type == (Type) null;
    }
  }
}
