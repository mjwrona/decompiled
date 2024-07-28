// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.Etw.TraceEvent
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;

namespace Microsoft.Cloud.Metrics.Client.Metrics.Etw
{
  internal struct TraceEvent
  {
    private readonly unsafe NativeMethods.EventRecord* eventRecord;
    private readonly TraceEventHeader eventHeader;
    private readonly TraceBufferContext bufferContext;

    public unsafe TraceEvent(NativeMethods.EventRecord* eventRecord)
      : this()
    {
      this.eventRecord = (IntPtr) eventRecord != IntPtr.Zero ? eventRecord : throw new ArgumentNullException(nameof (eventRecord));
      this.eventHeader = new TraceEventHeader(&eventRecord->EventHeader);
      this.bufferContext = new TraceBufferContext(&eventRecord->BufferContext);
    }

    public TraceEventHeader Header => this.eventHeader;

    public TraceBufferContext BufferContext => this.bufferContext;

    public unsafe ushort ExtendedDataCount => this.eventRecord->ExtendedDataCount;

    public unsafe ushort UserDataLength => this.eventRecord->UserDataLength;

    public unsafe IntPtr ExtendedData => this.eventRecord->ExtendedData;

    public unsafe IntPtr UserData => this.eventRecord->UserData;

    public unsafe IntPtr UserContext => this.eventRecord->UserContext;
  }
}
