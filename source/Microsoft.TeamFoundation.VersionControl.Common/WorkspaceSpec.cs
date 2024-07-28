// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.WorkspaceSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Text;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public static class WorkspaceSpec
  {
    private static readonly string m_workspaceSeparator = WorkspaceSpec.Separator.ToString();

    public static void Parse(
      string spec,
      string defaultUser,
      out string workspaceName,
      out string workspaceOwner)
    {
      WorkspaceSpec.Parse(spec, defaultUser, defaultUser, out workspaceName, out workspaceOwner, out string _);
    }

    public static void Parse(
      string spec,
      string defaultUser,
      string defaultUserDisplay,
      out string workspaceName,
      out string workspaceOwner,
      out string workspaceOwnerDisplay)
    {
      if (string.IsNullOrEmpty(spec))
      {
        workspaceName = (string) null;
        workspaceOwner = defaultUser;
        workspaceOwnerDisplay = defaultUserDisplay;
      }
      else
      {
        int length1 = spec.IndexOf(WorkspaceSpec.m_workspaceSeparator, StringComparison.OrdinalIgnoreCase);
        if (length1 < 0)
        {
          workspaceName = spec;
          workspaceOwner = defaultUser;
          workspaceOwnerDisplay = defaultUserDisplay;
        }
        else
        {
          workspaceName = length1 != 0 ? spec.Substring(0, length1) : (string) null;
          if (length1 + 1 == spec.Length)
          {
            workspaceOwner = defaultUser;
            workspaceOwnerDisplay = defaultUserDisplay;
          }
          else
          {
            workspaceOwnerDisplay = spec.Substring(length1 + 1).Trim();
            int length2 = workspaceOwnerDisplay.IndexOf(WorkspaceSpec.OwnerDisambiguationSeparatorStart, StringComparison.OrdinalIgnoreCase);
            if (length2 != -1)
            {
              if (workspaceOwnerDisplay[workspaceOwnerDisplay.Length - 1].ToString() == WorkspaceSpec.OwnerDisambiguationSeparatorEnd)
              {
                workspaceOwner = workspaceOwnerDisplay.Substring(length2 + WorkspaceSpec.OwnerDisambiguationSeparatorStart.Length, workspaceOwnerDisplay.Length - WorkspaceSpec.OwnerDisambiguationSeparatorEnd.Length - length2 - WorkspaceSpec.OwnerDisambiguationSeparatorStart.Length);
              }
              else
              {
                workspaceOwnerDisplay = workspaceOwnerDisplay.Substring(0, length2);
                workspaceOwner = workspaceOwnerDisplay;
              }
            }
            else
              workspaceOwner = workspaceOwnerDisplay;
          }
        }
      }
    }

    public static string Combine(string workspace, string owner) => workspace + WorkspaceSpec.m_workspaceSeparator + owner;

    public static string Combine(string workspace, string ownerDisplay, string ownerUnique) => workspace + WorkspaceSpec.m_workspaceSeparator + ownerDisplay + WorkspaceSpec.OwnerDisambiguationSeparatorStart + ownerUnique + WorkspaceSpec.OwnerDisambiguationSeparatorEnd;

    public static bool IsLegalName(string workspaceName) => !string.IsNullOrEmpty(workspaceName) && workspaceName[workspaceName.Length - 1] != ' ' && workspaceName.Length <= 64 && workspaceName.IndexOfAny(FileSpec.IllegalNtfsCharsAndWildcards) < 0 && workspaceName.IndexOf(WorkspaceSpec.m_workspaceSeparator, StringComparison.OrdinalIgnoreCase) < 0;

    public static string SanitizeProposedName(string proposedName)
    {
      StringBuilder stringBuilder = new StringBuilder(64);
      foreach (char c in proposedName)
      {
        if (stringBuilder.Length < stringBuilder.Capacity)
        {
          if (FileSpec.IsValidNtfsChar(c) && '*' != c && '?' != c && (int) WorkspaceSpec.Separator != (int) c)
            stringBuilder.Append(c);
        }
        else
          break;
      }
      return stringBuilder.ToString();
    }

    public static char Separator => ';';

    public static string OwnerDisambiguationSeparatorStart => " <";

    public static string OwnerDisambiguationSeparatorEnd => ">";
  }
}
