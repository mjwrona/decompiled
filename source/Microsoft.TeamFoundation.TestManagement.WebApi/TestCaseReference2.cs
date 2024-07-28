// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseReference2
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class TestCaseReference2
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid ProjectId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestCaseId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestPointId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ConfigurationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestCaseRefId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string AutomatedTestName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string AutomatedTestStorage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string AutomatedTestType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string AutomatedTestId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TestCaseTitle { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestCaseRevision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte Priority { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Owner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int AreaId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime CreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid CreatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime LastRefTestRunDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte[] AutomatedTestNameHash { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte[] AutomatedTestStorageHash { get; set; }
  }
}
