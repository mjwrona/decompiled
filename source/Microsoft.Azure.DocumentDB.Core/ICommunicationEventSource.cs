// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ICommunicationEventSource
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Net.Http.Headers;

namespace Microsoft.Azure.Documents
{
  internal interface ICommunicationEventSource
  {
    void Request(
      Guid activityId,
      Guid localId,
      string uri,
      string resourceType,
      HttpRequestHeaders requestHeaders);

    void Response(
      Guid activityId,
      Guid localId,
      short statusCode,
      double milliseconds,
      HttpResponseHeaders responseHeaders);
  }
}
