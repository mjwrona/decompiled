// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.PageContributionProviderDetails
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class PageContributionProviderDetails : WebSdkMetadata
  {
    [DataMember(EmitDefaultValue = false)]
    public string Name;
    [DataMember(EmitDefaultValue = false)]
    public string DisplayName;
    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> Properties;

    public static PageContributionProviderDetails FromContributionProviderDetails(
      ContributionProviderDetails details)
    {
      return new PageContributionProviderDetails()
      {
        Name = details.Name,
        DisplayName = details.DisplayName,
        Properties = (IDictionary<string, string>) details.Properties
      };
    }
  }
}
