// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Messages.SecurityMessageConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Security.Messages
{
  public static class SecurityMessageConstants
  {
    public const string SecurityMessageBusName = "Microsoft.VisualStudio.Services.Security";
    public const int NoWorkPerformedSequenceId = 0;
    public const int DropCacheSequenceId = -2;
    public const int UnconditionalRefreshSequenceId = -3;
    public const string SkipSendingSecurityServiceBusMessage = "SkipSendingSecurityServiceBusMessage";
    public const string DisableSkipServiceBusMessageDuringServicing = "VisualStudio.Services.Security.DisableSkipServiceBusMessageDuringServicing";
  }
}
