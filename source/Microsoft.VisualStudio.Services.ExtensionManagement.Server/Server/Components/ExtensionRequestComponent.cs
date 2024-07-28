// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.ExtensionRequestComponent
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
  internal class ExtensionRequestComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1660005,
        new SqlExceptionFactory(typeof (ExtensionRequestAlreadyExistsException))
      },
      {
        1660007,
        new SqlExceptionFactory(typeof (ExtensionAlreadyInstalledException))
      }
    };
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ExtensionRequestComponent>(1),
      (IComponentCreator) new ComponentCreator<ExtensionRequestComponent2>(2),
      (IComponentCreator) new ComponentCreator<ExtensionRequestComponent3>(3)
    }, "ExtensionRequests");
    private const string s_area = "ExtensionRequestComponent";

    public ExtensionRequestComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override string TraceArea => nameof (ExtensionRequestComponent);

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ExtensionRequestComponent.s_sqlExceptionFactories;

    public virtual ResultCollection RequestExtension(
      string publisherName,
      string extensionName,
      Guid requesterId,
      string requestMessage)
    {
      try
      {
        this.TraceEnter(70210, nameof (RequestExtension));
        this.PrepareStoredProcedure("Extension.prc_CreateExtensionRequest");
        this.BindString("@publisherName", publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
        this.BindString("@extensionName", extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
        this.BindGuid("@requesterId", requesterId);
        this.BindString("@requestMessage", requestMessage, 1000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<RequestedExtension>((ObjectBinder<RequestedExtension>) this.GetRequestedExtensionColumns());
        return resultCollection;
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (RequestExtension));
      }
    }

    public virtual ResultCollection ResolveExtensionRequest(
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
        this.ExecuteScalar();
        return (ResultCollection) null;
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

    public void DeleteExtensionRequest(
      string publisherName,
      string extensionName,
      Guid requesterId)
    {
      try
      {
        this.TraceEnter(70210, nameof (DeleteExtensionRequest));
        this.PrepareStoredProcedure("Extension.prc_DeleteExtensionRequest");
        this.BindString("@publisherName", publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
        this.BindString("@extensionName", extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
        this.BindNullableGuid("@requesterId", requesterId);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (DeleteExtensionRequest));
      }
    }

    public virtual ResultCollection DeleteExtensionRequests(
      List<RequestedExtension> requestedExtensions)
    {
      return (ResultCollection) null;
    }

    public virtual ResultCollection GetRequests()
    {
      try
      {
        this.TraceEnter(70210, "GetExtensionRequests");
        this.PrepareStoredProcedure("Extension.prc_GetExtensionRequests");
        ResultCollection requests = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        requests.AddBinder<RequestedExtension>((ObjectBinder<RequestedExtension>) this.GetRequestedExtensionColumns());
        return requests;
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, "GetOutstandingRequests");
      }
    }

    protected virtual RequestedExtensionColumns GetRequestedExtensionColumns() => new RequestedExtensionColumns(this.RequestContext);

    protected virtual ExtensionRequestColumns GetExtensionRequestColumns() => new ExtensionRequestColumns(this.RequestContext);
  }
}
