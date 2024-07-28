// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryGetEntitiesRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  public class DirectoryGetEntitiesRequest : DirectoryRequest
  {
    public IEnumerable<string> EntityIds { get; set; }

    public IEnumerable<string> PropertiesToReturn { get; set; }

    internal override DirectoryResponse Execute(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories)
    {
      string[] strArray = this.SanitizeAndFilterDirectories(context);
      DirectoryDiscoveryService service = context.GetService<DirectoryDiscoveryService>();
      DirectoryGetEntitiesInternalRequest entitiesInternalRequest1 = new DirectoryGetEntitiesInternalRequest();
      entitiesInternalRequest1.Directories = (IEnumerable<string>) strArray;
      entitiesInternalRequest1.Preferences = this.Preferences;
      entitiesInternalRequest1.EntityIds = this.EntityIds;
      entitiesInternalRequest1.PropertiesToReturn = this.PropertiesToReturn;
      DirectoryGetEntitiesInternalRequest entitiesInternalRequest2 = entitiesInternalRequest1;
      IVssRequestContext context1 = context;
      DirectoryGetEntitiesInternalRequest request = entitiesInternalRequest2;
      return (DirectoryResponse) service.GetEntitiesInternal(context1, request);
    }
  }
}
