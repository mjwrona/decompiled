// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryName
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Directories
{
  [GenerateSpecificConstants(null)]
  public static class DirectoryName
  {
    [GenerateConstant(null)]
    public const string VisualStudioDirectory = "vsd";
    [GenerateConstant(null)]
    public const string AzureActiveDirectory = "aad";
    public const string ActiveDirectory = "ad";
    public const string WindowsMachineDirectory = "wmd";
    [GenerateConstant(null)]
    public const string MicrosoftAccount = "msa";
    [GenerateConstant(null)]
    public const string GitHub = "ghb";
    public const string SourceDirectory = "src";
    public const string Any = "any";
  }
}
