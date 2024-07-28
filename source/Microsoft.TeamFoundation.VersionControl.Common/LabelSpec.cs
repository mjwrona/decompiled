// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.LabelSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public static class LabelSpec
  {
    private static readonly string m_LabelSeparator = "@";

    public static void Parse(
      string spec,
      string defaultScope,
      bool permitWildcardNames,
      out string labelName,
      out string labelScope)
    {
      if (string.IsNullOrEmpty(spec))
      {
        labelName = (string) null;
        labelScope = defaultScope;
      }
      else
      {
        int length = spec.IndexOf(LabelSpec.m_LabelSeparator, StringComparison.OrdinalIgnoreCase);
        if (length < 0)
        {
          labelName = spec;
          labelScope = defaultScope;
        }
        else
        {
          labelName = length != 0 ? spec.Substring(0, length) : (string) null;
          labelScope = length + 1 != spec.Length ? spec.Substring(length + 1) : defaultScope;
        }
        if (labelScope == null || LabelSpec.IsLegalScope(labelScope))
          return;
        if (VersionControlPath.IsWildcard(labelScope))
          throw new ArgumentException(Resources.Format("LabelScopeWildcard"));
        throw new ArgumentException(Resources.Format("LabelScopeIllegal", (object) labelScope));
      }
    }

    public static string Combine(string labelName, string labelScope) => labelScope == null ? labelName : labelName + LabelSpec.m_LabelSeparator + labelScope;

    public static bool IsLegalSpec(string labelSpec, bool permitWildcardNames)
    {
      string labelName;
      string labelScope;
      LabelSpec.Parse(labelSpec, (string) null, permitWildcardNames, out labelName, out labelScope);
      if (!LabelSpec.IsLegalName(labelName))
        return false;
      return labelScope == null || LabelSpec.IsLegalScope(labelScope);
    }

    public static bool IsLegalSpec(string labelSpec) => LabelSpec.IsLegalSpec(labelSpec, false);

    public static bool IsLegalName(string labelName, bool permitWildcards)
    {
      char[] anyOf = permitWildcards ? FileSpec.IllegalNtfsChars : FileSpec.IllegalNtfsCharsAndWildcards;
      return !string.IsNullOrEmpty(labelName) && labelName[labelName.Length - 1] != ' ' && labelName.Length <= 64 && labelName.IndexOfAny(anyOf) < 0 && labelName.IndexOf(LabelSpec.m_LabelSeparator, StringComparison.OrdinalIgnoreCase) < 0;
    }

    public static bool IsLegalName(string labelName) => LabelSpec.IsLegalName(labelName, false);

    public static bool IsLegalScope(string labelScope) => VersionControlPath.IsServerItem(labelScope) && !VersionControlPath.IsWildcard(labelScope);
  }
}
