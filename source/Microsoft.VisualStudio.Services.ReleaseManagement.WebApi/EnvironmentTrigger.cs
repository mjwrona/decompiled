// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class EnvironmentTrigger : ReleaseManagementSecuredObject
  {
    public EnvironmentTrigger()
    {
    }

    public EnvironmentTrigger(
      int releaseDefinitionId,
      int definitionEnvironmentId,
      EnvironmentTriggerType triggerType,
      string triggerContent)
    {
      this.ReleaseDefinitionId = releaseDefinitionId;
      this.DefinitionEnvironmentId = definitionEnvironmentId;
      this.TriggerType = triggerType;
      this.TriggerContent = triggerContent;
    }

    [DataMember]
    public int ReleaseDefinitionId { get; set; }

    [DataMember]
    public int DefinitionEnvironmentId { get; set; }

    [DataMember]
    public EnvironmentTriggerType TriggerType { get; set; }

    [DataMember]
    public string TriggerContent { get; set; }

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj)
    {
      if (obj == null || obj.GetType() != this.GetType())
        return false;
      EnvironmentTrigger environmentTrigger = (EnvironmentTrigger) obj;
      return environmentTrigger.ReleaseDefinitionId == this.ReleaseDefinitionId && environmentTrigger.DefinitionEnvironmentId == this.DefinitionEnvironmentId && environmentTrigger.TriggerType == this.TriggerType && environmentTrigger.TriggerContent == this.TriggerContent;
    }
  }
}
