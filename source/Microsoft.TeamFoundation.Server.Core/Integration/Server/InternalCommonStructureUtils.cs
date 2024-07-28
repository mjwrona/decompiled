// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.InternalCommonStructureUtils
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class InternalCommonStructureUtils
  {
    internal static readonly HashSet<string> BackCompatSystemPropertyNames = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.TeamProjectPropertyName)
    {
      ProcessTemplateIdPropertyNames.CurrentProcessTemplateId,
      ProcessTemplateIdPropertyNames.OriginalProcessTemplateId,
      ProcessTemplateIdPropertyNames.ProcessTemplateType,
      ProjectApiConstants.ProcessTemplateNameProjectPropertyName,
      "System.MSPROJ",
      TeamConstants.DefaultTeamPropertyName,
      "System.SourceControlCapabilityFlags",
      "System.SourceControlGitEnabled",
      "System.SourceControlTfvcEnabled"
    };

    internal static void TranslatePropertyNames(
      CommonStructureProjectProperty[] projectProperties,
      bool reverse = false)
    {
      if (projectProperties == null)
        return;
      for (int index = 0; index < projectProperties.Length; ++index)
      {
        if (projectProperties[index] != null)
          projectProperties[index].Name = InternalCommonStructureUtils.TranslatePropertyName(projectProperties[index].Name, reverse);
      }
    }

    internal static string TranslatePropertyName(string projectPropertyName, bool reverse = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectPropertyName, nameof (projectPropertyName));
      if (!reverse)
      {
        string str = "System." + projectPropertyName;
        if (InternalCommonStructureUtils.BackCompatSystemPropertyNames.Contains(str))
          return str;
      }
      else if (InternalCommonStructureUtils.BackCompatSystemPropertyNames.Contains(projectPropertyName))
        return projectPropertyName.Substring("System.".Length);
      return projectPropertyName;
    }
  }
}
