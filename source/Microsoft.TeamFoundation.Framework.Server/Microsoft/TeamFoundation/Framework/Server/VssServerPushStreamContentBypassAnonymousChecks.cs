// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssServerPushStreamContentBypassAnonymousChecks
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
  public class VssServerPushStreamContentBypassAnonymousChecks : 
    PushStreamContent,
    IVssServerHttpContent
  {
    public VssServerPushStreamContentBypassAnonymousChecks(
      Action<Stream, HttpContent, TransportContext> onStreamAvailable)
      : base(onStreamAvailable)
    {
    }

    public VssServerPushStreamContentBypassAnonymousChecks(
      Func<Stream, HttpContent, TransportContext, Task> onStreamAvailable)
      : base(onStreamAvailable)
    {
    }

    public VssServerPushStreamContentBypassAnonymousChecks(
      Action<Stream, HttpContent, TransportContext> onStreamAvailable,
      string mediaType)
      : base(onStreamAvailable, mediaType)
    {
    }

    public VssServerPushStreamContentBypassAnonymousChecks(
      Func<Stream, HttpContent, TransportContext, Task> onStreamAvailable,
      string mediaType)
      : base(onStreamAvailable, mediaType)
    {
    }

    public VssServerPushStreamContentBypassAnonymousChecks(
      Action<Stream, HttpContent, TransportContext> onStreamAvailable,
      MediaTypeHeaderValue mediaType)
      : base(onStreamAvailable, mediaType)
    {
    }

    public VssServerPushStreamContentBypassAnonymousChecks(
      Func<Stream, HttpContent, TransportContext, Task> onStreamAvailable,
      MediaTypeHeaderValue mediaType)
      : base(onStreamAvailable, mediaType)
    {
    }
  }
}
