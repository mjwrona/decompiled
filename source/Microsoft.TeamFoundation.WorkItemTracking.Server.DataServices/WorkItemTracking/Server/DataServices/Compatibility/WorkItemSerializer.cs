// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.WorkItemSerializer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  public sealed class WorkItemSerializer
  {
    public XmlDocument Serialize(
      IVssRequestContext requestContext,
      WorkItem workItem,
      int revision,
      string userLocale)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItem>(workItem, nameof (workItem));
      DocumentBuilder documentBuilder = new DocumentBuilder(requestContext);
      Payload workItemPayload1 = new Payload();
      PayloadCompatibilityUtils.FillPayloadWithWorkItemXml(requestContext.WitContext(), workItem, workItemPayload1, revision);
      Payload workItemPayload2 = workItemPayload1;
      string userLocale1 = userLocale;
      XmlDocument xmlDocument = documentBuilder.RenderWorkItemXml(workItemPayload2, userLocale1);
      requestContext.TraceLeave(900538, "DataAccessLayer", "DataAccessLayerImpl", "GetWorkItemXml");
      return xmlDocument;
    }
  }
}
