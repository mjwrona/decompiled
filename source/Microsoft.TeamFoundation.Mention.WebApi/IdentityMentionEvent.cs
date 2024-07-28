// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.WebApi.IdentityMentionEvent
// Assembly: Microsoft.TeamFoundation.Mention.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A216308E-82C1-47B3-82AD-22E5F3709BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.WebApi.dll

using Microsoft.VisualStudio.Services.Notifications;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Mention.WebApi
{
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-mentions.identity-mention-event")]
  [DataContract]
  public class IdentityMentionEvent : BaseMentionEvent
  {
  }
}
