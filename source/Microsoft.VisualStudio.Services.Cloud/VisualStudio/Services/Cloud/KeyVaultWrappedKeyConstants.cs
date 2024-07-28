// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KeyVaultWrappedKeyConstants
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ExcludeFromCodeCoverage]
  internal static class KeyVaultWrappedKeyConstants
  {
    private const string BaseName = "KeyVaultWrappedKey";

    internal static class Settings
    {
      internal static readonly TimeSpan DefaultKeyIdTimeToLive = TimeSpan.FromHours(1.0);
    }

    internal static class TracePoints
    {
      public static readonly string Area = "KeyVaultWrappedKey";
      public static readonly string Layer = "Service";
      internal const int WrapKey_Enter = 90003321;
      internal const int WrapKey_Exception = 90003322;
      internal const int WrapKey_Leave = 90003323;
      internal const int UnwrapKey_Always = 90003324;
      internal const int UnwrapKey_Enter = 90003331;
      internal const int UnwrapKey_Exception = 90003332;
      internal const int UnwrapKey_Leave = 90003333;
      internal const int DefaultKeyVaultCredentials = 90003341;
      internal const int GetKeyVaultClientAdapter = 90003342;
      internal const int GetPrimaryKeyEncryptionKeyIdentifier_Count = 90003351;
      internal const int GetPrimaryKeyEncryptionKeyIdentifier_None = 90003352;
      internal const int GetPrimaryKeyEncryptionKeyIdentifier_NoneValid = 90003353;
    }
  }
}
