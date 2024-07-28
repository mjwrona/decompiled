// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Owin.ServerResponse
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Owin
{
  public class ServerResponse : IResponse
  {
    private readonly CancellationToken _callCancelled;
    private readonly OwinResponse _response;
    private readonly Stream _responseBody;

    public ServerResponse(IDictionary<string, object> environment)
    {
      this._response = new OwinResponse(environment);
      this._callCancelled = this._response.Get<CancellationToken>("owin.CallCancelled");
      this._responseBody = this._response.Body;
    }

    public CancellationToken CancellationToken => this._callCancelled;

    public int StatusCode
    {
      get => this._response.StatusCode;
      set => this._response.StatusCode = value;
    }

    public string ContentType
    {
      get => this._response.ContentType;
      set => this._response.ContentType = value;
    }

    public void Write(ArraySegment<byte> data) => this._responseBody.Write(data.Array, data.Offset, data.Count);

    public Task Flush() => this._responseBody.FlushAsync();
  }
}
