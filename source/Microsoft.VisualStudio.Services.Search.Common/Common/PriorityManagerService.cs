// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.PriorityManagerService
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Search.Platform.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class PriorityManagerService : IVssFrameworkService
  {
    private IDisposableReadOnlyList<IPriorityManager> m_priorityManagers;
    private PriorityManager m_corePriorityManager = new PriorityManager();
    private List<string> m_allChangeTypes;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      HashSet<string> source = new HashSet<string>((IEnumerable<string>) this.m_corePriorityManager.GetPriorityMap().Keys);
      this.m_priorityManagers = SearchPlatformHelper.GetExtensions<IPriorityManager>(systemRequestContext, false);
      foreach (IPriorityManager priorityManager in (IEnumerable<IPriorityManager>) this.m_priorityManagers)
      {
        foreach (string key in priorityManager.GetPriorityMap().Keys)
        {
          if (!source.Add(key))
            throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Operation '{0}' in '{1}' conflicts with a supported operation type. Update the operation type to a different value.", (object) key, (object) priorityManager)));
        }
      }
      this.m_allChangeTypes = source.ToList<string>();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_priorityManagers != null)
      {
        this.m_priorityManagers.Dispose();
        this.m_priorityManagers = (IDisposableReadOnlyList<IPriorityManager>) null;
      }
      this.m_corePriorityManager = (PriorityManager) null;
    }

    public virtual JobPriorityLevel GetPriority(
      IVssRequestContext requestContext,
      IndexingUnitChangeEvent changeEvent)
    {
      if (changeEvent == null)
        throw new ArgumentNullException(nameof (changeEvent));
      JobPriorityLevel priority = this.m_corePriorityManager.GetPriority(requestContext, changeEvent);
      if (priority != JobPriorityLevel.None)
        return priority;
      if (this.m_priorityManagers != null)
      {
        foreach (IPriorityManager priorityManager in (IEnumerable<IPriorityManager>) this.m_priorityManagers)
        {
          if (priorityManager.GetPriorityMap().TryGetValue(changeEvent.ChangeType, out priority))
            return priority;
        }
      }
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("'{0}' is not configured to specify the priority of an operation.", (object) changeEvent.ChangeType)));
    }

    public virtual List<string> GetAllChangeTypes() => this.m_allChangeTypes;
  }
}
