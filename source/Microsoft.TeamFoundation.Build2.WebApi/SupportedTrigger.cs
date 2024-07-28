// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.SupportedTrigger
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class SupportedTrigger
  {
    [DataMember]
    public DefinitionTriggerType Type { get; set; }

    [DataMember]
    public string NotificationType { get; set; }

    [DataMember]
    public int DefaultPollingInterval { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, SupportLevel> SupportedCapabilities { get; set; }
  }
}
