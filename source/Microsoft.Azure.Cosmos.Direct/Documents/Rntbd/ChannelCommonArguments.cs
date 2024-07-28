// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.ChannelCommonArguments
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class ChannelCommonArguments
  {
    private readonly object mutex = new object();
    private TransportErrorCode timeoutCode;
    private bool payloadSent;

    public ChannelCommonArguments(
      Guid activityId,
      TransportErrorCode initialTimeoutCode,
      bool userPayload)
    {
      this.ActivityId = activityId;
      this.UserPayload = userPayload;
      this.SetTimeoutCode(initialTimeoutCode);
    }

    public Guid ActivityId { get; set; }

    public bool UserPayload { get; private set; }

    public bool PayloadSent
    {
      get
      {
        lock (this.mutex)
          return this.payloadSent;
      }
    }

    public void SnapshotCallState(out TransportErrorCode timeoutCode, out bool payloadSent)
    {
      lock (this.mutex)
      {
        timeoutCode = this.timeoutCode;
        payloadSent = this.payloadSent;
      }
    }

    public void SetTimeoutCode(TransportErrorCode errorCode)
    {
      if (!TransportException.IsTimeout(errorCode))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} is not a timeout error code", (object) errorCode), nameof (errorCode));
      lock (this.mutex)
        this.timeoutCode = errorCode;
    }

    public void SetPayloadSent()
    {
      lock (this.mutex)
        this.payloadSent = !this.payloadSent ? true : throw new InvalidOperationException("TransportException.SetPayloadSent cannot be called more than once.");
    }
  }
}
