// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.CommonWorkItemTrackingConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  internal static class CommonWorkItemTrackingConstants
  {
    public const char AttachmentUrlReplacementCharacter = '\a';
    public const char ProjectAttachmentReplacementCharacter = '\u0006';
    public const char TcmAttachmentUrlReplacementCharacter = '\b';
    public const string GuidRegex = "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}";
    public static readonly TimeSpan RegexMatchTimeout = TimeSpan.FromSeconds(30.0);
  }
}
