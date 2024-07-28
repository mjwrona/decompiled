// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TemporaryDataConstants
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class TemporaryDataConstants
  {
    public const string TemporaryDataAreaName = "properties";
    public static readonly Guid TemporaryDataWebApiLocationId = new Guid("B4B570EF-1775-4093-9218-AFB7E4C8AEF6");
    public static readonly Guid TemporaryDataArtifactKind = new Guid("1430a5de-0198-484a-a7ee-12a653d19161");
    public const string TemporaryDataContentValuePropertyName = "Microsoft.TeamFoundation.Server.Core.TemporaryData.Content";
    public const string TemporaryDataFileIdValuePropertyName = "Microsoft.TeamFoundation.Server.Core.TemporaryData.FileServiceId";
    public const string TemporaryDataExpirationDateValuePropertyName = "Microsoft.TeamFoundation.Server.Core.TemporaryData.ExpirationDate";
    public const string TemporaryDataOriginValuePropertyName = "Microsoft.TeamFoundation.Server.Core.TemporaryData.Origin";
    public const string TemporaryDataMaxBytesPath = "/Service/TemporaryData/TemporaryDataMaxBytes";
    public const int TemporaryDataDefaultMaxBytes = 131072;
    public static readonly string TemporaryDataCutoffIntervalRegistryPath = "/Service/TemporaryData/CleanupJobCutoffIntervalInSeconds";
    public static readonly int TemporaryData_defaultCutoffIntervalInSeconds = 31536000;
    public static readonly int TemporaryQuery_defaultCutoffIntervalInSeconds = 259200;
    public const string TemporaryDataOriginWeb = "Web";
    public const string TemporaryDataOriginWit = "Wit";
    public const string TemporaryDataOriginVS = "VS";
  }
}
