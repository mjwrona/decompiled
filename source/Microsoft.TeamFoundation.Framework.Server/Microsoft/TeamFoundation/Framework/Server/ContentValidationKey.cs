// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContentValidationKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct ContentValidationKey
  {
    public readonly Uri Uri;
    public readonly ContentValidationScanType ScanType;
    public readonly string FileName;

    public ContentValidationKey(Uri anonymouslyAccessableContentUri, string fileName)
      : this(anonymouslyAccessableContentUri, ContentValidationUtil.GetScanTypeFromFileName(fileName), fileName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(fileName, nameof (fileName));
    }

    public ContentValidationKey(
      Uri anonymouslyAccessableContentUri,
      ContentValidationScanType scanType,
      string fileName)
    {
      if (!Enum.IsDefined(typeof (ContentValidationScanType), (object) scanType))
        throw new ArgumentException(nameof (scanType));
      this.Uri = anonymouslyAccessableContentUri;
      this.ScanType = scanType;
      this.FileName = fileName;
    }

    public override string ToString() => this.Serialize<ContentValidationKey>();
  }
}
