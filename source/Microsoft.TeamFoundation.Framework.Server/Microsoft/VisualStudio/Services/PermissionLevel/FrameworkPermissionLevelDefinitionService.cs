// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.FrameworkPermissionLevelDefinitionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.PermissionLevel.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkPermissionLevelDefinitionService : 
    IPermissionLevelDefinitionService,
    IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private const string c_area = "PermissionLevel";
    private const string c_layer = "FrameworkPermissionLevelDefinitionService";

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public IDictionary<Guid, Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition> GetPermissionLevelDefinitions(
      IVssRequestContext context,
      IEnumerable<Guid> definitionIds)
    {
      this.ValidateRequestContext(context);
      IEnumerable<Guid> list = (IEnumerable<Guid>) definitionIds.ToList<Guid>();
      return list.IsNullOrEmpty<Guid>() ? (IDictionary<Guid, Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition>) new Dictionary<Guid, Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition>() : (IDictionary<Guid, Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition>) this.GetPermissionLevelHttpClient(context).GetPermissionLevelDefinitionsByIdAsync(list).SyncResult<Dictionary<Guid, Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>>().ToDictionary<KeyValuePair<Guid, Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>, Guid, Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition>((Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>, Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition>) (x => x.Value.ToServer()));
    }

    public IEnumerable<Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition> GetPermissionLevelDefinitions(
      IVssRequestContext context,
      PermissionLevelDefinitionScope definitionScope,
      PermissionLevelDefinitionType definitionType)
    {
      this.ValidateRequestContext(context);
      ArgumentValidator.ValidatePermissionLevelDefinitionType(definitionType);
      return (IEnumerable<Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition>) this.GetPermissionLevelHttpClient(context).GetPermissionLevelDefinitionsByScopeAsync(definitionScope, definitionType).SyncResult<List<Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition>>().Select<Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition, Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition>((Func<Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition, Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition>) (x => x.ToServer())).ToList<Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition>();
    }

    private void ValidateRequestContext(IVssRequestContext context) => context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);

    private PermissionLevelHttpClient GetPermissionLevelHttpClient(IVssRequestContext requestContext) => requestContext.GetClient<PermissionLevelHttpClient>();
  }
}
