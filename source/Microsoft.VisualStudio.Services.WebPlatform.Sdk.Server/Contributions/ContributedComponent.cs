// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributedComponent
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  [DataContract]
  public class ContributedComponent : WebSdkMetadata
  {
    [DataMember(Name = "componentId")]
    public string ComponentId;
    public string ComponentRegion;
    [DataMember(Name = "componentType")]
    public string ComponentType;
    [DataMember(Name = "componentStyle", EmitDefaultValue = false)]
    public string ComponentStyle;
    [DataMember(Name = "componentProperties", EmitDefaultValue = false)]
    public object ComponentProperties;
    [DataMember(Name = "providerName", EmitDefaultValue = false)]
    public string ProviderName;

    public ContributedComponent(
      IVssRequestContext requestContext,
      ContributionNode componentContribution)
    {
      this.ComponentId = componentContribution.Id;
      this.ComponentRegion = componentContribution.Contribution.GetProperty<string>("componentRegion");
      this.ComponentType = componentContribution.Contribution.GetProperty<string>("componentType");
      this.ComponentStyle = componentContribution.Contribution.GetProperty<string>("componentStyle");
      this.ComponentProperties = componentContribution.Contribution.GetProperty<object>("componentProperties");
      this.ProviderName = componentContribution.Contribution.GetProperty<string>("providerName");
    }
  }
}
