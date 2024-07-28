// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.DataspaceServiceHelper
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public static class DataspaceServiceHelper
  {
    public static void CreateDataspaces(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      List<Guid> projectIds,
      DataspaceState dataspaceState)
    {
      IDataspaceService service = requestContext.GetService<IDataspaceService>();
      try
      {
        foreach (Guid projectId in projectIds)
          DataspaceServiceHelper.CreateDataspace(requestContext, dataspaceCategory, projectId, dataspaceState, service);
        string format = string.Format("CreateDataspaces() Succeeded for the collection : {0}", (object) requestContext.ServiceHost.InstanceId);
        requestContext.Trace(1024400, TraceLayer.Job, format);
      }
      catch (Exception ex)
      {
        string message = string.Format("CreateDataspaces() failed for the collection : {0} with the following exception : {1} ", (object) requestContext.ServiceHost.InstanceId, (object) ex.ToString());
        requestContext.Trace(1024405, TraceLevel.Error, "CodeSense", TraceLayer.Job, message);
        throw;
      }
    }

    public static void CreateDataspace(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid projectId,
      DataspaceState dataspaceState,
      IDataspaceService dataspaceService = null)
    {
      dataspaceService = dataspaceService ?? requestContext.GetService<IDataspaceService>();
      try
      {
        if (dataspaceService.QueryDataspace(requestContext, dataspaceCategory, projectId, false) == null)
        {
          dataspaceService.CreateDataspace(requestContext, dataspaceCategory, projectId, dataspaceState);
          DataspaceServiceHelper.PublishDataspaceData(requestContext, projectId);
        }
        string format = string.Format("CreateDataspace() Succeeded for the following collection Id : {0} and projectId : {1}", (object) requestContext.ServiceHost.InstanceId, (object) projectId);
        requestContext.Trace(1024410, TraceLayer.Job, format);
      }
      catch (Exception ex)
      {
        string message = string.Format("CreateDataspace() failed for collection : {0} with the following exception : {1}", (object) requestContext.ServiceHost.InstanceId, (object) ex.ToString());
        requestContext.Trace(1024415, TraceLevel.Error, "CodeSense", TraceLayer.Job, message);
        throw;
      }
    }

    public static Dataspace QueryDataspace(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      bool throwOnMissing = false)
    {
      IDataspaceService service = requestContext.GetService<IDataspaceService>();
      try
      {
        return service.QueryDataspace(requestContext, dataspaceCategory, dataspaceIdentifier, throwOnMissing);
      }
      catch (Exception ex)
      {
        string message = string.Format("QueryDataspace() failed for Dataspace Identifier : {0} with the following exception : {1}", (object) dataspaceIdentifier, (object) ex.ToString());
        requestContext.Trace(1024420, TraceLevel.Error, "CodeSense", TraceLayer.Job, message);
        throw;
      }
    }

    private static void PublishDataspaceData(IVssRequestContext requestContext, Guid projectId) => requestContext.PublishCI(CodeLensCILevel.Important, CodeLensCIArea.CodeLensService, CodeLensCIFeature.DataspaceMapping, new Dictionary<string, object>()
    {
      {
        CodeLensCIProperty.ActivityId,
        (object) requestContext.ActivityId.ToString()
      },
      {
        CodeLensCIProperty.CollectionId,
        (object) requestContext.ServiceHost.InstanceId.ToString()
      },
      {
        CodeLensCIProperty.ProjectId,
        (object) projectId.ToString()
      }
    });
  }
}
