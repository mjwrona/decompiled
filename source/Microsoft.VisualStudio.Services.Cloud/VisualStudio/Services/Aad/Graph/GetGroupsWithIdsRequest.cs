// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetGroupsWithIdsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetGroupsWithIdsRequest : AadGraphClientRequest<GetGroupsWithIdsResponse>
  {
    public IEnumerable<Guid> ObjectIds { get; set; }

    internal override void Validate() => ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) this.ObjectIds, "ObjectIds");

    internal override GetGroupsWithIdsResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      Dictionary<Guid, Group> dictionary = (connection.GetObjectsByObjectIds((IList<string>) this.ObjectIds.Select<Guid, string>((Func<Guid, string>) (id => id.ToString())).ToList<string>()) ?? throw new AadException("Failed to get groups: connection returned an invalid response.")).OfType<Group>().ToDictionary<Group, Guid, Group>((Func<Group, Guid>) (o => AadGraphClient.CreateGuid(o.ObjectId)), (Func<Group, Group>) (o => o));
      return new GetGroupsWithIdsResponse()
      {
        Groups = (IDictionary<Guid, AadGroup>) this.ObjectIds.ToDictionary<Guid, Guid, AadGroup>((Func<Guid, Guid>) (id => id), (Func<Guid, AadGroup>) (id => AadGraphClient.ConvertGroup(AadServiceUtils.GetValueOrDefault<Guid, Group>((IDictionary<Guid, Group>) dictionary, id))))
      };
    }

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
      return string.Format("GetGroupsWithIdsRequest{0}ObjectIds={1}{2}", (object) "{", (object) str, (object) "}");
    }
  }
}
