// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssServerPushStreamContent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssServerPushStreamContent : PushStreamContent, IVssServerHttpContent
  {
    public VssServerPushStreamContent(
      Action<Stream, HttpContent, TransportContext> onStreamAvailable,
      object securedObject)
      : base(onStreamAvailable)
    {
      this.Validate(securedObject);
    }

    public VssServerPushStreamContent(
      Func<Stream, HttpContent, TransportContext, Task> onStreamAvailable,
      object securedObject)
      : base(onStreamAvailable)
    {
      this.Validate(securedObject);
    }

    public VssServerPushStreamContent(
      Action<Stream, HttpContent, TransportContext> onStreamAvailable,
      string mediaType,
      object securedObject)
      : base(onStreamAvailable, mediaType)
    {
      this.Validate(securedObject);
    }

    public VssServerPushStreamContent(
      Func<Stream, HttpContent, TransportContext, Task> onStreamAvailable,
      string mediaType,
      object securedObject)
      : base(onStreamAvailable, mediaType)
    {
      this.Validate(securedObject);
    }

    public VssServerPushStreamContent(
      Action<Stream, HttpContent, TransportContext> onStreamAvailable,
      MediaTypeHeaderValue mediaType,
      object securedObject)
      : base(onStreamAvailable, mediaType)
    {
      this.Validate(securedObject);
    }

    public VssServerPushStreamContent(
      Func<Stream, HttpContent, TransportContext, Task> onStreamAvailable,
      MediaTypeHeaderValue mediaType,
      object securedObject)
      : base(onStreamAvailable, mediaType)
    {
      this.Validate(securedObject);
    }
  }
}
