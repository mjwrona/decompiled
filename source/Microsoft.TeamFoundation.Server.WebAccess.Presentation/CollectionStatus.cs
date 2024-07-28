// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.CollectionStatus
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  public class CollectionStatus
  {
    public bool CollectionIsReady { get; set; }

    public bool CanCreateProject { get; set; }

    public bool ShowNewProjectControl { get; set; }

    public bool ShowNewProjectVisibilityDropDown { get; set; }

    public bool ShowProjects { get; set; }

    public bool ProjectExists { get; set; }

    public bool HasCollectionPermission { get; set; }
  }
}
