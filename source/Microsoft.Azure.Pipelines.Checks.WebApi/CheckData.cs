// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.CheckData
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [DataContract]
  public class CheckData
  {
    private IDictionary<string, string> m_defaultCheckSettings;

    [DataMember]
    public IList<CheckConfigurationData> CheckConfigurationDataList { get; set; }

    [DataMember]
    public IList<TimeZone> TimeZoneList { get; set; }

    [DataMember]
    public IList<CheckDefinitionData> CheckDefinitions { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public IDictionary<string, string> DefaultCheckSettings
    {
      get
      {
        if (this.m_defaultCheckSettings == null)
          this.m_defaultCheckSettings = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_defaultCheckSettings;
      }
    }
  }
}
