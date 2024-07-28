// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestAuthoringDetails
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int PointId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid TesterId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ConfigurationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int SuiteId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestPointState State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastUpdated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public byte? Priority { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool? IsAutomated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? RunBy { get; set; }
  }
}
