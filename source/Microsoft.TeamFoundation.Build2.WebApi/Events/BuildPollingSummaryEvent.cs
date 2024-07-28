// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Events.BuildPollingSummaryEvent
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi.Events
{
  [DataContract]
  [ServiceEventObject]
  [Obsolete("No longer used")]
  public class BuildPollingSummaryEvent
  {
    private Dictionary<string, string> m_ciData;

    public BuildPollingSummaryEvent(Dictionary<string, string> ciData) => this.m_ciData = ciData;

    public Dictionary<string, string> CIData => this.m_ciData;
  }
}
