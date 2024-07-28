// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.ChannelCommonArguments
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
