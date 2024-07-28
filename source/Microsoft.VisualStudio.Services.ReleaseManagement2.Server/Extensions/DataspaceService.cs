// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.DataspaceService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class DataspaceService
  {
    public static bool CreateDataspaces(
      this IVssRequestContext requestContext,
      Guid dataspaceIdentifier)
    {
      DataspaceService.CreateDefaultDataspace(requestContext, dataspaceIdentifier);
      return DataspaceService.CreateReleaseManagementDataspace(requestContext, dataspaceIdentifier);
    }

    public static bool IsRequiredDataspaceAvailable(
      this IVssRequestContext requestContext,
      Guid dataspaceIdentifier)
    {
      return DataspaceService.IsDataspaceAvailable(requestContext, dataspaceIdentifier, "ReleaseManagement") && DataspaceService.IsDataspaceAvailable(requestContext, dataspaceIdentifier, "Default");
    }

    public static bool IsReleaseManagementDataspaceAvailable(
      this IVssRequestContext requestContext,
      Guid dataspaceIdentifier)
    {
      return DataspaceService.IsDataspaceAvailable(requestContext, dataspaceIdentifier, "ReleaseManagement");
    }

    private static bool IsDataspaceAvailable(
      IVssRequestContext requestContext,
      Guid dataspaceIdentifier,
      string dataspaceCategory)
    {
      string actionName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Service.CheckFor{0}Dataspace", (object) dataspaceCategory);
      using (ReleaseManagementTimer.Create(requestContext, "Service", actionName, 1961021))
      {
        try
        {
          requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, dataspaceCategory, dataspaceIdentifier, true);
          return true;
        }
        catch (DataspaceNotFoundException ex)
        {
          return false;
        }
      }
    }

    private static bool CreateReleaseManagementDataspace(
      IVssRequestContext requestContext,
      Guid dataspaceIdentifier)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return DataspaceService.CreateDataspace(requestContext, dataspaceIdentifier, "ReleaseManagement", DataspaceService.\u003C\u003EO.\u003C0\u003E__OnReleaseManagementDataspaceCreation ?? (DataspaceService.\u003C\u003EO.\u003C0\u003E__OnReleaseManagementDataspaceCreation = new Action<IVssRequestContext, Guid>(DataspaceService.OnReleaseManagementDataspaceCreation)));
    }

    private static bool CreateDefaultDataspace(
      IVssRequestContext requestContext,
      Guid dataspaceIdentifier)
    {
      return DataspaceService.CreateDataspace(requestContext, dataspaceIdentifier, "Default", (Action<IVssRequestContext, Guid>) ((context, guid) => { }));
    }

    private static bool CreateDataspace(
      IVssRequestContext requestContext,
      Guid dataspaceIdentifier,
      string dataspaceCategory,
      Action<IVssRequestContext, Guid> actionOnDataspaceCreation)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      string actionName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Service.Create{0}Dataspace", (object) dataspaceCategory);
      using (ReleaseManagementTimer.Create(requestContext, "Service", actionName, 1961020))
      {
        try
        {
          DataspaceService.TraceInfo(requestContext, 1961020, "Creating {1} Dataspace for the Project {0} from {2}", (object) dataspaceCategory, (object) dataspaceIdentifier, (object) new StackTrace());
          requestContext.GetService<IDataspaceService>().CreateDataspace(requestContext, dataspaceCategory, dataspaceIdentifier, DataspaceState.Active);
          actionOnDataspaceCreation(requestContext, dataspaceIdentifier);
          return true;
        }
        catch (SqlException ex)
        {
          if (ex.Number == 2601)
          {
            DataspaceService.TraceInfo(requestContext, 1961020, "Caught sql exception number 2601, while creating {0} Dataspace for the Project {1} from {2}", (object) dataspaceCategory, (object) dataspaceIdentifier, (object) new StackTrace());
            return false;
          }
          DataspaceService.TraceError(requestContext, 1961036, "Exception occurred while {0} Dataspace creation for the Project {1}. Exception: {2}", (object) dataspaceCategory, (object) dataspaceIdentifier, (object) ex);
          throw;
        }
      }
    }

    private static void OnReleaseManagementDataspaceCreation(
      IVssRequestContext requestContext,
      Guid dataspaceIdentifier)
    {
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "DataspaceCreator.OnReleaseManagementDataspaceCreation", 1961023))
      {
        int dataspaceId = requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, "ReleaseManagement", dataspaceIdentifier, true).DataspaceId;
        Action<OnDataspaceCreationComponent> action1 = (Action<OnDataspaceCreationComponent>) (component => component.PopulateCounters(dataspaceId));
        requestContext.ExecuteWithinUsingWithComponent<OnDataspaceCreationComponent>(action1);
        Action<OnDataspaceCreationComponent> action2 = (Action<OnDataspaceCreationComponent>) (component => component.CreateResourcesForNewDataspace(dataspaceId));
        requestContext.ExecuteWithinUsingWithComponent<OnDataspaceCreationComponent>(action2);
        if (!requestContext.ServiceHost.HostType.Equals((object) TeamFoundationHostType.ProjectCollection))
          return;
        requestContext.GetService<DistributedTaskHubService>().GetReleaseTaskHub(requestContext).CreateScope(requestContext, dataspaceIdentifier);
      }
    }

    private static void TraceInfo(
      IVssRequestContext requestContext,
      int tracepoint,
      string traceMessageFormat,
      params object[] messageParams)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Info, "ReleaseManagementService", "Service", traceMessageFormat, messageParams);
    }

    private static void TraceError(
      IVssRequestContext requestContext,
      int tracepoint,
      string traceMessageFormat,
      params object[] messageParams)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Error, "ReleaseManagementService", "Service", traceMessageFormat, messageParams);
    }
  }
}
