// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.ProjectAboutData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97A9928B-E499-4978-909F-1EBC8C5535AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview
{
  [DataContract]
  public class ProjectAboutData : AbstractProjectSecuredObject
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Visibility { get; set; }

    [DataMember]
    public bool IsProjectImageSet { get; set; }

    [DataMember]
    public bool IsOrganizationActivated { get; set; }
  }
}
