// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.GlobalMessagesData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3690C2EA-1623-4663-B65B-BB4B63BFE368
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.dll

using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TfsCommon
{
  [DataContract]
  public class GlobalMessagesData : WebSdkMetadata
  {
    [DataMember(EmitDefaultValue = false)]
    public IList<GlobalMessageBanner> Banners { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<GlobalMessageAction> Actions { get; set; }
  }
}
