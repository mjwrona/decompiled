// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Version.Items.MavenVersionString
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Version.Helpers;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server.Version.Items
{
  public class MavenVersionString : IMavenVersionItem
  {
    private static readonly List<string> Qualifiers = new List<string>()
    {
      "alpha",
      "beta",
      "milestone",
      "rc",
      "snapshot",
      string.Empty,
      "sp"
    };
    private static readonly Dictionary<string, string> Aliases = new Dictionary<string, string>()
    {
      {
        "ga",
        string.Empty
      },
      {
        "final",
        string.Empty
      },
      {
        "release",
        string.Empty
      },
      {
        "cr",
        "rc"
      }
    };

    public MavenVersionString(string value, bool followedByDigit)
    {
      this.RawValue = value;
      if (followedByDigit && value.Length == 1)
      {
        switch (value[0])
        {
          case 'a':
            value = "alpha";
            break;
          case 'b':
            value = "beta";
            break;
          case 'm':
            value = "milestone";
            break;
          case 'r':
            value = "release";
            break;
        }
      }
      this.ResolvedValue = MavenVersionString.Aliases.ContainsKey(value) ? MavenVersionString.Aliases[value] : value;
      this.ComparableValue = this.ComparableQualifier(this.ResolvedValue);
    }

    public bool IsEquivalentToNull() => this.ResolvedValue == string.Empty;

    public override string ToString() => this.ResolvedValue;

    public string RawValue { get; }

    public string ResolvedValue { get; }

    public string ComparableValue { get; }

    public static int ReleaseVersionIndex { get; } = MavenVersionString.Qualifiers.IndexOf(string.Empty);

    public string ComparableQualifier(string qualifier)
    {
      int num = MavenVersionString.Qualifiers.IndexOf(qualifier);
      return num != -1 ? num.ToString() : MavenVersionString.Qualifiers.Count.ToString() + "-" + qualifier;
    }
  }
}
