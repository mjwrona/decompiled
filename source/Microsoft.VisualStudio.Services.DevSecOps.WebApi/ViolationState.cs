// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.ViolationState
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  public static class ViolationState
  {
    public const string Valid = "Valid";
    public const string NotSuppressed = "NotSuppressed";
    public const string SuppressedByHashInUserSidecar = "SuppressedByHashInUserSidecar";
    public const string SuppressedByHashAndFileInUserSidecar = "SuppressedByHashAndFileInUserSidecar";
    public const string SuppressedByPlaceholderInUserSidecar = "SuppressedByPlaceholderInUserSidecar";
    public const string SuppressedByFileLocationInUserSidecar = "SuppressedByFileLocationInUserSidecar";
  }
}
