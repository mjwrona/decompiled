// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetDirectoryRolesWithIdsRequest
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
  public class GetDirectoryRolesWithIdsRequest : 
    AadGraphClientRequest<GetDirectoryRolesWithIdsResponse>
  {
    public IEnumerable<Guid> ObjectIds { get; set; }

    internal override void Validate() => ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) this.ObjectIds, "ObjectIds");

    internal override GetDirectoryRolesWithIdsResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      IEnumerable<Guid> source = this.SanitizeObjectIds();
      Dictionary<Guid, DirectoryRole> dictionary = (connection.GetObjectsByObjectIds((IList<string>) source.Select<Guid, string>((Func<Guid, string>) (id => id.ToString())).ToList<string>()) ?? throw new AadException("Failed to get directory Roles: connection returned an invalid response.")).OfType<DirectoryRole>().ToDictionary<DirectoryRole, Guid, DirectoryRole>((Func<DirectoryRole, Guid>) (o => AadGraphClient.CreateGuid(o.ObjectId)), (Func<DirectoryRole, DirectoryRole>) (o => o));
      return new GetDirectoryRolesWithIdsResponse()
      {
        DirectoryRoles = (IDictionary<Guid, AadDirectoryRole>) source.ToDictionary<Guid, Guid, AadDirectoryRole>((Func<Guid, Guid>) (id => id), (Func<Guid, AadDirectoryRole>) (id => AadGraphClient.ConvertDirectoryRole(AadServiceUtils.GetValueOrDefault<Guid, DirectoryRole>((IDictionary<Guid, DirectoryRole>) dictionary, id))))
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
      return string.Format("GetDirectoryRolesWithIdsRequest{0}ObjectIds={1}{2}", (object) "{", (object) str, (object) "}");
    }
  }
}
