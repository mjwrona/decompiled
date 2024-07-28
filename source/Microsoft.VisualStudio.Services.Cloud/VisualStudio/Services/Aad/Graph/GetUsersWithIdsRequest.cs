// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetUsersWithIdsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetUsersWithIdsRequest : AadGraphClientRequest<GetUsersWithIdsResponse>
  {
    protected const string TraceArea = "GetUsersWithIdsRequest";
    protected const string TraceLayer = "GetUsersWithIdsRequest";

    public IEnumerable<Guid> ObjectIds { get; set; }

    public string ExpandProperty { get; set; }

    internal override void Validate() => ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) this.ObjectIds, "ObjectIds");

    internal override GetUsersWithIdsResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      IEnumerable<Guid> source1 = this.SanitizeObjectIds();
      context.Trace(AadGraphTrace.AadGraphClientGetUsesrWithIdsRequestExecute, TraceLevel.Info, nameof (GetUsersWithIdsRequest), nameof (GetUsersWithIdsRequest), "Calling AAD Graph API for {0} object ids", (object) source1.Count<Guid>());
      LinkProperty expandProperty = LinkProperty.None;
      switch (this.ExpandProperty)
      {
        case "Manager":
          expandProperty = LinkProperty.Manager;
          break;
        case "DirectReports":
          expandProperty = LinkProperty.DirectReports;
          break;
      }
      IList<GraphObject> source2;
      if (expandProperty == LinkProperty.None)
      {
        source2 = connection.GetObjectsByObjectIds((IList<string>) source1.Select<Guid, string>((Func<Guid, string>) (id => id.ToString())).ToList<string>());
      }
      else
      {
        source2 = (IList<GraphObject>) new List<GraphObject>(source1.Count<Guid>());
        foreach (Guid guid in source1)
          source2.Add(connection.Get(typeof (User), guid.ToString(), expandProperty));
      }
      if (source2 == null)
        throw new AadException("Failed to get users: connection returned an invalid response.");
      Dictionary<Guid, User> dictionary = source2.OfType<User>().ToDictionary<User, Guid, User>((Func<User, Guid>) (o => AadGraphClient.CreateGuid(o.ObjectId)), (Func<User, User>) (o => o));
      return new GetUsersWithIdsResponse()
      {
        Users = (IDictionary<Guid, AadUser>) source1.ToDictionary<Guid, Guid, AadUser>((Func<Guid, Guid>) (id => id), (Func<Guid, AadUser>) (id => AadGraphClient.ConvertUser(AadServiceUtils.GetValueOrDefault<Guid, User>((IDictionary<Guid, User>) dictionary, id))))
      };
    }

    private IEnumerable<Guid> SanitizeObjectIds() => (IEnumerable<Guid>) new HashSet<Guid>(this.ObjectIds);

    public override string ToString()
    {
      string str = (string) null;
      if (this.ObjectIds != null)
      {
        switch (this.ObjectIds.Count<Guid>())
        {
          case 0:
            str = "[]";
            break;
          case 1:
            str = string.Format("[{0}]", (object) this.ObjectIds.First<Guid>());
            break;
          default:
            str = string.Format("[{0}...]", (object) this.ObjectIds.First<Guid>());
            break;
        }
      }
      return string.Format("GetUsersWithIdsRequest{0}ObjectIds={1}{2}", (object) "{", (object) str, (object) "}");
    }
  }
}
