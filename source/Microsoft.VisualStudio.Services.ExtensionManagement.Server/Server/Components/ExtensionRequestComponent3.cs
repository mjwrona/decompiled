// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.ExtensionRequestComponent3
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class ExtensionRequestComponent3 : ExtensionRequestComponent2
  {
    public override ResultCollection ResolveExtensionRequest(
      string publisherName,
      string extensionName,
      Guid resolverId,
      Guid requesterId,
      string rejectMessage,
      ExtensionRequestState state)
    {
      try
      {
        this.TraceEnter(70210, nameof (ResolveExtensionRequest));
        this.PrepareStoredProcedure("Extension.prc_ResolveExtensionRequest");
        this.BindString("@publisherName", publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
        this.BindString("@extensionName", extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
        this.BindGuid("@resolverId", resolverId);
        this.BindByte("@requestState", (byte) state);
        if (requesterId != Guid.Empty)
          this.BindGuid("@requesterId", requesterId);
        this.BindString("@rejectMessage", rejectMessage, 1000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<ExtensionRequest>((ObjectBinder<ExtensionRequest>) this.GetExtensionRequestColumns());
        return resultCollection;
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (ResolveExtensionRequest));
      }
    }

    public override ResultCollection DeleteExtensionRequests(
      List<RequestedExtension> requestedExtensions)
    {
      try
      {
        this.PrepareStoredProcedure("Extension.prc_DeleteExtensionRequests");
        this.BindExtensionRequestTable("@extensionRequests", (IEnumerable<RequestedExtension>) requestedExtensions);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<ExtensionRequest>((ObjectBinder<ExtensionRequest>) this.GetExtensionRequestColumns());
        return resultCollection;
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (DeleteExtensionRequests));
      }
    }
  }
}
