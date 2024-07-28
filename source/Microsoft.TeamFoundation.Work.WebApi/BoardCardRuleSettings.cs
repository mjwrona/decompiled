// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.BoardCardRuleSettings
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class BoardCardRuleSettings
  {
    [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
    public string url { get; set; }

    [DataMember(Order = 2)]
    public Dictionary<string, List<Rule>> rules { get; set; }

    [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }
  }
}
