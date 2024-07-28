// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.DashboardResources
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class DashboardResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal DashboardResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (DashboardResources.resourceMan == null)
          DashboardResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Dashboards.WebApi.DashboardResources", typeof (DashboardResources).GetTypeInfo().Assembly);
        return DashboardResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => DashboardResources.resourceCulture;
      set => DashboardResources.resourceCulture = value;
    }

    public static string ErrorAccessDenied => DashboardResources.ResourceManager.GetString(nameof (ErrorAccessDenied), DashboardResources.resourceCulture);

    public static string ErrorCannotHaveDashboardId => DashboardResources.ResourceManager.GetString(nameof (ErrorCannotHaveDashboardId), DashboardResources.resourceCulture);

    public static string ErrorCannotHaveWidgetId => DashboardResources.ResourceManager.GetString(nameof (ErrorCannotHaveWidgetId), DashboardResources.resourceCulture);

    public static string ErrorDashboardCountExceeded => DashboardResources.ResourceManager.GetString(nameof (ErrorDashboardCountExceeded), DashboardResources.resourceCulture);

    public static string ErrorDashboardDoesNotExist => DashboardResources.ResourceManager.GetString(nameof (ErrorDashboardDoesNotExist), DashboardResources.resourceCulture);

    public static string ErrorDashboardNameMoreThanMaxLength => DashboardResources.ResourceManager.GetString(nameof (ErrorDashboardNameMoreThanMaxLength), DashboardResources.resourceCulture);

    public static string ErrorDashboardDescriptionMoreThanMaxLength => DashboardResources.ResourceManager.GetString(nameof (ErrorDashboardDescriptionMoreThanMaxLength), DashboardResources.resourceCulture);

    public static string ErrorDashboardPositionCollision => DashboardResources.ResourceManager.GetString(nameof (ErrorDashboardPositionCollision), DashboardResources.resourceCulture);

    public static string ErrorDashboardsDoNotExist => DashboardResources.ResourceManager.GetString(nameof (ErrorDashboardsDoNotExist), DashboardResources.resourceCulture);

    public static string ErrorDeletingLastDashboard => DashboardResources.ResourceManager.GetString(nameof (ErrorDeletingLastDashboard), DashboardResources.resourceCulture);

    public static string ErrorEmptyDashboardName => DashboardResources.ResourceManager.GetString(nameof (ErrorEmptyDashboardName), DashboardResources.resourceCulture);

    public static string ErrorEmptyWidgetName => DashboardResources.ResourceManager.GetString(nameof (ErrorEmptyWidgetName), DashboardResources.resourceCulture);

    public static string ErrorInvalidDashboardPosition => DashboardResources.ResourceManager.GetString(nameof (ErrorInvalidDashboardPosition), DashboardResources.resourceCulture);

    public static string ErrorInvalidMetaDataConfigurationFromCatalog => DashboardResources.ResourceManager.GetString(nameof (ErrorInvalidMetaDataConfigurationFromCatalog), DashboardResources.resourceCulture);

    public static string ErrorLimitExceeded => DashboardResources.ResourceManager.GetString(nameof (ErrorLimitExceeded), DashboardResources.resourceCulture);

    public static string ErrorMissingWidgetId => DashboardResources.ResourceManager.GetString(nameof (ErrorMissingWidgetId), DashboardResources.resourceCulture);

    public static string ErrorMoreThanMaxLength => DashboardResources.ResourceManager.GetString(nameof (ErrorMoreThanMaxLength), DashboardResources.resourceCulture);

    public static string ErrorNoMetaDataFoundInCatalog => DashboardResources.ResourceManager.GetString(nameof (ErrorNoMetaDataFoundInCatalog), DashboardResources.resourceCulture);

    public static string ErrorTeamAdminOnlyAccess => DashboardResources.ResourceManager.GetString(nameof (ErrorTeamAdminOnlyAccess), DashboardResources.resourceCulture);

    public static string ErrorWidgetCollision => DashboardResources.ResourceManager.GetString(nameof (ErrorWidgetCollision), DashboardResources.resourceCulture);

    public static string ErrorWidgetCountExceeded => DashboardResources.ResourceManager.GetString(nameof (ErrorWidgetCountExceeded), DashboardResources.resourceCulture);

    public static string ErrorWidgetDoesNotExist => DashboardResources.ResourceManager.GetString(nameof (ErrorWidgetDoesNotExist), DashboardResources.resourceCulture);

    public static string ErrorWidgetsDoNotExist => DashboardResources.ResourceManager.GetString(nameof (ErrorWidgetsDoNotExist), DashboardResources.resourceCulture);

    public static string ErrorWidgetSettingsVersionDowngrade => DashboardResources.ResourceManager.GetString(nameof (ErrorWidgetSettingsVersionDowngrade), DashboardResources.resourceCulture);

    public static string ErrorWidgetSettingsVersionInvalid => DashboardResources.ResourceManager.GetString(nameof (ErrorWidgetSettingsVersionInvalid), DashboardResources.resourceCulture);

    public static string ErrorWidgetSizeNotSupported => DashboardResources.ResourceManager.GetString(nameof (ErrorWidgetSizeNotSupported), DashboardResources.resourceCulture);

    public static string ErrorWidgetServiceUnavailable => DashboardResources.ResourceManager.GetString(nameof (ErrorWidgetServiceUnavailable), DashboardResources.resourceCulture);

    public static string ErrorDuplicateDashboardName => DashboardResources.ResourceManager.GetString(nameof (ErrorDuplicateDashboardName), DashboardResources.resourceCulture);
  }
}
