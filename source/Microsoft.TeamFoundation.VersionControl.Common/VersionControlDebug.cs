// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VersionControlDebug
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class VersionControlDebug
  {
    [Conditional("DEBUG")]
    public static void AssertIsServerItem(string path, string message)
    {
      try
      {
      }
      catch (Exception ex)
      {
      }
    }

    [Conditional("DEBUG")]
    public static void AssertIsNotServerItem(string path, string message)
    {
      try
      {
      }
      catch (Exception ex)
      {
      }
    }

    [Conditional("DEBUG")]
    public static void AssertIsTeamProject(string path, string message)
    {
      try
      {
      }
      catch (Exception ex)
      {
      }
    }

    [Conditional("DEBUG")]
    public static void AssertServerPathCanonicalized(
      bool preCondition,
      string path,
      PathLength maxServerPathLength,
      string message)
    {
      int num = preCondition ? 1 : 0;
    }

    [Conditional("DEBUG")]
    public static void AssertServerPathCanonicalized(
      string path,
      PathLength maxServerPathLength,
      string message)
    {
      try
      {
      }
      catch (Exception ex)
      {
      }
    }

    [Conditional("DEBUG")]
    public static void AssertLocalPathCanonicalized(string path, string message)
    {
      try
      {
      }
      catch (Exception ex)
      {
      }
    }

    [Conditional("DEBUG")]
    public static void AssertLocalFileExists(string path, string message)
    {
      try
      {
      }
      catch (Exception ex)
      {
      }
    }

    [Conditional("DEBUG")]
    public static void AssertLocalFileDoesNotExist(string path, string message)
    {
      try
      {
      }
      catch (Exception ex)
      {
      }
    }

    [Conditional("DEBUG")]
    public static void AssertLocalDirectoryExists(string path, string message)
    {
      try
      {
      }
      catch (Exception ex)
      {
      }
    }

    [Conditional("DEBUG")]
    public static void AssertLocalDirectoryDoesNotExist(string path, string message)
    {
      try
      {
      }
      catch (Exception ex)
      {
      }
    }
  }
}
