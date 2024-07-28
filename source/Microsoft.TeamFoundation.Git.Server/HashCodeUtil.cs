// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.HashCodeUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class HashCodeUtil
  {
    private const int c_init = 17;
    private const int c_factor = 486187739;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode<T1, T2>(T1 x1, T2 x2) => (17 * 486187739 + x1.GetHashCode()) * 486187739 + x2.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode<T1, T2, T3>(T1 x1, T2 x2, T3 x3) => ((17 * 486187739 + x1.GetHashCode()) * 486187739 + x2.GetHashCode()) * 486187739 + x3.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode<T1, T2, T3, T4>(T1 x1, T2 x2, T3 x3, T4 x4) => (((17 * 486187739 + x1.GetHashCode()) * 486187739 + x2.GetHashCode()) * 486187739 + x3.GetHashCode()) * 486187739 + x4.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode<T1, T2, T3, T4, T5>(T1 x1, T2 x2, T3 x3, T4 x4, T5 x5) => ((((17 * 486187739 + x1.GetHashCode()) * 486187739 + x2.GetHashCode()) * 486187739 + x3.GetHashCode()) * 486187739 + x4.GetHashCode()) * 486187739 + x5.GetHashCode();
  }
}
