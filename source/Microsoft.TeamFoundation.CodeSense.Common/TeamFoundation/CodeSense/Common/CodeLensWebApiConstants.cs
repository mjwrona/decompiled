// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Common.CodeLensWebApiConstants
// Assembly: Microsoft.TeamFoundation.CodeSense.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 77D96756-A6EC-4CC5-958E-440F0412CE7F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.CodeSense.Common
{
  [GenerateAllConstants(null)]
  public class CodeLensWebApiConstants
  {
    public const string AreaId = "DB4B1D4B-13B4-4CEB-8F84-1001B5500EBC";
    public const string AreaName = "codelens";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string InstanceType = "0000000F-0000-8888-8000-000000000000";
    public const string DateTimeFormat = "yyyyMMddHHmmss";
    public const string FilesLocationIdString = "81F2AE10-F9F2-4008-B40A-C3AC826050D4";
    public const string FileSummariesLocationIdString = "347D49F3-3E68-4F0F-A82A-DF4CE88999D7";
    public const string PathParameterKey = "codelensServerPath";
    public const string PathVersionParameterKey = "pathVersion";
    public const string TimeStampKey = "timeStamp";
    public const string VersionParameterKey = "codelensVersion";
    public const char VersionSeparator = '@';
    public static readonly Guid FilesLocationId = new Guid("81F2AE10-F9F2-4008-B40A-C3AC826050D4");
    public static readonly Guid FileSummariesLocationId = new Guid("347D49F3-3E68-4F0F-A82A-DF4CE88999D7");
  }
}
