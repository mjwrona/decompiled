// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCaseExportService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using HtmlAgilityPack;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.RestControllers.Helpers;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestCaseExportService : 
    TfsTestManagementService,
    ITestCaseExportService,
    IVssFrameworkService
  {
    private TestManagementRequestContext requestContext;
    private static ITelemetryLogger m_telemetryLogger;

    public byte[] ExportSuiteTestCasesToExcel(
      TestManagementRequestContext context,
      int testPlanId,
      int testSuiteId,
      List<int> testCaseIds)
    {
      this.requestContext = context;
      using (PerfManager.Measure(this.requestContext.RequestContext, "BusinessLayer", "TestCaseExportService.ExportSuiteTestCasesToExcel"))
      {
        this.GetWorkItems(testCaseIds);
        MemoryStream memoryStream = new MemoryStream();
        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create((Stream) memoryStream, SpreadsheetDocumentType.Workbook))
        {
          WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
          workbookPart.Workbook = new Workbook();
          WorksheetPart part = workbookPart.AddNewPart<WorksheetPart>();
          part.Worksheet = new Worksheet(new OpenXmlElement[1]
          {
            (OpenXmlElement) new SheetData()
          });
          workbookPart.Workbook.AppendChild<Sheets>(new Sheets()).Append(new OpenXmlElement[1]
          {
            (OpenXmlElement) new Sheet()
            {
              Id = (StringValue) workbookPart.GetIdOfPart((OpenXmlPart) part),
              SheetId = (UInt32Value) 1U,
              Name = (StringValue) "Test Cases"
            }
          });
          SheetData firstChild = part.Worksheet.GetFirstChild<SheetData>();
          foreach (Row testCaseRow in this.GetTestCaseRows(testCaseIds))
            firstChild.Append(new OpenXmlElement[1]
            {
              (OpenXmlElement) testCaseRow
            });
          workbookPart.Workbook.Save();
        }
        byte[] array = memoryStream.ToArray();
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "PlanId",
            (object) testPlanId.ToString()
          },
          {
            "SuiteId",
            (object) testSuiteId.ToString()
          },
          {
            "TestCaseIds",
            (object) string.Join<int>(",", (IEnumerable<int>) testCaseIds)
          },
          {
            "ExportedTestCaseFileLength",
            (object) array.Length.ToString()
          }
        });
        TestCaseExportService.TelemetryLogger.PublishData(context.RequestContext, "ExportTestCase", cid);
        return array;
      }
    }

    internal List<WorkItem> GetWorkItems(List<int> workItemIds)
    {
      using (PerfManager.Measure(this.requestContext.RequestContext, "BusinessLayer", "TestCaseExportService.GetWorkItems"))
      {
        List<WorkItem> workItems = new List<WorkItem>();
        using (PerfManager.Measure(this.requestContext.RequestContext, "BusinessLayer", "TestResultFailureTypeHelper.DeleteTestResultFailureType"))
        {
          IWorkItemRemotableService service = this.requestContext.RequestContext.GetService<IWorkItemRemotableService>();
          foreach (int workItemId in workItemIds)
          {
            try
            {
              WorkItem workItem = service.GetWorkItem(this.requestContext.RequestContext, workItemId, (IEnumerable<string>) new List<string>(), expand: WorkItemExpand.All);
              workItems.Add(workItem);
            }
            catch (Exception ex)
            {
              this.requestContext.RequestContext.TraceError("BusinessLayer", "TestCaseExportService.GetWorkItems: Exception while getting work item with id {0}. Exception: {1}", (object) workItemId, (object) ex.Message);
            }
          }
        }
        return workItems;
      }
    }

    internal Dictionary<string, string> ReadStep(XmlNode childNode)
    {
      using (PerfManager.Measure(this.requestContext.RequestContext, "BusinessLayer", "TestCaseExportService.ReadStep"))
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary["Id"] = childNode.Attributes["id"].Value;
        dictionary["Type"] = childNode.Attributes["type"].Value;
        foreach (XmlNode selectNode in childNode.SelectNodes("parameterizedString"))
        {
          string str = selectNode.Attributes["isformatted"].Value;
          string innerText = selectNode.InnerText;
          if (str == "true")
          {
            if (!dictionary.ContainsKey("StepAction"))
              dictionary["StepAction"] = innerText;
            else
              dictionary["StepResult"] = innerText;
          }
        }
        return dictionary;
      }
    }

    private List<XmlNode> FlattenXml(XmlNode node)
    {
      List<XmlNode> flattenedNodes = new List<XmlNode>();
      this.FlattenXmlHelper(node, flattenedNodes);
      return flattenedNodes;
    }

    internal void FlattenXmlHelper(XmlNode node, List<XmlNode> flattenedNodes)
    {
      switch (node)
      {
        case XmlElement xmlElement:
          flattenedNodes.Add((XmlNode) xmlElement);
          foreach (XmlAttribute attribute in (XmlNamedNodeMap) xmlElement.Attributes)
            flattenedNodes.Add((XmlNode) attribute);
          IEnumerator enumerator = xmlElement.ChildNodes.GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
              this.FlattenXmlHelper((XmlNode) enumerator.Current, flattenedNodes);
            break;
          }
          finally
          {
            if (enumerator is IDisposable disposable)
              disposable.Dispose();
          }
        case XmlText xmlText:
          flattenedNodes.Add((XmlNode) xmlText);
          break;
      }
    }

    internal List<List<Cell>> GetTestCaseStepRows(WorkItem workItem, int sharedStepIndex)
    {
      using (PerfManager.Measure(this.requestContext.RequestContext, "BusinessLayer", "TestCaseExportService.GetTestCaseStepRows"))
      {
        List<List<Cell>> testCaseStepRows1 = new List<List<Cell>>();
        if (workItem.Fields.ContainsKey("Microsoft.VSTS.TCM.Steps"))
        {
          string xml = workItem.Fields.GetValueOrDefault<string, object>("Microsoft.VSTS.TCM.Steps").ToString();
          if (!string.IsNullOrEmpty(xml))
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            XmlNode documentElement = (XmlNode) xmlDocument.DocumentElement;
            if (documentElement.NodeType == XmlNodeType.Element)
            {
              List<XmlNode> xmlNodeList = this.FlattenXml(documentElement);
              int num = 1;
              foreach (XmlNode childNode in xmlNodeList)
              {
                switch (childNode.Name.ToLower())
                {
                  case "step":
                    List<Cell> cellList = this.PopulateRowForTestCaseStep(this.ReadStep(childNode), num, sharedStepIndex);
                    testCaseStepRows1.Add(cellList);
                    ++num;
                    continue;
                  case "compref":
                    WorkItem workItem1 = this.GetWorkItems(new List<int>()
                    {
                      Convert.ToInt32(childNode.Attributes["ref"].Value)
                    }).FirstOrDefault<WorkItem>();
                    List<Cell> headerDescriptionRow = this.GetSharedStepsHeaderDescriptionRow(workItem1, num);
                    testCaseStepRows1.Add(headerDescriptionRow);
                    List<List<Cell>> testCaseStepRows2 = this.GetTestCaseStepRows(workItem1, num);
                    testCaseStepRows1.AddRange((IEnumerable<List<Cell>>) testCaseStepRows2);
                    ++num;
                    continue;
                  default:
                    continue;
                }
              }
            }
          }
        }
        return testCaseStepRows1;
      }
    }

    internal List<Cell> GetTestCaseHeaderCells() => new List<Cell>()
    {
      this.CreateCell((object) "ID"),
      this.CreateCell((object) "Work Item Type"),
      this.CreateCell((object) "Title"),
      this.CreateCell((object) "Test Step"),
      this.CreateCell((object) "Step Action"),
      this.CreateCell((object) "Step Expected"),
      this.CreateCell((object) "Area Path"),
      this.CreateCell((object) "Assigned To"),
      this.CreateCell((object) "State")
    };

    internal List<Cell> GetTestCaseDescriptionRow(WorkItem testCaseWorkItem)
    {
      using (PerfManager.Measure(this.requestContext.RequestContext, "BusinessLayer", "TestCaseExportService.GetTestCaseDescriptionRow"))
      {
        List<Cell> caseDescriptionRow = new List<Cell>();
        caseDescriptionRow.Add(this.CreateCell(testCaseWorkItem.Fields["System.Id"]));
        caseDescriptionRow.Add(this.CreateCell((object) "Test Case"));
        caseDescriptionRow.Add(this.CreateCell((object) testCaseWorkItem.Fields["System.Title"].ToString()));
        caseDescriptionRow.Add(this.CreateCell((object) string.Empty));
        caseDescriptionRow.Add(this.CreateCell((object) string.Empty));
        caseDescriptionRow.Add(this.CreateCell((object) string.Empty));
        caseDescriptionRow.Add(this.CreateCell((object) testCaseWorkItem.Fields["System.AreaPath"].ToString()));
        string cellValue = CommonUtils.DistinctDisplayName(testCaseWorkItem.Fields["System.AssignedTo"] as IdentityRef);
        if (cellValue != null)
          caseDescriptionRow.Add(this.CreateCell((object) cellValue));
        else
          caseDescriptionRow.Add(this.CreateCell((object) string.Empty));
        caseDescriptionRow.Add(this.CreateCell((object) testCaseWorkItem.Fields["System.State"].ToString()));
        return caseDescriptionRow;
      }
    }

    internal List<Cell> GetSharedStepsHeaderDescriptionRow(
      WorkItem sharedStepWorkItem,
      int sharedStepsIndex)
    {
      using (PerfManager.Measure(this.requestContext.RequestContext, "BusinessLayer", "TestCaseExportService.GetSharedStepsHeaderDescriptionRow"))
        return new List<Cell>()
        {
          this.CreateCell(sharedStepWorkItem.Fields["System.Id"]),
          this.CreateCell((object) "Shared Steps"),
          this.CreateCell((object) string.Empty),
          this.CreateCell((object) sharedStepsIndex),
          this.CreateCell((object) sharedStepWorkItem.Fields["System.Title"].ToString()),
          this.CreateCell((object) string.Empty),
          this.CreateCell((object) string.Empty),
          this.CreateCell((object) string.Empty),
          this.CreateCell((object) string.Empty)
        };
    }

    internal List<Cell> PopulateRowForTestCaseStep(
      Dictionary<string, string> step,
      int i,
      int sharedStepIndex)
    {
      using (PerfManager.Measure(this.requestContext.RequestContext, "BusinessLayer", "TestCaseExportService.PopulateRowForTestCaseStep"))
        return new List<Cell>()
        {
          this.CreateCell((object) string.Empty),
          this.CreateCell((object) string.Empty),
          this.CreateCell((object) string.Empty),
          this.CreateCell((object) (sharedStepIndex > 0 ? (double) (sharedStepIndex * 10 + i) / 10.0 : (double) i)),
          this.CreateFormattedCell(step.GetValueOrDefault<string, string>("StepAction", (string) null)),
          this.CreateFormattedCell(step.GetValueOrDefault<string, string>("StepResult", (string) null)),
          this.CreateCell((object) string.Empty),
          this.CreateCell((object) string.Empty),
          this.CreateCell((object) string.Empty)
        };
    }

    internal List<Row> GetTestCaseRows(List<int> testCaseIds)
    {
      List<WorkItem> workItems = this.GetWorkItems(testCaseIds);
      List<Row> testCaseRows = new List<Row>();
      Row row1 = new Row();
      foreach (Cell testCaseHeaderCell in this.GetTestCaseHeaderCells())
        row1.Append(new OpenXmlElement[1]
        {
          (OpenXmlElement) testCaseHeaderCell
        });
      testCaseRows.Add(row1);
      foreach (WorkItem workItem in workItems)
      {
        if (!(workItem.Fields["System.WorkItemType"].ToString() != "Test Case"))
        {
          Row row2 = new Row();
          foreach (Cell cell in this.GetTestCaseDescriptionRow(workItem))
            row2.Append(new OpenXmlElement[1]
            {
              (OpenXmlElement) cell
            });
          testCaseRows.Add(row2);
          foreach (List<Cell> testCaseStepRow in this.GetTestCaseStepRows(workItem, 0))
          {
            Row row3 = new Row();
            foreach (Cell cell in testCaseStepRow)
              row3.Append(new OpenXmlElement[1]
              {
                (OpenXmlElement) cell
              });
            testCaseRows.Add(row3);
          }
        }
      }
      return testCaseRows;
    }

    private Cell CreateCell(object cellValue)
    {
      Cell cell = new Cell();
      switch (cellValue)
      {
        case string text:
          cell.DataType = (EnumValue<CellValues>) CellValues.String;
          cell.CellValue = new CellValue(text);
          break;
        case double num:
          cell.DataType = (EnumValue<CellValues>) CellValues.Number;
          cell.CellValue = new CellValue(num.ToString());
          break;
        case int _:
        case long _:
          cell.DataType = (EnumValue<CellValues>) CellValues.Number;
          cell.CellValue = new CellValue(cellValue.ToString());
          break;
        default:
          throw new ArgumentException("Unsupported data type");
      }
      return cell;
    }

    private Cell CreateFormattedCell(string htmlString)
    {
      Cell formattedCell = new Cell();
      formattedCell.DataType = (EnumValue<CellValues>) CellValues.InlineString;
      htmlString = new Regex("<br\\s*[\\/]?>", RegexOptions.IgnoreCase).Replace(htmlString, "\n");
      HtmlDocument htmlDocument = new HtmlDocument();
      htmlDocument.LoadHtml(htmlString);
      IEnumerable<HtmlNode> htmlNodes = htmlDocument.DocumentNode.DescendantsAndSelf().Where<HtmlNode>((Func<HtmlNode, bool>) (n => n.NodeType == HtmlNodeType.Text));
      List<Run> runList = new List<Run>();
      foreach (HtmlNode htmlNode in htmlNodes)
      {
        bool flag1 = htmlNode.Ancestors("b").Any<HtmlNode>();
        bool flag2 = htmlNode.Ancestors("i").Any<HtmlNode>();
        bool flag3 = htmlNode.Ancestors("u").Any<HtmlNode>();
        bool flag4 = htmlNode.Ancestors("s").Any<HtmlNode>();
        Run run1 = new Run();
        string text1 = WebUtility.HtmlDecode(htmlNode.InnerHtml);
        Run run2 = run1;
        OpenXmlElement[] openXmlElementArray = new OpenXmlElement[1];
        DocumentFormat.OpenXml.Spreadsheet.Text text2 = new DocumentFormat.OpenXml.Spreadsheet.Text(text1);
        text2.Space = (EnumValue<SpaceProcessingModeValues>) SpaceProcessingModeValues.Preserve;
        openXmlElementArray[0] = (OpenXmlElement) text2;
        run2.Append(openXmlElementArray);
        RunProperties runProperties = new RunProperties();
        if (flag1)
          runProperties.Append(new OpenXmlElement[1]
          {
            (OpenXmlElement) new Bold()
          });
        if (flag2)
          runProperties.Append(new OpenXmlElement[1]
          {
            (OpenXmlElement) new Italic()
          });
        if (flag3)
          runProperties.Append(new OpenXmlElement[1]
          {
            (OpenXmlElement) new Underline()
          });
        if (flag4)
          runProperties.Append(new OpenXmlElement[1]
          {
            (OpenXmlElement) new Strike()
          });
        run1.RunProperties = runProperties;
        runList.Add(run1);
      }
      InlineString inlineString = new InlineString();
      foreach (Run run in runList)
        inlineString.Append(new OpenXmlElement[1]
        {
          (OpenXmlElement) run
        });
      formattedCell.Append(new OpenXmlElement[1]
      {
        (OpenXmlElement) inlineString
      });
      return formattedCell;
    }

    internal static ITelemetryLogger TelemetryLogger
    {
      get
      {
        if (TestCaseExportService.m_telemetryLogger == null)
          TestCaseExportService.m_telemetryLogger = (ITelemetryLogger) new Microsoft.TeamFoundation.TestManagement.Server.TelemetryLogger();
        return TestCaseExportService.m_telemetryLogger;
      }
      set => TestCaseExportService.m_telemetryLogger = value;
    }
  }
}
