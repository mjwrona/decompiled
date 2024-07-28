// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.GlobalMessageBanner
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3690C2EA-1623-4663-B65B-BB4B63BFE368
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TfsCommon
{
  [DataContract]
  public class GlobalMessageBanner : GlobalMessage
  {
    [DataMember(EmitDefaultValue = false)]
    public WebMessageLevel Level { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CustomIcon { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ContentContributionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ComponentType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<string> ContentDependencies { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SettingId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? Dismissable { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, object> ContentProperties { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GlobalMessagePosition Position { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime ExpirationDate { get; set; }
  }
}
