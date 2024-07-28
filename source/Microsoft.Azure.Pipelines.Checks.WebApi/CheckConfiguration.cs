// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.CheckConfiguration
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [DataContract]
  [KnownType(typeof (ApprovalCheckConfiguration))]
  [KnownType(typeof (GenericCheckConfiguration))]
  [JsonConverter(typeof (CheckConfigurationJsonConverter))]
  public abstract class CheckConfiguration : CheckConfigurationRef
  {
    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDisabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ModifiedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime ModifiedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Timeout { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [IgnoreDataMember]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int SettingsId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public CheckIssue Issue { get; set; }
  }
}
