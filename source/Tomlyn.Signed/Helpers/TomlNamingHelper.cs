// Decompiled with JetBrains decompiler
// Type: Tomlyn.Helpers.TomlNamingHelper
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Text;


#nullable enable
namespace Tomlyn.Helpers
{
  public class TomlNamingHelper
  {
    [ThreadStatic]
    private static StringBuilder? _builder;

    public static string PascalToSnakeCase(string name)
    {
      StringBuilder builder = TomlNamingHelper.Builder;
      try
      {
        char c1 = char.MinValue;
        foreach (char c2 in name)
        {
          if (char.IsUpper(c2) && !char.IsUpper(c1) && c1 != char.MinValue && c1 != '_')
            builder.Append('_');
          builder.Append(char.ToLowerInvariant(c2));
          c1 = c2;
        }
        return builder.ToString();
      }
      finally
      {
        builder.Length = 0;
      }
    }

    private static StringBuilder Builder => TomlNamingHelper._builder ?? (TomlNamingHelper._builder = new StringBuilder());
  }
}
