// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.FlagHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  internal class FlagHelper
  {
    private FlagHelper.ContextType m_type;
    private MetaID m_fromStateId;
    private MetaID m_toStateId;

    public static FlagHelper StateCreateFlagHelper(MetaID stateId) => new FlagHelper(FlagHelper.ContextType.State, stateId, (MetaID) null);

    public static FlagHelper TransitionCreateFlagHelper(MetaID fromStateId, MetaID toStateId) => new FlagHelper(FlagHelper.ContextType.Transition, fromStateId, toStateId);

    public static FlagHelper ReasonCreateFlagHelper(MetaID fromStateId, MetaID toStateId) => new FlagHelper(FlagHelper.ContextType.Reason, fromStateId, toStateId);

    public RuleFlags GetFlags(UpdatePackageFlagManager flagManager, XmlElement containerElement)
    {
      switch (this.m_type)
      {
        case FlagHelper.ContextType.State:
          return flagManager.StateGetFlags(this.m_fromStateId, containerElement);
        case FlagHelper.ContextType.Transition:
          return flagManager.TransitionGetFlags(this.m_fromStateId, this.m_toStateId, containerElement);
        case FlagHelper.ContextType.Reason:
          return flagManager.ReasonGetFlags(this.m_fromStateId, this.m_toStateId, containerElement);
        default:
          return (RuleFlags) 0;
      }
    }

    private FlagHelper(FlagHelper.ContextType type, MetaID fromStateId, MetaID toStateId)
    {
      this.m_type = type;
      this.m_fromStateId = fromStateId;
      this.m_toStateId = toStateId;
    }

    private enum ContextType
    {
      State,
      Transition,
      Reason,
    }
  }
}
