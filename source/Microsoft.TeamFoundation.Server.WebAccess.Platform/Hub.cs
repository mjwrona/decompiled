// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Hub
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class Hub : WebSdkMetadata
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string GroupId { get; set; }

    [DataMember]
    public string Uri { get; set; }

    [DataMember]
    public double Order { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsSelected { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool BuiltIn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Hidden { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Icon { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool SupportsXHRNavigate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string AriaLabel { get; set; }
  }
}
