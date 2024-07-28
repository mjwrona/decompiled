// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Helpers.BuildDefinitionServiceHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build2.Server.Helpers
{
  public class BuildDefinitionServiceHelper
  {
    public static bool NeedUpdateTriggers(
      IVssRequestContext requestContext,
      BuildDefinition originalDefinition,
      BuildDefinition updatedDefinition)
    {
      BuildProcess process = updatedDefinition.Process;
      if ((process != null ? (process.Type != 2 ? 1 : 0) : 1) != 0 || !requestContext.IsFeatureEnabled("DistributedTask.EnablePipelineTriggers") || updatedDefinition.Repository == null || originalDefinition.Repository == null)
        return false;
      return !updatedDefinition.Repository.Id.Equals(originalDefinition.Repository.Id, StringComparison.OrdinalIgnoreCase) || updatedDefinition.Repository.DefaultBranch == null || !updatedDefinition.Repository.DefaultBranch.Equals(originalDefinition.Repository.DefaultBranch, StringComparison.OrdinalIgnoreCase) || !updatedDefinition.GetProcess<YamlProcess>().YamlFilename.Equals(originalDefinition.GetProcess<YamlProcess>().YamlFilename, StringComparison.OrdinalIgnoreCase);
    }
  }
}
