// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.LanguageFilePathExtensions
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public static class LanguageFilePathExtensions
  {
    private static readonly IDictionary<string, List<KeyValuePair<string, string>>> s_ambiguousNames = (IDictionary<string, List<KeyValuePair<string, string>>>) new Dictionary<string, List<KeyValuePair<string, string>>>()
    {
      {
        ".src",
        new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>(".app.src", "Erlang")
        }
      },
      {
        ".in",
        new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>(".cmake.in", "Erlang"),
          new KeyValuePair<string, string>(".rs.in", "Rust"),
          new KeyValuePair<string, string>(".sh.in", "Shell")
        }
      },
      {
        ".hl",
        new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>(".cljs.hl", "Clojure")
        }
      },
      {
        ".desktop",
        new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>(".emacs.desktop", "Emacs Lisp")
        }
      },
      {
        ".deface",
        new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>(".erb.deface", "ERB"),
          new KeyValuePair<string, string>(".haml.deface", "Haml")
        }
      }
    };

    public static string GetLanguageExtension(this string filePath)
    {
      string lowerInvariant = Path.GetExtension(filePath).ToLowerInvariant();
      List<KeyValuePair<string, string>> keyValuePairList;
      if (LanguageFilePathExtensions.s_ambiguousNames.TryGetValue(lowerInvariant, out keyValuePairList))
      {
        foreach (KeyValuePair<string, string> keyValuePair in keyValuePairList)
        {
          if (filePath.EndsWith(keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
            return keyValuePair.Value;
        }
      }
      return lowerInvariant;
    }
  }
}
