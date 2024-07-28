// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PythonRequirementsParser
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class PythonRequirementsParser
  {
    public IEnumerable<string> EnumeratePackages(string requirements)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(requirements, nameof (requirements));
      return this.EnumeratePackages((TextReader) new StringReader(requirements));
    }

    public IEnumerable<string> EnumeratePackages(TextReader requirements)
    {
      ArgumentUtility.CheckForNull<TextReader>(requirements, nameof (requirements));
      for (string line = requirements.ReadLine(); !string.IsNullOrEmpty(line); line = requirements.ReadLine())
      {
        line = line.Trim();
        string packageName;
        if (PythonRequirementsParser.TryGetPackageName(line, out packageName))
          yield return packageName;
        while (line.EndsWith("\\", StringComparison.Ordinal))
          line = requirements.ReadLine();
      }
    }

    private static bool TryGetPackageName(string line, out string packageName)
    {
      packageName = (string) null;
      if (line.StartsWith("#", StringComparison.Ordinal))
        return false;
      Match match = Regex.Match(line, "^([A-Z0-9][A-Z0-9._-]*[A-Z0-9]|[A-Z0-9])", RegexOptions.IgnoreCase);
      if (!match.Success)
        return false;
      packageName = match.Groups[1].Value;
      return true;
    }
  }
}
