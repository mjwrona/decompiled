// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FileWriter
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class FileWriter : IDisposable
  {
    private FileStream m_fileStream;
    private StreamWriter m_streamWriter;
    private long m_noOfLinesThreshold;
    private long m_linesInStream;

    public FileWriter(string filePath, int flushAfterLines = 100)
    {
      this.m_fileStream = File.Open(filePath, FileMode.OpenOrCreate);
      this.m_streamWriter = new StreamWriter((Stream) this.m_fileStream);
      if (flushAfterLines > 0)
        this.m_noOfLinesThreshold = (long) flushAfterLines;
      this.m_linesInStream = 0L;
    }

    public void WriteLine(string lineToWrite)
    {
      this.m_streamWriter.WriteLine(lineToWrite);
      ++this.m_linesInStream;
      if (this.m_linesInStream < this.m_noOfLinesThreshold)
        return;
      this.m_linesInStream = 0L;
      this.m_streamWriter.Flush();
      this.m_fileStream.Flush(true);
    }

    public void Close()
    {
      this.m_streamWriter.Flush();
      this.m_fileStream.Flush(true);
      this.m_streamWriter.Close();
      this.m_fileStream.Close();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.m_streamWriter == null || this.m_fileStream == null)
        return;
      this.m_streamWriter.Flush();
      this.m_fileStream.Flush(true);
      this.m_streamWriter.Dispose();
      this.m_fileStream.Dispose();
      this.m_streamWriter = (StreamWriter) null;
      this.m_fileStream = (FileStream) null;
    }
  }
}
