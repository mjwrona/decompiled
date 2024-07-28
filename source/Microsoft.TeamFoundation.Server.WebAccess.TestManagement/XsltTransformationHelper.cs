// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.XsltTransformationHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class XsltTransformationHelper
  {
    public static string GetXsltFile(TestManagerRequestContext testContext)
    {
      using (StreamReader streamReader = new StreamReader(XsltTransformationHelper.GetXsltFileLocation(testContext)))
        return streamReader.ReadToEnd();
    }

    public static XmlReader GetXsltReader(TestManagerRequestContext testContext, string content) => XmlReader.Create((TextReader) new StringReader(content));

    public static string GetXsltFileLocation(TestManagerRequestContext testContext)
    {
      string contentPath = testContext.TestRequestContext.TestManagementHost.SiteHost.StaticContentDirectory.TrimEnd('/') + "/TestManagement/v1.0/Transforms/" + testContext.TfsRequestContext.ServiceHost.GetCulture(testContext.TfsRequestContext).LCID.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "/TestSuite.xsl";
      return testContext.Controller.HttpContext.Server.MapPath(UrlHelper.GenerateContentUrl(contentPath, testContext.Controller.HttpContext));
    }
  }
}
