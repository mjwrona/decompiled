// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildSourceProviders
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Build.Common
{
  public static class BuildSourceProviders
  {
    public static readonly string TfVersionControl = "TFVC";
    public static readonly string TfGit = "TFGIT";
    public static readonly string Git = "GIT";
    public static readonly string Svn = "SVN";
    private static readonly IReadOnlyList<string> s_supportedRefFilterPrefixes = (IReadOnlyList<string>) new string[3]
    {
      "refs/heads/",
      "refs/tags/",
      "refs/pull/"
    };

    public static bool IsTfGit(string sourceProvider) => StringComparer.OrdinalIgnoreCase.Equals(sourceProvider, BuildSourceProviders.TfGit);

    public static bool IsGit(string sourceProvider) => StringComparer.OrdinalIgnoreCase.Equals(sourceProvider, BuildSourceProviders.Git) || BuildSourceProviders.IsTfGit(sourceProvider);

    public static bool IsTfVersionControl(string sourceProvider) => string.IsNullOrEmpty(sourceProvider) || StringComparer.OrdinalIgnoreCase.Equals(sourceProvider, BuildSourceProviders.TfVersionControl);

    public static string GetProperty(IDictionary<string, string> propertyBag, string propertyName)
    {
      string str;
      return propertyBag.TryGetValue(propertyName, out str) ? str : (string) null;
    }

    public static class GitProperties
    {
      public static readonly char PathSeparator = '/';
      public static readonly string RepositoryUrl = nameof (RepositoryUrl);
      public static readonly string RepositoryName = nameof (RepositoryName);
      public static readonly string DefaultBranch = nameof (DefaultBranch);
      public static readonly string CIBranches = nameof (CIBranches);
      public static readonly string GitPathBeginning = LinkingUtilities.VSTFS + "Git" + BuildSourceProviders.GitProperties.PathSeparator.ToString() + "VersionedItem" + BuildSourceProviders.GitProperties.PathSeparator.ToString();
      public static readonly string BranchPrefix = "refs/heads/";
      private const string BranchSeparator = ":";
      private const string BranchInclusionOperator = "+";
      private const string BranchExclusionOperator = "-";

      public static void ParseBranchSpec(
        string branchSpec,
        out bool excludeBranch,
        out string branch)
      {
        BuildSourceProviders.GitProperties.ParseBranchSpec(branchSpec, out excludeBranch, out branch, out bool _, false);
      }

      public static void ParseBranchSpec(
        string branchSpec,
        out bool excludeBranch,
        out string branch,
        out bool isPattern,
        bool removePattern)
      {
        excludeBranch = false;
        isPattern = false;
        branch = branchSpec;
        if (string.IsNullOrEmpty(branchSpec))
          return;
        if (branchSpec.StartsWith("-", StringComparison.Ordinal))
        {
          excludeBranch = true;
          branch = branch.Substring(1);
        }
        else if (branchSpec.StartsWith("+", StringComparison.Ordinal))
        {
          excludeBranch = false;
          branch = branch.Substring(1);
        }
        if (!branchSpec.EndsWith("*", StringComparison.Ordinal))
          return;
        isPattern = true;
        if (!removePattern)
          return;
        branch = branch.Substring(0, branch.Length - 1);
      }

      public static bool IsExcludeFilter(string filter) => filter.StartsWith("-", StringComparison.Ordinal);

      public static bool IsRepositoryBranchIncluded(IEnumerable<string> branchSpecs, string branch)
      {
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        if (!string.IsNullOrEmpty(branch) && branchSpecs != null)
        {
          string refName = BuildSourceProviders.GitProperties.BranchToRefName(branch);
          string str = BuildSourceProviders.s_supportedRefFilterPrefixes.FirstOrDefault<string>((Func<string, bool>) (prefix => refName.StartsWith(prefix)));
          if (string.IsNullOrEmpty(str))
            return false;
          foreach (string branchSpec in branchSpecs)
          {
            string branch1 = branchSpec;
            bool flag4 = BuildSourceProviders.GitProperties.IsExcludeFilter(branchSpec);
            if (flag4 || branchSpec.StartsWith("+", StringComparison.Ordinal))
              branch1 = branchSpec.Substring(1);
            string refName1 = BuildSourceProviders.GitProperties.BranchToRefName(branch1);
            if (refName1.StartsWith(str) || refName1.StartsWith("refs/*", StringComparison.Ordinal) || refName1.StartsWith("refs/?", StringComparison.Ordinal))
            {
              flag1 = true;
              flag2 |= !flag4;
              if (!Wildcard.IsWildcard(refName1) ? refName.Equals(refName1, StringComparison.Ordinal) : Wildcard.Match(refName, refName1))
              {
                if (flag4)
                  return false;
                flag3 = true;
              }
            }
          }
        }
        if (flag3)
          return true;
        return flag1 && !flag2;
      }

      public static bool IsFilePathIncluded(IEnumerable<string> pathFilters, string filePath)
      {
        if (pathFilters == null || string.IsNullOrEmpty(filePath))
          return false;
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        int num = -1;
        foreach (string pathFilter in pathFilters)
        {
          bool flag4 = BuildSourceProviders.GitProperties.IsExcludeFilter(pathFilter);
          if (!flag4)
            flag1 = true;
          string str = pathFilter.TrimStart(char.Parse("+"), char.Parse("-")).Trim(BuildSourceProviders.GitProperties.PathSeparator);
          string filePath1 = filePath.TrimStart(BuildSourceProviders.GitProperties.PathSeparator);
          int depth1 = BuildSourceProviders.GitProperties.GetDepth(str);
          int depth2 = BuildSourceProviders.GitProperties.GetDepth(filePath);
          if (depth1 <= depth2 && BuildSourceProviders.GitProperties.IsFilePathMatchPattern(filePath1, str))
          {
            flag2 = true;
            if (depth1 >= num)
            {
              if (depth1 > num)
              {
                num = depth1;
                flag3 = !flag4;
              }
              else if (depth1 == num & flag4)
                flag3 = false;
            }
          }
        }
        return !flag2 ? !flag1 : flag3;
      }

      private static bool IsFilePathMatchPattern(string filePath, string pattern)
      {
        if (pattern.Equals("", StringComparison.Ordinal))
          return true;
        if (Wildcard.IsWildcard(pattern))
          return Wildcard.Match(filePath, pattern);
        return filePath.Equals(pattern, StringComparison.Ordinal) || Wildcard.Match(filePath, pattern + "/*");
      }

      private static int GetDepth(string path) => path.Split(BuildSourceProviders.GitProperties.PathSeparator).Length;

      public static string CreateBranchSpec(bool excludeBranch, string branch)
      {
        StringBuilder stringBuilder = new StringBuilder();
        if (excludeBranch)
          stringBuilder.Append("-");
        stringBuilder.Append(branch);
        return stringBuilder.ToString();
      }

      public static string JoinBranches(IEnumerable<string> branches)
      {
        if (branches != null)
        {
          string[] array = branches.ToArray<string>();
          if (array.Length != 0)
            return string.Join(":", array);
        }
        return string.Empty;
      }

      public static List<string> SplitBranches(string branchString)
      {
        if (string.IsNullOrEmpty(branchString))
          return new List<string>();
        return ((IEnumerable<string>) branchString.Split(new string[1]
        {
          ":"
        }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
      }

      public static bool ParseGitPath(
        string gitFullPath,
        out string projectName,
        out string repositoryName,
        out string branchAndPath)
      {
        projectName = (string) null;
        repositoryName = (string) null;
        branchAndPath = (string) null;
        string gitPath;
        if (!BuildSourceProviders.GitProperties.IsGitUri(gitFullPath, out gitPath))
          return false;
        int length1 = gitPath.IndexOf(BuildSourceProviders.GitProperties.PathSeparator);
        if (length1 < 0 || length1 > gitPath.Length - 2)
          return false;
        projectName = gitPath.Substring(0, length1);
        string str = gitPath.Substring(length1 + 1).Trim(BuildSourceProviders.GitProperties.PathSeparator);
        int length2 = str.IndexOf(BuildSourceProviders.GitProperties.PathSeparator);
        if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(str) || length2 < 0 || length2 > str.Length - 2)
          return false;
        repositoryName = str.Substring(0, length2);
        branchAndPath = str.Substring(length2 + 1);
        return !string.IsNullOrEmpty(branchAndPath);
      }

      public static bool ParseBranchAndPath(
        string branchAndPath,
        IEnumerable<GitRef> branches,
        out string branchName,
        out string path)
      {
        bool branchAndPath1 = false;
        branchName = string.Empty;
        path = string.Empty;
        foreach (GitRef branch in branches)
        {
          if (branch.Name.StartsWith(BuildSourceProviders.GitProperties.BranchPrefix, StringComparison.Ordinal))
          {
            branchName = BuildSourceProviders.GitProperties.RefToBranchName(branch.Name);
            if (branchAndPath.StartsWith(branchName + BuildSourceProviders.GitProperties.PathSeparator.ToString()))
            {
              branchAndPath1 = true;
              path = branchAndPath.Substring(branchName.Length).Trim(BuildSourceProviders.GitProperties.PathSeparator);
              break;
            }
          }
        }
        return branchAndPath1;
      }

      public static string GetItemName(string path)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
        string gitPath;
        if (BuildSourceProviders.GitProperties.IsGitUri(path, out gitPath))
        {
          int num = gitPath.LastIndexOf(BuildSourceProviders.GitProperties.PathSeparator);
          if (num > -1)
            return gitPath.Substring(num + 1);
        }
        return string.Empty;
      }

      public static bool IsUniqueRepoName(string repoName) => !string.IsNullOrEmpty(repoName) && repoName.Contains<char>(BuildSourceProviders.GitProperties.PathSeparator);

      public static string CreateUniqueRepoName(string teamProjectName, string repoName) => teamProjectName + BuildSourceProviders.GitProperties.PathSeparator.ToString() + repoName;

      public static bool GetRepositoryName(
        IDictionary<string, string> propertyBag,
        out string teamProjectName,
        out string repositoryName)
      {
        teamProjectName = string.Empty;
        repositoryName = string.Empty;
        return BuildSourceProviders.GitProperties.ParseUniqueRepoName(BuildSourceProviders.GetProperty(propertyBag, BuildSourceProviders.GitProperties.RepositoryName), out teamProjectName, out repositoryName);
      }

      public static bool ParseUniqueRepoName(
        string uniqueRepoName,
        out string teamProjectName,
        out string repoName)
      {
        teamProjectName = string.Empty;
        repoName = uniqueRepoName;
        if (string.IsNullOrEmpty(uniqueRepoName) || !BuildSourceProviders.GitProperties.IsUniqueRepoName(uniqueRepoName))
          return false;
        string[] strArray = uniqueRepoName.Split(BuildSourceProviders.GitProperties.PathSeparator);
        if (strArray.Length != 2)
          return false;
        teamProjectName = strArray[0];
        repoName = strArray[1];
        return true;
      }

      public static string CreateGitRepositoryUrl(
        string collectionUrl,
        string teamProjectName,
        string repoName)
      {
        if (string.IsNullOrEmpty(collectionUrl) || string.IsNullOrEmpty(repoName))
          return string.Empty;
        if (!collectionUrl.EndsWith("/", StringComparison.Ordinal))
          collectionUrl += "/";
        if (string.IsNullOrEmpty(teamProjectName))
          teamProjectName = repoName;
        return collectionUrl + Uri.EscapeDataString(teamProjectName) + "/_git/" + Uri.EscapeDataString(repoName);
      }

      public static string CreateGitRepositoryUrlFromLocationUrl(
        string locationUrl,
        string teamProjectName,
        string repoName)
      {
        if (string.IsNullOrEmpty(locationUrl) || string.IsNullOrEmpty(repoName))
          return string.Empty;
        locationUrl = locationUrl.Replace("/{teamName}", "");
        locationUrl = locationUrl.Replace("{projectName}", teamProjectName);
        locationUrl = locationUrl.Replace("{repoName}", repoName);
        return locationUrl;
      }

      public static bool IsGitUri(string gitFullPath) => BuildSourceProviders.GitProperties.IsGitUri(gitFullPath, out string _);

      public static bool IsGitUri(string gitFullPath, out string gitPath)
      {
        gitPath = string.Empty;
        ArgumentUtility.CheckForNull<string>(gitFullPath, nameof (gitFullPath));
        if (!Uri.TryCreate(gitFullPath, UriKind.Absolute, out Uri _) || !VssStringComparer.UriScheme.StartsWith(gitFullPath, BuildSourceProviders.GitProperties.GitPathBeginning))
          return false;
        gitPath = gitFullPath.Substring(BuildSourceProviders.GitProperties.GitPathBeginning.Length);
        gitPath = Uri.UnescapeDataString(gitPath).Trim(BuildSourceProviders.GitProperties.PathSeparator);
        return gitPath.IndexOf(BuildSourceProviders.GitProperties.PathSeparator) > 0;
      }

      public static string CreateGitUri(
        string projectName,
        string repoName,
        string branch,
        string path)
      {
        if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(repoName) || string.IsNullOrEmpty(branch))
          return string.Empty;
        projectName = projectName.Trim(BuildSourceProviders.GitProperties.PathSeparator);
        repoName = repoName.Trim(BuildSourceProviders.GitProperties.PathSeparator);
        StringBuilder stringBuilder = new StringBuilder(BuildSourceProviders.GitProperties.GitPathBeginning);
        stringBuilder.Append(projectName);
        stringBuilder.Append(BuildSourceProviders.GitProperties.PathSeparator);
        stringBuilder.Append(repoName);
        stringBuilder.Append(BuildSourceProviders.GitProperties.PathSeparator);
        stringBuilder.Append(BuildSourceProviders.GitProperties.RefToBranchName(branch).Trim(BuildSourceProviders.GitProperties.PathSeparator));
        stringBuilder.Append(BuildSourceProviders.GitProperties.PathSeparator);
        if (!string.IsNullOrEmpty(path))
        {
          string[] strArray = path.Trim(BuildSourceProviders.GitProperties.PathSeparator).Split(new char[2]
          {
            BuildSourceProviders.GitProperties.PathSeparator,
            '\\'
          }, StringSplitOptions.RemoveEmptyEntries);
          for (int index = 0; index < strArray.Length; ++index)
          {
            string error;
            if (!strArray[index].Equals("..") && !strArray[index].Equals(".") && !FileSpec.IsLegalNtfsName(strArray[index], BuildConstants.MaxPathNameLength, true, out error))
              throw new InvalidPathException(error);
            stringBuilder.AppendFormat("{0}{1}", (object) strArray[index], index == strArray.Length - 1 ? (object) string.Empty : (object) BuildSourceProviders.GitProperties.PathSeparator.ToString());
          }
        }
        return new Uri(stringBuilder.ToString()).AbsoluteUri;
      }

      public static string BranchToRefName(string branch)
      {
        if (branch == null || branch.StartsWith("refs/", StringComparison.Ordinal))
          return branch;
        return BuildSourceProviders.GitProperties.BranchPrefix + branch.TrimStart(BuildSourceProviders.GitProperties.PathSeparator);
      }

      public static string RefToBranchName(string branch) => branch.StartsWith(BuildSourceProviders.GitProperties.BranchPrefix, StringComparison.Ordinal) ? branch.Substring(BuildSourceProviders.GitProperties.BranchPrefix.Length) : branch;

      public static bool TryResolvePath(string defaultRoot, string path, out string result)
      {
        path = path ?? string.Empty;
        path = path.Replace('\\', '/');
        if (!path.StartsWith("/"))
          path = (defaultRoot ?? string.Empty) + "/" + path;
        string[] strArray = path.Split(new char[1]{ '/' }, StringSplitOptions.RemoveEmptyEntries);
        Stack<string> values = new Stack<string>(strArray.Length);
        int num = 0;
        for (int index = strArray.Length - 1; index >= 0; --index)
        {
          string a = strArray[index];
          if (string.Equals(a, "..", StringComparison.Ordinal))
            ++num;
          else if (num > 0)
            --num;
          else if (!string.Equals(a, ".", StringComparison.Ordinal))
            values.Push(a);
        }
        if (num > 0)
        {
          result = (string) null;
          return false;
        }
        result = "/" + string.Join("/", (IEnumerable<string>) values);
        return true;
      }
    }
  }
}
