// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MailTrace
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class MailTrace
  {
    public const int Min = 1001000;
    public const int SmtpServiceStartStop = 1001000;
    public const int IrisServiceStartStop = 1001001;
    public const int SmtpConfigChange = 1001020;
    public const int IrisConfigChange = 1001021;
    public const int SmtpSendMail = 1001030;
    public const int MailSender = 1001031;
    public const int MailSenderException = 1001032;
    public const int SmtpQueueMail = 1001040;
    public const int SmtpConfigureMail = 1001050;
    public const int SmtpLoadSettings = 1001060;
    public const int IrisLoadSettings = 1001061;
    public const int SmtpLoadSettingsBrandingError = 1001065;
    public const int SmtpSelectServer = 1001070;
    public const int MailQueueAddressParseError = 1001090;
    public const int MailQueueAddressUnrecoverableParseError = 1001091;
    public const int IrisDuplicateHeadersUnsuppported = 1001092;
    public const int IrisHeaderUnsuppported = 1001093;
    public const int CreatingPublisher = 1001094;
    public const int AcquireMsalTokenFailed = 1001095;
    public const int MsalTokenAuthMetadata = 1001096;
    public const int Max = 1001099;
  }
}
