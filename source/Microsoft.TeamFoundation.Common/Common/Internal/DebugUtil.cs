// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.DebugUtil
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Security;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class DebugUtil
  {
    private static string[] s_breakCategories;
    private static bool s_breakAlways;
    private static bool s_break = true;

    [Conditional("DEBUG")]
    [DebuggerHidden]
    public static void Break()
    {
      if (!DebugUtil.s_break)
        return;
      try
      {
        if (Debugger.IsAttached)
          Debugger.Break();
        else
          Debugger.Launch();
      }
      catch (SecurityException ex)
      {
      }
    }

    [Conditional("DEBUG")]
    [DebuggerHidden]
    public static void BreakIf(bool condition)
    {
      int num = condition ? 1 : 0;
    }

    [Conditional("DEBUG")]
    [DebuggerHidden]
    public static void Break(string category)
    {
      if (string.IsNullOrEmpty(category))
        return;
      DebugUtil.EnsurebreakCategoriesLoaded();
      if (DebugUtil.s_breakAlways)
        return;
      Array.BinarySearch<string>(DebugUtil.s_breakCategories, category, (IComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [Conditional("DEBUG")]
    [DebuggerHidden]
    public static void BreakIf(string category, bool condition)
    {
      if (!condition || string.IsNullOrEmpty(category))
        return;
      DebugUtil.EnsurebreakCategoriesLoaded();
      if (DebugUtil.s_breakAlways)
        return;
      Array.BinarySearch<string>(DebugUtil.s_breakCategories, category, (IComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private static void EnsurebreakCategoriesLoaded()
    {
      if (DebugUtil.s_breakCategories != null)
        return;
      string str1 = (string) null;
      string str2 = (string) null;
      try
      {
        str1 = Environment.GetEnvironmentVariable("ATTACH_DEBUGGER");
        str2 = Environment.GetEnvironmentVariable("ATTACH_DEBUGGER_" + Process.GetCurrentProcess().ProcessName);
      }
      catch (SecurityException ex)
      {
      }
      string[] array = (str1 + "," + str2).Split(new char[3]
      {
        ',',
        ';',
        ' '
      }, StringSplitOptions.RemoveEmptyEntries);
      Array.Sort<string>(array, (IComparer<string>) StringComparer.OrdinalIgnoreCase);
      DebugUtil.s_breakAlways = Array.BinarySearch<string>(array, "always", (IComparer<string>) StringComparer.OrdinalIgnoreCase) >= 0 || Array.BinarySearch<string>(array, "yes", (IComparer<string>) StringComparer.OrdinalIgnoreCase) >= 0 || Array.BinarySearch<string>(array, "y", (IComparer<string>) StringComparer.OrdinalIgnoreCase) >= 0;
      if (array.Length == 0)
        array = new string[1];
      DebugUtil.s_breakCategories = array;
    }
  }
}
