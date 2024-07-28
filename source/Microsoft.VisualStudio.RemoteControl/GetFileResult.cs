// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteControl.GetFileResult
// Assembly: Microsoft.VisualStudio.RemoteControl, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D9D0761-3208-49DD-A9E2-BF705DBE6B5D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.RemoteControl.dll

using System;
using System.IO;
using System.Net;

namespace Microsoft.VisualStudio.RemoteControl
{
  internal class GetFileResult : IDisposable
  {
    internal HttpStatusCode Code { get; set; }

    internal Stream RespStream { get; set; }

    internal bool IsFromCache { get; set; }

    internal int? AgeSeconds { get; set; }

    internal string ErrorMessage { get; set; }

    internal bool IsSuccessStatusCode => this.Code == HttpStatusCode.OK || this.Code == HttpStatusCode.NotFound;

    public void Dispose()
    {
      Stream respStream = this.RespStream;
      if (respStream == null)
        return;
      respStream.Dispose();
      this.RespStream = (Stream) null;
    }
  }
}
