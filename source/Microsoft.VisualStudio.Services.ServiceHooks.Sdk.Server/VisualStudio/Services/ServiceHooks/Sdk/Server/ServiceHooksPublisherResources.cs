// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ServiceHooksPublisherResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class ServiceHooksPublisherResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ServiceHooksPublisherResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ServiceHooksPublisherResources.resourceMan == null)
          ServiceHooksPublisherResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ServiceHooksPublishing.ServiceHooksPublisherResources", typeof (ServiceHooksPublisherResources).Assembly);
        return ServiceHooksPublisherResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ServiceHooksPublisherResources.resourceCulture;
      set => ServiceHooksPublisherResources.resourceCulture = value;
    }

    internal static string Error_CannotOverrideConsumerSettings => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_CannotOverrideConsumerSettings), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_DuplicateEventTypeRegistrationFormat => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_DuplicateEventTypeRegistrationFormat), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_DuplicatePublisherIdentifier => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_DuplicatePublisherIdentifier), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_EditPermissionDenied => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_EditPermissionDenied), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_EventNotAllowedAtCollectionLevel => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_EventNotAllowedAtCollectionLevel), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_FailedToStartPublisherTemplate => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_FailedToStartPublisherTemplate), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_InvalidEventIdFormat => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_InvalidEventIdFormat), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_InvalidPullRequestPayload => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_InvalidPullRequestPayload), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_MissingRequiredPublisherInputFormat => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_MissingRequiredPublisherInputFormat), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_NoRegisteredHandlerForSHEventType => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_NoRegisteredHandlerForSHEventType), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_NotificationSubscriptionIdNotSpecifiedOnUpdateFormat => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_NotificationSubscriptionIdNotSpecifiedOnUpdateFormat), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_NotificationSubscriptionNoLongerExistsFormat => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_NotificationSubscriptionNoLongerExistsFormat), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_OnPremisesNotSupported => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_OnPremisesNotSupported), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_SubscriberIdMustBeGroupFormat => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_SubscriberIdMustBeGroupFormat), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_TestNotificationProjectScope => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_TestNotificationProjectScope), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_TestWithRealDataNotSupportedForEventType => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_TestWithRealDataNotSupportedForEventType), ServiceHooksPublisherResources.resourceCulture);

    internal static string Error_UnknownPublisherInputFormat => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (Error_UnknownPublisherInputFormat), ServiceHooksPublisherResources.resourceCulture);

    internal static string TestNotification_MessageDetailsFormat => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (TestNotification_MessageDetailsFormat), ServiceHooksPublisherResources.resourceCulture);

    internal static string TestNotification_MessageFormat => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (TestNotification_MessageFormat), ServiceHooksPublisherResources.resourceCulture);

    internal static string TfsEventDescriptionNone => ServiceHooksPublisherResources.ResourceManager.GetString(nameof (TfsEventDescriptionNone), ServiceHooksPublisherResources.resourceCulture);
  }
}
