// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TestManagement.TestCaseParameterDataHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.TestManagement
{
  public class TestCaseParameterDataHelper
  {
    private const string c_sharedParametersSignature = "<!--10349AA0-2A06-4257-88F3-E209C7D423DB-->";
    private const string c_longTextTable = "LongTextItems";
    private const string c_tcmTestCaseDataFieldName = "Microsoft.VSTS.TCM.LocalDataSource";
    private const string c_tcmTestCaseParametersFieldName = "Microsoft.VSTS.TCM.Parameters";
    private const string c_workItemIdAttributeNameString = "WorkItemID";
    private static readonly string c_linkedTestCasesWiql = "SELECT [System.Links.LinkType], [System.Id]\r\n                                                                 FROM WorkItemLinks \r\n                                                                 WHERE [Source].[System.Id] = {0}\r\n                                                                 AND System.Links.LinkType = 'Microsoft.VSTS.TestCase.SharedStepReferencedBy-Forward' \r\n                                                                 AND [Target].[System.WorkItemType] IN GROUP '" + WitCategoryRefName.TestCase + "'\r\n                                                                 ORDER BY [System.Id]\r\n                                                                 MODE(MustContain)";
    private static readonly string c_linkedSharedStepsWiql = "SELECT [System.Links.LinkType], [System.Id]\r\n                                                                   FROM WorkItemLinks \r\n                                                                   WHERE [Source].[System.Id] = {0}\r\n                                                                   AND System.Links.LinkType = 'Microsoft.VSTS.TestCase.SharedStepReferencedBy-Reverse' \r\n                                                                   AND [Target].[System.WorkItemType] IN GROUP '" + WitCategoryRefName.SharedStep + "'\r\n                                                                   ORDER BY [System.Id]\r\n                                                                   MODE(MustContain)";

    public TestCaseParameterDataHelper(IVssRequestContext requestContext) => this.RequestContext = requestContext;

    public void GetParameterDataFieldValueForOldClients(PayloadTable table, PayloadColumn column)
    {
      FieldEntry field = (FieldEntry) null;
      if (!string.Equals(column.Name, "Words", StringComparison.OrdinalIgnoreCase))
        return;
      IFieldTypeDictionary fields = this.RequestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(this.RequestContext);
      if (fields == null)
        return;
      fields.TryGetField("Microsoft.VSTS.TCM.LocalDataSource", out field);
      if (field == null)
        return;
      int tcmTestCaseDataFieldId = field.FieldId;
      if (table.Converter == null)
        table.Converter = new PayloadTableConverter();
      table.Converter.AddWriteAction("Words", (PayloadTableWriteAction) ((tbl, payloadFieldIndex, row) =>
      {
        if (!tbl.Columns.Contains("FldID"))
          return;
        object obj1 = row["FldID"];
        int result;
        if (obj1 == null || !int.TryParse(obj1.ToString(), out result) || result != tcmTestCaseDataFieldId)
          return;
        object obj2 = row["Words"];
        if (obj2 == null)
          return;
        object parameterData = (object) this.ParseParameterData(obj2.ToString(), fields);
        row.SetValue("Words", parameterData);
      }));
    }

    public string ParseParameterData(string dataFieldValue, IFieldTypeDictionary fields)
    {
      try
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.ParseParameterData DataFieldValue = {0}", (object) dataFieldValue);
        StringBuilder sb = new StringBuilder();
        DataSet data = new DataSet();
        data.Locale = CultureInfo.InvariantCulture;
        if (!ParameterDataHelper.TryParseDataSetFromXml(data, dataFieldValue))
        {
          ISharedParameterData parameterDataFromJson = this.GetSharedParameterDataFromJson(dataFieldValue);
          if (parameterDataFromJson != null)
          {
            ParameterDataHelper.PopulateDataSetFromSharedParameterData(data, ParameterDataHelper.ReadParameterDataInfoFromJson(dataFieldValue), parameterDataFromJson);
            data.WriteXml((TextWriter) new StringWriter(sb), XmlWriteMode.WriteSchema);
            sb.Append("<!--10349AA0-2A06-4257-88F3-E209C7D423DB-->");
            dataFieldValue = sb.ToString();
          }
        }
        return dataFieldValue;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "DataAccessLayer", "WorkItemFieldData", ex);
        return dataFieldValue;
      }
    }

    private ISharedParameterData GetSharedParameterDataFromJson(string dataFieldValue)
    {
      try
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetSharedParameterDataFromJson DataFieldValue = {0}", (object) dataFieldValue);
        SharedParameterData parameterDataFromJson = (SharedParameterData) null;
        if (!string.IsNullOrEmpty(dataFieldValue))
        {
          TestCaseParameterDataInfo parameterDataInfo1 = ParameterDataHelper.ReadParameterDataInfoFromJson(dataFieldValue);
          if (parameterDataInfo1 != null && parameterDataInfo1.SharedParameterDataSetIds.Length != 0)
          {
            WorkItemFieldData parameterDataInfo2 = this.GetSharedParameterDataSetFromParameterDataInfo(parameterDataInfo1);
            if (parameterDataInfo2 != null)
            {
              string fieldValue = parameterDataInfo2.GetFieldValue<string>(this.RequestContext, "Microsoft.VSTS.TCM.Parameters");
              if (!string.IsNullOrEmpty(fieldValue))
                parameterDataFromJson = ParameterDataHelper.GetSharedParameterDataFromParametersFieldValue(fieldValue);
            }
          }
        }
        return (ISharedParameterData) parameterDataFromJson;
      }
      catch (InvalidOperationException ex)
      {
        this.RequestContext.TraceException(0, "DataAccessLayer", "DataAccessLayerImpl", (Exception) ex);
        return (ISharedParameterData) null;
      }
    }

    private WorkItemFieldData GetSharedParameterDataSetFromParameterDataInfo(
      TestCaseParameterDataInfo parameterDataInfo)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetSharedParameterDataSetFromParameterDataInfo");
      if (parameterDataInfo != null)
        this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetSharedParameterDataSetFromParameterDataInfo ParameterDataInfo = {0}", (object) parameterDataInfo);
      string[] fields = new string[1]
      {
        "Microsoft.VSTS.TCM.Parameters"
      };
      int num = ((IEnumerable<int>) parameterDataInfo.SharedParameterDataSetIds).First<int>();
      return this.RequestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemFieldValues(this.RequestContext, (IEnumerable<int>) new int[1]
      {
        num
      }, (IEnumerable<string>) fields).FirstOrDefault<WorkItemFieldData>();
    }

    private static string GetStringField(PayloadTable.PayloadRow testCase, string field)
    {
      try
      {
        object obj = testCase[field];
        return obj != null ? (string) obj : string.Empty;
      }
      catch
      {
        return string.Empty;
      }
    }

    public void HandleUpdate(XmlElement updateElement, bool isBulkUpdate)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.HandleUpdate");
      try
      {
        if (updateElement != null)
          this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.HandleUpdate UpdateElement xml = {0}", (object) updateElement.InnerXml);
        if (updateElement == null || !this.HasParametersOrDataFieldChanged(updateElement))
          return;
        List<XmlElement> updatedWorkItems = this.GetUpdatedWorkItems(updateElement);
        if (updatedWorkItems == null)
          return;
        foreach (XmlElement workItemElementNode in updatedWorkItems)
        {
          this.HandleUpdateForDataField(workItemElementNode, isBulkUpdate);
          this.HandleUpdateForParametersField(workItemElementNode);
        }
      }
      catch (Exception ex)
      {
        if (ex is TeamFoundationServiceException)
          throw;
        else
          this.RequestContext.TraceException(0, "DataAccessLayer", "DataAccessLayerImpl", ex);
      }
    }

    private bool HasParametersOrDataFieldChanged(XmlElement updateElement)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.HasParametersOrDataFieldChanged");
      if (updateElement != null)
        this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.HasParametersOrDataFieldChanged UpdateElement xml = {0}", (object) updateElement.InnerXml);
      if (updateElement == null || string.IsNullOrEmpty(updateElement.InnerXml))
        return false;
      return updateElement.InnerXml.IndexOf("Microsoft.VSTS.TCM.LocalDataSource", StringComparison.OrdinalIgnoreCase) >= 0 || updateElement.InnerXml.IndexOf("Microsoft.VSTS.TCM.Parameters", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void HandleUpdateForParametersField(XmlElement workItemElementNode)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.HandleUpdateForParametersField");
      if (workItemElementNode != null)
        this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.HandleUpdateForParametersField workItemElementNode Xml = {0}", (object) workItemElementNode.InnerXml);
      if (!this.IsUpdateAllowedForParametersField(workItemElementNode))
        throw new TeamFoundationServiceException(this.GetErrorMessageForSharedParameterUpdate(), 600171);
    }

    private string GetErrorMessageForSharedParameterUpdate()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(DalResourceStrings.Get("SharedStepSaveException"));
      stringBuilder.Append("\n");
      stringBuilder.Append(DalResourceStrings.Get("TestCaseDataFieldCannotBeModified"));
      return stringBuilder.ToString();
    }

    private bool IsUpdateAllowedForParametersField(XmlElement workItemElementNode)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.IsUpdateAllowedForParametersField");
      if (workItemElementNode != null)
        this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.IsUpdateAllowedForParametersField workItemElementNode innerXml= {0} outerXml={1}", (object) workItemElementNode.InnerXml, (object) workItemElementNode.OuterXml);
      try
      {
        if (workItemElementNode != null)
        {
          if (this.HasFieldChanged(workItemElementNode, "Microsoft.VSTS.TCM.Parameters"))
          {
            int workItemId = this.GetWorkItemId(workItemElementNode.OuterXml);
            if (workItemId != -1)
            {
              if (this.IsSharedStepWorkItem(workItemId))
              {
                if (this.HasLinkedTestCaseReferencingSharedParameters(workItemId))
                  return false;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "DataAccessLayer", "DataAccessLayerImpl", ex);
      }
      return true;
    }

    private bool IsSharedStepWorkItem(int workItemId)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.IsSharedStepWorkItem");
      this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.IsSharedStepWorkItem workItemId = {0}", (object) workItemId);
      WorkItemFieldData workItemFieldData = this.RequestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemFieldValues(this.RequestContext, (IEnumerable<int>) new int[1]
      {
        workItemId
      }, (IEnumerable<string>) new string[2]
      {
        "System.TeamProject",
        "System.WorkItemType"
      }).FirstOrDefault<WorkItemFieldData>();
      if (workItemFieldData != null)
      {
        Guid projectGuid = workItemFieldData.GetProjectGuid(this.RequestContext);
        string workItemType = workItemFieldData.WorkItemType;
        WorkItemTypeCategory workItemTypeCategory;
        if (this.RequestContext.GetService<IWorkItemTypeCategoryService>().TryGetWorkItemTypeCategory(this.RequestContext, projectGuid, "Microsoft.SharedStepCategory", out workItemTypeCategory))
          return workItemTypeCategory.WorkItemTypeNames.Contains<string>(workItemType, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
      return false;
    }

    private bool HasLinkedTestCaseReferencingSharedParameters(int sharedStepId)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.HasLinkedTestCaseReferencingSharedParameters");
      this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.HasLinkedTestCaseReferencingSharedParameters sharedStepId = {0}", (object) sharedStepId);
      List<int> referencingSharedStep = this.GetLinkedTestCasesReferencingSharedStep(sharedStepId);
      return referencingSharedStep != null && referencingSharedStep.Any<int>((System.Func<int, bool>) (workItemId => this.ReferencesSharedParameters(workItemId)));
    }

    private List<int> GetLinkedTestCasesReferencingSharedStep(int sharedStepId)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetLinkedTestCasesReferencingSharedStep");
      this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetLinkedTestCasesReferencingSharedStep sharedStepId = {0}", (object) sharedStepId);
      return this.GetLinkedWorkItems(sharedStepId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestCaseParameterDataHelper.c_linkedTestCasesWiql, (object) sharedStepId));
    }

    private List<int> GetLinkedSharedStepsForTestCase(int testCaseId)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetLinkedSharedStepsForTestCase");
      this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetLinkedSharedStepsForTestCase testCaseId = {0}", (object) testCaseId);
      return this.GetLinkedWorkItems(testCaseId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestCaseParameterDataHelper.c_linkedSharedStepsWiql, (object) testCaseId));
    }

    private List<int> GetLinkedWorkItems(int workItemId, string wiql)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetLinkedWorkItems");
      this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetLinkedWorkItems workItemId = {0}, wiql = {1}", (object) workItemId, (object) wiql);
      string projectName = this.GetProjectName(workItemId);
      return this.RequestContext.GetService<IWorkItemQueryService>().ExecuteQuery(this.RequestContext, wiql, (IDictionary) new Dictionary<string, object>()
      {
        {
          "project",
          (object) projectName
        }
      }).WorkItemLinks.Where<LinkQueryResultEntry>((System.Func<LinkQueryResultEntry, bool>) (link => link.SourceId > 0)).Select<LinkQueryResultEntry, int>((System.Func<LinkQueryResultEntry, int>) (link => link.TargetId)).ToList<int>();
    }

    private string GetProjectName(int workItemId)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetProjectName");
      this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetProjectName workItemId = {0}", (object) workItemId);
      WorkItemFieldData workItemFieldData = this.RequestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemFieldValues(this.RequestContext, (IEnumerable<int>) new int[1]
      {
        workItemId
      }, (IEnumerable<int>) new int[1]{ -42 }).FirstOrDefault<WorkItemFieldData>();
      return workItemFieldData == null ? string.Empty : workItemFieldData.GetProjectName(this.RequestContext);
    }

    private void HandleUpdateForDataField(XmlElement workItemElementNode, bool isBulkUpdate)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.HandleUpdateForDataField");
      if (workItemElementNode != null)
        this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.HandleUpdateForDataField workItemElementNode xml = {0} isBulkUpdate {1}", (object) workItemElementNode.InnerXml, (object) isBulkUpdate);
      if (!this.IsUpdateAllowedForDataField(workItemElementNode, isBulkUpdate))
        throw new TeamFoundationServiceException(DalResourceStrings.Get("TestCaseDataFieldCannotBeModified"), 600171);
    }

    private bool HasFieldChanged(XmlElement workItemElementNode, string fieldName)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.HasFieldChanged");
      if (workItemElementNode != null)
        this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.HasFieldChanged workItemElementNode xml= {0} fieldName = {1}", (object) workItemElementNode.InnerXml, (object) fieldName);
      bool flag = false;
      XmlNodeList elementsByTagName = workItemElementNode.GetElementsByTagName("InsertText");
      if (elementsByTagName != null)
      {
        foreach (XmlNode xmlNode in elementsByTagName)
        {
          if (xmlNode.Attributes != null && xmlNode.Attributes["FieldName"] != null && string.Equals(xmlNode.Attributes["FieldName"].Value, fieldName, StringComparison.OrdinalIgnoreCase))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    private bool IsUpdateAllowedForDataField(XmlElement workItemElementNode, bool isBulkUpdate)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.IsUpdateAllowedForDataField");
      if (workItemElementNode != null)
        this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.IsUpdateAllowedForDataField workItemElementNode = {0} isBulkUpdate = {1}", (object) workItemElementNode.InnerXml, (object) isBulkUpdate);
      try
      {
        if (workItemElementNode != null)
        {
          if (this.HasFieldChanged(workItemElementNode, "Microsoft.VSTS.TCM.LocalDataSource"))
          {
            int workItemId = this.GetWorkItemId(workItemElementNode.OuterXml);
            if (workItemId != -1)
            {
              if (!this.ReferencesSharedParameters(workItemId))
              {
                if (isBulkUpdate)
                {
                  if (!this.DoesAnySiblingTestCaseReferenceSharedParameters(workItemId))
                    goto label_11;
                }
                else
                  goto label_11;
              }
              return false;
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "DataAccessLayer", "DataAccessLayerImpl", ex);
        return true;
      }
label_11:
      return true;
    }

    private bool DoesAnySiblingTestCaseReferenceSharedParameters(int testCaseId)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.DoesAnySiblingTestCaseReferenceSharedParameters");
      this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.DoesAnySiblingTestCaseReferenceSharedParameters testCaseId = {0}", (object) testCaseId);
      List<int> stepsForTestCase = this.GetLinkedSharedStepsForTestCase(testCaseId);
      if (stepsForTestCase != null)
      {
        foreach (int sharedStepId in stepsForTestCase)
        {
          List<int> referencingSharedStep = this.GetLinkedTestCasesReferencingSharedStep(sharedStepId);
          if (referencingSharedStep != null && referencingSharedStep.Any<int>((System.Func<int, bool>) (testCase => this.ReferencesSharedParameters(testCase))))
            return true;
        }
      }
      return false;
    }

    private bool ReferencesSharedParameters(int workItemId)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.ReferencesSharedParameters");
      this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.ReferencesSharedParameters workItemId = {0}", (object) workItemId);
      string[] fields = new string[1]
      {
        "Microsoft.VSTS.TCM.LocalDataSource"
      };
      WorkItemFieldData workItemFieldData = this.RequestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemFieldValues(this.RequestContext, (IEnumerable<int>) new int[1]
      {
        workItemId
      }, (IEnumerable<string>) fields).FirstOrDefault<WorkItemFieldData>();
      if (workItemFieldData != null)
      {
        string fieldValue = workItemFieldData.GetFieldValue<string>(this.RequestContext, "Microsoft.VSTS.TCM.LocalDataSource");
        if (!string.IsNullOrEmpty(fieldValue) && this.GetSharedParameterDataFromJson(fieldValue) != null)
          return true;
      }
      return false;
    }

    private int GetWorkItemId(string workItemNodeXml)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetWorkItemId");
      this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetWorkItemId workItemNodeXml = {0}", (object) workItemNodeXml);
      int result = -1;
      XElement xelement = XElement.Parse(workItemNodeXml);
      if (xelement != null && xelement.Attributes() != null)
      {
        XAttribute xattribute = xelement.Attributes().FirstOrDefault<XAttribute>((System.Func<XAttribute, bool>) (attribute => attribute.Name == (XName) "WorkItemID"));
        if (xattribute != null && int.TryParse(xattribute.Value, out result))
          return result;
      }
      return -1;
    }

    private List<XmlElement> GetUpdatedWorkItems(XmlElement updateElement)
    {
      this.RequestContext.TraceEnter(0, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetUpdatedWorkItems");
      if (updateElement != null)
        this.RequestContext.Trace(0, TraceLevel.Verbose, "DataAccessLayer", "DataAccessLayerImpl", "TCM.TestCaseParameterDataHelper.GetUpdatedWorkItems updateElement xml = {0}", (object) updateElement.InnerXml);
      List<XmlElement> updatedWorkItems = new List<XmlElement>();
      try
      {
        if (updateElement != null)
        {
          XmlNodeList xmlNodeList = updateElement.SelectNodes("//UpdateWorkItem");
          if (xmlNodeList != null)
          {
            foreach (XmlNode xmlNode in xmlNodeList)
            {
              if (xmlNode is XmlElement xmlElement)
                updatedWorkItems.Add(xmlElement);
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "DataAccessLayer", "DataAccessLayerImpl", ex);
      }
      return updatedWorkItems;
    }

    public IVssRequestContext RequestContext { get; set; }
  }
}
