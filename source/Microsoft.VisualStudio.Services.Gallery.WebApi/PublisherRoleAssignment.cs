// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.PublisherRoleAssignment
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [DataContract]
  public class PublisherRoleAssignment
  {
    [DataMember(Name = "identity")]
    public IdentityRef Identity { get; set; }

    [DataMember(Name = "role")]
    public PublisherSecurityRole Role { get; set; }

    [DataMember(Name = "access")]
    public PublisherRoleAccess Access { get; set; }

    [DataMember(Name = "accessDisplayName")]
    public string AccessDisplayName
    {
      get
      {
        switch (this.Access)
        {
          case PublisherRoleAccess.Assigned:
            return "AccessAssigned";
          case PublisherRoleAccess.Inherited:
            return "AccessInherited";
          default:
            return "";
        }
      }
    }
  }
}
