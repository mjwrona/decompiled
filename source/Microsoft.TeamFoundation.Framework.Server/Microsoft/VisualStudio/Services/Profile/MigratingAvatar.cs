// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.MigratingAvatar
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Profile
{
  [DataContract]
  public class MigratingAvatar
  {
    [DataMember]
    public byte[] SmallAvatar { get; set; }

    [DataMember]
    public byte[] LargeAvatar { get; set; }

    [DataMember]
    public byte[] MediumAvatar { get; set; }

    [DataMember]
    public AvatarImageFormat AvatarImageFormat { get; set; }

    [DataMember]
    public bool HasAvatar { get; set; }

    [DataMember]
    public DateTimeOffset TimeStamp { get; set; }
  }
}
