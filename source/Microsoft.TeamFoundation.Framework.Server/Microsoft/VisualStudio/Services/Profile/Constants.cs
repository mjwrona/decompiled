// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.Constants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Profile
{
  public static class Constants
  {
    public const int MaxAttributeNameLength = 128;
    public const int MaxAttributeValueLength = 1048576;
    public const int MaxAzureTableStringPropertyLength = 65536;
    public const int MaxPartitionNameLength = 128;
    public const int MaxServiceSettingNameLength = 100;
    public const int MaxAvatarSizeInBytes = 4194304;
    public const int MaxAvatarWidthInPixels = 4000;
    public const int MinAvatarWidthInPixels = 10;
    public const int MaxAvatarHeightInPixels = 4000;
    public const int MinAvatarHeightInPixels = 10;
    public const int LargeAvatarPixelSize = 220;
    public const int MediumAvatarPixelSize = 44;
    public const int SmallAvatarPixelSize = 34;
    public const AvatarImageFormat DefaultAvatarImageFormat = AvatarImageFormat.Png;
    public const int GoodAvatarQualityLevel = 80;
    public const string AvatarImagePngType = "image/png";
    public const string AvatarImageJpegType = "image/jpeg";
    public const int DefaultAttributeFileId = -1;
    public const int DefaultAttributesPartitionFileId = -1;
    public const string ProviderEmailItemKeyName = "PS.ProviderEmailAddress";
    public const string MigratingProfile = "PS.MigratingProfile";
    public const string EnableServiceIdentitiesFeature = "VisualStudio.Profile.EnableServiceIdentities";
    public const string ProfileMessageBusName = "Microsoft.VisualStudio.Services.Profile";

    [GenerateAllConstants(null)]
    public class ProfileRoutekeys
    {
      public const string AcquisitionId = "acquisitionId";
      public const string Download = "download";
      public const string Account = "account";
      public const string Campaign = "campaign";
      public const string AlternateCampaign = "wt.mc_id";
      public const string WorkflowId = "workflowId";
      public const string Compact = "compact";
      public const string Scenario = "scenario";
      public const string ClientSku = "cs";
      public const string ClientVersion = "cv";
      public const string CreateProject = "createproject";
      public const string Mkt = "mkt";
      public const string ReplyTo = "reply_to";
      public const string JavascriptNotify = "javascriptnotify";
      public const string Protocol = "protocol";
    }

    public class RemoteProfileProviderDefaults
    {
      public const string HoursToWaitBeforeAvatarUpdateLocation = "/Service/Profile/HoursToWaitBeforeAvatarUpdate";
      public const string AadMaxConcurrentAvatarSyncCallsLocation = "/Service/Profile/AadMaxConcurrentAvatarSyncCalls";
      public const string MsaMaxConcurrentAvatarSyncCallsLocation = "/Service/Profile/MsaMaxConcurrentAvatarSyncCalls";
    }
  }
}
