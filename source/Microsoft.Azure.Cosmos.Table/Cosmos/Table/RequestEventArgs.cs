// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RequestEventArgs
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Net.Http;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class RequestEventArgs : EventArgs
  {
    public RequestEventArgs(RequestResult res) => this.RequestInformation = res;

    public RequestResult RequestInformation { get; internal set; }

    public HttpRequestMessage Request { get; internal set; }

    public HttpResponseMessage Response { get; internal set; }
  }
}
