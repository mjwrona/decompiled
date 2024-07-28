// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestExternalLink
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int LinkId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Uri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Description { get; set; }

    [XmlIgnore]
    internal int PlanId { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ExternalLink id={0} Uri={1}", (object) this.LinkId, (object) this.Uri);

    internal static List<TestExternalLink> QueryTestPlanLinks(
      TfsTestManagementRequestContext request,
      string projectName,
      int testPlanId)
    {
      try
      {
        IWitHelper service = request.RequestContext.GetService<IWitHelper>();
        request.TraceEnter("BusinessLayer", "TestExternalLink.QueryTestPlanLinks");
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) request, projectName);
        List<TestExternalLink> testExternalLinkList = new List<TestExternalLink>();
        return request.SecurityManager.HasViewTestResultsPermission((TestManagementRequestContext) request, projectFromName.String) ? service.QueryHyperLinks((TestManagementRequestContext) request, testPlanId) : testExternalLinkList;
      }
      finally
      {
        request.TraceLeave("BusinessLayer", "TestExternalLink.QueryTestPlanLinks");
      }
    }

    internal static FileLinkInfo ToHyperLink(
      TfsTestManagementRequestContext request,
      TestExternalLink link)
    {
      try
      {
        request.TraceEnter("BusinessLayer", "TestExternalLink.ToHyperLink");
        FileLinkInfo hyperLink = new FileLinkInfo();
        hyperLink.FieldId = link.LinkId;
        hyperLink.Path = link.Uri;
        hyperLink.Comment = link.Description;
        request.TraceVerbose("BusinessLayer", "TestExternalLink.ToHyperLink: Link Details: {0}", (object) link.ToString());
        return hyperLink;
      }
      finally
      {
        request.TraceLeave("BusinessLayer", "TestExternalLink.ToHyperLink");
      }
    }
  }
}
