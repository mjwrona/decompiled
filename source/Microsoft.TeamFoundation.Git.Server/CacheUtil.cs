// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CacheUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class CacheUtil
  {
    public static readonly int ObjectOverhead = 3 * IntPtr.Size;

    public static long GetMemoryFractionFromRegistry(
      RegistryEntryCollection ce,
      string registryPath,
      double defaultFraction,
      long? defaultMaxTotal = null)
    {
      return CacheUtil.GetMemoryFraction(ce.GetValueFromPath<double>(registryPath + "/MaxSizeAutoscale/Fraction", defaultFraction), ce.GetValueFromPath<long?>(registryPath + "/MaxSizeAutoscale/MaxTotal", defaultMaxTotal));
    }

    public static long GetMemoryFraction(double fraction, long? maxTotal = null)
    {
      ArgumentUtility.CheckForOutOfRange<double>(fraction, nameof (fraction), 0.0, 1.0);
      ulong val2 = !maxTotal.HasValue ? 137438953472UL : checked ((ulong) maxTotal.Value);
      ulong num = Math.Min(CacheUtil.GetTotalPhysicalMemory(), val2);
      return checked ((long) unchecked (fraction * (double) num));
    }

    public static ulong GetTotalPhysicalMemory()
    {
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.MEMORYSTATUSEX lpBuffer = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.MEMORYSTATUSEX();
      return Microsoft.TeamFoundation.Common.Internal.NativeMethods.GlobalMemoryStatusEx(lpBuffer) ? lpBuffer.ullTotalPhys : throw new Win32Exception();
    }
  }
}
