// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Packaging.WorkItemTrackingTemplateArtifacts
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Packaging
{
  public class WorkItemTrackingTemplateArtifacts
  {
    public IReadOnlyCollection<WorkItemTypeCategoryDeclaration> Categories { get; protected set; }

    public IReadOnlyCollection<LinkTypeDeclaration> LinkTypes { get; protected set; }

    public IReadOnlyCollection<WorkItemTypeDeclaration> Types { get; protected set; }

    public ProcessConfigurationDeclaration ProcessConfiguration { get; protected set; }

    public IReadOnlyCollection<GlobalListDeclaration> GlobalLists { get; protected set; }

    public WorkItemTrackingTemplateArtifacts(
      ProcessTemplatePackage template,
      Action<string> logError)
    {
      WorkItemTrackingProcessTemplatePackage processTemplatePackage = new WorkItemTrackingProcessTemplatePackage(template, logError);
      this.LinkTypes = processTemplatePackage.ReadLinkTypes(logError);
      this.Categories = processTemplatePackage.ReadWorkItemTypeCategories(logError);
      this.Types = processTemplatePackage.ReadWorkItemTypes(logError);
      this.ProcessConfiguration = processTemplatePackage.ReadProcessConfiguration(logError);
      this.GlobalLists = (IReadOnlyCollection<GlobalListDeclaration>) processTemplatePackage.ReadGlobalLists(logError);
    }

    public WorkItemTrackingTemplateArtifacts()
    {
    }
  }
}
