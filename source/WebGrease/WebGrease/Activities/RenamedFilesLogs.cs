// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.RenamedFilesLogs
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WebGrease.Activities
{
  internal sealed class RenamedFilesLogs
  {
    private readonly Dictionary<string, string> dictionary = new Dictionary<string, string>();
    private readonly Dictionary<string, List<string>> m_reverseDictionary = new Dictionary<string, List<string>>();

    public RenamedFilesLogs(ICollection<string> logFiles)
    {
      if (logFiles == null || logFiles.Count == 0)
        return;
      foreach (string logFile in (IEnumerable<string>) logFiles)
      {
        RenamedFilesLog renamedFilesLog = new RenamedFilesLog(logFile);
        if (File.Exists(logFile))
        {
          renamedFilesLog.RenamedFiles.ForEach((Action<RenamedFile>) (renamedFile => renamedFile.InputNames.ForEach((Action<string>) (inputName => this.dictionary.Add(RenamedFilesLogs.NormalizeSlash(inputName).ToLowerInvariant(), renamedFile.OutputName)))));
          renamedFilesLog.RenamedFiles.ForEach((Action<RenamedFile>) (renamedFile => this.m_reverseDictionary.Add(renamedFile.OutputName, renamedFile.InputNames.Select<string, string>((Func<string, string>) (inputName => inputName.ToLowerInvariant())).ToList<string>())));
        }
      }
    }

    public static RenamedFilesLogs LoadHashedImagesLogs(string hashedImagesLogFile)
    {
      if (!string.IsNullOrWhiteSpace(hashedImagesLogFile))
      {
        if (File.Exists(hashedImagesLogFile))
        {
          try
          {
            return new RenamedFilesLogs((ICollection<string>) new string[1]
            {
              hashedImagesLogFile
            });
          }
          catch (Exception ex)
          {
            throw new BuildWorkflowException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Unable to parse the log with the hashed image replacement names from '{0}'", new object[1]
            {
              (object) hashedImagesLogFile
            }), ex);
          }
        }
      }
      return (RenamedFilesLogs) null;
    }

    public static string NormalizeSlash(string path) => path != null && path.StartsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase) ? path.Remove(0, 1) : path;

    public bool HasItems() => this.dictionary.Count != 0;

    public string FindHashPath(string inputName)
    {
      if (string.IsNullOrWhiteSpace(inputName))
        return (string) null;
      inputName = RenamedFilesLogs.NormalizeSlash(inputName).ToLowerInvariant();
      string str;
      return !this.dictionary.TryGetValue(inputName, out str) ? (string) null : str;
    }

    public bool AllInputFileNamesMatch(string hashedFileName, List<string> inputFileNames)
    {
      if (string.IsNullOrWhiteSpace(hashedFileName) || !this.m_reverseDictionary.ContainsKey(hashedFileName))
        return false;
      List<string> reverse = this.m_reverseDictionary[hashedFileName];
      if (reverse.Count != inputFileNames.Count)
        return false;
      foreach (string str in reverse)
      {
        if (!inputFileNames.Contains(str))
          return false;
        inputFileNames.Remove(str);
      }
      return true;
    }
  }
}
