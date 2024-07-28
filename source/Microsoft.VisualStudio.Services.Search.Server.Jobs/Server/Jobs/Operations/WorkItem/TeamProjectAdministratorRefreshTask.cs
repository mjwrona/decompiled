// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.TeamProjectAdministratorRefreshTask
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class TeamProjectAdministratorRefreshTask : IIndexingPatchTask
  {
    private readonly IIndexingUnitDataAccess m_indexingUnitDataAccess;

    public string Name { get; } = nameof (TeamProjectAdministratorRefreshTask);

    public TeamProjectAdministratorRefreshTask(IDataAccessFactory dataAccessFactory) => this.m_indexingUnitDataAccess = dataAccessFactory.GetIndexingUnitDataAccess();

    public void Patch(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.m_indexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Project", (IEntityType) WorkItemEntityType.GetInstance(), -1);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnit1 = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(indexingUnits.Count);
      IdentityService service = indexingExecutionContext.RequestContext.GetService<IdentityService>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 in indexingUnits)
      {
        ProjectWorkItemIndexingProperties properties = (ProjectWorkItemIndexingProperties) indexingUnit2.Properties;
        try
        {
          IdentityDescriptor administrators = service.GetScope(indexingExecutionContext.RequestContext, indexingUnit2.TFSEntityId).Administrators;
          IdentityDescriptor projectAdministrators = properties.ProjectAdministrators;
          if (!(projectAdministrators == (IdentityDescriptor) null))
          {
            if (projectAdministrators.Equals(administrators))
              continue;
          }
          properties.ProjectAdministrators = administrators;
          indexingUnit1.Add(indexingUnit2);
        }
        catch (GroupScopeDoesNotExistException ex)
        {
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080487, "Indexing Pipeline", "IndexingOperation", ex);
        }
      }
      if (indexingUnit1.Count <= 0)
        return;
      this.m_indexingUnitDataAccess.UpdateIndexingUnits(indexingExecutionContext.RequestContext, indexingUnit1);
      indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Updated admin information in [{0}] project indexing units.", (object) indexingUnit1.Count)));
    }
  }
}
