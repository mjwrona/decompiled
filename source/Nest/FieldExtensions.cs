// Decompiled with JetBrains decompiler
// Type: Nest.FieldExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq;
using System.Reflection;

namespace Nest
{
  internal static class FieldExtensions
  {
    internal static bool IsConditionless(this Field field)
    {
      if (field == (Field) null)
        return true;
      return field.Name.IsNullOrEmpty() && field.Expression == null && field.Property == (PropertyInfo) null;
    }

    internal static bool IsConditionless(this Fields field) => field?.ListOfFields == null || field.ListOfFields.All<Field>((Func<Field, bool>) (l => l.IsConditionless()));
  }
}
