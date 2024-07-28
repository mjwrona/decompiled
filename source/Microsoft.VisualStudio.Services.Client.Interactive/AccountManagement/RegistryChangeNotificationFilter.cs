// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.RegistryChangeNotificationFilter
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public enum RegistryChangeNotificationFilter : uint
  {
    NameChange = 1,
    AttributeChange = 2,
    ValueChange = 4,
    SecurityChange = 8,
  }
}
