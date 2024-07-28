// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SwapIdentityInfo
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  public class SwapIdentityInfo
  {
    public SwapIdentityInfo()
    {
    }

    public SwapIdentityInfo(Guid id1, Guid id2)
    {
      this.Id1 = id1;
      this.Id2 = id2;
    }

    [DataMember]
    public Guid Id1 { get; private set; }

    [DataMember]
    public Guid Id2 { get; private set; }
  }
}
