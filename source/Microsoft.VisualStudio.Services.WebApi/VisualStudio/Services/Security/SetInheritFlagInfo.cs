// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.SetInheritFlagInfo
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Security
{
  [DataContract]
  public sealed class SetInheritFlagInfo
  {
    public SetInheritFlagInfo(string token, bool inherit)
    {
      this.Token = token;
      this.Inherit = inherit;
    }

    [DataMember]
    public string Token { get; private set; }

    [DataMember]
    public bool Inherit { get; private set; }
  }
}
