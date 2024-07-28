// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestResultsSession
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestResultsSession
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public List<int> TestRuns { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public long Id { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public Guid Uid { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public DateTime StartTimeUTC { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public DateTime EndTimeUTC { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Timeline<SessionTimelineType>> Timeline { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestResultsSessionState State { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public SessionResult Result { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public SessionSourcePipeline SessionSourcePipeline { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public Source Source { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Layout> Layout { get; set; }
  }
}
