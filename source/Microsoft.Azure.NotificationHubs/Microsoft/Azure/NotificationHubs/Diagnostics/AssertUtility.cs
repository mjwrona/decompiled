// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.AssertUtility
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal static class AssertUtility
  {
    [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.DebugAssert instead")]
    internal static void DebugAssertCore(string message)
    {
    }

    [Conditional("DEBUG")]
    [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.DebugAssert instead")]
    internal static void DebugAssert(bool condition, string message)
    {
      int num = condition ? 1 : 0;
    }

    [Conditional("DEBUG")]
    [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.DebugAssert instead")]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void DebugAssert(string message) => AssertUtility.DebugAssertCore(message);
  }
}
