// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class AgentPoolQueue : ShallowReference, ISecuredObject
  {
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    private ReferenceLinks m_links;
    private ISecuredObject m_securedObject;

    public AgentPoolQueue()
    {
    }

    internal AgentPoolQueue(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public new int Id
    {
      get => base.Id;
      set => base.Id = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public new string Name
    {
      get => base.Name;
      set => base.Name = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public new string Url
    {
      get => base.Url;
      set => base.Url = value;
    }

    [DataMember]
    public TaskAgentPoolReference Pool { get; set; }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
    }

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
