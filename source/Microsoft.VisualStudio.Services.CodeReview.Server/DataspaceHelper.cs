// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.DataspaceHelper
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public static class DataspaceHelper
  {
    private const string c_reviewCounterName = "ReviewId";

    public static int EnsureDataspaceExists(IVssRequestContext requestContext, Guid projectId)
    {
      Dataspace dataspace;
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
      {
        string dataspaceCategory = component.DataspaceCategory;
        using (requestContext.AcquireExemptionLock())
        {
          IDataspaceService service = requestContext.GetService<IDataspaceService>();
          dataspace = service.QueryDataspace(requestContext, dataspaceCategory, projectId, false);
          if (dataspace == null)
          {
            service.CreateDataspace(requestContext, dataspaceCategory, projectId, DataspaceState.Active);
            dataspace = service.QueryDataspace(requestContext, dataspaceCategory, projectId, true);
          }
        }
      }
      return dataspace.State == DataspaceState.Active ? dataspace.DataspaceId : throw new CodeReviewOperationFailedException(CodeReviewResources.DataspaceStateError((object) dataspace.DataspaceIdentifier));
    }
  }
}
