// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.ProjectReference
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [DataContract]
  public class ProjectReference : FeedSecuredObject, IEquatable<ProjectReference>
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Visibility { get; set; }

    public override bool Equals(object obj) => this.Equals(obj as ProjectReference);

    public bool Equals(ProjectReference other)
    {
      if ((object) other == null)
        return false;
      if ((object) this == (object) other)
        return true;
      return !(this.GetType() != other.GetType()) && this.Id == other.Id && this.Name == other.Name && this.Visibility == other.Visibility;
    }

    public static bool operator ==(ProjectReference lhs, ProjectReference rhs)
    {
      if ((object) lhs != null)
        return lhs.Equals(rhs);
      return (object) rhs == null;
    }

    public static bool operator !=(ProjectReference lhs, ProjectReference rhs) => !(lhs == rhs);

    public override int GetHashCode() => ((1390590971 * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(this.Id)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Name)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Visibility);
  }
}
