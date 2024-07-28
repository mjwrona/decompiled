// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Grafana.GrafanaConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Grafana
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class GrafanaConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal GrafanaConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (GrafanaConsumerResources.resourceMan == null)
          GrafanaConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Grafana.GrafanaConsumerResources", typeof (GrafanaConsumerResources).Assembly);
        return GrafanaConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => GrafanaConsumerResources.resourceCulture;
      set => GrafanaConsumerResources.resourceCulture = value;
    }

    internal static string AddAnnotationAction_ApiTokenInputDescription => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_ApiTokenInputDescription), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_ApiTokenInputName => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_ApiTokenInputName), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_DashboardIdInputDescription => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_DashboardIdInputDescription), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_DashboardIdInputName => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_DashboardIdInputName), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_DeploymentDurationInputDescription => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_DeploymentDurationInputDescription), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_DeploymentDurationInputName => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_DeploymentDurationInputName), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_ErrorWhileExtractingJToken => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_ErrorWhileExtractingJToken), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_QueryDashboardValues_InvalidInputIdError => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_QueryDashboardValues_InvalidInputIdError), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_TagsInputDescription => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_TagsInputDescription), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_TagsInputName => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_TagsInputName), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_TextDefaultDescription => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_TextDefaultDescription), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_TextInputDescription => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_TextInputDescription), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_TextInputName => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_TextInputName), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_UrlInputDescription => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_UrlInputDescription), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationAction_UrlInputName => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationAction_UrlInputName), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationActionDescription => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationActionDescription), GrafanaConsumerResources.resourceCulture);

    internal static string AddAnnotationActionName => GrafanaConsumerResources.ResourceManager.GetString(nameof (AddAnnotationActionName), GrafanaConsumerResources.resourceCulture);

    internal static string AnnotationDefaultDescriptionIfDeploymentIsNotThere => GrafanaConsumerResources.ResourceManager.GetString(nameof (AnnotationDefaultDescriptionIfDeploymentIsNotThere), GrafanaConsumerResources.resourceCulture);

    internal static string ConsumerDescription => GrafanaConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), GrafanaConsumerResources.resourceCulture);

    internal static string ConsumerName => GrafanaConsumerResources.ResourceManager.GetString(nameof (ConsumerName), GrafanaConsumerResources.resourceCulture);

    internal static string ErrorWhileCreatingGrafanaAnnotationPayLoad => GrafanaConsumerResources.ResourceManager.GetString(nameof (ErrorWhileCreatingGrafanaAnnotationPayLoad), GrafanaConsumerResources.resourceCulture);

    internal static string GrafanaTextLinkFormat => GrafanaConsumerResources.ResourceManager.GetString(nameof (GrafanaTextLinkFormat), GrafanaConsumerResources.resourceCulture);

    internal static string InvalidTagLength => GrafanaConsumerResources.ResourceManager.GetString(nameof (InvalidTagLength), GrafanaConsumerResources.resourceCulture);

    internal static string QueryExceptionFormat => GrafanaConsumerResources.ResourceManager.GetString(nameof (QueryExceptionFormat), GrafanaConsumerResources.resourceCulture);

    internal static string QueryResponseFailureFormat => GrafanaConsumerResources.ResourceManager.GetString(nameof (QueryResponseFailureFormat), GrafanaConsumerResources.resourceCulture);

    internal static string SuppliedTokenNotAuthorized => GrafanaConsumerResources.ResourceManager.GetString(nameof (SuppliedTokenNotAuthorized), GrafanaConsumerResources.resourceCulture);
  }
}
