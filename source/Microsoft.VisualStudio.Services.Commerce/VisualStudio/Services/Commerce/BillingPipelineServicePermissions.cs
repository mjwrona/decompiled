// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.BillingPipelineServicePermissions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class BillingPipelineServicePermissions
  {
    public const int ReadTable = 1;
    public const int ReadQueue = 2;
    public const int ReadRegistry = 4;
    public const int WriteToTable = 8;
    public const int WriteToQueue = 16;
    public const int RemoveFromQueue = 32;
    public const int SetRegistry = 64;
    public const int CleanupTables = 128;
    public const int All = 255;
  }
}
