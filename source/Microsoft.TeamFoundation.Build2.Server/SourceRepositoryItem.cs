// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.SourceRepositoryItem
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class SourceRepositoryItem
  {
    [DataMember(EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsContainer { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Path { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Uri Url { get; set; }
  }
}
