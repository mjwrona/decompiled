// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.WorkItemTrackingOutOfBoxRulesCache
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata
{
  internal class WorkItemTrackingOutOfBoxRulesCache : 
    WorkItemTrackingOutOfBoxValuesCacheBase<WorkItemFieldRule>
  {
    protected internal bool TryGetOutOfBoxRules(
      IVssRequestContext requestContext,
      ProcessDescriptor systemDescriptor,
      string workItemTypeReferenceName,
      out IReadOnlyCollection<WorkItemFieldRule> rules)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (this.TryGetOutOfBoxValues(requestContext, systemDescriptor, workItemTypeReferenceName, ProcessMetadataResourceType.RuleMetadata, nameof (rules), WorkItemTrackingOutOfBoxRulesCache.\u003C\u003EO.\u003C0\u003E__PrepareRules ?? (WorkItemTrackingOutOfBoxRulesCache.\u003C\u003EO.\u003C0\u003E__PrepareRules = new Func<IVssRequestContext, IEnumerable<WorkItemFieldRule>, IEnumerable<WorkItemFieldRule>>(WorkItemRulesService.PrepareRules)), out rules))
      {
        if (WorkItemTrackingFeatureFlags.IsPriorityZeroEnabled(requestContext))
        {
          rules = (IReadOnlyCollection<WorkItemFieldRule>) rules.Select<WorkItemFieldRule, WorkItemFieldRule>((Func<WorkItemFieldRule, WorkItemFieldRule>) (r => r.Clone() as WorkItemFieldRule)).ToList<WorkItemFieldRule>();
          foreach (WorkItemFieldRule workItemFieldRule in (IEnumerable<WorkItemFieldRule>) rules)
            WorkItemRulesService.UpdateRules(requestContext, workItemFieldRule.FieldId, workItemFieldRule.SubRules);
        }
        return true;
      }
      rules = (IReadOnlyCollection<WorkItemFieldRule>) null;
      return false;
    }
  }
}
