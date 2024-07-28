// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.StreamedHttpWebResponse
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

using System.IO;
using System.Net;

namespace Microsoft.VisualStudio.Utilities.Internal
{
  public class StreamedHttpWebResponse : IStreamedHttpWebResponse
  {
    public ErrorCode ErrorCode { get; set; }

    public WebExceptionStatus ExceptionCode { get; set; }

    public HttpWebResponse Response { get; set; }

    public HttpStatusCode StatusCode { get; set; }

    public Stream GetResponseStream()
    {
      Stream responseStream = (Stream) null;
      if (this.ErrorCode == ErrorCode.NoError)
        responseStream = this.Response.GetResponseStream();
      return responseStream;
    }
  }
}
