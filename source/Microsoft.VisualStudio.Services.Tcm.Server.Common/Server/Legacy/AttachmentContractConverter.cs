// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.AttachmentContractConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  public static class AttachmentContractConverter
  {
    internal static IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments)
    {
      return attachments == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) null : attachments.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) (attachment => AttachmentContractConverter.Convert(attachment)));
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment attachment)
    {
      if (attachment == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment()
      {
        Id = attachment.Id,
        FileName = attachment.FileName,
        AttachmentType = Enum.GetName(typeof (AttachmentType), (object) attachment.AttachmentType),
        TestRunId = attachment.TestRunId,
        TestResultId = attachment.TestResultId,
        CreationDate = attachment.CreationDate,
        ActionPath = attachment.ActionPath,
        Length = attachment.Length,
        IterationId = attachment.IterationId,
        TmiRunId = attachment.TmiRunId,
        SessionId = attachment.SessionId,
        Comment = attachment.Comment,
        DownloadQueryString = attachment.DownloadQueryString,
        IsComplete = attachment.IsComplete
      };
    }

    internal static IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment> attachments)
    {
      return attachments == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) null : attachments.Select<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) (attachment => AttachmentContractConverter.Convert(attachment)));
    }

    internal static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment Convert(
      Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment attachment)
    {
      if (attachment == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment) null;
      AttachmentType result;
      Enum.TryParse<AttachmentType>(attachment.AttachmentType, true, out result);
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment()
      {
        Id = attachment.Id,
        FileName = attachment.FileName,
        AttachmentType = result,
        TestRunId = attachment.TestRunId,
        TestResultId = attachment.TestResultId,
        CreationDate = attachment.CreationDate,
        ActionPath = attachment.ActionPath,
        Length = attachment.Length,
        IterationId = attachment.IterationId,
        TmiRunId = attachment.TmiRunId,
        SessionId = attachment.SessionId,
        Comment = attachment.Comment,
        DownloadQueryString = attachment.DownloadQueryString,
        IsComplete = attachment.IsComplete
      };
    }
  }
}
