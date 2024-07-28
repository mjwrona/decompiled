// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ArtifactHandler
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class ArtifactHandler
  {
    internal static Artifact[] GetArtifacts(
      TestManagementRequestContext context,
      string[] artifactUris)
    {
      ArgumentUtility.CheckForNull<string[]>(artifactUris, nameof (artifactUris), context.RequestContext.ServiceName);
      Artifact[] artifacts = new Artifact[artifactUris.Length];
      TestRunArtifactInfo info = new TestRunArtifactInfo();
      for (int index = 0; index < artifactUris.Length; ++index)
      {
        ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUris[index]);
        if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "TcmResult"))
          artifacts[index] = ArtifactHandler.GetResultOrSessionArtifact(context, artifactUris[index], ref info, artifactId);
        else if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "TcmResultAttachment"))
          artifacts[index] = ArtifactHandler.GetAttachmentArtifact(context, artifactUris[index], artifactId);
      }
      return artifacts;
    }

    private static Artifact GetAttachmentArtifact(
      TestManagementRequestContext context,
      string artifactUri,
      ArtifactId id)
    {
      Artifact attachmentArtifact = (Artifact) null;
      int attachmentId;
      if (ArtifactHelper.ParseAttachmentId(id.ToolSpecificId, out attachmentId))
      {
        TestResultAttachment resultAttachment = (TestResultAttachment) null;
        Guid? nullable = new Guid?();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          nullable = managementDatabase.GetProjectForAttachment(attachmentId);
        ProjectInfo projectInfo = new ProjectInfo();
        if (nullable.HasValue)
          projectInfo = context.ProjectServiceHelper.GetProjectFromGuid(nullable.Value);
        bool flag = context.SecurityManager.HasViewTestResultsPermission(context, projectInfo.Uri);
        attachmentArtifact = new Artifact()
        {
          ExternalId = attachmentId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
          Uri = artifactUri
        };
        try
        {
          if (flag)
          {
            List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
            string areaUri;
            using (context.RequestContext.AcquireExemptionLock())
            {
              using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
                resultAttachmentList = managementDatabase.QueryAttachments(context, projectInfo.Id, attachmentId, false, out areaUri);
            }
            if (!context.SecurityManager.CanViewTestResult(context, areaUri))
              resultAttachmentList = new List<TestResultAttachment>();
            if (resultAttachmentList != null)
            {
              if (resultAttachmentList.Any<TestResultAttachment>())
              {
                TestResultAttachment.SignAttachmentObjects(context.RequestContext, resultAttachmentList);
                resultAttachment = resultAttachmentList[0];
                context.TraceAndDebugAssert("BusinessLayer", resultAttachmentList.Count == 1, "DB returned wrong number of attachments");
                context.TraceAndDebugAssert("BusinessLayer", resultAttachment.Id == attachmentId, "DB returned wrong attachment");
              }
            }
          }
        }
        catch (TestObjectNotFoundException ex)
        {
          context.TraceWarning("BusinessLayer", "Attachment {0} not found. Project {1}", (object) attachmentId, (object) projectInfo);
        }
        if (resultAttachment == null)
        {
          int num1 = 0;
          int num2 = 0;
          attachmentArtifact.ArtifactTitle = string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeletedAttachmentArtifact, (object) num1, (object) num2);
        }
        else
          attachmentArtifact.ArtifactTitle = !resultAttachment.IsComplete ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.IncompleteAttachmentTitle, (object) resultAttachment.FileName) : resultAttachment.FileName;
      }
      return attachmentArtifact;
    }

    internal static Artifact[] GetArtifacts(
      TestManagementRequestContext context,
      string[] artifactUris,
      AttachmentsHelper attachmentsHelper,
      Guid projectGuid)
    {
      ArgumentUtility.CheckForNull<string[]>(artifactUris, nameof (artifactUris), context.RequestContext.ServiceName);
      Artifact[] artifacts = new Artifact[artifactUris.Length];
      TestRunArtifactInfo info = new TestRunArtifactInfo();
      for (int index = 0; index < artifactUris.Length; ++index)
      {
        ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUris[index]);
        if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "TcmResult"))
          artifacts[index] = ArtifactHandler.GetResultOrSessionArtifact(context, artifactUris[index], ref info, artifactId);
        else if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "TcmResultAttachment"))
          artifacts[index] = ArtifactHandler.GetAttachmentArtifact(context, artifactUris[index], artifactId, attachmentsHelper, projectGuid);
      }
      return artifacts;
    }

    private static Artifact GetAttachmentArtifact(
      TestManagementRequestContext context,
      string artifactUri,
      ArtifactId id,
      AttachmentsHelper attachmentsHelper,
      Guid projectGuid)
    {
      Artifact attachmentArtifact = (Artifact) null;
      int attachmentId;
      if (ArtifactHelper.ParseAttachmentId(id.ToolSpecificId, out attachmentId))
      {
        string str = (string) null;
        ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(projectGuid);
        int num = context.SecurityManager.HasViewTestResultsPermission(context, projectFromGuid.Uri) ? 1 : 0;
        attachmentArtifact = new Artifact()
        {
          ExternalId = attachmentId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
          Uri = artifactUri
        };
        if (num != 0)
          str = attachmentsHelper.GetAttachmentName(projectFromGuid.Name, attachmentId);
        attachmentArtifact.ArtifactTitle = !string.IsNullOrEmpty(str) ? str : string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeletedAttachmentArtifact, (object) 0, (object) 0);
      }
      return attachmentArtifact;
    }

    private static Artifact GetResultOrSessionArtifact(
      TestManagementRequestContext context,
      string artifactUri,
      ref TestRunArtifactInfo info,
      ArtifactId artifactId)
    {
      Artifact orSessionArtifact = (Artifact) null;
      int testRunId;
      int testResultId;
      if (ArtifactHelper.ParseTestCaseResultId(artifactId.ToolSpecificId, out testRunId, out testResultId))
      {
        orSessionArtifact = new Artifact()
        {
          Uri = artifactUri
        };
        TestCaseResult artifact = TestCaseResult.FindArtifact(context, testRunId, testResultId, ref info);
        orSessionArtifact.ArtifactTitle = artifact == null ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeletedResultArtifact, (object) testRunId, (object) testResultId) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.ArtifactTitle, (object) artifact.TestCaseTitle, (object) testRunId);
      }
      else
      {
        int sessionId;
        if (ArtifactHelper.ParseSessionId(new Uri(artifactUri), out sessionId))
        {
          orSessionArtifact = new Artifact()
          {
            Uri = artifactUri
          };
          Session artifact = Session.FindArtifact(context, sessionId, ref info);
          orSessionArtifact.ArtifactTitle = artifact == null ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeletedSessionArtifactTitle, (object) sessionId) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.SessionArtifactTitle, (object) artifact.Title, (object) sessionId);
        }
      }
      return orSessionArtifact;
    }
  }
}
