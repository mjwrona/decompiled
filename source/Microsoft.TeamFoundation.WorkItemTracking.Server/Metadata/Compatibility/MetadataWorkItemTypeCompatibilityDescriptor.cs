// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility.MetadataWorkItemTypeCompatibilityDescriptor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility
{
  public class MetadataWorkItemTypeCompatibilityDescriptor
  {
    private Lazy<string> m_lazyForm;
    private IReadOnlyCollection<WorkItemFieldRule> m_oobRules;
    private IReadOnlyCollection<HelpTextDescriptor> m_oobHelpTexts;

    public WorkItemType Type { get; set; }

    public IReadOnlyCollection<WorkItemStateDefinition> States { get; set; }

    public IReadOnlyCollection<WorkItemStateDefinition> DeltaStates { get; set; }

    public bool AreStatesCustomized => this.DeltaStates.Any<WorkItemStateDefinition>() || this.Type.IsCustomType;

    public string BaseTypeReferenceName => this.Type.IsDerived ? this.Type.InheritedWorkItemType.ReferenceName : this.Type.ReferenceName;

    public IReadOnlyCollection<WorkItemFieldRule> Rules { get; set; }

    public IReadOnlyCollection<FieldEntry> Fields { get; set; }

    public string Form
    {
      get => this.m_lazyForm?.Value;
      set => this.m_lazyForm = new Lazy<string>((Func<string>) (() => value));
    }

    public static MetadataWorkItemTypeCompatibilityDescriptor Create(
      IVssRequestContext requestContext,
      WorkItemType type,
      IReadOnlyCollection<WorkItemStateDefinition> states = null,
      IReadOnlyCollection<WorkItemStateDefinition> deltaStates = null)
    {
      return requestContext.TraceBlock<MetadataWorkItemTypeCompatibilityDescriptor>(900816, 900817, "Compatibility", "MetadataCompatibility", "MetadataWorkItemTypeCompatibilityDescriptor.Create", (Func<MetadataWorkItemTypeCompatibilityDescriptor>) (() => new MetadataWorkItemTypeCompatibilityDescriptor()
      {
        Type = type,
        States = states != null ? states : (IReadOnlyCollection<WorkItemStateDefinition>) new List<WorkItemStateDefinition>(),
        DeltaStates = deltaStates != null ? deltaStates : (IReadOnlyCollection<WorkItemStateDefinition>) new List<WorkItemStateDefinition>(),
        Fields = ((ILegacyMetadataCompatibilityParticipant) type).GetFields(requestContext),
        Rules = ((ILegacyMetadataCompatibilityParticipant) type).GetRules(requestContext),
        m_lazyForm = new Lazy<string>((Func<string>) (() => ((ILegacyMetadataCompatibilityParticipant) type).GetForm(requestContext)))
      }));
    }

    internal bool TryGetOutOfBoxRulesAndHelpTexts(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      out IReadOnlyCollection<WorkItemFieldRule> oobRules,
      out IReadOnlyCollection<HelpTextDescriptor> oobHelpTexts)
    {
      if (processDescriptor.IsCustom || this.Type.IsCustomType || !WorkItemTrackingFeatureFlags.IsFullOrPartialRuleGenerationEnabled(requestContext))
      {
        this.m_oobRules = (IReadOnlyCollection<WorkItemFieldRule>) null;
        this.m_oobHelpTexts = (IReadOnlyCollection<HelpTextDescriptor>) null;
      }
      else if (this.m_oobRules == null || this.m_oobHelpTexts == null)
      {
        if (!requestContext.GetService<IWorkItemRulesService>().TryGetOutOfBoxRulesAndHelpTexts(requestContext, processDescriptor, this.Type.ReferenceName, out this.m_oobRules, out this.m_oobHelpTexts))
        {
          this.m_oobRules = (IReadOnlyCollection<WorkItemFieldRule>) null;
          this.m_oobHelpTexts = (IReadOnlyCollection<HelpTextDescriptor>) null;
        }
        if (this.Type.IsDerived && this.m_oobRules != null && this.Type.InheritedWorkItemType.DisabledRules != null && this.Type.InheritedWorkItemType.DisabledRules.Any<Guid>())
          this.m_oobRules = (IReadOnlyCollection<WorkItemFieldRule>) DisabledRulesFilter.RunFilter(this.m_oobRules, new HashSet<Guid>(this.Type.InheritedWorkItemType.DisabledRules));
      }
      oobRules = this.m_oobRules;
      oobHelpTexts = this.m_oobHelpTexts;
      return oobRules != null && oobHelpTexts != null;
    }
  }
}
