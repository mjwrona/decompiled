// Decompiled with JetBrains decompiler
// Type: Nest.PropertyNameExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Reflection;

namespace Nest
{
  internal static class PropertyNameExtensions
  {
    internal static bool IsConditionless(this PropertyName property)
    {
      if (property == (PropertyName) null)
        return true;
      return property.Name.IsNullOrEmpty() && property.Expression == null && property.Property == (PropertyInfo) null;
    }
  }
}
