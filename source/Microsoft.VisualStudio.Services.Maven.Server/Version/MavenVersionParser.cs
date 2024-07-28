// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Version.MavenVersionParser
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Version.Helpers;
using Microsoft.VisualStudio.Services.Maven.Server.Version.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Version
{
  public class MavenVersionParser
  {
    public MavenVersionParser(string version)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(version, nameof (version));
      this.IsHash = version.Length >= 32 && version.IndexOfAny(MavenVersionSeparators.All) == -1;
      this.IsGuid = Guid.TryParse(version, out Guid _);
      bool flag = this.IsHash || this.IsGuid;
      if (flag)
      {
        this.VersionList = (MavenVersionList) null;
        this.CanonicalVersion = version.ToLowerInvariant();
      }
      else
      {
        this.VersionList = MavenVersionParser.ParseVersion(version);
        this.CanonicalVersion = this.VersionList.ToString();
      }
      this.RawVersion = version;
      this.IsRelease = flag || !version.Contains<char>('-');
    }

    public string RawVersion { get; }

    public string CanonicalVersion { get; }

    public bool IsGuid { get; }

    public bool IsHash { get; }

    public bool IsRelease { get; }

    public MavenVersionList VersionList { get; }

    private static MavenVersionList ParseVersion(string value)
    {
      string lowerInvariant = value.ToLowerInvariant();
      MavenVersionList version = new MavenVersionList();
      MavenVersionList mavenVersionList = version;
      Stack<MavenVersionList> source = new Stack<MavenVersionList>();
      source.Push(mavenVersionList);
      bool isDigit = false;
      int length = lowerInvariant.Length;
      int startIndexInclusive = 0;
      for (int index = 0; index < length; ++index)
      {
        char c = lowerInvariant[index];
        switch (c)
        {
          case '-':
            if (index == startIndexInclusive)
              mavenVersionList.Add((IMavenVersionItem) MavenVersionInteger.Zero);
            else
              mavenVersionList.Add(MavenVersionParser.ParseItem(isDigit, MavenVersionParser.Substring(lowerInvariant, startIndexInclusive, new int?(index))));
            startIndexInclusive = index + 1;
            mavenVersionList.Add((IMavenVersionItem) (mavenVersionList = new MavenVersionList()));
            source.Push(mavenVersionList);
            break;
          case '.':
            if (index == startIndexInclusive)
              mavenVersionList.Add((IMavenVersionItem) MavenVersionInteger.Zero);
            else
              mavenVersionList.Add(MavenVersionParser.ParseItem(isDigit, MavenVersionParser.Substring(lowerInvariant, startIndexInclusive, new int?(index))));
            startIndexInclusive = index + 1;
            break;
          default:
            if (char.IsDigit(c))
            {
              if (!isDigit && index > startIndexInclusive)
              {
                mavenVersionList.Add((IMavenVersionItem) new MavenVersionString(MavenVersionParser.Substring(lowerInvariant, startIndexInclusive, new int?(index)), true));
                startIndexInclusive = index;
                mavenVersionList.Add((IMavenVersionItem) (mavenVersionList = new MavenVersionList()));
                source.Push(mavenVersionList);
              }
              isDigit = true;
              break;
            }
            if (isDigit && index > startIndexInclusive)
            {
              mavenVersionList.Add(MavenVersionParser.ParseItem(true, MavenVersionParser.Substring(lowerInvariant, startIndexInclusive, new int?(index))));
              startIndexInclusive = index;
              mavenVersionList.Add((IMavenVersionItem) (mavenVersionList = new MavenVersionList()));
              source.Push(mavenVersionList);
            }
            isDigit = false;
            break;
        }
      }
      if (lowerInvariant.Length > startIndexInclusive)
        mavenVersionList.Add(MavenVersionParser.ParseItem(isDigit, MavenVersionParser.Substring(lowerInvariant, startIndexInclusive)));
      while (source.Any<MavenVersionList>())
        source.Pop().Normalize();
      return version;
    }

    private static string Substring(string value, int startIndexInclusive, int? endIndexExclusive = null)
    {
      int length1 = (endIndexExclusive ?? value.Length) - startIndexInclusive;
      if (startIndexInclusive == 0)
      {
        int? nullable = endIndexExclusive;
        int length2 = value.Length;
        if (nullable.GetValueOrDefault() == length2 & nullable.HasValue)
          return value;
      }
      return value.Substring(startIndexInclusive, length1);
    }

    private static IMavenVersionItem ParseItem(bool isDigit, string buf) => isDigit ? (IMavenVersionItem) new MavenVersionInteger(buf) : (IMavenVersionItem) new MavenVersionString(buf, false);
  }
}
