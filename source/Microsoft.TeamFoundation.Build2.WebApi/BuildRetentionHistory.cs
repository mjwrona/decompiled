// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildRetentionHistory
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public sealed class BuildRetentionHistory : BaseSecuredObject
  {
    private IEnumerable<BuildRetentionSample> m_buildRetentionSamples;

    internal BuildRetentionHistory()
    {
    }

    internal BuildRetentionHistory(
      IEnumerable<BuildRetentionSample> buildRetentionSamples)
    {
      this.m_buildRetentionSamples = buildRetentionSamples;
    }

    [DataMember]
    public IEnumerable<BuildRetentionSample> BuildRetentionSamples
    {
      get => this.m_buildRetentionSamples ?? (this.m_buildRetentionSamples = (IEnumerable<BuildRetentionSample>) new List<BuildRetentionSample>());
      internal set => this.m_buildRetentionSamples = value;
    }
  }
}
