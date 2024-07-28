// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTagDefinition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class WorkItemTagDefinition : ISecuredObject
  {
    private ISecuredObject m_securedObject;

    public WorkItemTagDefinition()
    {
    }

    public WorkItemTagDefinition(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    public void SetSecuredObject(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime LastUpdated { get; set; }

    [IgnoreDataMember]
    Guid ISecuredObject.NamespaceId
    {
      get
      {
        ArgumentUtility.CheckForNull<ISecuredObject>(this.m_securedObject, "m_securedObject");
        return this.m_securedObject.NamespaceId;
      }
    }

    [IgnoreDataMember]
    int ISecuredObject.RequiredPermissions
    {
      get
      {
        ArgumentUtility.CheckForNull<ISecuredObject>(this.m_securedObject, "m_securedObject");
        return this.m_securedObject.RequiredPermissions;
      }
    }

    string ISecuredObject.GetToken()
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(this.m_securedObject, "m_securedObject");
      return this.m_securedObject.GetToken();
    }
  }
}
