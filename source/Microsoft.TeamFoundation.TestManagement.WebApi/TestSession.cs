// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestSession
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestSession
  {
    public TestSession() => this.PropertyBag = new PropertyBag();

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Title { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef LastUpdatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    public DateTime LastUpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    public DateTime StartDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    public DateTime EndDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Area { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Revision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestSessionSource Source { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TestSessionState State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef Owner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PropertyBag PropertyBag { get; set; }
  }
}
