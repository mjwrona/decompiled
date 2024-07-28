// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle.TokenSigningKeyLifecycleServiceTracePoints
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle
{
  public static class TokenSigningKeyLifecycleServiceTracePoints
  {
    public const int GetValidationKeyEnter = 10050000;
    public const int GetValidationKeyLeave = 10050009;
    public const int GetKeyError = 10050003;
    public const int GetCachedValidationKeyEnter = 10050010;
    public const int GetCachedValidationKeyDecision = 10050011;
    public const int GetCachedValidationKeyLeave = 10050019;
    public const int GetKeyFromStronBoxEnter = 10050020;
    public const int GetKeyFromStrongBoxDecision = 10050021;
    public const int GetKeyFromStronBoxLeave = 10050029;
    public const int GetSigningKeyEnter = 10050030;
    public const int GetSigningKeyLeave = 10050039;
    public const int GetCachedSigningKeyEnter = 10050040;
    public const int GetCachedSigningKeyLeave = 10050049;
  }
}
