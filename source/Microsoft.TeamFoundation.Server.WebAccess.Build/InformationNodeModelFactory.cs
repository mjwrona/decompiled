// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.InformationNodeModelFactory
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  internal static class InformationNodeModelFactory
  {
    public static InformationNodeModel Create(BuildInformationNode node) => !string.Equals(InformationTypes.ActivityProperties, node.Type, StringComparison.OrdinalIgnoreCase) ? (!string.Equals(InformationTypes.ActivityTracking, node.Type, StringComparison.OrdinalIgnoreCase) ? (!string.Equals(InformationTypes.AgentScopeActivityTracking, node.Type, StringComparison.OrdinalIgnoreCase) ? (!string.Equals(InformationTypes.BuildProject, node.Type, StringComparison.OrdinalIgnoreCase) ? (!string.Equals(InformationTypes.BuildStep, node.Type, StringComparison.OrdinalIgnoreCase) ? (string.Equals(InformationTypes.BuildMessage, node.Type, StringComparison.OrdinalIgnoreCase) || string.Equals(InformationTypes.BuildError, node.Type, StringComparison.OrdinalIgnoreCase) || string.Equals(InformationTypes.BuildWarning, node.Type, StringComparison.OrdinalIgnoreCase) ? (InformationNodeModel) new BuildMessageInfoModel(node) : (!string.Equals(InformationTypes.OpenedWorkItem, node.Type, StringComparison.OrdinalIgnoreCase) ? (!string.Equals(InformationTypes.ExternalLink, node.Type, StringComparison.OrdinalIgnoreCase) ? (!string.Equals(InformationTypes.ConfigurationSummary, node.Type, StringComparison.OrdinalIgnoreCase) ? (!string.Equals(InformationTypes.IntermediateLogInformation, node.Type, StringComparison.OrdinalIgnoreCase) ? new InformationNodeModel(node) : (InformationNodeModel) new IntermediateLogNodeModel(node)) : (InformationNodeModel) new BuildConfigurationModel(node)) : (InformationNodeModel) new ExternalLinkNodeModel(node)) : (InformationNodeModel) new OpenedWorkItemInfoModel(node))) : (InformationNodeModel) new BuildStepInfoModel(node)) : (InformationNodeModel) new BuildProjectInfoModel(node)) : (InformationNodeModel) new AgentScopeActivityTrackingInfoModel(node)) : (InformationNodeModel) new ActivityTrackingInfoModel(node)) : (InformationNodeModel) new ActivityPropertiesInfoModel(node);
  }
}
