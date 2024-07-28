// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileInformation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FileInformation : FileInformationBase
  {
    public FileInformation(string repositoryGuid, int fileId)
      : this(repositoryGuid, fileId, (byte[]) null)
    {
    }

    public FileInformation(string repositoryGuid, int fileId, byte[] hashValue)
      : base(repositoryGuid, fileId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo), hashValue)
    {
      this.FileId = fileId;
    }

    public FileInformation(Guid repositoryId, int fileId, byte[] hashValue)
      : base(repositoryId, fileId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo), hashValue)
    {
      this.FileId = fileId;
    }

    public int FileId { get; set; }

    protected override string GetRelativePath() => FileInformationBase.ComputeFilePathFromInteger(this.FileId);
  }
}
