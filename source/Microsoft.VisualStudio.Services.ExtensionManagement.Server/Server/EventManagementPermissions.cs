// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.EventManagementPermissions
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [GenerateSpecificConstants(null)]
  public static class EventManagementPermissions
  {
    [GenerateConstant(null)]
    public const int ExtensionPublish = 1;
    [GenerateConstant(null)]
    public const int Publish = 2;
    public const int AllPermissions = 3;
  }
}
