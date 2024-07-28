// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ReceivedResponseEventArgs
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Net.Http;

namespace Microsoft.Azure.Documents
{
  internal sealed class ReceivedResponseEventArgs : EventArgs
  {
    public ReceivedResponseEventArgs(
      DocumentServiceRequest request,
      DocumentServiceResponse response)
    {
      this.DocumentServiceResponse = response;
      this.DocumentServiceRequest = request;
    }

    public ReceivedResponseEventArgs(HttpRequestMessage request, HttpResponseMessage response)
    {
      this.HttpResponse = response;
      this.HttpRequest = request;
    }

    public DocumentServiceResponse DocumentServiceResponse { get; }

    public HttpResponseMessage HttpResponse { get; }

    public HttpRequestMessage HttpRequest { get; }

    public DocumentServiceRequest DocumentServiceRequest { get; }

    public bool IsHttpResponse() => this.HttpResponse != null;
  }
}
