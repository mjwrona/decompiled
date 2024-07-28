// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations.LegacyTfsTestUtilsService
// Assembly: Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6D9A1E77-52F6-4366-807D-D0FABA8CDE81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.TestManagement;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations
{
  public class LegacyTfsTestUtilsService : ILegacyTestUtilsService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public string GenerateTestStepsHtml(string tcmStepsValue)
    {
      string testStepsHtml = tcmStepsValue;
      if (!string.IsNullOrEmpty(tcmStepsValue))
      {
        List<TestActionModel> testStepsArray = TestStepXmlParserHelper.GetTestStepsArray(tcmStepsValue);
        using (StringWriter writer = new StringWriter())
        {
          using (HtmlTextWriter htmlTextWriter = new HtmlTextWriter((TextWriter) writer))
          {
            htmlTextWriter.AddAttribute("cellpadding", "4");
            htmlTextWriter.AddAttribute("cellspacing", "5");
            htmlTextWriter.AddAttribute("align", "left");
            htmlTextWriter.AddAttribute("width", "100%");
            htmlTextWriter.RenderBeginTag("table");
            htmlTextWriter.RenderBeginTag("tr");
            htmlTextWriter.AddAttribute("align", "left");
            htmlTextWriter.RenderBeginTag("th");
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.AddAttribute("align", "left");
            htmlTextWriter.RenderBeginTag("th");
            htmlTextWriter.Write("Action");
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.AddAttribute("align", "left");
            htmlTextWriter.RenderBeginTag("th");
            htmlTextWriter.Write("Expected Result");
            htmlTextWriter.RenderEndTag();
            htmlTextWriter.RenderEndTag();
            int num = 1;
            foreach (TestActionModel testActionModel in testStepsArray)
            {
              htmlTextWriter.RenderBeginTag("tr");
              htmlTextWriter.RenderBeginTag("td");
              htmlTextWriter.Write(num++);
              htmlTextWriter.RenderEndTag();
              if (testActionModel is TestStepModel)
              {
                TestStepModel testStepModel = (TestStepModel) testActionModel;
                htmlTextWriter.RenderBeginTag("td");
                htmlTextWriter.Write(testStepModel.Action);
                htmlTextWriter.RenderEndTag();
                htmlTextWriter.RenderBeginTag("td");
                htmlTextWriter.Write(testStepModel.ExpectedResult);
                htmlTextWriter.RenderEndTag();
              }
              htmlTextWriter.RenderEndTag();
            }
            htmlTextWriter.RenderEndTag();
            testStepsHtml = writer.ToString();
          }
        }
      }
      return testStepsHtml;
    }
  }
}
