// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IdentityDefaultsTransformer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class IdentityDefaultsTransformer : IdentityRuleTransformer
  {
    private Func<IEnumerable<Guid>, IEnumerable<ConstantRecord>> GetConstants;

    public IdentityDefaultsTransformer(IVssRequestContext requestContext)
    {
      ITeamFoundationWorkItemTrackingMetadataService metadataService = requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>();
      this.GetConstants = (Func<IEnumerable<Guid>, IEnumerable<ConstantRecord>>) (keys => metadataService.GetConstantRecords(requestContext, keys, true));
    }

    public IdentityDefaultsTransformer(
      Func<IEnumerable<Guid>, IEnumerable<ConstantRecord>> getConstants)
    {
      this.GetConstants = getConstants;
    }

    public IReadOnlyCollection<ProcessTypelet> PopulateDisplayNames(
      IEnumerable<ProcessTypelet> typelets)
    {
      IList<IdentityDefaultRule> idRules;
      Dictionary<string, IEnumerable<WorkItemFieldRule>> fieldRulesMap;
      this.GetRules(typelets, out idRules, out fieldRulesMap);
      this.ConvertRules((IEnumerable<IdentityDefaultRule>) idRules);
      return this.ReplaceRules(typelets, fieldRulesMap);
    }

    private void GetRules(
      IEnumerable<ProcessTypelet> typelets,
      out IList<IdentityDefaultRule> idRules,
      out Dictionary<string, IEnumerable<WorkItemFieldRule>> fieldRulesMap)
    {
      idRules = (IList<IdentityDefaultRule>) new List<IdentityDefaultRule>();
      fieldRulesMap = new Dictionary<string, IEnumerable<WorkItemFieldRule>>();
      foreach (ProcessTypelet typelet in typelets)
      {
        IEnumerable<WorkItemFieldRule> fieldRules = typelet.FieldRules;
        WorkItemFieldRule[] array = fieldRules != null ? fieldRules.ToArray<WorkItemFieldRule>() : (WorkItemFieldRule[]) null;
        if (array != null)
        {
          foreach (RuleBlock ruleBlock in array)
          {
            Queue<WorkItemRule> source = new Queue<WorkItemRule>((IEnumerable<WorkItemRule>) ruleBlock.SubRules);
            while (source.Any<WorkItemRule>())
            {
              WorkItemRule workItemRule = source.Dequeue();
              if (workItemRule is IdentityDefaultRule)
              {
                idRules.Add(workItemRule as IdentityDefaultRule);
                if (!fieldRulesMap.ContainsKey(typelet.ReferenceName))
                  fieldRulesMap[typelet.ReferenceName] = (IEnumerable<WorkItemFieldRule>) array;
              }
              else if (workItemRule is RuleBlock)
              {
                foreach (WorkItemRule subRule in ((RuleBlock) workItemRule).SubRules)
                  source.Enqueue(subRule);
              }
            }
          }
        }
      }
    }

    private void ConvertRules(IEnumerable<IdentityDefaultRule> idRules)
    {
      Dictionary<Guid, List<IdentityDefaultRule>> dictionary = new Dictionary<Guid, List<IdentityDefaultRule>>();
      foreach (IdentityDefaultRule idRule in idRules)
      {
        Guid vsid = idRule.Vsid;
        if (!dictionary.ContainsKey(vsid))
          dictionary[vsid] = new List<IdentityDefaultRule>();
        dictionary[vsid].Add(idRule);
      }
      foreach (ConstantRecord constantRecord in this.GetConstants((IEnumerable<Guid>) dictionary.Keys))
      {
        foreach (IdentityDefaultRule identityDefaultRule in dictionary[constantRecord.TeamFoundationId])
        {
          identityDefaultRule.Value = constantRecord.DisplayText;
          identityDefaultRule.ConstId = constantRecord.Id;
        }
      }
    }
  }
}
