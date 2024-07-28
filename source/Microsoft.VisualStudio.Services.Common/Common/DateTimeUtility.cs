// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.DateTimeUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class DateTimeUtility
  {
    private static bool isEntryPointNotFound;

    [DllImport("kernel32.dll")]
    private static extern void GetSystemTimePreciseAsFileTime(out long filetime);

    public static DateTime GetHighResolutionUtcNow()
    {
      if (!DateTimeUtility.isEntryPointNotFound)
      {
        try
        {
          long filetime;
          DateTimeUtility.GetSystemTimePreciseAsFileTime(out filetime);
          return DateTime.FromFileTimeUtc(filetime);
        }
        catch (EntryPointNotFoundException ex)
        {
          DateTimeUtility.isEntryPointNotFound = true;
        }
      }
      return DateTime.UtcNow;
    }
  }
}
