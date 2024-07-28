// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.NativeMethods
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Runtime.InteropServices;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal static class NativeMethods
  {
    private const string ADVAPI32 = "advapi32.dll";

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool ReportEvent(
      SafeHandle hEventLog,
      ushort type,
      ushort category,
      uint eventID,
      byte[] userSID,
      ushort numStrings,
      uint dataLen,
      HandleRef strings,
      byte[] rawData);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern SafeEventLogWriteHandle RegisterEventSource(
      string uncServerName,
      string sourceName);
  }
}
