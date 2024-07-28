// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountRights
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [DataContract]
  public class AccountRights
  {
    public AccountRights()
      : this(VisualStudioOnlineServiceLevel.None, string.Empty)
    {
    }

    public AccountRights(VisualStudioOnlineServiceLevel level)
      : this(level, string.Empty)
    {
    }

    public AccountRights(VisualStudioOnlineServiceLevel level, string reason)
    {
      this.Level = level;
      this.Reason = reason;
    }

    [DataMember(IsRequired = true)]
    public VisualStudioOnlineServiceLevel Level { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Reason { get; private set; }
  }
}
