// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.PyPiPackageVersionParser
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.VersionDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity
{
  public static class PyPiPackageVersionParser
  {
    public const string NormalizedAlphaPrefix = "a";
    public const string NormalizedBetaPrefix = "b";
    public const string NormalizedReleaseCandidatePrefix = "rc";
    public const string NormalizedPostPrefix = "post";
    public const string NormalizedDevPrefix = "dev";
    private const string CanonicalVersionPattern = "\r\n            ^\\s*\r\n            v?\r\n            (\r\n                ((?<epoch>[0-9]+)!)?\r\n                (?<release>[0-9]+(\\.[0-9]+)*)\r\n                (?<pre>\r\n                    [-_\\.]?\r\n                    (?<pre_l>(a|b|c|rc|alpha|beta|pre|preview))\r\n                    [-_\\.]?\r\n                    (?<pre_n>[0-9]+)?\r\n                )?\r\n                (?<post>\r\n                    (-(?<post_n1>[0-9]+))\r\n                    |\r\n                    (\r\n                        [-_\\.]?\r\n                        (?<post_l>post|rev|r)\r\n                        [-_\\.]?\r\n                        (?<post_n2>[0-9]+)?\r\n                    )\r\n                )?\r\n                (?<dev>\r\n                    [-_\\.]?\r\n                    (?<dev_l>dev)\r\n                    [-_\\.]?\r\n                    (?<dev_n>[0-9]+)?\r\n                )?\r\n            )\r\n            (\\+(?<local>[a-z0-9]+([-_\\.] [a-z0-9]+)*))?\r\n            \\s*$\r\n            ";
    private static readonly Regex CanonicalVersionRegex = new Regex("\r\n            ^\\s*\r\n            v?\r\n            (\r\n                ((?<epoch>[0-9]+)!)?\r\n                (?<release>[0-9]+(\\.[0-9]+)*)\r\n                (?<pre>\r\n                    [-_\\.]?\r\n                    (?<pre_l>(a|b|c|rc|alpha|beta|pre|preview))\r\n                    [-_\\.]?\r\n                    (?<pre_n>[0-9]+)?\r\n                )?\r\n                (?<post>\r\n                    (-(?<post_n1>[0-9]+))\r\n                    |\r\n                    (\r\n                        [-_\\.]?\r\n                        (?<post_l>post|rev|r)\r\n                        [-_\\.]?\r\n                        (?<post_n2>[0-9]+)?\r\n                    )\r\n                )?\r\n                (?<dev>\r\n                    [-_\\.]?\r\n                    (?<dev_l>dev)\r\n                    [-_\\.]?\r\n                    (?<dev_n>[0-9]+)?\r\n                )?\r\n            )\r\n            (\\+(?<local>[a-z0-9]+([-_\\.] [a-z0-9]+)*))?\r\n            \\s*$\r\n            ", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant, new TimeSpan(0, 0, 10));

    public static PyPiPackageVersion Parse(string rawVersion)
    {
      PyPiPackageVersion parsedVersion;
      if (PyPiPackageVersionParser.TryParse(rawVersion, out parsedVersion))
        return parsedVersion;
      throw new InvalidVersionException("Could not parse PyPI version " + rawVersion);
    }

    public static bool TryParse(string rawVersion, out PyPiPackageVersion parsedVersion)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(rawVersion, nameof (rawVersion));
      Match match = PyPiPackageVersionParser.CanonicalVersionRegex.Match(rawVersion);
      if (!match.Success)
      {
        parsedVersion = (PyPiPackageVersion) null;
        return false;
      }
      GroupCollection groups = match.Groups;
      try
      {
        BigInteger epoch = groups["epoch"].Success ? BigInteger.Parse(groups["epoch"].Value) : (BigInteger) 0;
        BigInteger[] bigIntegerArray;
        if (!groups["release"].Success)
        {
          bigIntegerArray = (BigInteger[]) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          bigIntegerArray = ((IEnumerable<string>) groups["release"].Value.Split('.')).Select<string, BigInteger>(PyPiPackageVersionParser.\u003C\u003EO.\u003C0\u003E__Parse ?? (PyPiPackageVersionParser.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, BigInteger>(BigInteger.Parse))).ToArray<BigInteger>();
        }
        BigInteger[] release = bigIntegerArray;
        string preLabel = (string) null;
        BigInteger? preNumber = new BigInteger?();
        if (groups["pre_l"].Success)
        {
          preLabel = PyPiPackageVersionParser.NormalizePrereleaseLabel(groups["pre_l"].Value);
          preNumber = new BigInteger?(groups["pre_n"].Success ? BigInteger.Parse(groups["pre_n"].Value) : (BigInteger) 0);
        }
        string postLabel = (string) null;
        BigInteger? postNumber = new BigInteger?();
        if (groups["post_n1"].Success)
        {
          postLabel = "post";
          postNumber = new BigInteger?(BigInteger.Parse(groups["post_n1"].Value));
        }
        else if (groups["post_l"].Success)
        {
          postLabel = PyPiPackageVersionParser.NormalizePostreleaseLabel(groups["post_l"].Value);
          postNumber = new BigInteger?(groups["post_n2"].Success ? BigInteger.Parse(groups["post_n2"].Value) : (BigInteger) 0);
        }
        string devLabel = (string) null;
        BigInteger? devNumber = new BigInteger?();
        if (groups["dev_l"].Success)
        {
          devLabel = "dev";
          devNumber = new BigInteger?(groups["dev_n"].Success ? BigInteger.Parse(groups["dev_n"].Value) : (BigInteger) 0);
        }
        IEnumerable<IVersionLabelSegment> segments = groups["local"].Success ? PyPiPackageLocalVersionUtils.ParseSegments(groups["local"].Value) : (IEnumerable<IVersionLabelSegment>) null;
        PyPiLocalVersionLabel local = segments != null ? new PyPiLocalVersionLabel(segments) : (PyPiLocalVersionLabel) null;
        parsedVersion = new PyPiPackageVersion(rawVersion, epoch, release, preLabel, preNumber, postLabel, postNumber, devLabel, devNumber, local);
      }
      catch (FormatException ex)
      {
        parsedVersion = (PyPiPackageVersion) null;
        return false;
      }
      return true;
    }

    private static string NormalizePrereleaseLabel(string pre)
    {
      pre = pre.ToLowerInvariant();
      switch (pre)
      {
        case "alpha":
          return "a";
        case "beta":
          return "b";
        case "c":
        case nameof (pre):
        case "preview":
          return "rc";
        default:
          return pre;
      }
    }

    private static string NormalizePostreleaseLabel(string post)
    {
      post = post.ToLowerInvariant();
      return post.Equals("rev") || post.Equals("r") ? nameof (post) : post;
    }
  }
}
