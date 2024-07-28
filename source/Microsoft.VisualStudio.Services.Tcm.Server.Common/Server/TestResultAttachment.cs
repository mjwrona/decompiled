// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestResultAttachment : ISignable
  {
    private int m_id;
    private string m_fileName;
    private string m_comment;
    private DateTime m_creationDate;
    private long m_length;
    private int m_iterationId;
    private string m_actionPath;
    private int m_subResultId;
    private string m_attachmentType;
    private int m_testRunId;
    private int m_sessionId;
    private int m_testResultId;
    private bool m_isComplete = true;
    private int m_fileId;
    private string m_queryString;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(SqlFieldName = "AttachmentId")]
    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string FileName
    {
      get => this.m_fileName;
      set => this.m_fileName = value;
    }

    [XmlElement]
    [ClientProperty(ClientVisibility.Private)]
    public string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public string AttachmentType
    {
      get => this.m_attachmentType;
      set => this.m_attachmentType = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [DefaultValue(0)]
    [QueryMapping]
    public int TestRunId
    {
      get => this.m_testRunId;
      set => this.m_testRunId = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int TestResultId
    {
      get => this.m_testResultId;
      set => this.m_testResultId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public DateTime CreationDate
    {
      get => this.m_creationDate;
      set => this.m_creationDate = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping(SqlFieldName = "UncompressedLength")]
    public long Length
    {
      get => this.m_length;
      set => this.m_length = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int IterationId
    {
      get => this.m_iterationId;
      set => this.m_iterationId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string ActionPath
    {
      get => this.m_actionPath;
      set => this.m_actionPath = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int SubResultId
    {
      get => this.m_subResultId;
      set => this.m_subResultId = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public int SessionId
    {
      get => this.m_sessionId;
      set => this.m_sessionId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [DefaultValue(true)]
    [QueryMapping]
    public bool IsComplete
    {
      get => this.m_isComplete;
      set => this.m_isComplete = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [QueryMapping]
    public Guid TmiRunId { get; set; }

    [XmlIgnore]
    [QueryMapping(EnumType = typeof (TestOutcome))]
    internal byte ResultOutcome { get; set; }

    [XmlIgnore]
    internal string AreaUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string DownloadQueryString
    {
      get => this.m_queryString;
      set => this.m_queryString = value;
    }

    internal int FileId
    {
      get => this.m_fileId;
      set => this.m_fileId = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestResultAttachment Id={0} FileName={1}", (object) this.m_id, (object) this.m_fileName);

    public int GetDownloadUrlCount() => 1;

    public int GetFileId(int index) => this.m_fileId;

    public byte[] GetHashValue(int index) => (byte[]) null;

    public void SetDownloadUrl(int index, string downloadUrl) => this.m_queryString = downloadUrl;

    internal static int[] Create(
      TestManagementRequestContext context,
      TestResultAttachment[] attachments,
      string projectName,
      bool areSessionAttachments,
      bool isFeedBackSession = false,
      bool isAttachmentUploadComplete = false)
    {
      if (attachments == null || attachments.Length == 0)
        return (int[]) null;
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!isFeedBackSession)
        context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      List<int> intList = new List<int>();
      bool changeCounterInterval = ServiceMigrationHelper.ShouldChangeCounterInterval(context.RequestContext);
      if (!isAttachmentUploadComplete)
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        {
          foreach (List<int> collection in managementDatabase.CreateAttachments(projectFromName.GuidId, (IEnumerable<TestResultAttachment>) attachments, areSessionAttachments, changeCounterInterval).Values)
            intList.AddRange((IEnumerable<int>) collection);
          intList.Sort();
          return intList.ToArray();
        }
      }
      else
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        {
          foreach (List<int> collection in managementDatabase.CreateAttachmentsWithFileId(projectFromName.GuidId, (IEnumerable<TestResultAttachment>) attachments, changeCounterInterval).Values)
            intList.AddRange((IEnumerable<int>) collection);
          intList.Sort();
          return intList.ToArray();
        }
      }
    }

    internal static List<TestResultAttachment> CreateAttachmentIdMappingsForLogStore(
      TestManagementRequestContext context,
      string projectId,
      List<TestResultAttachment> attachments)
    {
      if (attachments == null || attachments.Count == 0)
        return new List<TestResultAttachment>();
      Guid projectId1 = new Guid(projectId);
      List<TestResultAttachment> mappingsForLogStore = new List<TestResultAttachment>();
      try
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          mappingsForLogStore = managementDatabase.CreateLogStoreTestAttachmentIdMappings(projectId1, (IEnumerable<TestResultAttachment>) attachments);
      }
      catch (Exception ex)
      {
        context.TraceError("AttachmentHandler", "AttachmentDatabase.CreateLogStoreTestAttachmentIdMappings threw exception - ProjectId: {0}, Exception message: {1}", (object) projectId, (object) ex.Message);
      }
      return mappingsForLogStore;
    }

    internal static TestResultAttachment GetAttachmentIdMappingForLogStore(
      TestManagementRequestContext context,
      string projectId,
      TestResultAttachment attachment)
    {
      TestResultAttachment mappingForLogStore = (TestResultAttachment) null;
      if (attachment == null)
        return mappingForLogStore;
      Guid projectId1 = new Guid(projectId);
      try
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          mappingForLogStore = managementDatabase.GetLogStoreTestAttachmentIdMapping(projectId1, attachment.TestRunId, attachment.TestResultId, attachment.SubResultId, attachment.Id);
      }
      catch (Exception ex)
      {
        context.TraceError("AttachmentHandler", "AttachmentDatabase.GetLogStoreTestAttachmentIdMapping threw exception - ProjectId: {0}, RunId: {1}, ResultId: {2}, Exception message: {3}", (object) projectId, (object) attachment.TestRunId, (object) attachment.TestResultId, (object) ex.Message);
      }
      return mappingForLogStore;
    }

    internal static int[] CreateAttachmentInLogStoreMapper(
      TestManagementRequestContext context,
      TestResultAttachment[] attachments,
      string projectName)
    {
      if (attachments == null || attachments.Length == 0)
        return (int[]) null;
      string str1 = string.Join<int>(", ", (IEnumerable<int>) ((IEnumerable<TestResultAttachment>) attachments).Select<TestResultAttachment, int>((Func<TestResultAttachment, int>) (a => a.TestRunId)).ToList<int>());
      string str2 = string.Join<int>(", ", (IEnumerable<int>) ((IEnumerable<TestResultAttachment>) attachments).Select<TestResultAttachment, int>((Func<TestResultAttachment, int>) (a => a.TestResultId)).ToList<int>());
      context.RequestContext.Trace(1015852, TraceLevel.Info, "TestManagement", "AttachmentHandler", "TestResultAttachment.CreateAttachmentInLogStoreMapper projectName = {0}, runIds = {1}, resultIds = {2}", (object) projectName, (object) str1, (object) str2);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      List<int> intList = new List<int>();
      try
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        {
          intList = managementDatabase.CreateTestAttachmentsInLogStoreMapper(projectFromName.GuidId, (IEnumerable<TestResultAttachment>) attachments);
          intList.Sort();
        }
      }
      catch (Exception ex)
      {
        context.TraceError("AttachmentHandler", "AttachmentDatabase.CreateAttachmentInLogStoreMapper threw excpetion - ProjectId: {0}, RunIds: {1}, ResultIds: {2}, Exception message: {3}", (object) projectFromName.String, (object) str1, (object) str2, (object) ex.Message);
      }
      return intList.ToArray();
    }

    internal static bool Delete(
      TestManagementRequestContext context,
      TestResultAttachmentIdentity[] attachments,
      string projectName)
    {
      if (attachments == null || attachments.Length == 0)
        return false;
      string str1 = string.Join<int>(", ", (IEnumerable<int>) ((IEnumerable<TestResultAttachmentIdentity>) attachments).Select<TestResultAttachmentIdentity, int>((Func<TestResultAttachmentIdentity, int>) (a => a.AttachmentId)).ToList<int>());
      string str2 = string.Join<int>(", ", (IEnumerable<int>) ((IEnumerable<TestResultAttachmentIdentity>) attachments).Select<TestResultAttachmentIdentity, int>((Func<TestResultAttachmentIdentity, int>) (a => a.TestRunId)).ToList<int>());
      string str3 = string.Join<int>(", ", (IEnumerable<int>) ((IEnumerable<TestResultAttachmentIdentity>) attachments).Select<TestResultAttachmentIdentity, int>((Func<TestResultAttachmentIdentity, int>) (a => a.TestResultId)).ToList<int>());
      context.RequestContext.Trace(1015851, TraceLevel.Info, "TestManagement", "DeleteAttachment", "TestResultAttachment.Delete projectName = {0}, attachments = {1}, runIds = {2}, resultIds = {3}", (object) projectName, (object) str1, (object) str2, (object) str3);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      try
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          managementDatabase.DeleteAttachments(projectFromName.GuidId, (IEnumerable<TestResultAttachmentIdentity>) attachments);
      }
      catch (Exception ex)
      {
        context.TraceError("AttachmentHandler", "AttachmentDatabase.Delete threw excpetion - ProjectId: {0}, AttachmentIds: {1}, RunIds: {2}, ResultIds: {3}, Exception message: {4}", (object) projectFromName.String, (object) str1, (object) str2, (object) str3, (object) ex.Message);
        return false;
      }
      return true;
    }

    internal static List<string> DeleteAttachmentsFromLogStoreMapper(
      TestManagementRequestContext context,
      TestResultAttachmentIdentity[] attachments,
      string projectName)
    {
      if (attachments == null || attachments.Length == 0)
        return new List<string>();
      string str1 = string.Join<int>(", ", (IEnumerable<int>) ((IEnumerable<TestResultAttachmentIdentity>) attachments).Select<TestResultAttachmentIdentity, int>((Func<TestResultAttachmentIdentity, int>) (a => a.AttachmentId)).ToList<int>());
      string str2 = string.Join<int>(", ", (IEnumerable<int>) ((IEnumerable<TestResultAttachmentIdentity>) attachments).Select<TestResultAttachmentIdentity, int>((Func<TestResultAttachmentIdentity, int>) (a => a.TestRunId)).ToList<int>());
      string str3 = string.Join<int>(", ", (IEnumerable<int>) ((IEnumerable<TestResultAttachmentIdentity>) attachments).Select<TestResultAttachmentIdentity, int>((Func<TestResultAttachmentIdentity, int>) (a => a.TestResultId)).ToList<int>());
      context.RequestContext.Trace(1015851, TraceLevel.Info, "TestManagement", "DeleteAttachment", "TestResultAttachment.DeleteAttachmentsFromLogStoreMapper projectName = {0}, attachments = {1}, runIds = {2}, resultIds = {3}", (object) projectName, (object) str1, (object) str2, (object) str3);
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      context.SecurityManager.CheckPublishTestResultsPermission(context, projectFromName.String);
      List<string> stringList = new List<string>();
      try
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          stringList = managementDatabase.DeleteAttachmentsFromLogStoreMapper(projectFromName.GuidId, (IEnumerable<TestResultAttachmentIdentity>) attachments);
      }
      catch (Exception ex)
      {
        context.TraceError("AttachmentHandler", "AttachmentDatabase.DeleteAttachmentsFromLogStoreMapper threw excpetion - ProjectId: {0}, AttachmentIds: {1}, RunIds: {2}, ResultIds: {3}, Exception message: {4}", (object) projectFromName.String, (object) str1, (object) str2, (object) str3, (object) ex.Message);
      }
      return stringList;
    }

    internal static List<TestResultAttachment> Query(
      TestManagementRequestContext context,
      string projectName,
      int attachmentId,
      bool getSiblingAttachments)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      return TestResultAttachment.Query(context, projectFromName, attachmentId, getSiblingAttachments);
    }

    internal static List<TestResultAttachment> Query(
      TestManagementRequestContext context,
      Guid projectId,
      int attachmentId,
      bool getSiblingAttachments)
    {
      string projectUriFromId = Validator.CheckAndGetProjectUriFromId(context, projectId);
      return TestResultAttachment.Query(context, new GuidAndString(projectUriFromId, projectId), attachmentId, getSiblingAttachments);
    }

    private static List<TestResultAttachment> Query(
      TestManagementRequestContext context,
      GuidAndString project,
      int attachmentId,
      bool getSiblingAttachments)
    {
      List<TestResultAttachment> attachments = new List<TestResultAttachment>();
      if (!context.SecurityManager.HasViewTestResultsPermission(context, project.String))
        return attachments;
      string areaUri;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        attachments = managementDatabase.QueryAttachments(context, project.GuidId, attachmentId, getSiblingAttachments, out areaUri);
      if (!context.SecurityManager.CanViewTestResult(context, areaUri))
        return new List<TestResultAttachment>();
      TestResultAttachment.SignAttachmentObjects(context.RequestContext, attachments);
      return attachments;
    }

    internal static List<TestResultAttachment> Query(
      TestManagementRequestContext context,
      int testRunId,
      int testResultId,
      int sessionId,
      string projectName,
      int subResultId = 0,
      int attachmentId = 0)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestResultAttachment.Query"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        return TestResultAttachment.Query(context, testRunId, testResultId, sessionId, projectFromName, subResultId, attachmentId);
      }
    }

    internal static List<TestResultAttachment> Query(
      TestManagementRequestContext context,
      int testRunId,
      int testResultId,
      int sessionId,
      Guid projectId,
      int subResultId = 0,
      int attachmentId = 0)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestResultAttachment.Query"))
      {
        string projectUriFromId = Validator.CheckAndGetProjectUriFromId(context, projectId);
        return TestResultAttachment.Query(context, testRunId, testResultId, sessionId, new GuidAndString(projectUriFromId, projectId), subResultId, attachmentId);
      }
    }

    private static List<TestResultAttachment> Query(
      TestManagementRequestContext context,
      int testRunId,
      int testResultId,
      int sessionId,
      GuidAndString project,
      int subResultId = 0,
      int attachmentId = 0)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestResultAttachment.Query"))
      {
        List<TestResultAttachment> attachments = new List<TestResultAttachment>();
        if (!context.SecurityManager.HasViewTestResultsPermission(context, project.String))
          return attachments;
        string areaUri;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          attachments = managementDatabase.QueryAttachments(context, project.GuidId, testRunId, testResultId, attachmentId, sessionId, out areaUri, subResultId);
        if (testResultId > 0 && !context.SecurityManager.CanViewTestResult(context, areaUri))
          return new List<TestResultAttachment>();
        TestResultAttachment.SignAttachmentObjects(context.RequestContext, attachments);
        return attachments;
      }
    }

    internal static List<TestResultAttachment> Query2(
      TestManagementRequestContext context,
      ResultsStoreQuery query)
    {
      ArgumentUtility.CheckForNull<ResultsStoreQuery>(query, nameof (query), "Test Results");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, query.TeamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestResultAttachment>();
      QueryEngine queryEngine = (QueryEngine) new ServerQueryEngine(context, query);
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      if (context.IsFeatureEnabled("TestManagement.Server.UseStaticSprocInsteadOfDynamic"))
      {
        Dictionary<string, List<object>> parametersMap = new TcmQueryParser(query.QueryText).GetParametersMap();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          resultAttachmentList = managementDatabase.QueryAttachments3(projectFromName.GuidId, parametersMap);
      }
      else
      {
        int lazyInitialization;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          lazyInitialization = managementDatabase.GetDataspaceIdWithLazyInitialization(projectFromName.GuidId);
        string whereClause = queryEngine.GenerateWhereClause(lazyInitialization);
        string orderClause = queryEngine.GenerateOrderClause();
        List<KeyValuePair<int, string>> valueLists = queryEngine.GenerateValueLists();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          resultAttachmentList = managementDatabase.QueryAttachments2(whereClause, orderClause, valueLists);
      }
      if (resultAttachmentList != null && resultAttachmentList.Any<TestResultAttachment>())
      {
        resultAttachmentList = resultAttachmentList.Where<TestResultAttachment>((Func<TestResultAttachment, bool>) (a => context.SecurityManager.CanViewTestResult(context, a.AreaUri))).ToList<TestResultAttachment>();
        TestResultAttachment.SignAttachmentObjects(context.RequestContext, resultAttachmentList);
      }
      return resultAttachmentList;
    }

    internal static List<TestResultAttachment> QueryIterationAttachments(
      TestManagementRequestContext context,
      int testRunId,
      int testResultId,
      string attachmentType,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      List<TestResultAttachment> attachments = new List<TestResultAttachment>();
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return attachments;
      string areaUri;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        attachments = managementDatabase.QueryIterationAttachments(context, projectFromName.GuidId, testRunId, testResultId, attachmentType, out areaUri);
      if (!context.SecurityManager.CanViewTestResult(context, areaUri))
        return new List<TestResultAttachment>();
      TestResultAttachment.SignAttachmentObjects(context.RequestContext, attachments);
      return attachments;
    }

    internal static List<TestResultAttachment> QueryDefaultStrips(
      TestManagementRequestContext context,
      int[] testCaseIds,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<TestResultAttachment>();
      Dictionary<string, List<TestResultAttachment>> source = new Dictionary<string, List<TestResultAttachment>>();
      List<TestResultAttachment> attachments = new List<TestResultAttachment>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        for (int index = 0; index < testCaseIds.Length; ++index)
        {
          string areaUri;
          TestResultAttachment resultAttachment = managementDatabase.QueryDefaultStrip(testCaseIds[index], projectFromName.GuidId, out areaUri);
          if (resultAttachment != null)
          {
            if (string.IsNullOrEmpty(areaUri))
              attachments.Add(resultAttachment);
            else if (source.ContainsKey(areaUri))
              source[areaUri].Add(resultAttachment);
            else
              source[areaUri] = new List<TestResultAttachment>()
              {
                resultAttachment
              };
          }
        }
      }
      if (source != null && source.Any<KeyValuePair<string, List<TestResultAttachment>>>())
      {
        foreach (KeyValuePair<string, List<TestResultAttachment>> keyValuePair in source)
        {
          if (context.SecurityManager.CanViewTestResult(context, keyValuePair.Key))
            attachments.AddRange((IEnumerable<TestResultAttachment>) keyValuePair.Value);
        }
      }
      TestResultAttachment.SignAttachmentObjects(context.RequestContext, attachments);
      return attachments;
    }

    internal static bool AppendAttachment(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int attachmentId,
      int sessionId,
      long uncompressedLength,
      long compressedLength,
      byte[] hashValue,
      CompressionType compressionType,
      long offsetFrom,
      byte[] content,
      int contentLength,
      int defaultAfnStripFlag)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (AppendAttachment), "File")))
      {
        int fileId = 0;
        if (offsetFrom != 0L)
        {
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            fileId = managementDatabase.GetAttachmentTfsFileId(context, projectId, testRunId, testResultId, sessionId, attachmentId);
          if (fileId == 0)
          {
            TestResultAttachment.LogAttachmentUploadError(context, projectId, testRunId, testResultId, attachmentId, sessionId, uncompressedLength, compressedLength, offsetFrom, contentLength);
            throw new TestObjectNotFoundException(context.RequestContext, attachmentId, ObjectTypes.Attachment);
          }
        }
        ITeamFoundationFileService service = context.RequestContext.GetService<ITeamFoundationFileService>();
        bool isFileUploadComplete;
        try
        {
          isFileUploadComplete = service.UploadFile(context.RequestContext, ref fileId, hashValue, uncompressedLength, compressedLength, compressionType, offsetFrom, content, contentLength, OwnerId.TeamTest, projectId, (string) null);
        }
        catch (FileAlreadyUploadedException ex)
        {
          return false;
        }
        catch (FileIdNotFoundException ex1)
        {
          if (fileId != 0)
          {
            try
            {
              isFileUploadComplete = service.UploadFile(context.RequestContext, ref fileId, hashValue, uncompressedLength, compressedLength, compressionType, offsetFrom, content, contentLength, OwnerId.TeamTest, Guid.Empty, (string) null);
            }
            catch (FileAlreadyUploadedException ex2)
            {
              return false;
            }
            catch (FileIdNotFoundException ex3)
            {
              TestResultAttachment.LogAttachmentUploadError(context, projectId, testRunId, testResultId, attachmentId, sessionId, uncompressedLength, compressedLength, offsetFrom, contentLength);
              throw new TestObjectNotFoundException(context.RequestContext, attachmentId, ObjectTypes.Attachment);
            }
            catch (Exception ex4)
            {
              TestResultAttachment.LogAttachmentUploadError(context, projectId, testRunId, testResultId, attachmentId, sessionId, uncompressedLength, compressedLength, offsetFrom, contentLength);
              throw;
            }
          }
          else
          {
            TestResultAttachment.LogAttachmentUploadError(context, projectId, testRunId, testResultId, attachmentId, sessionId, uncompressedLength, compressedLength, offsetFrom, contentLength);
            throw new TestObjectNotFoundException(context.RequestContext, attachmentId, ObjectTypes.Attachment);
          }
        }
        catch (Exception ex)
        {
          TestResultAttachment.LogAttachmentUploadError(context, projectId, testRunId, testResultId, attachmentId, sessionId, uncompressedLength, compressedLength, offsetFrom, contentLength);
          throw;
        }
        if (fileId == 0)
        {
          TestResultAttachment.LogAttachmentUploadError(context, projectId, testRunId, testResultId, attachmentId, sessionId, uncompressedLength, compressedLength, offsetFrom, contentLength);
          throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidAttachmentFile, (object) attachmentId));
        }
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          return managementDatabase.GetAppendAttachmentStatusForUploadedFile(context, projectId, testRunId, testResultId, sessionId, fileId, attachmentId, isFileUploadComplete, defaultAfnStripFlag, uncompressedLength);
      }
    }

    internal static int UploadAttachment(
      TestManagementRequestContext context,
      Guid projectId,
      Stream content)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName("CreateAttachment", "File")))
      {
        ITeamFoundationFileService service = context.RequestContext.GetService<ITeamFoundationFileService>();
        int num;
        try
        {
          num = service.UploadFile(context.RequestContext, content, OwnerId.TeamTest, projectId);
        }
        catch (Exception ex)
        {
          throw;
        }
        return num != 0 ? num : throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidAttachmentFile, (object) 0));
      }
    }

    private static void LogAttachmentUploadError(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int attachmentId,
      int sessionId,
      long uncompressedLength,
      long compressedLength,
      long offsetFrom,
      int contentLength)
    {
      string str = string.Empty;
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      string areaUri;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        resultAttachmentList = managementDatabase.QueryAttachments(context, projectId, attachmentId, false, out areaUri);
      if (!context.SecurityManager.CanViewTestResult(context, areaUri))
        resultAttachmentList = new List<TestResultAttachment>();
      TestResultAttachment.SignAttachmentObjects(context.RequestContext, resultAttachmentList);
      if (resultAttachmentList != null && resultAttachmentList.Any<TestResultAttachment>())
      {
        TestResultAttachment resultAttachment = resultAttachmentList[0];
        str = string.Format("RunId:{0} ResultId:{1} AttachmentId:{2} SessionId:{3} UnCompressedLength:{4} TfsFileId:{5} FileName:{6} AttachmentType:{7} CreationDate:{8} IsComplete:{9} ", (object) resultAttachment.TestRunId, (object) resultAttachment.TestResultId, (object) resultAttachment.Id, (object) resultAttachment.SessionId, (object) resultAttachment.Length, (object) resultAttachment.FileId, (object) resultAttachment.FileName, (object) resultAttachment.AttachmentType.ToString(), (object) resultAttachment.CreationDate.ToString(), (object) resultAttachment.IsComplete.ToString());
      }
      context.TraceError("AttachmentHandler", "AttachmentDatabase.AppendAttachment - RunId:{0} ResultId:{1} AttachmentId:{2} SessionId:{3} UnCompressedLength:{4} CompressedLength:{5} OffsetFrom:{6}, ContentLength:{7} TcmAttachmentInfo:{8}", (object) testRunId, (object) testResultId, (object) attachmentId, (object) sessionId, (object) uncompressedLength, (object) compressedLength, (object) offsetFrom, (object) contentLength, (object) str);
    }

    internal static List<bool> CheckActionRecordingExists(
      TestManagementRequestContext context,
      int[] testCaseIds,
      Guid projectId)
    {
      string projectUriFromId = Validator.CheckAndGetProjectUriFromId(context, projectId);
      return TestResultAttachment.CheckActionRecordingExists(context, testCaseIds, new GuidAndString(projectUriFromId, projectId));
    }

    internal static List<bool> CheckActionRecordingExists(
      TestManagementRequestContext context,
      int[] testCaseIds,
      string projectName)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      return TestResultAttachment.CheckActionRecordingExists(context, testCaseIds, projectFromName);
    }

    private static List<bool> CheckActionRecordingExists(
      TestManagementRequestContext context,
      int[] testCaseIds,
      GuidAndString project)
    {
      int duplicateTestId = 0;
      if (Validator.TryCheckDuplicateTests(testCaseIds, out duplicateTestId))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateTestCaseId, (object) duplicateTestId));
      if (!context.SecurityManager.HasViewTestResultsPermission(context, project.String))
        return (List<bool>) null;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.CheckActionRecordingExists(testCaseIds);
    }

    internal static void SignAttachmentObjects(
      IVssRequestContext requestContext,
      List<TestResultAttachment> attachments)
    {
      if (attachments == null || !attachments.Any<TestResultAttachment>())
        return;
      long attachmentSizeForProxy = RegistryCache.GetMaxAttachmentSizeForProxy(requestContext);
      using (UrlSigner urlSigner = new UrlSigner(requestContext))
      {
        foreach (TestResultAttachment attachment in attachments)
        {
          if (attachment.Length <= attachmentSizeForProxy)
            urlSigner.SignObject((ISignable) attachment);
        }
        urlSigner.FlushDeferredSignatures();
      }
    }

    internal static Stream GetAttachmentStream(
      TestManagementRequestContext context,
      Guid projectId,
      int attachmentId,
      out string attachmentName,
      out CompressionType compressionType)
    {
      List<TestResultAttachment> source;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        source = managementDatabase.QueryAttachments(context, projectId, attachmentId, false, out string _);
      if (source == null || !source.Any<TestResultAttachment>())
      {
        attachmentName = (string) null;
        compressionType = CompressionType.None;
        return (Stream) null;
      }
      attachmentName = source[0].FileName;
      return TestResultAttachment.GetAttachmentStream(context, source[0].FileId, out compressionType);
    }

    internal static Stream GetAttachmentStream(
      TestManagementRequestContext context,
      int tfsFileId,
      out CompressionType compressionType)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (GetAttachmentStream), "File")))
      {
        TeamFoundationFileService service = context.RequestContext.GetService<TeamFoundationFileService>();
        long num = 0;
        IVssRequestContext requestContext = context.RequestContext;
        long fileId = (long) tfsFileId;
        byte[] numArray;
        ref byte[] local1 = ref numArray;
        ref long local2 = ref num;
        ref CompressionType local3 = ref compressionType;
        return service.RetrieveFile(requestContext, fileId, false, out local1, out local2, out local3);
      }
    }
  }
}
