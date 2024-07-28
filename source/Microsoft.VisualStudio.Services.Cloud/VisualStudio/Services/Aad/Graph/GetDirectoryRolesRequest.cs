// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetDirectoryRolesRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetDirectoryRolesRequest : AadGraphClientRequest<GetDirectoryRolesResponse>
  {
    internal override GetDirectoryRolesResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      try
      {
        context.TraceEnter(8525620, "VisualStudio.Services.Aad", "Graph", nameof (Execute));
        BatchRequestItem batchRequestItem = new BatchRequestItem("GET", false, this.GetRequestUrl(connection), (WebHeaderCollection) null, (string) null);
        BatchResponseItem batchResponseItem = connection.ExecuteBatch((IList<BatchRequestItem>) new BatchRequestItem[1]
        {
          batchRequestItem
        })[0];
        if (batchResponseItem.Exception != null)
        {
          context.TraceException(8525621, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", (Exception) batchResponseItem.Exception);
          return new GetDirectoryRolesResponse()
          {
            Exception = (Exception) batchResponseItem.Exception
          };
        }
        if (batchResponseItem.Failed)
        {
          context.TraceAlways(8525622, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", "GetDirectoryRolesRequest BatchResponseItem failed.");
          return new GetDirectoryRolesResponse()
          {
            Exception = (Exception) new AadGraphException(HostingResources.FailureRetrieveDirectoryRoles())
          };
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return new GetDirectoryRolesResponse()
        {
          DirectoryRoles = (IEnumerable<AadDirectoryRole>) batchResponseItem.ResultSet.Results.Where<GraphObject>((Func<GraphObject, bool>) (o => o is DirectoryRole)).Cast<DirectoryRole>().Select<DirectoryRole, AadDirectoryRole>(GetDirectoryRolesRequest.\u003C\u003EO.\u003C0\u003E__ConvertDirectoryRole ?? (GetDirectoryRolesRequest.\u003C\u003EO.\u003C0\u003E__ConvertDirectoryRole = new Func<DirectoryRole, AadDirectoryRole>(AadGraphClient.ConvertDirectoryRole))).ToList<AadDirectoryRole>()
        };
      }
      catch (Exception ex)
      {
        context.TraceException(8525623, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", ex);
        return new GetDirectoryRolesResponse()
        {
          Exception = ex
        };
      }
      finally
      {
        context.TraceLeave(8525625, "VisualStudio.Services.Aad", "Graph", nameof (Execute));
      }
    }

    internal virtual Uri GetRequestUrl(GraphConnection connection) => Utils.GetRequestUri(connection, (Type) null, (string) null, "directoryRoles");
  }
}
