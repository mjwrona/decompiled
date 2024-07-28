// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Repository.ZipFileXmlDocumentRepository
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Repository
{
  public class ZipFileXmlDocumentRepository : IXmlDocumentRepository, IDisposable
  {
    private ZipArchive zipArchive;
    private MemoryStream memoryStream;
    private bool disposedValue;

    public ZipFileXmlDocumentRepository(string zipFilePath)
    {
      using (FileStream fileStream = new FileStream(zipFilePath, FileMode.Open))
      {
        this.memoryStream = new MemoryStream();
        fileStream.CopyTo((Stream) this.memoryStream);
      }
      this.zipArchive = new ZipArchive((Stream) this.memoryStream, ZipArchiveMode.Read, false, ZipArchiveProcessTemplatePackage.ZipEntryNameEnconding);
    }

    public ZipFileXmlDocumentRepository(Stream inputStream)
    {
      this.memoryStream = new MemoryStream();
      inputStream.CopyTo((Stream) this.memoryStream);
      this.zipArchive = new ZipArchive((Stream) this.memoryStream, ZipArchiveMode.Read, false, ZipArchiveProcessTemplatePackage.ZipEntryNameEnconding);
    }

    public XDocument TryGetDocument(string filePath, Action<string> logError)
    {
      string alternatePath = filePath.Replace("\\", "/");
      ZipArchiveEntry zipArchiveEntry = this.zipArchive.Entries.FirstOrDefault<ZipArchiveEntry>((Func<ZipArchiveEntry, bool>) (e => e.FullName.Equals(filePath, StringComparison.OrdinalIgnoreCase) || e.FullName.Equals(alternatePath, StringComparison.OrdinalIgnoreCase)));
      if (zipArchiveEntry == null)
        return (XDocument) null;
      try
      {
        using (Stream stream = zipArchiveEntry.Open())
          return XDocument.Load(stream);
      }
      catch (Exception ex)
      {
        logError("Failed to load file from zip '" + filePath + "': " + ex.ToString());
      }
      return (XDocument) null;
    }

    public void Dispose()
    {
      if (this.disposedValue)
        return;
      if (this.zipArchive != null)
      {
        this.zipArchive.Dispose();
        this.zipArchive = (ZipArchive) null;
      }
      if (this.memoryStream != null)
      {
        this.memoryStream.Dispose();
        this.memoryStream = (MemoryStream) null;
      }
      this.disposedValue = true;
    }
  }
}
