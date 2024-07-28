// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionManagementSecurity
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [GenerateSpecificConstants(null)]
  public static class ExtensionManagementSecurity
  {
    public static readonly Guid ExtensionManagementNamespaceId = new Guid("5d6d7b80-3c63-4ab0-b699-b6a5910f8029");
    [GenerateConstant(null)]
    public static readonly string ManageScopeId = "ems.manage.ui";
  }
}
