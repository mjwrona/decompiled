// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssServerHttpContentExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class IVssServerHttpContentExtensions
  {
    private static readonly StreamingContext s_unusedStreamingContext = new StreamingContext();
    private static readonly ServerVssJsonMediaTypeFormatter s_mediaTypeFormatter = new ServerVssJsonMediaTypeFormatter();

    internal static void Validate(this IVssServerHttpContent content, object securedObject)
    {
      HttpContextBase current = HttpContextFactory.Current;
      TrackedSecurityCollection securityCollection;
      if (!(current?.Items[(object) HttpContextConstants.IVssRequestContext] is IVssRequestContext vssRequestContext) || !vssRequestContext.RootContext.Items.TryGetValue<TrackedSecurityCollection>(RequestContextItemsKeys.SecurityTracking, out securityCollection))
        return;
      if (!current.Items.Contains((object) HttpContextConstants.SecurityTracking))
        current.Items[(object) HttpContextConstants.SecurityTracking] = (object) securityCollection;
      switch (securedObject)
      {
        case IEnumerable<ISecuredObject> securedObjects:
          using (IEnumerator<ISecuredObject> enumerator = securedObjects.GetEnumerator())
          {
            while (enumerator.MoveNext())
              ServerJsonSerializationHelper.ValidateSecurity((object) enumerator.Current, IVssServerHttpContentExtensions.s_unusedStreamingContext);
            break;
          }
        case ISecuredObjectContainer _:
          using (IVssServerHttpContentExtensions.FakeStream writeStream = new IVssServerHttpContentExtensions.FakeStream())
          {
            IVssServerHttpContentExtensions.s_mediaTypeFormatter.WriteToStreamAsync(securedObject.GetType(), securedObject, (Stream) writeStream, (HttpContent) null, (TransportContext) null).SyncResult();
            break;
          }
        default:
          ServerJsonSerializationHelper.ValidateSecurity(securedObject, IVssServerHttpContentExtensions.s_unusedStreamingContext);
          break;
      }
    }

    private class FakeStream : Stream
    {
      public override bool CanRead => true;

      public override bool CanSeek => true;

      public override bool CanWrite => true;

      public override long Length => 0;

      public override long Position
      {
        get => 0;
        set => throw new NotImplementedException();
      }

      public override void Flush()
      {
      }

      public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

      public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

      public override void SetLength(long value) => throw new NotImplementedException();

      public override void Write(byte[] buffer, int offset, int count)
      {
      }
    }
  }
}
