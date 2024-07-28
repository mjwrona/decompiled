// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultUpdateResponseConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class ResultUpdateResponseConverter
  {
    public static Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse resultUpdateResponse)
    {
      if (resultUpdateResponse == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse()
      {
        TestResultId = resultUpdateResponse.TestResultId,
        TestPlanId = resultUpdateResponse.TestPlanId,
        Revision = resultUpdateResponse.Revision,
        LastUpdated = resultUpdateResponse.LastUpdated,
        LastUpdatedBy = resultUpdateResponse.LastUpdatedBy,
        LastUpdatedByName = resultUpdateResponse.LastUpdatedByName,
        AttachmentIds = resultUpdateResponse.AttachmentIds,
        MaxReservedSubResultId = resultUpdateResponse.MaxReservedSubResultId
      };
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse Convert(
      Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse resultUpdateResponse)
    {
      if (resultUpdateResponse == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse()
      {
        TestResultId = resultUpdateResponse.TestResultId,
        TestPlanId = resultUpdateResponse.TestPlanId,
        Revision = resultUpdateResponse.Revision,
        LastUpdated = resultUpdateResponse.LastUpdated,
        LastUpdatedBy = resultUpdateResponse.LastUpdatedBy,
        LastUpdatedByName = resultUpdateResponse.LastUpdatedByName,
        AttachmentIds = resultUpdateResponse.AttachmentIds,
        MaxReservedSubResultId = resultUpdateResponse.MaxReservedSubResultId
      };
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse> resultUpdateResponses)
    {
      return resultUpdateResponses == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse>) null : resultUpdateResponses.Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse, Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse, Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse>) (response => ResultUpdateResponseConverter.Convert(response)));
    }

    public static IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse> Convert(
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse> resultUpdateResponses)
    {
      return resultUpdateResponses == null ? (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse>) null : resultUpdateResponses.Select<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse>((Func<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse>) (response => ResultUpdateResponseConverter.Convert(response)));
    }
  }
}
