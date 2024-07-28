// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ReceivedResponseEventArgs
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

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
