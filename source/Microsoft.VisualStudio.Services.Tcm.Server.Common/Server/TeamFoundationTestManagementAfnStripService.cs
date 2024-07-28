// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementAfnStripService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementAfnStripService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestManagementAfnStripService,
    IVssFrameworkService
  {
    public TeamFoundationTestManagementAfnStripService()
    {
    }

    public TeamFoundationTestManagementAfnStripService(TestManagementRequestContext context)
      : base(context)
    {
    }

    public AfnStrip CreateAfnStrip(
      TestManagementRequestContext context,
      Guid projectId,
      AfnStrip afnStrip)
    {
      TeamFoundationTestManagementAfnStripService.ValidateProperties(afnStrip);
      this.CheckForViewTestResultPermission(context, projectId);
      Guid userId = context.RequestContext.GetUserId();
      long streamLength;
      int tfsFileId = TeamFoundationTestManagementAfnStripService.UploadAfnStrip(context, projectId, afnStrip.Stream, out streamLength);
      afnStrip.UnCompressedStreamLength = streamLength;
      return this.CreateAttachmentRecord(context, projectId, afnStrip, userId, tfsFileId);
    }

    public AfnStrip CreateAfnStrip(
      TestManagementRequestContext context,
      Guid projectId,
      int testCaseId,
      Stream stream)
    {
      ArgumentUtility.CheckForNonnegativeInt(testCaseId, nameof (testCaseId), "Test Results");
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream), "Test Results");
      this.CheckForViewTestResultPermission(context, projectId);
      Guid userId = context.RequestContext.GetUserId();
      int num = TeamFoundationTestManagementAfnStripService.Upload(context, projectId, stream);
      TestManagementRequestContext context1 = context;
      Guid projectId1 = projectId;
      AfnStrip afnStrip = new AfnStrip();
      afnStrip.TestCaseId = testCaseId;
      afnStrip.UnCompressedStreamLength = stream.Length;
      Guid createdBy = userId;
      int tfsFileId = num;
      return this.CreateAttachmentRecord(context1, projectId1, afnStrip, createdBy, tfsFileId);
    }

    public AfnStrip GetDefaultAfnStrip(
      TestManagementRequestContext context,
      Guid projectId,
      int testCaseId)
    {
      ArgumentUtility.CheckForNonnegativeInt(testCaseId, nameof (testCaseId), "Test Results");
      this.CheckForViewTestResultPermission(context, projectId);
      TestResultAttachment resultAttachment;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        resultAttachment = managementDatabase.QueryDefaultStrip(testCaseId, projectId, out string _);
      if (resultAttachment == null)
        return (AfnStrip) null;
      TestResultAttachment.SignAttachmentObjects(context.RequestContext, new List<TestResultAttachment>()
      {
        resultAttachment
      });
      return new AfnStrip()
      {
        Id = resultAttachment.Id,
        TestCaseId = testCaseId,
        TestRunId = resultAttachment.TestRunId,
        TestResultId = resultAttachment.TestResultId,
        CreationDate = resultAttachment.CreationDate,
        FileName = resultAttachment.FileName,
        Url = resultAttachment.DownloadQueryString,
        Project = projectId.ToString()
      };
    }

    public List<AfnStrip> GetDefaultAfnStrips(
      TestManagementRequestContext context,
      Guid projectId,
      IList<int> testCaseIds)
    {
      ArgumentUtility.CheckForNull<IList<int>>(testCaseIds, nameof (testCaseIds), "Test Results");
      this.CheckForViewTestResultPermission(context, projectId);
      List<AfnStrip> defaultAfnStrips = new List<AfnStrip>();
      foreach (int testCaseId in (IEnumerable<int>) testCaseIds)
      {
        AfnStrip defaultAfnStrip = this.GetDefaultAfnStrip(context, projectId, testCaseId);
        if (defaultAfnStrip != null)
          defaultAfnStrips.Add(defaultAfnStrip);
      }
      return defaultAfnStrips;
    }

    public void UpdateDefaultStrip(
      TestManagementRequestContext context,
      Guid projectId,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding> bindings)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) bindings, nameof (bindings), "Test Results");
      this.CheckForViewTestResultPermission(context, projectId);
      DefaultAfnStripBinding.UpdateAfnStripBindingList(context, bindings.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding, DefaultAfnStripBinding>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding, DefaultAfnStripBinding>) (binding => AfnStripContractConverter.Convert(binding))).ToArray<DefaultAfnStripBinding>(), projectId);
    }

    public void UpdateDefaultStrip(
      TestManagementRequestContext context,
      string projectName,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding> bindings)
    {
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName), "Test Results");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) bindings, nameof (bindings), "Test Results");
      this.CheckForViewTestResultPermission(context, projectName);
      DefaultAfnStripBinding.UpdateAfnStripBindingList(context, bindings.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding, DefaultAfnStripBinding>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding, DefaultAfnStripBinding>) (binding => AfnStripContractConverter.Convert(binding))).ToArray<DefaultAfnStripBinding>(), projectName);
    }

    private static int UploadAfnStrip(
      TestManagementRequestContext context,
      Guid projectId,
      string afnStripStream,
      out long streamLength)
    {
      byte[] decodedStream = TeamFoundationTestManagementAfnStripService.GetDecodedStream(afnStripStream);
      return TeamFoundationTestManagementAfnStripService.Upload(context, projectId, out streamLength, decodedStream);
    }

    private static int Upload(
      TestManagementRequestContext context,
      Guid projectId,
      out long byteStreamLength,
      byte[] buffer)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length, false))
      {
        memoryStream.Seek(0L, SeekOrigin.Begin);
        byteStreamLength = memoryStream.Length;
        return TeamFoundationTestManagementAfnStripService.Upload(context, projectId, (Stream) memoryStream);
      }
    }

    private static int Upload(
      TestManagementRequestContext context,
      Guid projectId,
      Stream memoryStream)
    {
      return TestResultAttachment.UploadAttachment(context, projectId, memoryStream);
    }

    private static byte[] GetDecodedStream(string stream)
    {
      try
      {
        return Convert.FromBase64String(stream);
      }
      catch (Exception ex)
      {
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.StreamNotObtained), ex);
      }
    }

    private string GetAttachmentUrl(
      TestManagementRequestContext context,
      Guid projectId,
      AfnStrip afnStrip)
    {
      this.GetProjectReference(context.RequestContext, projectId.ToString());
      RestApiResourceDetails resourceMapping = context.ResourceMappings[ResourceMappingConstants.TestResultAttachmentsV2];
      return UrlBuildHelper.GetResourceUrl(context.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
      {
        runId = afnStrip.TestRunId,
        project = projectId.ToString(),
        testCaseResultId = afnStrip.TestResultId,
        attachmentId = afnStrip.Id
      });
    }

    private static void ValidateProperties(AfnStrip afnStrip)
    {
      ArgumentUtility.CheckForNull<AfnStrip>(afnStrip, nameof (afnStrip), "Test Results");
      ArgumentUtility.CheckForNonnegativeInt(afnStrip.TestCaseId, "TestCaseId", "Test Results");
      ArgumentUtility.CheckForNull<string>(afnStrip.Stream, "Stream", "Test Results");
    }

    private AfnStrip CreateAttachmentRecord(
      TestManagementRequestContext context,
      Guid projectId,
      AfnStrip afnStrip,
      Guid createdBy,
      int tfsFileId)
    {
      AfnStrip afnStrip1 = (AfnStrip) null;
      bool changeCounterInterval = ServiceMigrationHelper.ShouldChangeCounterInterval(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        afnStrip1 = managementDatabase.CreateAfnStrip(projectId, tfsFileId, afnStrip, createdBy, changeCounterInterval);
      afnStrip1.Url = this.GetAttachmentUrl(context, projectId, afnStrip1);
      return afnStrip1;
    }
  }
}
