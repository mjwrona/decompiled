// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ResourceReference
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public abstract class ResourceReference
  {
    public abstract string Type { get; }

    public string Name { get; set; }

    public bool Authorized { get; set; }

    public Guid? AuthorizedBy { get; set; }

    public DateTime? AuthorizedOn { get; set; }

    public int? DefinitionId { get; set; }

    public abstract string GetId();
  }
}
