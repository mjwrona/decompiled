// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetDirectoryRoleMembersRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetDirectoryRoleMembersRequest : 
    AadGraphClientRequest<GetDirectoryRoleMembersResponse>
  {
    public Guid DirectoryRoleObjectId { get; set; }

    internal override GetDirectoryRoleMembersResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      try
      {
        context.TraceEnter(8525610, "VisualStudio.Services.Aad", "Graph", nameof (Execute));
        BatchRequestItem batchRequestItem = new BatchRequestItem("GET", false, this.GetRequestUrl(connection, this.DirectoryRoleObjectId.ToString()), (WebHeaderCollection) null, (string) null);
        BatchResponseItem batchResponseItem = connection.ExecuteBatch((IList<BatchRequestItem>) new BatchRequestItem[1]
        {
          batchRequestItem
        })[0];
        if (batchResponseItem.Exception != null)
        {
          context.TraceException(8525611, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", (Exception) batchResponseItem.Exception);
          return new GetDirectoryRoleMembersResponse()
          {
            Exception = (Exception) batchResponseItem.Exception
          };
        }
        if (batchResponseItem.Failed)
        {
          context.TraceAlways(8525612, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", string.Format("{0} BatchResponseItem failed for objectId:{1}", (object) nameof (GetDirectoryRoleMembersRequest), (object) this.DirectoryRoleObjectId));
          return new GetDirectoryRoleMembersResponse()
          {
            Exception = (Exception) new AadGraphException(HostingResources.FailureRetrieveRoleMembers((object) this.DirectoryRoleObjectId))
          };
        }
        return new GetDirectoryRoleMembersResponse()
        {
          Members = (ISet<GraphObject>) new HashSet<GraphObject>((IEnumerable<GraphObject>) batchResponseItem.ResultSet.Results)
        };
      }
      catch (Exception ex)
      {
        context.TraceException(8525613, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", ex);
        return new GetDirectoryRoleMembersResponse()
        {
          Exception = ex
        };
      }
      finally
      {
        context.TraceLeave(8525615, "VisualStudio.Services.Aad", "Graph", nameof (Execute));
      }
    }

    internal virtual Uri GetRequestUrl(GraphConnection connection, string directoryRoleObjectId)
    {
      string str = "directoryRoles/" + directoryRoleObjectId + "/members";
      return Utils.GetRequestUri(connection, (Type) null, (string) null, str);
    }
  }
}
