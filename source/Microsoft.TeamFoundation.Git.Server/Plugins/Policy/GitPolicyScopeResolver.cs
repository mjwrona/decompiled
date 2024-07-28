// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Plugins.Policy.GitPolicyScopeResolver
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server.Plugins.Policy
{
  public sealed class GitPolicyScopeResolver
  {
    internal static string PolicySettingsToScope(
      Guid? repositoryId = null,
      string refName = null,
      RefNameMatchType? matchKind = null)
    {
      RefNameMatchType? nullable1 = matchKind;
      RefNameMatchType refNameMatchType1 = RefNameMatchType.PrefixDeprecated;
      if (nullable1.GetValueOrDefault() == refNameMatchType1 & nullable1.HasValue)
        return "*";
      string scope = !repositoryId.HasValue ? "*" : repositoryId.Value.ToString("N");
      RefNameMatchType? nullable2 = matchKind;
      RefNameMatchType refNameMatchType2 = RefNameMatchType.DefaultBranch;
      if (nullable2.GetValueOrDefault() == refNameMatchType2 & nullable2.HasValue)
        return scope + ":~default";
      if (string.IsNullOrEmpty(refName))
        return scope;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(scope);
      stringBuilder.Append(":");
      RefNameMatchType? nullable3 = matchKind;
      RefNameMatchType refNameMatchType3 = RefNameMatchType.Prefix;
      if (nullable3.GetValueOrDefault() == refNameMatchType3 & nullable3.HasValue)
      {
        stringBuilder.Append(refName);
        stringBuilder.Append(refName.EndsWith("/") ? "*" : "/*");
      }
      else
        stringBuilder.Append(refName);
      return stringBuilder.ToString();
    }

    public static string[] RepositoryPathToScopes(
      Guid? repositoryId,
      bool includesDefaultBranch = false,
      params string[] refNames)
    {
      HashSet<string> source = new HashSet<string>();
      source.Add("*");
      if (repositoryId.HasValue)
      {
        Guid? nullable = repositoryId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          source.Add(repositoryId.Value.ToString("N"));
      }
      List<string> stringList = new List<string>();
      if (refNames != null)
        stringList.AddRange(((IEnumerable<string>) refNames).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))));
      if (includesDefaultBranch)
        stringList.Add("~default");
      using (List<string>.Enumerator enumerator = stringList.GetEnumerator())
      {
label_17:
        while (enumerator.MoveNext())
        {
          string[] strArray = enumerator.Current.Split(new char[1]
          {
            '/'
          }, StringSplitOptions.RemoveEmptyEntries);
          int index = 0;
          while (true)
          {
            if (index < strArray.Length && !(strArray[index] == "*"))
            {
              StringBuilder stringBuilder = new StringBuilder(":");
              stringBuilder.Append(string.Join("/", strArray, 0, index + 1));
              if (index != strArray.Length - 1)
                stringBuilder.Append("/*");
              source.Add("*" + stringBuilder.ToString());
              if (repositoryId.HasValue)
              {
                Guid? nullable = repositoryId;
                Guid empty = Guid.Empty;
                if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
                  source.Add(repositoryId.Value.ToString("N") + stringBuilder.ToString());
              }
              ++index;
            }
            else
              goto label_17;
          }
        }
      }
      return source.ToArray<string>();
    }
  }
}
