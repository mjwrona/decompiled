// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.Resource
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [DataContract]
  public class Resource : IEquatable<Resource>
  {
    [DataMember(EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Id { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string Name { get; set; }

    public Resource()
    {
    }

    public Resource(string resourceType, string resourceId, string resourceName = null)
    {
      this.Type = resourceType;
      this.Id = resourceId;
      this.Name = resourceName;
    }

    public bool Equals(Resource other)
    {
      if (this.Type == other.Type)
      {
        if (!string.IsNullOrWhiteSpace(this.Id) && !string.IsNullOrWhiteSpace(other.Id))
          return string.Equals(this.Id, other.Id, StringComparison.InvariantCultureIgnoreCase);
        if (!string.IsNullOrWhiteSpace(this.Name) && !string.IsNullOrWhiteSpace(other.Name))
          return string.Equals(this.Name, other.Name, StringComparison.CurrentCultureIgnoreCase);
      }
      return false;
    }

    public override int GetHashCode() => this == null || this.Type == null ? 0 : this.Type.GetHashCode();

    public string GetScopeString() => JsonUtility.ToString((object) new Resource()
    {
      Id = this.Id,
      Type = this.Type
    });
  }
}
