// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.IdentityPickerSecurityConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  public static class IdentityPickerSecurityConstants
  {
    public static readonly Guid NamespaceId = new Guid("A60E0D84-C2F8-48E4-9C0C-F32DA48D5FD1");
    public static readonly string RootToken = "/";
    public const int ReadBasic = 1;
    public const int ReadRestricted = 2;
  }
}
