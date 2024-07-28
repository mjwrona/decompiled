// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FileContainer.Client.ReportTraceEventArgs
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.FileContainer.Client
{
  public class ReportTraceEventArgs : EventArgs
  {
    public ReportTraceEventArgs(string file, string message)
    {
      this.File = file;
      this.Message = message;
    }

    public string File { get; private set; }

    public string Message { get; private set; }
  }
}
