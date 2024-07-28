// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.WorkItemDestroyHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  internal class WorkItemDestroyHandler : IUpdateHandler
  {
    private Lazy<List<int>> m_workItemsToDestroy = new Lazy<List<int>>();

    public XElement ProcessUpdate(
      IVssRequestContext requestContext,
      XElement updatePackage,
      out IEnumerable<XElement> results)
    {
      results = (IEnumerable<XElement>) null;
      foreach (XElement xelement in updatePackage.Elements().Where<XElement>((Func<XElement, bool>) (x => TFStringComparer.UpdateAction.Equals(x.Name.LocalName, "DestroyWorkItem"))).ToArray<XElement>())
      {
        XAttribute xattribute = xelement.Attribute((XName) "WorkItemID");
        if (xattribute == null)
          throw new ArgumentException(DalResourceStrings.Format("MissingAttributeInXmlException", (object) "WorkItemID"));
        int result;
        if (!int.TryParse(xattribute.Value, out result))
          throw new ArgumentException(DalResourceStrings.Get("UpdateInvalidAttributeIntegerException"), "updateElement");
        this.m_workItemsToDestroy.Value.Add(result);
        xelement.Remove();
      }
      if (this.m_workItemsToDestroy.Value.Any<int>())
      {
        requestContext.GetService<ITeamFoundationWorkItemService>().DestroyWorkItems(requestContext, (IEnumerable<int>) this.m_workItemsToDestroy.Value);
        results = (IEnumerable<XElement>) this.m_workItemsToDestroy.Value.Select<int, XElement>((Func<int, XElement>) (x => new XElement((XName) "DestroyWorkItem"))).ToArray<XElement>();
      }
      return updatePackage;
    }
  }
}
