// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class ShallowReference : TestManagementBaseSecuredObject
  {
    public ShallowReference(string id = "", string name = "", string url = "")
    {
      this.Id = id;
      this.Name = name;
      this.Url = url;
    }

    public ShallowReference(ShallowReference shallowRef)
    {
      if (shallowRef == null)
        throw new ArgumentNullException(nameof (shallowRef));
      if (!string.IsNullOrEmpty(shallowRef.Id))
        this.Id = shallowRef.Id;
      if (!string.IsNullOrEmpty(shallowRef.Name))
        this.Name = shallowRef.Name;
      if (string.IsNullOrEmpty(shallowRef.Url))
        return;
      this.Url = shallowRef.Url;
    }

    public ShallowReference()
    {
    }

    public ShallowReference(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; set; }
  }
}
