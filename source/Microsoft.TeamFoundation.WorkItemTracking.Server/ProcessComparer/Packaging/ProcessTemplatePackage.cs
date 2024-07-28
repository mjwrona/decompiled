// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Packaging.ProcessTemplatePackage
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Repository;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Packaging
{
  public class ProcessTemplatePackage
  {
    private IXmlDocumentRepository documentRepository;
    private XDocument processTemplateXml;

    public ProcessTemplatePackage(
      IXmlDocumentRepository documentRepository,
      Action<string> logError)
    {
      this.documentRepository = documentRepository;
      this.GetManifestDocument(logError);
    }

    private XDocument GetManifestDocument(Action<string> logError)
    {
      if (this.processTemplateXml == null)
        this.processTemplateXml = this.documentRepository.TryGetDocument("ProcessTemplate.xml", logError);
      return this.processTemplateXml;
    }

    public XDocument GetDocument(string fileName, Action<string> logError) => this.documentRepository.TryGetDocument(fileName, logError);

    public XDocument GetGroupTaskDocument(string groupName, Action<string> logError)
    {
      XElement root = this.GetManifestDocument(logError).Root;
      string str;
      if (root == null)
      {
        str = (string) null;
      }
      else
      {
        XElement xelement = root.Element((XName) "groups");
        str = xelement != null ? xelement.Elements((XName) "group").FirstOrDefault<XElement>((Func<XElement, bool>) (e => StringComparer.Ordinal.Equals(e.Attribute((XName) "id")?.Value, groupName)))?.Element((XName) "taskList")?.Attribute((XName) "filename")?.Value : (string) null;
      }
      string filePath = str;
      if (string.IsNullOrEmpty(filePath))
        logError("Group task file name cannot be found.");
      return this.documentRepository.TryGetDocument(filePath, logError);
    }
  }
}
