// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.BuildOutcome
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

namespace Microsoft.VisualStudio.Services.Analytics
{
  public static class BuildOutcome
  {
    public const int None = 0;
    public const string None_Text = "None";
    public const int Succeed = 2;
    public const string Succeed_Text = "Succeed";
    public const int PartiallySucceeded = 4;
    public const string PartiallySucceeded_Text = "PartiallySucceeded";
    public const int Failed = 8;
    public const string Failed_Text = "Failed";
    public const int Canceled = 32;
    public const string Canceled_Text = "Canceled";
  }
}
