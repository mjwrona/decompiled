// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ActionRequestSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ActionRequestSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ActionRequestSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<ActionRequestSqlComponent2>(2),
      (IComponentCreator) new ComponentCreator<ActionRequestSqlComponent3>(3)
    }, "ReleaseManagementActionRequests", "ReleaseManagement");

    public virtual IEnumerable<ActionRequest> GetActionRequests(
      int actionRequestType,
      int top,
      int maxRetries,
      int continuationToken)
    {
      this.PrepareStoredProcedure("Release.prc_GetActionRequests");
      this.BindInt(nameof (top), top);
      this.BindInt(nameof (maxRetries), maxRetries);
      this.BindInt(nameof (continuationToken), continuationToken);
      return this.GetActionRequestsObject().Where<ActionRequest>((System.Func<ActionRequest, bool>) (x => x.ActionRequestType == (ActionRequestType) actionRequestType));
    }

    public ActionRequest AddActionRequest(Guid projectId, ActionRequest actionRequest)
    {
      if (actionRequest == null)
        throw new ArgumentNullException(nameof (actionRequest));
      this.PrepareStoredProcedure("Release.prc_AddActionRequest", projectId);
      this.BindString("resourceId", actionRequest.ResourceId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("actionRequestType", (int) actionRequest.ActionRequestType);
      this.BindString("actionRequestData", ActionRequestSqlComponent.SerializeActionRequestData(actionRequest.ActionRequestData), 4000, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("numAttempts", actionRequest.NumberOfAttempts);
      this.BindString("lastException", actionRequest.LastException, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      return this.GetActionRequestObject();
    }

    public ActionRequest UpdateActionRequest(ActionRequest actionRequest)
    {
      if (actionRequest == null)
        throw new ArgumentNullException(nameof (actionRequest));
      this.PrepareStoredProcedure("Release.prc_UpdateActionRequest");
      this.BindInt("actionRequestId", actionRequest.Id);
      this.BindInt("numAttempts", actionRequest.NumberOfAttempts);
      this.BindString("lastException", actionRequest.LastException, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      return this.GetActionRequestObject();
    }

    public void DeleteActionRequest(int actionRequestId)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteActionRequest");
      this.BindInt(nameof (actionRequestId), actionRequestId);
      this.ExecuteScalar();
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method is fine here")]
    protected virtual IEnumerable<ActionRequest> GetActionRequestsObject()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ActionRequest>((ObjectBinder<ActionRequest>) this.GetActionRequestBinder());
        return (IEnumerable<ActionRequest>) resultCollection.GetCurrent<ActionRequest>().Items;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method is fine here")]
    protected virtual ActionRequest GetActionRequestObject()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ActionRequest>((ObjectBinder<ActionRequest>) this.GetActionRequestBinder());
        return resultCollection.GetCurrent<ActionRequest>().Items.FirstOrDefault<ActionRequest>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method is fine here")]
    protected virtual ActionRequestBinder GetActionRequestBinder() => new ActionRequestBinder((ReleaseManagementSqlResourceComponentBase) this);

    private static string SerializeActionRequestData(ActionRequestData actionRequestData)
    {
      MemoryStream memoryStream = (MemoryStream) null;
      try
      {
        memoryStream = new MemoryStream();
        new DataContractJsonSerializer(typeof (ActionRequestData)).WriteObject((Stream) memoryStream, (object) actionRequestData);
        memoryStream.Position = 0L;
        return new StreamReader((Stream) memoryStream).ReadToEnd();
      }
      finally
      {
        memoryStream?.Close();
      }
    }
  }
}
