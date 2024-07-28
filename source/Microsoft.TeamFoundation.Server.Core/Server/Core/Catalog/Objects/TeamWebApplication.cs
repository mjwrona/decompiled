// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.TeamWebApplication
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TeamWebApplication : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.TeamFoundationWebApplication;

    public static TeamWebApplication Register(Machine machine, string displayName)
    {
      ArgumentUtility.CheckForNull<Machine>(machine, nameof (machine));
      ArgumentUtility.CheckStringForNullOrEmpty(displayName, nameof (displayName));
      TeamWebApplication teamWebApplication = machine.WebApplications.FirstOrDefault<TeamWebApplication>();
      if (teamWebApplication == null)
        teamWebApplication = machine.CreateChild<TeamWebApplication>(displayName);
      else
        teamWebApplication.DisplayName = displayName;
      return teamWebApplication;
    }
  }
}
