// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Attachment
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class Attachment : ISecuredObject
  {
    private readonly string m_token;

    public Attachment()
    {
    }

    public Attachment(string token) => this.m_token = token;

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public IdentityRef Author { get; set; }

    [DataMember]
    public DateTime? CreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ContentHash { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PropertiesCollection Properties { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    Guid ISecuredObject.NamespaceId => GitConstants.GitSecurityNamespaceId;

    int ISecuredObject.RequiredPermissions => 2;

    string ISecuredObject.GetToken()
    {
      ArgumentUtility.CheckForNull<string>(this.m_token, "m_token");
      return this.m_token;
    }
  }
}
