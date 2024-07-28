// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Repository.FolderXmlDocumentRepository
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.IO;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Repository
{
  public class FolderXmlDocumentRepository : IXmlDocumentRepository, IDisposable
  {
    private readonly string folder;

    public FolderXmlDocumentRepository(string folder) => this.folder = folder;

    public XDocument TryGetDocument(string filePath, Action<string> logError)
    {
      string str = Path.Combine(this.folder, filePath);
      if (File.Exists(str))
      {
        try
        {
          return XDocument.Load(str);
        }
        catch (Exception ex)
        {
          logError("Failed to load file '" + str + "': " + ex.ToString());
        }
      }
      return (XDocument) null;
    }

    public void Dispose()
    {
    }
  }
}
