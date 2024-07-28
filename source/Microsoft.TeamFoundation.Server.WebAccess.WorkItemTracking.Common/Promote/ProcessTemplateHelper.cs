// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote.ProcessTemplateHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote
{
  public static class ProcessTemplateHelper
  {
    public static XmlDocument GetWorkItemTrackingProcessData(
      IVssRequestContext requestContext,
      IProcessTemplate template)
    {
      using (StreamReader streamReader1 = new StreamReader(template.GetResource("ProcessTemplate.xml")))
      {
        XmlNode xmlNode = XmlUtility.GetDocument(streamReader1.ReadToEnd()).DocumentElement.SelectSingleNode("//groups/group[@id='WorkItemTracking']/taskList/@filename");
        if (xmlNode != null)
        {
          using (StreamReader streamReader2 = new StreamReader(template.GetResource(xmlNode.Value)))
            return XmlUtility.GetDocument(streamReader2.ReadToEnd());
        }
      }
      return (XmlDocument) null;
    }

    public static string GetProcessTemplateTypeString(
      IVssRequestContext requestContext,
      IProcessTemplate template)
    {
      using (StreamReader streamReader = new StreamReader(template.GetResource("ProcessTemplate.xml")))
      {
        XmlNode xmlNode = XmlUtility.GetDocument(streamReader.ReadToEnd()).DocumentElement.SelectSingleNode("//version/@type");
        if (xmlNode != null)
          return xmlNode.Value;
      }
      return (string) null;
    }
  }
}
