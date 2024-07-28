// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ServiceInstanceTypes
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [GenerateAllConstants(null)]
  public static class ServiceInstanceTypes
  {
    public const string MPSString = "00000000-0000-8888-8000-000000000000";
    public static readonly Guid MPS = new Guid("00000000-0000-8888-8000-000000000000");
    public const string SPSString = "951917AC-A960-4999-8464-E3F0AA25B381";
    public static readonly Guid SPS = new Guid("951917AC-A960-4999-8464-E3F0AA25B381");
    public const string TFSString = "00025394-6065-48CA-87D9-7F5672854EF7";
    public static readonly Guid TFS = new Guid("00025394-6065-48CA-87D9-7F5672854EF7");
    public const string TFSOnPremisesString = "87966EAA-CB2A-443F-BE3C-47BD3B5BF3CB";
    public static readonly Guid TFSOnPremises = new Guid("87966EAA-CB2A-443F-BE3C-47BD3B5BF3CB");
    [Obsolete]
    public const string SpsExtensionString = "00000024-0000-8888-8000-000000000000";
    [Obsolete]
    public static readonly Guid SpsExtension = new Guid("00000024-0000-8888-8000-000000000000");
    public const string SDKSampleString = "FFFFFFFF-0000-8888-8000-000000000000";
    public static readonly Guid SDKSample = new Guid("FFFFFFFF-0000-8888-8000-000000000000");
  }
}
