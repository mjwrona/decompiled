// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Compression.ZLibException
// Assembly: Microsoft.TeamFoundation.Server.Compression, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E666AAE4-36CD-4581-80AF-1B631308AB46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.Compression.dll

using System;
using System.IO;
using System.Security;

namespace Microsoft.TeamFoundation.Server.Compression
{
  internal class ZLibException : IOException
  {
    private string _zlibErrorContext;
    private string _zlibErrorMessage;
    private ZLibNative.ErrorCode _zlibErrorCode;

    public ZLibException(
      string message,
      string zlibErrorContext,
      int zlibErrorCode,
      string zlibErrorMessage)
      : base(message)
    {
      this.Init(zlibErrorContext, (ZLibNative.ErrorCode) zlibErrorCode, zlibErrorMessage);
    }

    public ZLibException() => this.Init();

    public ZLibException(string message)
      : base(message)
    {
      this.Init();
    }

    public ZLibException(string message, Exception inner)
      : base(message, inner)
    {
      this.Init();
    }

    private void Init() => this.Init("", ZLibNative.ErrorCode.Ok, "");

    private void Init(
      string zlibErrorContext,
      ZLibNative.ErrorCode zlibErrorCode,
      string zlibErrorMessage)
    {
      this._zlibErrorContext = zlibErrorContext;
      this._zlibErrorCode = zlibErrorCode;
      this._zlibErrorMessage = zlibErrorMessage;
    }

    public string ZLibContext
    {
      [SecurityCritical] get => this._zlibErrorContext;
    }

    public int ZLibErrorCode
    {
      [SecurityCritical] get => (int) this._zlibErrorCode;
    }

    public string ZLibErrorMessage
    {
      [SecurityCritical] get => this._zlibErrorMessage;
    }
  }
}
