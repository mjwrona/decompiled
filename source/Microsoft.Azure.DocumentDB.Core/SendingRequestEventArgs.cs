// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SendingRequestEventArgs
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Net.Http;

namespace Microsoft.Azure.Documents
{
  internal sealed class SendingRequestEventArgs : EventArgs
  {
    public SendingRequestEventArgs(DocumentServiceRequest request) => this.DocumentServiceRequest = request;

    public SendingRequestEventArgs(HttpRequestMessage request) => this.HttpRequest = request;

    public HttpRequestMessage HttpRequest { get; }

    public DocumentServiceRequest DocumentServiceRequest { get; }

    public bool IsHttpRequest() => this.HttpRequest != null;
  }
}
