// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ActionRequestSqlComponent3
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ActionRequestSqlComponent3 : ActionRequestSqlComponent2
  {
    public override IEnumerable<ActionRequest> GetActionRequests(
      int actionRequestType,
      int top,
      int maxRetries,
      int continuationToken)
    {
      this.PrepareStoredProcedure("Release.prc_GetActionRequests");
      this.BindInt("requestType", actionRequestType);
      this.BindInt(nameof (top), top);
      this.BindInt(nameof (maxRetries), maxRetries);
      this.BindInt(nameof (continuationToken), continuationToken);
      return this.GetActionRequestsObject();
    }
  }
}
