// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestStepXmlParserHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class TestStepXmlParserHelper
  {
    private const string c_sharedStepNodeName = "compref";
    private const string c_testStepNodeName = "step";
    private const string c_parameterizedStringNodeName = "parameterizedString";
    private const string c_isFormattedAttribute = "isformatted";
    private const string c_actionNodeName = "action";
    private const string c_expectedNodeName = "expected";
    private const string c_parameterNodeName = "parameter";
    private const string c_outputparameterNodeName = "outputparameter";
    private const string c_textNodeName = "#text";

    public static List<TestActionModel> GetTestStepsArray(string testStepsXml)
    {
      if (testStepsXml == null || testStepsXml == string.Empty)
        return new List<TestActionModel>();
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(testStepsXml);
      return TestStepXmlParserHelper.ParseSteps((XmlNode) xmlDocument.DocumentElement);
    }

    private static List<TestActionModel> ParseSteps(XmlNode testStepsXml)
    {
      List<TestActionModel> first = new List<TestActionModel>();
      foreach (XmlNode childNode in testStepsXml.ChildNodes)
      {
        switch (childNode.Name.ToLower())
        {
          case "step":
            TestStepModel testStepModel = TestStepXmlParserHelper.ReadStep(childNode);
            if (testStepModel != null)
            {
              first.Add((TestActionModel) testStepModel);
              continue;
            }
            continue;
          case "compref":
            List<TestActionModel> second = TestStepXmlParserHelper.ReadSharedStep(childNode);
            first = first.Concat<TestActionModel>((IEnumerable<TestActionModel>) second).ToList<TestActionModel>();
            continue;
          default:
            continue;
        }
      }
      return first;
    }

    private static List<TestActionModel> ReadSharedStep(XmlNode step) => new List<TestActionModel>()
    {
      (TestActionModel) SharedStepModel.CreateSharedStep(int.Parse(step.Attributes["id"].Value), int.Parse(step.Attributes["ref"].Value))
    }.Concat<TestActionModel>((IEnumerable<TestActionModel>) TestStepXmlParserHelper.ParseSteps(step)).ToList<TestActionModel>();

    private static TestStepModel ReadStep(XmlNode step)
    {
      if (step == null || step.Attributes["id"] == null || step.Attributes["type"] == null)
        return (TestStepModel) null;
      XmlNodeList xmlNodeList = step.SelectNodes("parameterizedString");
      int num = 0;
      string action = (string) null;
      string expectedResult = (string) null;
      bool flag1 = false;
      bool flag2 = false;
      bool isFormatted = false;
      int id = int.Parse(step.Attributes["id"].Value);
      string stepType = step.Attributes["type"].Value;
      foreach (XmlNode parameterizedString in xmlNodeList)
      {
        if (num == 0)
        {
          action = TestStepXmlParserHelper.ReadParameterizedString(parameterizedString);
          flag1 = parameterizedString.Attributes["isformatted"]?.Value?.ToLower() == bool.TrueString.ToLowerInvariant();
        }
        else
        {
          expectedResult = TestStepXmlParserHelper.ReadParameterizedString(parameterizedString);
          flag2 = parameterizedString.Attributes["isformatted"]?.Value?.ToLower() == bool.TrueString.ToLowerInvariant();
        }
        ++num;
      }
      if (flag1 & flag2)
        isFormatted = true;
      if (action == null && expectedResult == null)
      {
        XmlNodeList childNodes = step.ChildNodes;
        if (childNodes != null && childNodes.Count > 0)
        {
          foreach (XmlNode xmlNode in childNodes)
          {
            if (xmlNode != null && xmlNode.Name != null)
            {
              switch (xmlNode.Name.ToLower())
              {
                case "action":
                  action = xmlNode.InnerText;
                  continue;
                case "expected":
                  expectedResult = xmlNode.InnerText;
                  continue;
                default:
                  continue;
              }
            }
          }
        }
      }
      return TestStepModel.CreateStep(id, stepType, action, expectedResult, isFormatted);
    }

    private static string ReadParameterizedString(XmlNode parameterizedString)
    {
      List<string> values = new List<string>();
      foreach (XmlNode childNode in parameterizedString.ChildNodes)
      {
        switch (childNode.Name.ToLower())
        {
          case "parameter":
            values.Add(string.Format("@{0}", (object) childNode.InnerText));
            continue;
          case "outputparameter":
            values.Add(string.Format("@?{0}", (object) childNode.InnerText));
            continue;
          case "#text":
            values.Add(childNode.InnerText);
            continue;
          default:
            continue;
        }
      }
      return string.Join("", (IEnumerable<string>) values);
    }
  }
}
