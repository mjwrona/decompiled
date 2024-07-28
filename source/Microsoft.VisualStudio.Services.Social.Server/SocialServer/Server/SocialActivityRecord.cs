// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.SocialActivityRecord
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  public class SocialActivityRecord
  {
    public const int ActivityTypeMaxLength = 24;
    public const int DataMaxLength = 1024;
    public const int ExtendedDataMaxLength = 4000;

    public string ActivityType { get; set; }

    public Guid ActivityId { get; set; }

    public DateTime ActivityTimeStamp { get; set; }

    public DateTime? CreationTime { get; set; }

    public Guid UserId { get; set; }

    public string Data { get; set; }

    public string ExtendedData { get; set; }
  }
}
