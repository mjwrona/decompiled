// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetDirectoryRolesWithIdsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphGetDirectoryRolesWithIdsRequest : 
    MicrosoftGraphClientRequest<MsGraphGetDirectoryRolesWithIdsResponse>
  {
    private readonly IEnumerable<string> Types = (IEnumerable<string>) new List<string>()
    {
      "directoryRole"
    };
    private const string PropertiesToFetch = "id, displayName, description, roleTemplateId";
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetDirectoryRolesWithIdsRequest";

    public IEnumerable<Guid> ObjectIds { get; set; }

    internal override void Validate() => ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) this.ObjectIds, "ObjectIds");

    internal override MsGraphGetDirectoryRolesWithIdsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.TraceConditionally(44750045, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetDirectoryRolesWithIdsRequest), (Func<string>) (() => "Calling Microsoft Graph API for Get 'DirectoryRolesWithIds' with parameters ='" + (this.ObjectIds.Serialize<IEnumerable<Guid>>() ?? "null") + "'"));
        Dictionary<Guid, DirectoryRole> dictionary = ((IEnumerable) (context.RunSynchronously<IDirectoryObjectGetByIdsCollectionPage>((Func<Task<IDirectoryObjectGetByIdsCollectionPage>>) (() => graphServiceClient.DirectoryObjects.GetByIds(this.ObjectIds.Select<Guid, string>((Func<Guid, string>) (id => id.ToString())), this.Types).Request((IEnumerable<Option>) null).Select("id, displayName, description, roleTemplateId").PostAsync(new CancellationToken()))) ?? throw new MicrosoftGraphException("Microsoft Graph API Get 'DirectoryRolesWithIds' call returned null"))).OfType<DirectoryRole>().ToDictionary<DirectoryRole, Guid, DirectoryRole>((Func<DirectoryRole, Guid>) (o => MicrosoftGraphConverters.CreateGuid(((Entity) o).Id)), (Func<DirectoryRole, DirectoryRole>) (o => o));
        return new MsGraphGetDirectoryRolesWithIdsResponse()
        {
          DirectoryRoles = (IDictionary<Guid, AadDirectoryRole>) this.ObjectIds.ToDictionary<Guid, Guid, AadDirectoryRole>((Func<Guid, Guid>) (id => id), (Func<Guid, AadDirectoryRole>) (id => MicrosoftGraphConverters.ConvertDirectoryRole(AadServiceUtils.GetValueOrDefault<Guid, DirectoryRole>((IDictionary<Guid, DirectoryRole>) dictionary, id))))
        };
      }
      finally
      {
        context.Trace(44750046, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetDirectoryRolesWithIdsRequest), "Leaving Execute method for 'DirectoryRolesWithIds'");
      }
    }

    public override string ToString()
    {
      IEnumerable<Guid> objectIds = this.ObjectIds;
      return "MsGraphGetDirectoryRolesWithIdsRequest{ObjectIds=" + (objectIds != null ? objectIds.Serialize<IEnumerable<Guid>>() : (string) null) + "}";
    }
  }
}
