// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.CustomSqlError
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  internal static class CustomSqlError
  {
    public const int GenericWrapperCode = 50000;
    public const int DocumentAlreadyExists = 1660000;
    public const int DocumentDoesNotExist = 1660001;
    public const int CollectionDoesNotExist = 1660002;
    public const int DocumentVersionMismatch = 1660003;
    public const int ExtensionNotInstalled = 1660004;
    public const int ExtensionRequestAlreadyExists = 1660005;
    public const int ExtensionRequestDoesNotExist = 1660006;
    public const int ExtensionAlreadyInstalled = 1660007;
    public const int MAX_SQL_ERROR = 1660007;
  }
}
