// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCaseCloner
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestCaseCloner : WITCreator
  {
    private Dictionary<int, int> sharedStepCloneCache;
    private HashSet<int> itemsUnderCloning;
    private TeamFoundationFileService fileService;
    private Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory m_testCaseCategory;
    private Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory m_sharedStepCategory;
    private TfsTestManagementRequestContext tcmRequestContext;
    private AttachmentsHelper m_attachmentsHelper;
    private AfnStripsHelper m_afnStripsHelper;

    public TestCaseCloner(TfsTestManagementRequestContext requestContext)
      : base((TestManagementRequestContext) requestContext)
    {
      this.opId = 0;
      this.sharedStepCloneCache = (Dictionary<int, int>) null;
      this.options = (CloneOperationInformation) null;
      this.tcmRequestContext = requestContext;
    }

    public int AddCloneTestCaseEntry(int testCaseId, bool suppressNotifications)
    {
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(this.RequestContext))
        planningDatabase.UpdateCloneRelationship(this.opId, new Dictionary<int, int>()
        {
          {
            testCaseId,
            testCaseId
          }
        }, CloneItemType.TestCase, false);
      return testCaseId;
    }

    public int CloneTestCase(
      int testCaseId,
      bool suppressNotifications,
      bool includeAttachments = true,
      bool includeLinks = true,
      string targetAreaPath = null)
    {
      int newTestCaseId = -1;
      try
      {
        this.itemsUnderCloning.Add(testCaseId);
        RetryHelper.RetryOnExceptions((Action) (() => newTestCaseId = this.CloneTestCaseWithoutRetry(this.options.DestinationProjectName, testCaseId, suppressNotifications, includeAttachments, includeLinks, targetAreaPath)), 1, typeof (WorkItemLinkInvalidException));
        if (newTestCaseId == -1)
          return newTestCaseId;
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(this.RequestContext))
          planningDatabase.UpdateCloneRelationship(this.opId, new Dictionary<int, int>()
          {
            {
              testCaseId,
              newTestCaseId
            }
          }, CloneItemType.TestCase);
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(this.TcmRequestContext, this.options.SourceProjectName);
        Validator.CheckAndGetProjectFromName(this.TcmRequestContext, this.options.DestinationProjectName);
        AfnStrip afnStrip = this.AfnStripsHelper.GetDefaultAfnStrips(projectFromName.GuidId, (IList<int>) new List<int>()
        {
          testCaseId
        }).FirstOrDefault<AfnStrip>();
        if (afnStrip != null)
        {
          Stream testAttachment = this.AttachmentsHelper.GetTestAttachment(projectFromName.GuidId.ToString(), afnStrip.TestRunId, afnStrip.TestResultId, afnStrip.Id, out string _, out CompressionType _);
          FileStream outStream = new FileStream(Path.GetTempFileName(), FileMode.Truncate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None, 1024, FileOptions.DeleteOnClose);
          this.FixSSIdInXml(testAttachment, "//*/@SharedSetSetId", (Stream) outStream);
          outStream.Seek(0L, SeekOrigin.Begin);
          using (StreamReader streamReader = new StreamReader((Stream) outStream))
          {
            byte[] bytes = Encoding.ASCII.GetBytes(streamReader.ReadToEnd());
            this.AfnStripsHelper.CreateAfnStrip(projectFromName.GuidId, new AfnStrip()
            {
              TestCaseId = newTestCaseId,
              Stream = Convert.ToBase64String(bytes)
            });
          }
        }
      }
      catch (WorkItemTrackingServiceException ex)
      {
        this.RequestContext.TraceException(0, "TestManagement", "BusinessLayer", (Exception) ex);
        throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyFailedWithWITError, (object) testCaseId, (object) ex.Message));
      }
      catch (TestManagementServiceException ex)
      {
        this.RequestContext.TraceException(0, "TestManagement", "BusinessLayer", (Exception) ex);
        throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyFailedForTestCase, (object) testCaseId, (object) ex.Message));
      }
      finally
      {
        this.itemsUnderCloning.Remove(testCaseId);
      }
      return newTestCaseId;
    }

    public int CloneTestCaseWithoutRetry(
      string projectName,
      int testCaseId,
      bool suppressNotifications,
      bool includeAttachments = true,
      bool includeLinks = true,
      string targetAreaPath = null)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "WITCreator.CloneTestCase {0}", (object) testCaseId);
      int num = 0;
      if (targetAreaPath != null)
        num = this.RequestContext.GetService<WebAccessWorkItemService>().GetAreaId(this.RequestContext, targetAreaPath);
      WITCreator witCreator = new WITCreator((TestManagementRequestContext) this.tcmRequestContext);
      IWorkItemRemotableService service = this.RequestContext.GetService<IWorkItemRemotableService>();
      WorkItem workItem;
      try
      {
        workItem = service.GetWorkItem(this.RequestContext, testCaseId, (IEnumerable<string>) new List<string>(), expand: WorkItemExpand.All);
      }
      catch (WorkItemNotFoundException ex)
      {
        return -1;
      }
      IList<WorkItemRelation> customizedRelations = this.GetCustomizedRelations(projectName, workItem, testCaseId, suppressNotifications, includeAttachments, includeLinks);
      witCreator.AddRelatedLinkInfo(workItem.Url, customizedRelations);
      IDictionary<string, object> customizedFieldUpdates = this.GetCustomizedFieldUpdates(workItem, testCaseId, true);
      if (num > 0)
        customizedFieldUpdates["System.AreaId"] = (object) num;
      string workItemtype;
      workItem.Fields.TryGetValue<string, string>("System.WorkItemType", out workItemtype);
      JsonPatchDocument jsonPatchDocument = WorkHelper.ConvertToJsonPatchDocument(this.RequestContext, customizedFieldUpdates, customizedRelations);
      int? id = service.CreateWorkItem(this.RequestContext, projectName, workItemtype, jsonPatchDocument, bypassRules: true, suppressNotifications: suppressNotifications).Id;
      return !id.HasValue ? -1 : id.Value;
    }

    protected override void PostInitialize(CloneOperationInformation options)
    {
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(this.RequestContext))
        this.sharedStepCloneCache = planningDatabase.GetCloneRelationship(this.opId, CloneItemType.SharedStep, true);
      this.itemsUnderCloning = new HashSet<int>();
      this.fileService = this.RequestContext.GetService<TeamFoundationFileService>();
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory> itemTypeCategories = this.RequestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategories(this.RequestContext, this.options.DestinationProjectName);
      this.m_testCaseCategory = itemTypeCategories.FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory, bool>) (cat => cat.ReferenceName.Equals("Microsoft.TestCaseCategory", StringComparison.OrdinalIgnoreCase)));
      this.m_sharedStepCategory = itemTypeCategories.FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory, bool>) (cat => cat.ReferenceName.Equals("Microsoft.SharedStepCategory", StringComparison.OrdinalIgnoreCase)));
    }

    private void CloneTestCaseAfnStrip(
      Guid projectId,
      int testCaseId,
      TestResultAttachment originalAfnStrip,
      int newTcId)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "WITCreator: Cloning AFNStrip of test case {0}", (object) testCaseId);
      long contentLength = 0;
      byte[] hashValue = (byte[]) null;
      Stream inStream = (Stream) null;
      Stream stream1 = (Stream) null;
      Stream stream2 = (Stream) null;
      try
      {
        inStream = this.fileService.RetrieveFile(this.RequestContext, (long) originalAfnStrip.FileId, false, out hashValue, out contentLength, out CompressionType _);
        inStream.Seek(0L, SeekOrigin.Begin);
        stream1 = (Stream) new FileStream(Path.GetTempFileName(), FileMode.Truncate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None, 1024, FileOptions.DeleteOnClose);
        stream2 = (Stream) new FileStream(Path.GetTempFileName(), FileMode.Truncate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None, 1024, FileOptions.DeleteOnClose);
        this.FixSSIdInXml(inStream, "//*/@SharedSetSetId", stream1);
        contentLength = stream1.Position;
        stream1.Seek(0L, SeekOrigin.Begin);
        byte[] md5 = MD5Util.CalculateMD5(stream1, false);
        stream1.Seek(0L, SeekOrigin.Begin);
        using (GZipStream destination = new GZipStream(stream2, CompressionMode.Compress, true))
          stream1.CopyTo((Stream) destination);
        stream2.Seek(0L, SeekOrigin.Begin);
        int tfsFileId = this.fileService.UploadFile(this.RequestContext, stream2, md5, contentLength, CompressionType.GZip, OwnerId.TeamTest, projectId, (string) null);
        bool changeCounterInterval = ServiceMigrationHelper.ShouldChangeCounterInterval(this.RequestContext);
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(this.RequestContext))
          planningDatabase.CloneAfnStrip(this.options.OpId, newTcId, tfsFileId, contentLength, originalAfnStrip.Comment, changeCounterInterval);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceError("BusinessLayer", "Encountered an TestManagement error while cloning AFNStrip. Discarding the AFNStrip since it is regeneratable information");
        this.RequestContext.TraceException("BusinessLayer", ex);
      }
      finally
      {
        stream2?.Close();
        stream1?.Close();
        inStream?.Close();
      }
    }

    private void FixSSIdInXml(Stream inStream, string xpathToSharedStepAttrib, Stream outStream) => this.FixSSIdInXml((TextReader) new StreamReader(inStream), xpathToSharedStepAttrib, (TextWriter) new StreamWriter(outStream));

    private void FixSSIdInXml(
      TextReader inStream,
      string xpathToSharedStepAttrib,
      TextWriter outStream)
    {
      XmlDocument xmlDocument;
      try
      {
        xmlDocument = XmlUtility.LoadXmlDocumentFromTextReader(inStream);
      }
      catch (XmlException ex)
      {
        throw new TestManagementValidationException(ServerResources.DeepCopySharedStepXmlParseError);
      }
      foreach (XmlAttribute selectNode in xmlDocument.SelectNodes(xpathToSharedStepAttrib))
      {
        int result;
        if (!int.TryParse(selectNode.Value, out result))
          throw new TestManagementValidationException(ServerResources.DeepCopySharedStepXmlParseError);
        if (this.sharedStepCloneCache.ContainsKey(result))
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "WITCreator: fixing up SharedStep {0} to {1} for xml {2}", (object) result, (object) this.sharedStepCloneCache[result], (object) xpathToSharedStepAttrib);
          selectNode.Value = Convert.ToString(this.sharedStepCloneCache[result]);
        }
      }
      xmlDocument.Save(outStream);
    }

    private IList<WorkItemRelation> GetCustomizedRelations(
      string projectName,
      WorkItem wi,
      int oldTcID,
      bool suppressNotifications,
      bool includeAttachments = true,
      bool includeLinks = true)
    {
      List<WorkItemRelation> customizedRelations = new List<WorkItemRelation>();
      if (wi == null || wi.Relations == null)
        return (IList<WorkItemRelation>) customizedRelations;
      foreach (WorkItemRelation relation in (IEnumerable<WorkItemRelation>) wi.Relations)
      {
        if (relation.Rel.Equals(WitCategoryRefName.SharedStepByReverseLink, StringComparison.OrdinalIgnoreCase))
        {
          customizedRelations.Add(this.CloneRelation(relation));
        }
        else
        {
          string str;
          if (!relation.Rel.Equals(WitCategoryRefName.SharedStepByForwardLink, StringComparison.OrdinalIgnoreCase) && !relation.Rel.Equals(TestCaseClonerConstants.TestedByReverseLinkName, StringComparison.OrdinalIgnoreCase) && !relation.Rel.Equals("System.LinkTypes.Hierarchy-Forward", StringComparison.OrdinalIgnoreCase) && (!relation.Rel.Equals("System.LinkTypes.Related", StringComparison.OrdinalIgnoreCase) || !relation.Attributes.TryGetValue<string, string>("comment", out str) || !str.Contains("TF237027")))
          {
            string empty;
            if (!relation.Attributes.TryGetValue<string, string>("name", out empty))
              empty = string.Empty;
            if (includeLinks && relation.Rel != "ArtifactLink" && relation.Rel != "AttachedFile" && empty != null && !empty.Equals("Session", StringComparison.Ordinal) && !empty.Equals("Test Result", StringComparison.Ordinal) && !empty.Equals("Result Attachment", StringComparison.Ordinal))
              customizedRelations.Add(this.CloneRelation(relation));
            if (includeAttachments && relation.Rel == "AttachedFile")
              customizedRelations.Add(this.CloneRelation(relation));
          }
        }
      }
      return (IList<WorkItemRelation>) customizedRelations;
    }

    private WorkItemRelation CloneRelation(WorkItemRelation relation)
    {
      WorkItemRelation workItemRelation1 = new WorkItemRelation();
      workItemRelation1.Rel = relation.Rel;
      workItemRelation1.Url = relation.Url;
      workItemRelation1.Title = relation.Title;
      workItemRelation1.Attributes = (IDictionary<string, object>) new Dictionary<string, object>();
      WorkItemRelation workItemRelation2 = workItemRelation1;
      if (relation.Attributes != null)
      {
        foreach (KeyValuePair<string, object> attribute in (IEnumerable<KeyValuePair<string, object>>) relation.Attributes)
          workItemRelation2.Attributes.Add(attribute.Key, attribute.Value);
      }
      return workItemRelation2;
    }

    private WorkItemRelation CloneSharedStepRelation(
      string projectName,
      int oldTcID,
      WorkItemRelation relation,
      bool suppressNotifications)
    {
      (int targetId, Guid? remoteHostId) target;
      if (!relation.TryGetTarget(out target))
        return (WorkItemRelation) null;
      WorkItemRelation workItemRelation = this.CloneRelation(relation);
      this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "WITCreator: Found a SharedStepLink from {0} to {1}", (object) oldTcID, (object) relation.Url);
      string empty = string.Empty;
      int num = 0;
      int id;
      if (this.sharedStepCloneCache.ContainsKey(target.targetId))
      {
        id = this.sharedStepCloneCache[target.targetId];
        this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "WITCreator: SharedStep {0} already cloned to {1}", (object) relation.Url, (object) id);
      }
      else if (this.sharedStepCloneCache.ContainsValue(target.targetId))
      {
        this.RequestContext.Trace(0, TraceLevel.Warning, "TestManagement", "BusinessLayer", "WITCreator: SharedStep {0} is actually a clone created in this operation. We will not clone it again");
        id = target.targetId;
      }
      else
      {
        int targetId;
        if (this.itemsUnderCloning.Contains(target.targetId))
        {
          this.RequestContext.Trace(0, TraceLevel.Warning, "TestManagement", "BusinessLayer", "WITCreator: SharedStep {0} under cloning. Backing off and leaving link as is.", (object) target.targetId, (object) num);
          targetId = target.targetId;
          return workItemRelation;
        }
        if (this.itemsUnderCloning.Count > 20)
        {
          this.RequestContext.Trace(0, TraceLevel.Warning, "TestManagement", "BusinessLayer", "WITCreator: Recursion getting too deep. Bailing out");
          targetId = target.targetId;
          return workItemRelation;
        }
        id = this.CloneSharedStep(projectName, target.targetId, suppressNotifications);
      }
      WorkItem workItem = this.RequestContext.GetService<IWorkItemRemotableService>().GetWorkItem(this.RequestContext, id, (IEnumerable<string>) new List<string>());
      workItemRelation.Url = workItem.Url;
      return workItemRelation;
    }

    private IDictionary<string, object> GetCustomizedFieldUpdates(
      WorkItem wi,
      int oldId,
      bool isTestCase,
      bool insertFieldIfAbsent = true,
      bool includeAttachments = true)
    {
      IDictionary<string, object> cloneableFieldData = (IDictionary<string, object>) this.GetCloneableFieldData(wi);
      this.CustomizeFields(cloneableFieldData, oldId, isTestCase, insertFieldIfAbsent);
      return (IDictionary<string, object>) cloneableFieldData.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key.ToString((IFormatProvider) CultureInfo.CurrentCulture)), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value));
    }

    private void CustomizeFields(
      IDictionary<string, object> oldTCFields,
      int oldId,
      bool isTestCase,
      bool insertFieldIfAbsent = true)
    {
      this.CustomizeExplicitlyOverriddenFields(oldTCFields, insertFieldIfAbsent);
      this.CustomizeWorkitemType(oldTCFields, isTestCase);
      this.UpdateSpecialFields(oldId, oldTCFields);
      this.UpdateIdentityFields(oldTCFields);
      WorkItemField2 field = this.FieldsService.GetField(this.RequestContext, WorkItemFieldNames.Actions, (string) null);
      if (!oldTCFields.ContainsKey(field.ReferenceName))
        return;
      using (StringReader inStream = new StringReader((string) oldTCFields[field.ReferenceName]))
      {
        using (StringWriter outStream = new StringWriter())
        {
          this.FixSSIdInXml((TextReader) inStream, "//compref/@ref", (TextWriter) outStream);
          oldTCFields[field.ReferenceName] = (object) outStream.ToString();
        }
      }
    }

    private void CustomizeWorkitemType(IDictionary<string, object> oldTCFields, bool isTestCase)
    {
      if (!string.IsNullOrEmpty(this.options.DestinationWorkItemType) & isTestCase)
      {
        oldTCFields[WorkItemFieldRefNames.WorkItemType] = (object) this.options.DestinationWorkItemType;
      }
      else
      {
        string oldtype = oldTCFields[WorkItemFieldRefNames.WorkItemType] as string;
        if (isTestCase)
        {
          if (this.m_testCaseCategory.WorkItemTypeNames.Any<string>((Func<string, bool>) (n => string.Equals(n, oldtype, StringComparison.OrdinalIgnoreCase))))
            return;
          oldTCFields[WorkItemFieldRefNames.WorkItemType] = (object) this.m_testCaseCategory.DefaultWorkItemTypeName;
        }
        else
        {
          if (this.m_sharedStepCategory.WorkItemTypeNames.Any<string>((Func<string, bool>) (n => string.Equals(n, oldtype, StringComparison.OrdinalIgnoreCase))))
            return;
          oldTCFields[WorkItemFieldRefNames.WorkItemType] = (object) this.m_sharedStepCategory.DefaultWorkItemTypeName;
        }
      }
    }

    public int CloneSharedStep(string projectName, int srcSSId, bool suppressNotifications)
    {
      if (this.opId == 0)
        throw new InvalidOperationException(ServerResources.TestCaseClonerNotInitialized);
      int num = -1;
      try
      {
        this.itemsUnderCloning.Add(srcSSId);
        IWorkItemRemotableService service = this.RequestContext.GetService<IWorkItemRemotableService>();
        WorkItem workItem = service.GetWorkItem(this.RequestContext, srcSSId, (IEnumerable<string>) new List<string>(), expand: WorkItemExpand.All);
        JsonPatchDocument jsonPatchDocument = WorkHelper.ConvertToJsonPatchDocument(this.RequestContext, this.GetCustomizedFieldUpdates(workItem, srcSSId, false, false), this.GetCustomizedRelations(projectName, workItem, srcSSId, false));
        string workItemtype = workItem.Fields[CoreFieldReferenceNames.WorkItemType].ToString();
        int? id = service.CreateWorkItem(this.RequestContext, projectName, workItemtype, jsonPatchDocument, bypassRules: true, suppressNotifications: suppressNotifications).Id;
        num = id.HasValue ? id.Value : -1;
        this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "WITCreator: SharedStep {0} now cloned to {1}", (object) srcSSId, (object) num);
        this.sharedStepCloneCache.Add(srcSSId, num);
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(this.RequestContext))
          planningDatabase.UpdateCloneRelationship(this.opId, new Dictionary<int, int>()
          {
            {
              srcSSId,
              num
            }
          }, CloneItemType.SharedStep);
      }
      finally
      {
        this.itemsUnderCloning.Remove(srcSSId);
      }
      return num;
    }

    internal AttachmentsHelper AttachmentsHelper
    {
      get
      {
        if (this.m_attachmentsHelper == null)
          this.m_attachmentsHelper = new AttachmentsHelper((TestManagementRequestContext) this.tcmRequestContext);
        return this.m_attachmentsHelper;
      }
      set => this.m_attachmentsHelper = value;
    }

    internal AfnStripsHelper AfnStripsHelper
    {
      get
      {
        if (this.m_afnStripsHelper == null)
          this.m_afnStripsHelper = new AfnStripsHelper(this.tcmRequestContext);
        return this.m_afnStripsHelper;
      }
      set => this.m_afnStripsHelper = value;
    }
  }
}
