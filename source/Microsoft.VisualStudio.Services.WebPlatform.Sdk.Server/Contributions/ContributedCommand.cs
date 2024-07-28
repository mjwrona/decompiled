// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributedCommand
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  [DataContract]
  public class ContributedCommand : WebSdkMetadata
  {
    [DataMember(Name = "methodName")]
    public string MethodName { get; set; }

    [DataMember(Name = "serviceName")]
    public string ServiceName { get; set; }

    [DataMember(Name = "dependencies", EmitDefaultValue = false)]
    public string[] Dependencies { get; set; }
  }
}
