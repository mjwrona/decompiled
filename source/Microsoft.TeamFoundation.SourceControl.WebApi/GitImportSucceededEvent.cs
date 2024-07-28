// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitImportSucceededEvent
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Notifications;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-code.git-import-succeeded-event")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitImportSucceededEvent
  {
    [DataMember]
    public string SourceRepositoryName { get; set; }

    [DataMember]
    public GitRepository TargetRepository { get; set; }
  }
}
