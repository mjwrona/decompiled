// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.Etw.TraceEventHeader
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;

namespace Microsoft.Cloud.Metrics.Client.Metrics.Etw
{
  internal struct TraceEventHeader
  {
    private readonly unsafe NativeMethods.EventHeader* eventHeader;

    public unsafe TraceEventHeader(NativeMethods.EventHeader* eventHeader) => this.eventHeader = (IntPtr) eventHeader != IntPtr.Zero ? eventHeader : throw new ArgumentNullException(nameof (eventHeader));

    public unsafe ushort Size => this.eventHeader->Size;

    public unsafe ushort HeaderType => this.eventHeader->HeaderType;

    public unsafe ushort Flags => this.eventHeader->Flags;

    public unsafe ushort EventProperty => this.eventHeader->EventProperty;

    public unsafe int ThreadId => this.eventHeader->ThreadId;

    public unsafe int ProcessId => this.eventHeader->ProcessId;

    public unsafe long Timestamp => this.eventHeader->TimeStamp;

    public unsafe Guid ProviderId => this.eventHeader->ProviderId;

    public unsafe ushort Id => this.eventHeader->Id;

    public unsafe byte Version => this.eventHeader->Version;

    public unsafe byte Channel => this.eventHeader->Channel;

    public unsafe byte Level => this.eventHeader->Level;

    public unsafe byte Opcode => this.eventHeader->Opcode;

    public unsafe ushort Task => this.eventHeader->Task;

    public unsafe ulong Keyword => this.eventHeader->Keyword;

    public unsafe int KernelTime => this.eventHeader->KernelTime;

    public unsafe int UserTime => this.eventHeader->UserTime;

    public unsafe Guid ActivityId => this.eventHeader->ActivityId;
  }
}
