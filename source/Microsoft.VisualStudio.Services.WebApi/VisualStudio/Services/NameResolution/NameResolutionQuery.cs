// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.NameResolutionQuery
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NameResolution
{
  [DataContract]
  public class NameResolutionQuery
  {
    public NameResolutionQuery()
    {
    }

    public NameResolutionQuery(string @namespace, string name)
    {
      this.Namespace = @namespace;
      this.Name = name;
    }

    [DataMember]
    public string Namespace { get; set; }

    [DataMember]
    public string Name { get; set; }

    public override bool Equals(object obj) => obj is NameResolutionQuery nameResolutionQuery && StringComparer.OrdinalIgnoreCase.Equals(this.Namespace, nameResolutionQuery.Namespace) && StringComparer.OrdinalIgnoreCase.Equals(this.Name, nameResolutionQuery.Name);

    public override int GetHashCode() => this.Namespace.GetHashCode() ^ this.Name.GetHashCode();
  }
}
