// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestResultAttachmentIdentityConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class TestResultAttachmentIdentityConverter
  {
    public static Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity attachmentIdentity)
    {
      if (attachmentIdentity == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity()
      {
        TestRunId = attachmentIdentity.TestRunId,
        TestResultId = attachmentIdentity.TestResultId,
        SessionId = attachmentIdentity.SessionId,
        AttachmentId = attachmentIdentity.AttachmentId
      };
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity> attachmentIds)
    {
      return attachmentIds == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity>) null : attachmentIds.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity>) (attachmentId => TestResultAttachmentIdentityConverter.Convert(attachmentId)));
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity Convert(
      Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity attachmentIdentity)
    {
      if (attachmentIdentity == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity()
      {
        TestRunId = attachmentIdentity.TestRunId,
        TestResultId = attachmentIdentity.TestResultId,
        SessionId = attachmentIdentity.SessionId,
        AttachmentId = attachmentIdentity.AttachmentId
      };
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity> attachmentIds)
    {
      return attachmentIds == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>) null : attachmentIds.Select<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>) (attachmentId => TestResultAttachmentIdentityConverter.Convert(attachmentId)));
    }
  }
}
