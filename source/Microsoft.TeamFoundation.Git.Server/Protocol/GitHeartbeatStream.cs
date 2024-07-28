// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.GitHeartbeatStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server.Protocol
{
  internal class GitHeartbeatStream : Stream
  {
    private readonly Stream m_stream;
    private readonly int m_heartbeatIntervalMillis;
    private readonly int m_minimumWriteThresholdMillis;
    private readonly Func<bool> m_shouldSendHeartbeats;
    private readonly Timer m_heartbeatTimer;
    private bool m_closed;
    private bool m_heartbeatTimerEnabled;
    private long m_lastWriteTicks;
    private static readonly byte[] s_keepalivePacket = new byte[5]
    {
      (byte) 48,
      (byte) 48,
      (byte) 48,
      (byte) 53,
      (byte) 1
    };
    private const string c_layer = "GitHeartbeatStream";

    public override bool CanRead => false;

    public override bool CanWrite => true;

    public override bool CanSeek => false;

    public GitHeartbeatStream(
      Stream stream,
      int heartbeatIntervalMillis = 5000,
      int minimumWriteThresholdMillis = 100,
      Func<bool> shouldSendHeartbeats = null)
    {
      this.m_stream = stream;
      this.m_heartbeatIntervalMillis = heartbeatIntervalMillis;
      this.m_minimumWriteThresholdMillis = minimumWriteThresholdMillis;
      this.m_shouldSendHeartbeats = shouldSendHeartbeats;
      this.m_heartbeatTimerEnabled = true;
      this.m_lastWriteTicks = Stopwatch.GetTimestamp();
      this.m_heartbeatTimer = new Timer(new TimerCallback(this.HeartbeatTimerCallback));
      this.m_heartbeatTimer.Change(heartbeatIntervalMillis, heartbeatIntervalMillis);
      TeamFoundationTracingService.TraceRaw(1013933, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitHeartbeatStream), "Heartbeat is started.");
    }

    ~GitHeartbeatStream() => this.StopHeartbeats();

    public override void Write(byte[] buffer, int offset, int count)
    {
      lock (this.m_stream)
      {
        this.m_stream.Write(buffer, offset, count);
        this.m_lastWriteTicks = Stopwatch.GetTimestamp();
        if (!this.m_heartbeatTimerEnabled)
          return;
        this.m_heartbeatTimer.Change(this.m_heartbeatIntervalMillis, this.m_heartbeatIntervalMillis);
        TeamFoundationTracingService.TraceRaw(1013934, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitHeartbeatStream), "Heartbeat is restarted.");
      }
    }

    public override void Flush()
    {
      lock (this.m_stream)
        this.m_stream.Flush();
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.StopHeartbeats();
        this.Flush();
        this.m_closed = true;
      }
      base.Close();
    }

    private void HeartbeatTimerCallback(object state)
    {
      try
      {
        if (!this.m_stream.CanWrite || !this.m_heartbeatTimerEnabled)
        {
          TeamFoundationTracingService.TraceRaw(1013935, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitHeartbeatStream), string.Format("Stream is not writable or timer is disabled. {0} = {1} {2} = {3}", (object) "CanWrite", (object) this.m_stream.CanWrite, (object) "m_heartbeatTimerEnabled", (object) this.m_heartbeatTimerEnabled));
        }
        else
        {
          lock (this.m_stream)
          {
            if (!this.m_stream.CanWrite || !this.m_heartbeatTimerEnabled)
            {
              TeamFoundationTracingService.TraceRaw(1013936, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitHeartbeatStream), string.Format("Stream is not writable or timer is disabled. {0} = {1} {2} = {3}", (object) "CanWrite", (object) this.m_stream.CanWrite, (object) "m_heartbeatTimerEnabled", (object) this.m_heartbeatTimerEnabled));
            }
            else
            {
              long num = (Stopwatch.GetTimestamp() - this.m_lastWriteTicks) / (Stopwatch.Frequency / 1000L);
              if (num < (long) this.m_minimumWriteThresholdMillis)
              {
                TeamFoundationTracingService.TraceRaw(1013937, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitHeartbeatStream), string.Format("Less than the threshold passed since latest write. {0} = {1} {2} = {3} {4} = {5}", (object) "m_heartbeatIntervalMillis", (object) this.m_heartbeatIntervalMillis, (object) "millisSinceLatestWrite", (object) num, (object) "m_minimumWriteThresholdMillis", (object) this.m_minimumWriteThresholdMillis));
              }
              else
              {
                Func<bool> shouldSendHeartbeats = this.m_shouldSendHeartbeats;
                if ((shouldSendHeartbeats != null ? (shouldSendHeartbeats() ? 1 : 0) : 1) != 0)
                {
                  TeamFoundationTracingService.TraceRaw(1013938, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitHeartbeatStream), string.Format("Sending a heartbeat (keep alive packet). {0} = {1} {2} = {3}", (object) "m_heartbeatIntervalMillis", (object) this.m_heartbeatIntervalMillis, (object) "millisSinceLatestWrite", (object) num));
                  this.m_stream.Write(GitHeartbeatStream.s_keepalivePacket, 0, GitHeartbeatStream.s_keepalivePacket.Length);
                  this.m_stream.Flush();
                  this.m_lastWriteTicks = Stopwatch.GetTimestamp();
                }
                else
                  TeamFoundationTracingService.TraceRaw(1013939, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitHeartbeatStream), string.Format("We would send a heartbeat, but {0} returned false. {1} = {2} {3} = {4}", (object) "m_shouldSendHeartbeats", (object) "m_heartbeatIntervalMillis", (object) this.m_heartbeatIntervalMillis, (object) "millisSinceLatestWrite", (object) num));
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1013940, GitServerUtils.TraceArea, nameof (GitHeartbeatStream), ex);
        this.StopHeartbeats();
      }
    }

    private void StopHeartbeats()
    {
      try
      {
        if (!this.m_heartbeatTimerEnabled)
        {
          TeamFoundationTracingService.TraceRaw(1013941, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitHeartbeatStream), "Heartbeat is already stopped.");
        }
        else
        {
          lock (this.m_stream)
          {
            if (!this.m_heartbeatTimerEnabled)
            {
              TeamFoundationTracingService.TraceRaw(1013942, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitHeartbeatStream), "Heartbeat is already stopped.");
            }
            else
            {
              this.m_heartbeatTimer.Dispose();
              this.m_heartbeatTimerEnabled = false;
              TeamFoundationTracingService.TraceRaw(1013943, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitHeartbeatStream), "Heartbeat is stopped.");
              GC.SuppressFinalize((object) this);
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1013944, GitServerUtils.TraceArea, nameof (GitHeartbeatStream), ex);
      }
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    public override long Position
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override long Length => throw new NotImplementedException();
  }
}
