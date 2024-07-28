// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.SecuritySecurityConstants
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SecuritySecurityConstants
  {
    public static readonly Guid NamespaceId = new Guid("9A82C708-BFBE-4F31-984C-E860C2196781");
    public const char Separator = '/';
    public const string RootToken = "";
    public const int Read = 1;
  }
}
