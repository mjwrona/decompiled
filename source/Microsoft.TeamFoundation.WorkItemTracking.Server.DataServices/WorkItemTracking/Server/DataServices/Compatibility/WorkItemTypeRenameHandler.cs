// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.WorkItemTypeRenameHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  internal class WorkItemTypeRenameHandler : IUpdateHandler
  {
    public XElement ProcessUpdate(
      IVssRequestContext requestContext,
      XElement updatePackage,
      out IEnumerable<XElement> results)
    {
      results = (IEnumerable<XElement>) null;
      List<XElement> xelementList = new List<XElement>();
      foreach (XElement xelement1 in updatePackage.Elements().Where<XElement>((Func<XElement, bool>) (x => TFStringComparer.UpdateAction.Equals(x.Name.LocalName, "RenameWorkItemType"))).ToArray<XElement>())
      {
        XAttribute xattribute1 = xelement1.Attribute((XName) "ProjectName");
        XAttribute xattribute2 = xelement1.Attribute((XName) "WorkItemTypeName");
        XAttribute xattribute3 = xelement1.Attribute((XName) "NewName");
        if (xattribute1 == null)
          throw new ArgumentException(DalResourceStrings.Format("MissingAttributeInXmlException", (object) "ProjectName"));
        if (xattribute2 == null)
          throw new ArgumentException(DalResourceStrings.Format("MissingAttributeInXmlException", (object) "WorkItemTypeName"));
        if (xattribute3 == null)
          throw new ArgumentException(DalResourceStrings.Format("MissingAttributeInXmlException", (object) "NewName"));
        string projectName = xattribute1.Value;
        string oldWorkItemTypeName = xattribute2.Value;
        string newWorkItemTypeName = xattribute3.Value;
        ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext.Elevate(), projectName);
        requestContext.GetService<IWorkItemTypeService>().RenameWorkItemType(requestContext, project.Id, oldWorkItemTypeName, newWorkItemTypeName);
        XElement xelement2 = new XElement((XName) "RenameWorkItemType");
        XAttribute content = new XAttribute((XName) "Number", (object) "0");
        xelement2.Add((object) content);
        xelementList.Add(xelement2);
        xelement1.Remove();
      }
      if (xelementList.Count > 0)
        results = (IEnumerable<XElement>) xelementList;
      return updatePackage;
    }
  }
}
