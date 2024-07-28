// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.AdminUpdatePackageHelpers
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class AdminUpdatePackageHelpers
  {
    public static XmlDocument BuildDestroyWorkItemTypeUpdatePackage(string name, string projectName)
    {
      XmlDocument xmlDocument = new XmlDocument();
      XmlElement element1 = xmlDocument.CreateElement("Package");
      xmlDocument.AppendChild((XmlNode) element1);
      XmlElement element2 = xmlDocument.CreateElement("DestroyWorkItemType");
      element2.SetAttribute("WorkItemTypeName", name);
      element2.SetAttribute("ProjectName", projectName);
      element1.AppendChild((XmlNode) element2);
      return xmlDocument;
    }

    public static XmlDocument BuildRenameWorkItemTypeUpdatePackage(
      string witName,
      string newWitName,
      string projectName)
    {
      if (newWitName.Contains("\\") || newWitName.Length > 256)
        throw new ArgumentException(InternalsResourceStrings.Format("ErrorWorkItemTypeNameInvalid", (object) newWitName));
      XmlDocument xmlDocument = new XmlDocument();
      XmlElement element1 = xmlDocument.CreateElement("Package");
      xmlDocument.AppendChild((XmlNode) element1);
      XmlElement element2 = xmlDocument.CreateElement("RenameWorkItemType");
      element2.SetAttribute("WorkItemTypeName", witName);
      element2.SetAttribute("ProjectName", projectName);
      element2.SetAttribute("NewName", newWitName);
      element1.AppendChild((XmlNode) element2);
      return xmlDocument;
    }
  }
}
