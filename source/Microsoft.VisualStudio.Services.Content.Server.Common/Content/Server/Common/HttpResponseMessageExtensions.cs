// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.HttpResponseMessageExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public static class HttpResponseMessageExtensions
  {
    public static void AddAttachment(
      this HttpResponseMessage response,
      byte[] attachement,
      string attachementName,
      string mediaType)
    {
      response.Content = (HttpContent) new ByteArrayContent(attachement);
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = attachementName
      };
      response.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
    }
  }
}
