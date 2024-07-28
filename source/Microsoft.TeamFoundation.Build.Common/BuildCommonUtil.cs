// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildCommonUtil
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Build.Common
{
  public static class BuildCommonUtil
  {
    internal static readonly DateTime DefaultDateTime = DateTime.MinValue;
    private static readonly string s_defaultWorkingDir = "$(SystemDrive)\\Builds\\$(BuildAgentId)\\$(BuildDefinitionPath)";
    private static readonly string s_defaultHostedWorkingDir = "$(SystemDrive)\\a";
    private static readonly string s_buildAgentId = "$(BuildAgentId)";
    private static readonly string s_buildAgentName = "$(BuildAgentName)";
    private static readonly string s_buildDefinitionId = "$(BuildDefinitionId)";
    private static readonly string s_buildDefinitionPath = "$(BuildDefinitionPath)";
    private static readonly string s_buildDirEnv = "$(BuildDir)";
    private static readonly string s_sourceDir = "$(SourceDir)";
    private static Regex s_envRegex;
    private static readonly int s_defaultAgentPort = 9191;

    public static string BuildRegistryKeyPath => "Software\\Microsoft\\VisualStudio\\17.0\\TeamFoundation\\Build";

    public static string BuildAgentIdVariable => BuildCommonUtil.s_buildAgentId;

    public static string BuildAgentNameVariable => BuildCommonUtil.s_buildAgentName;

    public static string BuildDefinitionIdVariable => BuildCommonUtil.s_buildDefinitionId;

    public static string BuildDefinitionPathVariable => BuildCommonUtil.s_buildDefinitionPath;

    public static string BuildDirEnvironmentVariable => BuildCommonUtil.s_buildDirEnv;

    public static string NoCICheckInComment => "***NO_CI***";

    public static string NoTriggerCheckInComment => "//***NO_CI***//";

    public static string DoNotCopyLogsToFileContainer => nameof (DoNotCopyLogsToFileContainer);

    public static string SourceDirEnvironmentVariable => BuildCommonUtil.s_sourceDir;

    public static string DefaultWorkingDirectory => BuildCommonUtil.s_defaultWorkingDir;

    public static string DefaultHostedWorkingDirectory => BuildCommonUtil.s_defaultHostedWorkingDir;

    public static int DefaultAgentPort => BuildCommonUtil.s_defaultAgentPort;

    public static Regex MacroPattern
    {
      get
      {
        if (BuildCommonUtil.s_envRegex == null)
          BuildCommonUtil.s_envRegex = new Regex("\\$\\(([^)]+)\\)", RegexOptions.Singleline);
        return BuildCommonUtil.s_envRegex;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsStar(string value) => string.Equals(value, BuildConstants.Star, StringComparison.Ordinal);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsDefaultDateTime(DateTime value) => value.Year == 1;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string DateTimeToString(DateTime value) => BuildCommonUtil.IsDefaultDateTime(value) ? string.Empty : value.ToString();

    public static string GetCommonLocalPath(IEnumerable<string> localItems)
    {
      if (localItems == null)
        return string.Empty;
      using (IEnumerator<string> enumerator = localItems.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return string.Empty;
        string path1 = enumerator.Current;
        while (enumerator.MoveNext())
        {
          path1 = BuildCommonUtil.GetCommonLocalPath(path1, enumerator.Current);
          if (string.IsNullOrEmpty(path1))
            break;
        }
        return path1;
      }
    }

    public static string GetCommonServerPath(IEnumerable<string> serverItems)
    {
      if (serverItems == null)
        return string.Empty;
      using (IEnumerator<string> enumerator = serverItems.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return string.Empty;
        string path1 = enumerator.Current;
        while (enumerator.MoveNext())
        {
          path1 = BuildCommonUtil.GetCommonServerPath(path1, enumerator.Current);
          if (string.IsNullOrEmpty(path1))
            break;
        }
        return path1;
      }
    }

    private static string GetCommonLocalPath(string path1, string path2)
    {
      string commonLocalPath;
      string str;
      if (FileSpec.GetFolderDepth(path1) >= FileSpec.GetFolderDepth(path2))
      {
        commonLocalPath = path2;
        str = path1;
      }
      else
      {
        commonLocalPath = path1;
        str = path2;
      }
      string directoryName;
      for (; !FileSpec.IsSubItem(str, commonLocalPath); commonLocalPath = directoryName)
      {
        directoryName = FileSpec.GetDirectoryName(commonLocalPath);
        if (FileSpec.Equals(directoryName, commonLocalPath))
          break;
      }
      return commonLocalPath;
    }

    private static string GetCommonServerPath(string path1, string path2)
    {
      if (!VersionControlPath.IsServerItem(path1) || !VersionControlPath.IsServerItem(path2))
        return "$/";
      string parent;
      string str;
      if (VersionControlPath.GetFolderDepth(path1) >= VersionControlPath.GetFolderDepth(path2))
      {
        parent = path2;
        str = path1;
      }
      else
      {
        parent = path1;
        str = path2;
      }
      while (!VersionControlPath.IsSubItem(str, parent))
        parent = VersionControlPath.GetFolderName(parent);
      return parent;
    }

    public static string Replace(
      string original,
      string pattern,
      string replacement,
      bool ignoreCase)
    {
      StringBuilder stringBuilder = new StringBuilder();
      StringComparison comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
      int indexA = 0;
      while (indexA < original.Length)
      {
        if (string.Compare(original, indexA, pattern, 0, pattern.Length, comparisonType) != 0)
        {
          stringBuilder.Append(original[indexA++]);
        }
        else
        {
          stringBuilder.Append(replacement);
          indexA += pattern.Length;
        }
      }
      return stringBuilder.ToString();
    }

    public static string ExpandEnvironmentVariables(string inputValue) => BuildCommonUtil.ExpandEnvironmentVariables(inputValue, (IDictionary<string, string>) null);

    public static string ExpandEnvironmentVariables(
      string inputValue,
      IDictionary<string, string> additionalVariableReplacements)
    {
      Dictionary<string, string> variables = additionalVariableReplacements == null || additionalVariableReplacements.Count == 0 ? new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new Dictionary<string, string>(additionalVariableReplacements, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
        variables[(string) environmentVariable.Key] = (string) environmentVariable.Value;
      return BuildCommonUtil.ExpandEnvironmentVariables(inputValue, (IDictionary<string, string>) variables, (Func<string, string, string>) null);
    }

    public static string ExpandEnvironmentVariables(
      string inputValue,
      IDictionary<string, string> variables,
      Func<string, string, string> expandVariable)
    {
      if (string.IsNullOrEmpty(inputValue))
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder(inputValue);
      IList<VariableMatch> environmentVariableMatches = BuildCommonUtil.GetEnvironmentVariableMatches(inputValue);
      for (int index = environmentVariableMatches.Count - 1; index >= 0; --index)
      {
        string environmentVariableName = BuildCommonUtil.GetEnvironmentVariableName(environmentVariableMatches[index].Name);
        string newValue = (string) null;
        if (variables != null && !variables.TryGetValue(environmentVariableName, out newValue))
          variables.TryGetValue(environmentVariableMatches[index].Name, out newValue);
        if (expandVariable != null)
          newValue = expandVariable(environmentVariableMatches[index].Name, newValue);
        if (newValue != null)
          stringBuilder.Replace(environmentVariableMatches[index].Name, newValue);
      }
      return stringBuilder.ToString();
    }

    private static string GetEnvironmentVariableName(string matchValue) => matchValue.Substring(2, matchValue.Length - 3);

    internal static IList<VariableMatch> GetEnvironmentVariableMatches(string inputValue)
    {
      bool flag = false;
      int num = -1;
      int startIndex = 0;
      StringBuilder stringBuilder = new StringBuilder();
      List<VariableMatch> environmentVariableMatches = new List<VariableMatch>();
      for (int index = 0; index < inputValue.Length; ++index)
      {
        if (!flag && inputValue[index] == '$' && index + 1 < inputValue.Length && inputValue[index + 1] == '(')
        {
          startIndex = index;
          flag = true;
        }
        if (flag)
          stringBuilder.Append(inputValue[index]);
        if (flag && inputValue[index] == '(')
          ++num;
        if (flag && inputValue[index] == ')')
        {
          if (num == 0)
          {
            environmentVariableMatches.Add(new VariableMatch(stringBuilder.ToString(), startIndex, index));
            stringBuilder.Clear();
            flag = false;
            num = -1;
          }
          else
            --num;
        }
      }
      if (!flag)
        ;
      return (IList<VariableMatch>) environmentVariableMatches;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsEnvironmentVariable(string value) => BuildCommonUtil.MacroPattern.IsMatch(value, 0);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T GetRegistryValue<T>(string keyPath, string name, T defaultValue)
    {
      T registryValue;
      if (!BuildCommonUtil.GetValueFromKey<T>(Registry.CurrentUser.OpenSubKey(keyPath), name, defaultValue, out registryValue) && !BuildCommonUtil.GetValueFromKey<T>(Registry.LocalMachine.OpenSubKey(keyPath), name, defaultValue, out registryValue))
        registryValue = defaultValue;
      return registryValue;
    }

    public static bool VariableInUse(string value) => !BuildCommonUtil.ExpandEnvironmentVariables(value, (IDictionary<string, string>) null, (Func<string, string, string>) ((variableName, replacement) => "abc")).Equals(value);

    private static bool GetValueFromKey<T>(
      RegistryKey registryKey,
      string name,
      T defaultValue,
      out T value)
    {
      value = defaultValue;
      if (registryKey == null)
        return false;
      using (registryKey)
      {
        object obj = registryKey.GetValue(name);
        if (obj == null)
          return false;
        try
        {
          if (typeof (T) == typeof (bool))
            obj = (object) ((int) obj == 1);
          value = (T) obj;
          return true;
        }
        catch (InvalidCastException ex)
        {
          return false;
        }
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetRegistryValue<T>(string keyPath, string name, T value)
    {
      RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(keyPath, true) ?? Registry.CurrentUser.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
      if (registryKey == null)
        return;
      using (registryKey)
      {
        if ((object) value is string)
          registryKey.SetValue(name, (object) value, RegistryValueKind.String);
        else
          registryKey.SetValue(name, (object) value, RegistryValueKind.DWord);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetBuildReportUrl(TfsTeamProjectCollection collection, Uri buildUri)
    {
      string buildReportUrl = (string) null;
      TswaClientHyperlinkService service1 = collection.GetService<TswaClientHyperlinkService>();
      if (service1 != null)
      {
        try
        {
          Uri viewBuildDetailsUrl = service1.GetViewBuildDetailsUrl(buildUri);
          if (viewBuildDetailsUrl != (Uri) null)
            buildReportUrl = viewBuildDetailsUrl.AbsoluteUri;
        }
        catch (NotSupportedException ex)
        {
          ILinking service2 = collection.GetService<ILinking>();
          if (service2 != null)
            buildReportUrl = service2.GetArtifactUrl(buildUri.AbsoluteUri);
        }
      }
      return buildReportUrl;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void OpenDropFolder(string folderPath)
    {
      if (string.IsNullOrEmpty(folderPath))
        throw new Exception(BuildTypeResource.FolderOpenErrorMissingPath());
      try
      {
        Directory.GetDirectories(folderPath);
        TFUtil.BeginRunApp("\"" + folderPath + "\"", (string) null);
      }
      catch (Exception ex)
      {
        throw new Exception(BuildTypeResource.FolderOpenError((object) folderPath, (object) ex.Message), ex);
      }
    }

    internal static string GetServerPathForUrl(string rootPath, string url)
    {
      Uri result;
      if (Uri.TryCreate(url, UriKind.Absolute, out result))
      {
        string fragment = result.Fragment;
        if (string.IsNullOrEmpty(rootPath))
          rootPath = "$/";
        if (!string.IsNullOrEmpty(fragment))
        {
          int startIndex1 = fragment.IndexOf(rootPath, StringComparison.OrdinalIgnoreCase);
          if (startIndex1 != -1)
            return fragment.Substring(startIndex1);
          string str = Uri.EscapeDataString(rootPath);
          int startIndex2 = fragment.IndexOf(str, StringComparison.OrdinalIgnoreCase);
          return Uri.UnescapeDataString(fragment.Substring(startIndex2));
        }
      }
      throw new InvalidPathException(string.Empty);
    }
  }
}
