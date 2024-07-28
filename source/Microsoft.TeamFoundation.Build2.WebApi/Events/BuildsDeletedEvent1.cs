// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Events.BuildsDeletedEvent1
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi.Events
{
  [DataContract]
  [ServiceEventObject]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BuildsDeletedEvent1
  {
    [DataMember(Name = "BuildIds")]
    private List<int> m_buildIds;

    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public int DefinitionId { get; set; }

    public List<int> BuildIds
    {
      get
      {
        if (this.m_buildIds == null)
          this.m_buildIds = new List<int>();
        return this.m_buildIds;
      }
    }
  }
}
