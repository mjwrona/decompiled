// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebAccessPluginBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class WebAccessPluginBase : WebSdkMetadata
  {
    [DataMember(Name = "namespace")]
    public string @namespace;
    [DataMember(Name = "base")]
    public string @base;
  }
}
