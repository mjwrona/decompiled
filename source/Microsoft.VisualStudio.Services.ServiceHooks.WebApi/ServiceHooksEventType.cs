// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.ServiceHooksEventType
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  public static class ServiceHooksEventType
  {
    public static readonly int EtmBaseEventType = 4500;
    public static readonly int ServiceHookException = ServiceHooksEventType.EtmBaseEventType + 1;
  }
}
