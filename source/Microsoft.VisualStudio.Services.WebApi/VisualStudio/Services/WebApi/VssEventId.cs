// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssEventId
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class VssEventId
  {
    public static readonly int DefaultEventId = 0;
    public static readonly int ExceptionBaseEventId = 3000;
    private static readonly int EtmBaseEventId = VssEventId.ExceptionBaseEventId + 1200;
    public static readonly int VssIdentityServiceException = VssEventId.EtmBaseEventId + 7;
    public static readonly int AccountException = VssEventId.EtmBaseEventId + 36;
    public static readonly int FileContainerBaseEventId = VssEventId.ExceptionBaseEventId + 1700;
  }
}
