// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultPage
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Globalization;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestResultPage : TeamFoundationPage
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      TfsTestManagementRequestContext context = new TfsTestManagementRequestContext(this.RequestContext);
      try
      {
        this.EnterMethod(new MethodInformation("TCMTestResultPage", MethodType.Normal, EstimatedMethodCost.Low));
        string toolSpecificId = this.Request["artifactMoniker"];
        if (string.IsNullOrEmpty(toolSpecificId))
        {
          this.Response.StatusCode = 400;
        }
        else
        {
          TestRunArtifactInfo info = new TestRunArtifactInfo();
          TestCaseResult artifact = TestCaseResult.FindArtifact((TestManagementRequestContext) context, toolSpecificId, ref info);
          if (artifact != null)
          {
            this.Response.Cache.SetLastModified(artifact.LastUpdated);
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            this.Response.ContentType = "text/xml";
            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(this.Response.Output))
            {
              string str = context.TestManagementHost.SiteHost.StaticContentDirectory.TrimEnd('/') + "/TestManagement/v1.0/Transforms/" + this.RequestContext.ServiceHost.GetCulture(this.RequestContext).LCID.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "/TestResult.xsl";
              xmlTextWriter.WriteStartDocument();
              xmlTextWriter.WriteProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"" + str + "\"");
              xmlTextWriter.WriteStartElement("TestResult");
              xmlTextWriter.WriteElementString("DisplayName", artifact.OwnerName);
              new XmlSerializer(typeof (TestCaseResult)).Serialize((XmlWriter) xmlTextWriter, (object) artifact);
              xmlTextWriter.WriteEndElement();
              xmlTextWriter.WriteEndDocument();
              xmlTextWriter.Flush();
            }
          }
          else
            this.Response.StatusCode = 404;
        }
      }
      catch (Exception ex)
      {
        context.TraceException("Pages", ex);
        this.Response.StatusCode = 500;
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
