// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.TeamPanelItem
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class TeamPanelItem
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Href { get; set; }

    [DataMember]
    public string IconClassName { get; set; }

    [DataMember]
    public string ArtifactId { get; set; }

    [DataMember]
    public Favorite Favorite { get; set; }
  }
}
