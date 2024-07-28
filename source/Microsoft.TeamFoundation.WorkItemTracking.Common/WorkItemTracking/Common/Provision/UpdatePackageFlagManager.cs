// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackageFlagManager
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  internal class UpdatePackageFlagManager
  {
    private RuleFlags m_baseFlags;
    private Dictionary<MetaID, RuleFlags> m_stateFlags = new Dictionary<MetaID, RuleFlags>(5);
    private Dictionary<TransitionKey, RuleFlags> m_transitionFlags = new Dictionary<TransitionKey, RuleFlags>(25);

    public UpdatePackageFlagManager() => this.m_baseFlags = (RuleFlags) 0;

    public UpdatePackageFlagManager(XmlElement containerElement) => this.m_baseFlags = UpdatePackageFlagManager.ExtractFlags(containerElement);

    public RuleFlags StateGetFlags(MetaID stateId, XmlElement containerElement)
    {
      RuleFlags flags = this.m_baseFlags | UpdatePackageFlagManager.ExtractFlags(containerElement);
      this.m_stateFlags[stateId] = flags;
      return flags;
    }

    public RuleFlags TransitionGetFlags(
      MetaID fromStateId,
      MetaID toStateId,
      XmlElement containerElement)
    {
      TransitionKey key = new TransitionKey(fromStateId, toStateId);
      RuleFlags flags = this.InternalTransitionGetFlags(key) | UpdatePackageFlagManager.ExtractFlags(containerElement);
      this.m_transitionFlags[key] = flags;
      return flags;
    }

    public RuleFlags ReasonGetFlags(
      MetaID fromStateId,
      MetaID toStateId,
      XmlElement containerElement)
    {
      return this.InternalTransitionGetFlags(new TransitionKey(fromStateId, toStateId)) | UpdatePackageFlagManager.ExtractFlags(containerElement);
    }

    internal static RuleFlags ExtractFlags(XmlElement containerElement)
    {
      RuleFlags flags = (RuleFlags) 0;
      if (containerElement.SelectSingleNode(ProvisionTags.AllowExistingValueRule) != null)
        flags |= RuleFlags.AllowExistingValue;
      return flags;
    }

    private RuleFlags InternalTransitionGetFlags(TransitionKey key)
    {
      RuleFlags baseFlags;
      if (!this.m_transitionFlags.TryGetValue(key, out baseFlags))
      {
        if (!this.m_stateFlags.TryGetValue(key.ToStateId, out baseFlags))
        {
          baseFlags = this.m_baseFlags;
          this.m_stateFlags[key.ToStateId] = baseFlags;
        }
        this.m_transitionFlags[key] = baseFlags;
      }
      return baseFlags;
    }

    public RuleFlags BaseFlags => this.m_baseFlags;
  }
}
