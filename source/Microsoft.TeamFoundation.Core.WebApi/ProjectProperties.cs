// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ProjectProperties
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [DataContract]
  public class ProjectProperties : ISecuredObject
  {
    [DataMember]
    public Guid ProjectId;
    [DataMember]
    public IEnumerable<ProjectProperty> Properties;

    Guid ISecuredObject.NamespaceId => TeamProjectSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => TeamProjectSecurityConstants.GenericRead;

    string ISecuredObject.GetToken()
    {
      ArgumentUtility.CheckForEmptyGuid(this.ProjectId, "ProjectId");
      return TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(this.ProjectId));
    }
  }
}
