// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.XamlIntegration.XamlActivityArchiveReader
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.MachineManagement.XamlIntegration
{
  public static class XamlActivityArchiveReader
  {
    public static Collection<XElement> ReadActivities(string archivePath)
    {
      Collection<XElement> collection = new Collection<XElement>();
      using (ZipArchive zipArchive = new ZipArchive((Stream) File.OpenRead(archivePath), ZipArchiveMode.Read, false))
      {
        foreach (ZipArchiveEntry entry in zipArchive.Entries)
        {
          using (Stream stream = entry.Open())
          {
            using (StreamReader streamReader = new StreamReader(stream))
              collection.Add(XElement.Parse(streamReader.ReadToEnd()));
          }
        }
      }
      return collection;
    }
  }
}
