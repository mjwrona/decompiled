// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ArgumentValidation
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class ArgumentValidation
  {
    private static readonly char[] s_invalidCharacters = ((IEnumerable<char>) FileSpec.IllegalNtfsCharsAndWildcards).Union<char>((IEnumerable<char>) new char[1]
    {
      '@'
    }).ToArray<char>();

    public static void CheckBuildArtifact(BuildArtifact artifact)
    {
      ArgumentUtility.CheckForNull<BuildArtifact>(artifact, nameof (artifact));
      ArgumentUtility.CheckForNull<string>(artifact.Name, "artifact.Name");
      ArgumentUtility.CheckForNull<BuildArtifactResource>(artifact.Resource, "artifact.Resource");
      ArgumentUtility.CheckForNull<string>(artifact.Resource.Data, "artifact.Resource.Data");
    }

    public static void CheckBuild(BuildData build, bool requireDefinition = true)
    {
      ArgumentUtility.CheckForNull<BuildData>(build, nameof (build));
      ArgumentUtility.CheckForEmptyGuid(build.ProjectId, "build.Project.Id");
      if (!requireDefinition)
        return;
      ArgumentUtility.CheckForNull<MinimalBuildDefinition>(build.Definition, "build.Definition");
    }

    public static bool IsValidBuildNumber(string buildNumber) => !string.IsNullOrEmpty(buildNumber) && !buildNumber.EndsWith(".") && buildNumber.Length <= (int) byte.MaxValue && buildNumber.IndexOfAny(ArgumentValidation.s_invalidCharacters) < 0;

    public static bool IsValidBuildNumberFormat(string buildNumberFormat)
    {
      bool flag = true;
      if (!string.IsNullOrEmpty(buildNumberFormat))
      {
        flag = buildNumberFormat.Length <= (int) byte.MaxValue;
        if (flag)
        {
          IList<VariableMatch> environmentVariableMatches = BuildCommonUtil.GetEnvironmentVariableMatches(buildNumberFormat);
          if (environmentVariableMatches.Count > 0)
          {
            int startIndex = 0;
            foreach (VariableMatch variableMatch in (IEnumerable<VariableMatch>) environmentVariableMatches)
            {
              if (startIndex < variableMatch.StartIndex)
                flag = buildNumberFormat.IndexOfAny(ArgumentValidation.s_invalidCharacters, startIndex, variableMatch.StartIndex - startIndex) < 0;
              if (flag)
                startIndex = variableMatch.EndIndex + 1;
              else
                break;
            }
            if (flag && startIndex < buildNumberFormat.Length - 1)
              flag = buildNumberFormat.IndexOfAny(ArgumentValidation.s_invalidCharacters, startIndex, buildNumberFormat.Length - startIndex) < 0;
          }
          else
            flag = buildNumberFormat.IndexOfAny(ArgumentValidation.s_invalidCharacters) < 0;
        }
      }
      return flag;
    }
  }
}
