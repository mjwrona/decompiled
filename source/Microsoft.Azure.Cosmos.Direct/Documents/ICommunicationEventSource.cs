// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ICommunicationEventSource
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

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
