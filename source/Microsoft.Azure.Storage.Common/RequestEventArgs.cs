// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.RequestEventArgs
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Net.Http;

namespace Microsoft.Azure.Storage
{
  public sealed class RequestEventArgs : EventArgs
  {
    public RequestEventArgs(RequestResult res) => this.RequestInformation = res;

    public RequestResult RequestInformation { get; internal set; }

    public HttpRequestMessage Request { get; internal set; }

    public HttpResponseMessage Response { get; internal set; }
  }
}
