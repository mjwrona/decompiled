// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildLog
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class BuildLog : BuildLogReference
  {
    public BuildLog()
    {
    }

    public BuildLog(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public long LineCount { get; set; }

    [DataMember]
    public DateTime? CreatedOn { get; set; }

    [DataMember]
    public DateTime? LastChangedOn { get; set; }
  }
}
