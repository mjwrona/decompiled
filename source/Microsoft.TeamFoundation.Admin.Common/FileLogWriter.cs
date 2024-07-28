// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.FileLogWriter
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using System;
using System.Globalization;
using System.IO;

namespace Microsoft.TeamFoundation.Admin
{
  internal sealed class FileLogWriter : ILogWriter, IDisposable
  {
    private bool m_disposed;
    private int m_errors;
    private string m_fileName;
    private object m_lock = new object();
    private StreamWriter m_writer;

    public FileLogWriter(string fileName) => this.m_fileName = fileName;

    public void Write(string text)
    {
      lock (this.m_lock)
      {
        this.ThrowIfDisposed();
        this.EnsureStreamIsOpenForWriting();
        if (this.m_writer == null)
          return;
        try
        {
          this.m_writer.Write(text);
          this.m_writer.Flush();
        }
        catch (Exception ex1)
        {
          this.TraceException(ex1);
          try
          {
            using (this.m_writer)
              ;
          }
          catch (Exception ex2)
          {
            AdminTrace.Error("Failure to close the stream: {0}", (object) ex2.ToString());
          }
          this.m_writer = (StreamWriter) null;
        }
      }
    }

    private void EnsureStreamIsOpenForWriting()
    {
      if (this.m_writer != null && this.m_writer.BaseStream != null && this.m_writer.BaseStream.CanWrite)
        return;
      this.ReleaseStreamWriter();
      FileStream fileStream = (FileStream) null;
      try
      {
        this.EnsureFileNameIsUnique();
        fileStream = new FileStream(this.m_fileName, FileMode.Append, FileAccess.Write, FileShare.Read | FileShare.Delete);
        this.m_writer = new StreamWriter((Stream) fileStream);
      }
      catch (Exception ex)
      {
        this.TraceException(ex);
        if (fileStream == null)
          return;
        try
        {
          using (fileStream)
            ;
        }
        catch
        {
        }
      }
    }

    private void TraceException(Exception ex)
    {
      if (this.m_errors > 3)
        return;
      AdminTrace.Error("Failure to log: {0}", (object) ex.ToString());
      ++this.m_errors;
    }

    private void ReleaseStreamWriter()
    {
      try
      {
        using (this.m_writer)
          ;
      }
      catch (Exception ex)
      {
        AdminTrace.Error(ex);
      }
    }

    private void EnsureFileNameIsUnique()
    {
      if (!File.Exists(this.m_fileName))
        return;
      string directoryName = Path.GetDirectoryName(this.m_fileName);
      string withoutExtension = Path.GetFileNameWithoutExtension(this.m_fileName);
      string extension = Path.GetExtension(this.m_fileName);
      string path;
      string path2;
      for (path = this.m_fileName; File.Exists(path); path = Path.Combine(directoryName, path2))
        path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}{2}", (object) withoutExtension, (object) Environment.TickCount, (object) extension);
      this.m_fileName = path;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!(!this.m_disposed & disposing))
        return;
      lock (this.m_lock)
      {
        if (this.m_writer != null)
        {
          this.m_writer.Flush();
          this.m_writer.Dispose();
          this.m_writer = (StreamWriter) null;
        }
        this.m_disposed = true;
      }
    }

    private void ThrowIfDisposed()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(nameof (FileLogWriter));
    }
  }
}
