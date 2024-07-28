// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.AccessMappingConstants
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Location
{
  [GenerateAllConstants(null)]
  public static class AccessMappingConstants
  {
    public static readonly string PublicAccessMappingMoniker = "PublicAccessMapping";
    public static readonly string ServerAccessMappingMoniker = "ServerAccessMapping";
    public static readonly string ClientAccessMappingMoniker = "ClientAccessMapping";
    public static readonly string HostGuidAccessMappingMoniker = "HostGuidAccessMapping";
    public static readonly string RootDomainMappingMoniker = "RootDomainMapping";
    public static readonly string AzureInstanceMappingMoniker = "AzureInstanceMapping";
    public static readonly string ServicePathMappingMoniker = "ServicePathMapping";
    public static readonly string ServiceDomainMappingMoniker = "ServiceDomainMapping";
    public static readonly string LegacyPublicAccessMappingMoniker = "LegacyPublicAccessMapping";
    public static readonly string MessageQueueAccessMappingMoniker = "MessageQueueAccessMapping";
    public static readonly string LegacyAppDotAccessMappingMoniker = "LegacyAppDotDomain";
    public static readonly string AffinitizedMultiInstanceAccessMappingMoniker = "AffinitizedMultiInstanceAccessMapping";
    public static readonly string VstsAccessMapping = nameof (VstsAccessMapping);
    public static readonly string DevOpsAccessMapping = "CodexAccessMapping";
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string ServiceAccessMappingMoniker = nameof (ServiceAccessMappingMoniker);
  }
}
