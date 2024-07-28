// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.PageContribution
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class PageContribution : WebSdkMetadata
  {
    [DataMember(EmitDefaultValue = false)]
    public string Id;
    [DataMember(EmitDefaultValue = false)]
    public string Type;
    [DataMember(EmitDefaultValue = false)]
    public List<string> Includes;
    [DataMember(EmitDefaultValue = false)]
    public List<string> Targets;
    [DataMember(EmitDefaultValue = false)]
    public JObject Properties;

    public static PageContribution FromContribution(Contribution contribution) => new PageContribution()
    {
      Id = contribution.Id,
      Type = contribution.Type,
      Properties = contribution.Properties,
      Includes = contribution.Includes,
      Targets = contribution.Targets
    };
  }
}
