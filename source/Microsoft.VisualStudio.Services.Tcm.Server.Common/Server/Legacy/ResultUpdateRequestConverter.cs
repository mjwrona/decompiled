// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultUpdateRequestConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class ResultUpdateRequestConverter
  {
    public static Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest resultUpdateRequest)
    {
      if (resultUpdateRequest == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest) null;
      Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest resultUpdateRequest1 = new Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest();
      resultUpdateRequest1.TestRunId = resultUpdateRequest.TestRunId;
      resultUpdateRequest1.TestResultId = resultUpdateRequest.TestResultId;
      resultUpdateRequest1.TestCaseResult = TestCaseResultContractConverter.Convert(resultUpdateRequest.TestCaseResult);
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult> source1 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) resultUpdateRequest.ActionResults);
      resultUpdateRequest1.ActionResults = source1 != null ? source1.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>() : (Microsoft.TeamFoundation.TestManagement.Server.TestActionResult[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult> source2 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) resultUpdateRequest.ActionResultDeletes);
      resultUpdateRequest1.ActionResultDeletes = source2 != null ? source2.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>() : (Microsoft.TeamFoundation.TestManagement.Server.TestActionResult[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter> source3 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) resultUpdateRequest.Parameters);
      resultUpdateRequest1.Parameters = source3 != null ? source3.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>() : (Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter> source4 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) resultUpdateRequest.ParameterDeletes);
      resultUpdateRequest1.ParameterDeletes = source4 != null ? source4.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>() : (Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment> source5 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) resultUpdateRequest.Attachments);
      resultUpdateRequest1.Attachments = source5 != null ? source5.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>() : (Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity> source6 = TestResultAttachmentIdentityConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>) resultUpdateRequest.AttachmentDeletes);
      resultUpdateRequest1.AttachmentDeletes = source6 != null ? source6.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity>() : (Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity[]) null;
      return resultUpdateRequest1;
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest Convert(
      Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest resultUpdateRequest)
    {
      if (resultUpdateRequest == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest) null;
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest resultUpdateRequest1 = new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest();
      resultUpdateRequest1.TestRunId = resultUpdateRequest.TestRunId;
      resultUpdateRequest1.TestResultId = resultUpdateRequest.TestResultId;
      resultUpdateRequest1.TestCaseResult = TestCaseResultContractConverter.Convert(resultUpdateRequest.TestCaseResult);
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> source1 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>) resultUpdateRequest.ActionResults);
      resultUpdateRequest1.ActionResults = source1 != null ? source1.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> source2 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>) resultUpdateRequest.ActionResultDeletes);
      resultUpdateRequest1.ActionResultDeletes = source2 != null ? source2.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> source3 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>) resultUpdateRequest.Parameters);
      resultUpdateRequest1.Parameters = source3 != null ? source3.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> source4 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>) resultUpdateRequest.ParameterDeletes);
      resultUpdateRequest1.ParameterDeletes = source4 != null ? source4.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> source5 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) resultUpdateRequest.Attachments);
      resultUpdateRequest1.Attachments = source5 != null ? source5.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity> source6 = TestResultAttachmentIdentityConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity>) resultUpdateRequest.AttachmentDeletes);
      resultUpdateRequest1.AttachmentDeletes = source6 != null ? source6.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>() : (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[]) null;
      return resultUpdateRequest1;
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest> resultUpdateRequests)
    {
      return resultUpdateRequests == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest>) null : resultUpdateRequests.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest, Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest, Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest>) (request => ResultUpdateRequestConverter.Convert(request)));
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest> resultUpdateRequests)
    {
      return resultUpdateRequests == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>) null : resultUpdateRequests.Select<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>((Func<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>) (request => ResultUpdateRequestConverter.Convert(request)));
    }
  }
}
