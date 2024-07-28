// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.ZipFileStreamResult
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class ZipFileStreamResult : FileResult
  {
    private IEnumerable<ZipFileStreamEntry> m_entries;
    private IEnumerator<ZipFileStreamEntry> m_entriesEnumerator;

    public ZipFileStreamResult(IEnumerable<ZipFileStreamEntry> zipFileEntries)
      : this()
    {
      this.m_entries = zipFileEntries;
    }

    protected ZipFileStreamResult()
      : base("application/zip")
    {
    }

    protected override void WriteFile(HttpResponseBase response)
    {
      response.BufferOutput = false;
      using (SmartPushStreamContentStream streamContentStream = new SmartPushStreamContentStream(response.OutputStream))
      {
        using (ZipArchive zipArchive = new ZipArchive((Stream) streamContentStream, ZipArchiveMode.Create))
        {
          while (response.IsClientConnected)
          {
            ZipFileStreamEntry nextEntry = this.GetNextEntry();
            if (nextEntry == null)
              break;
            if (nextEntry.IsFolder)
            {
              string relativePath = nextEntry.RelativePath;
              if (!relativePath.EndsWith("/", StringComparison.Ordinal) && !relativePath.EndsWith("\\", StringComparison.Ordinal))
                relativePath += "/";
              zipArchive.CreateEntry(relativePath);
            }
            else
            {
              try
              {
                using (Stream destination = zipArchive.CreateEntry(nextEntry.RelativePath, GitStreamUtil.OptimalCompressionLevel).Open())
                  GitStreamUtil.SmartCopyTo(nextEntry.Contents, destination, true);
              }
              finally
              {
                nextEntry.Contents.Dispose();
              }
            }
          }
        }
      }
    }

    protected virtual ZipFileStreamEntry GetNextEntry()
    {
      if (this.m_entries == null)
        return (ZipFileStreamEntry) null;
      if (this.m_entriesEnumerator == null)
        this.m_entriesEnumerator = this.m_entries.GetEnumerator();
      return !this.m_entriesEnumerator.MoveNext() ? (ZipFileStreamEntry) null : this.m_entriesEnumerator.Current;
    }
  }
}
