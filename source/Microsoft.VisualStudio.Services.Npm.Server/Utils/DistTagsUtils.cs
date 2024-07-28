// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.DistTagsUtils
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public class DistTagsUtils
  {
    public const string LatestTag = "latest";

    public Dictionary<string, string> MergeDistTags(
      IDictionary<string, string> local,
      IDictionary<string, string> remote,
      IEnumerable<IPackageVersion> unpublishedVersions,
      IEnumerable<IPackageVersion> allLocalAndUpstreamPublishedVersions)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      HashSet<IPackageVersion> second = new HashSet<IPackageVersion>(unpublishedVersions, (IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion);
      HashSet<IPackageVersion> first = new HashSet<IPackageVersion>(allLocalAndUpstreamPublishedVersions, (IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion);
      foreach (KeyValuePair<string, string> keyValuePair in local.Concat<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) remote))
      {
        string key = keyValuePair.Key;
        string str = keyValuePair.Value;
        SemanticVersion version;
        if (NpmVersionUtils.TryParseNpmPackageVersion(str.ToString(), out version) && !second.Contains((IPackageVersion) version) && first.Contains((IPackageVersion) version))
        {
          if (!dictionary.ContainsKey(key))
            dictionary.Add(key, str);
          else if (key == "latest" && NpmVersionUtils.ParseNpmPackageVersion(dictionary["latest"]).CompareTo(version) < 0)
            dictionary["latest"] = str;
        }
      }
      if (!dictionary.ContainsKey("latest"))
      {
        IPackageVersion packageVersion = first.Except<IPackageVersion>((IEnumerable<IPackageVersion>) second).OrderBy<IPackageVersion, IPackageVersion>((Func<IPackageVersion, IPackageVersion>) (x => x)).LastOrDefault<IPackageVersion>();
        if (packageVersion != null)
          dictionary["latest"] = packageVersion.ToString();
      }
      return dictionary;
    }
  }
}
