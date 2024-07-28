// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.FileTraceListener
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System.IO;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  public class FileTraceListener : AppTraceListener
  {
    private readonly TextWriter writer;

    public FileTraceListener(string fullfileName)
    {
      Directory.CreateDirectory(Path.GetDirectoryName(fullfileName));
      this.writer = (TextWriter) new StreamWriter((Stream) new FileStream(fullfileName, FileMode.Append, FileAccess.Write))
      {
        AutoFlush = true
      };
    }

    public override bool DetailedMessageFormat
    {
      get => true;
      set
      {
      }
    }

    public override void WriteLine(string message) => this.writer.WriteLine(message);

    protected override void Dispose(bool disposing)
    {
      this.writer.Dispose();
      base.Dispose(disposing);
    }
  }
}
