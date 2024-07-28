// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.AgileResources
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class AgileResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AgileResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (AgileResources.resourceMan == null)
          AgileResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Work.WebApi.AgileResources", typeof (AgileResources).Assembly);
        return AgileResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => AgileResources.resourceCulture;
      set => AgileResources.resourceCulture = value;
    }

    public static string CardSettingsDuplicateAdditionalFieldsException => AgileResources.ResourceManager.GetString(nameof (CardSettingsDuplicateAdditionalFieldsException), AgileResources.resourceCulture);

    public static string CardSettingsInvalidAdditionalFieldException => AgileResources.ResourceManager.GetString(nameof (CardSettingsInvalidAdditionalFieldException), AgileResources.resourceCulture);

    public static string CardSettingsInvalidFieldIdentifierException => AgileResources.ResourceManager.GetString(nameof (CardSettingsInvalidFieldIdentifierException), AgileResources.resourceCulture);

    public static string CardSettingsInvalidFormatException => AgileResources.ResourceManager.GetString(nameof (CardSettingsInvalidFormatException), AgileResources.resourceCulture);

    public static string CardSettingsMaxAdditionalFieldsExceededException => AgileResources.ResourceManager.GetString(nameof (CardSettingsMaxAdditionalFieldsExceededException), AgileResources.resourceCulture);

    public static string NoPermissionReadTeamException => AgileResources.ResourceManager.GetString(nameof (NoPermissionReadTeamException), AgileResources.resourceCulture);

    public static string NotAvailable => AgileResources.ResourceManager.GetString(nameof (NotAvailable), AgileResources.resourceCulture);

    public static string PlanLimitExceededException => AgileResources.ResourceManager.GetString(nameof (PlanLimitExceededException), AgileResources.resourceCulture);

    public static string TeamDoesNotExistException => AgileResources.ResourceManager.GetString(nameof (TeamDoesNotExistException), AgileResources.resourceCulture);

    public static string TeamFieldValuesNoTeamFieldValuesSelected => AgileResources.ResourceManager.GetString(nameof (TeamFieldValuesNoTeamFieldValuesSelected), AgileResources.resourceCulture);

    public static string ViewNotFoundExceptionMessage => AgileResources.ResourceManager.GetString(nameof (ViewNotFoundExceptionMessage), AgileResources.resourceCulture);

    public static string ViewPropertiesFormatException => AgileResources.ResourceManager.GetString(nameof (ViewPropertiesFormatException), AgileResources.resourceCulture);

    public static string ViewPropertiesMissingException => AgileResources.ResourceManager.GetString(nameof (ViewPropertiesMissingException), AgileResources.resourceCulture);

    public static string ViewRevisionMismatchException => AgileResources.ResourceManager.GetString(nameof (ViewRevisionMismatchException), AgileResources.resourceCulture);

    public static string ViewTypeDoesNotExistException => AgileResources.ResourceManager.GetString(nameof (ViewTypeDoesNotExistException), AgileResources.resourceCulture);

    public static string BoardPageWorkItemsLimitException => AgileResources.ResourceManager.GetString(nameof (BoardPageWorkItemsLimitException), AgileResources.resourceCulture);

    public static string TeamFieldValuesContainsDuplicate => AgileResources.ResourceManager.GetString(nameof (TeamFieldValuesContainsDuplicate), AgileResources.resourceCulture);
  }
}
