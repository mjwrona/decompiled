// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestSettings
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.WebApi.Legacy
{
  [DataContract]
  public sealed class LegacyTestSettings
  {
    private XmlElement m_settings;

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid CreatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string CreatedByName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime CreatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TestSettingsMachineRole[] MachineRoles { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsPublic { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsAutomated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Settings
    {
      get => this.m_settings != null ? this.m_settings.OuterXml : (string) null;
      set
      {
        if (value != null)
          this.m_settings = XmlUtility.GetDocument(value).DocumentElement;
        else
          this.m_settings = (XmlElement) null;
      }
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string AreaPath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int AreaId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid LastUpdatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string LastUpdatedByName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime LastUpdated { get; set; }

    [DefaultValue(0)]
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Revision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string TeamProjectUri { get; set; }
  }
}
