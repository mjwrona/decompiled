// Decompiled with JetBrains decompiler
// Type: System.Threading.Tasks.Sources.ManualResetValueTaskSourceCoreShared
// Assembly: Microsoft.Bcl.AsyncInterfaces, Version=7.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// MVID: 8B2E828D-BD93-4580-BC63-F76024589A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Bcl.AsyncInterfaces.dll

namespace System.Threading.Tasks.Sources
{
  internal static class ManualResetValueTaskSourceCoreShared
  {
    internal static readonly Action<object> s_sentinel = new Action<object>(ManualResetValueTaskSourceCoreShared.CompletionSentinel);

    private static void CompletionSentinel(object _) => throw new InvalidOperationException();
  }
}
