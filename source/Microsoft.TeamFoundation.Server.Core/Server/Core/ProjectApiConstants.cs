// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectApiConstants
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectApiConstants
  {
    internal static readonly string ProcessTemplateIdProjectPropertyName = ProcessTemplateIdPropertyNames.CurrentProcessTemplateId;
    internal static readonly string ProcessTemplateNameProjectPropertyName = "System.Process Template";
    internal const string SourceControlProjectPropertyName = "System.SourceControlCapabilityFlags";
    internal const string SourceControlGitEnabled = "System.SourceControlGitEnabled";
    internal const string SourceControlTfvcEnabled = "System.SourceControlTfvcEnabled";
    internal static readonly string[] CapabilitiesProperties = new string[5]
    {
      ProjectApiConstants.ProcessTemplateIdProjectPropertyName,
      ProjectApiConstants.ProcessTemplateNameProjectPropertyName,
      "System.SourceControlCapabilityFlags",
      "System.SourceControlGitEnabled",
      "System.SourceControlTfvcEnabled"
    };
    internal const string LinksCollectionString = "collection";
  }
}
