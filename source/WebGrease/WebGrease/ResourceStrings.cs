// Decompiled with JetBrains decompiler
// Type: WebGrease.ResourceStrings
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace WebGrease
{
  [DebuggerNonUserCode]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [CompilerGenerated]
  internal class ResourceStrings
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ResourceStrings()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) ResourceStrings.resourceMan, (object) null))
          ResourceStrings.resourceMan = new ResourceManager("WebGrease.ResourceStrings", typeof (ResourceStrings).Assembly);
        return ResourceStrings.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ResourceStrings.resourceCulture;
      set => ResourceStrings.resourceCulture = value;
    }

    internal static string BundlingFiles => ResourceStrings.ResourceManager.GetString(nameof (BundlingFiles), ResourceStrings.resourceCulture);

    internal static string ConfigurationFileParseError => ResourceStrings.ResourceManager.GetString(nameof (ConfigurationFileParseError), ResourceStrings.resourceCulture);

    internal static string DuplicateFoundFormat => ResourceStrings.ResourceManager.GetString(nameof (DuplicateFoundFormat), ResourceStrings.resourceCulture);

    internal static string ErrorsInFileFormat => ResourceStrings.ResourceManager.GetString(nameof (ErrorsInFileFormat), ResourceStrings.resourceCulture);

    internal static string FileHasheActivityCouldNotLocateDirectory => ResourceStrings.ResourceManager.GetString(nameof (FileHasheActivityCouldNotLocateDirectory), ResourceStrings.resourceCulture);

    internal static string FileHasherActivityErrorOccurred => ResourceStrings.ResourceManager.GetString(nameof (FileHasherActivityErrorOccurred), ResourceStrings.resourceCulture);

    internal static string GeneralErrorMessage => ResourceStrings.ResourceManager.GetString(nameof (GeneralErrorMessage), ResourceStrings.resourceCulture);

    internal static string InvalidBundlingOutputFile => ResourceStrings.ResourceManager.GetString(nameof (InvalidBundlingOutputFile), ResourceStrings.resourceCulture);

    internal static string MinifyingCssFilesAndSpritingBackgroundImages => ResourceStrings.ResourceManager.GetString(nameof (MinifyingCssFilesAndSpritingBackgroundImages), ResourceStrings.resourceCulture);

    internal static string MoreThan256Colours => ResourceStrings.ResourceManager.GetString(nameof (MoreThan256Colours), ResourceStrings.resourceCulture);

    internal static string MultipleSwitches => ResourceStrings.ResourceManager.GetString(nameof (MultipleSwitches), ResourceStrings.resourceCulture);

    internal static string NoFilesProcessed => ResourceStrings.ResourceManager.GetString(nameof (NoFilesProcessed), ResourceStrings.resourceCulture);

    internal static string OverrideFileLoadErrorMessage => ResourceStrings.ResourceManager.GetString(nameof (OverrideFileLoadErrorMessage), ResourceStrings.resourceCulture);

    internal static string PreprocessingCouldNotFindThePluginPath => ResourceStrings.ResourceManager.GetString(nameof (PreprocessingCouldNotFindThePluginPath), ResourceStrings.resourceCulture);

    internal static string PreprocessingEngineFound => ResourceStrings.ResourceManager.GetString(nameof (PreprocessingEngineFound), ResourceStrings.resourceCulture);

    internal static string PreprocessingInitializeEnd => ResourceStrings.ResourceManager.GetString(nameof (PreprocessingInitializeEnd), ResourceStrings.resourceCulture);

    internal static string PreprocessingInitializeStart => ResourceStrings.ResourceManager.GetString(nameof (PreprocessingInitializeStart), ResourceStrings.resourceCulture);

    internal static string PreprocessingLoadingError => ResourceStrings.ResourceManager.GetString(nameof (PreprocessingLoadingError), ResourceStrings.resourceCulture);

    internal static string PreprocessingPluginPath => ResourceStrings.ResourceManager.GetString(nameof (PreprocessingPluginPath), ResourceStrings.resourceCulture);

    internal static string ResolvingTokensAndPerformingLocalization => ResourceStrings.ResourceManager.GetString(nameof (ResolvingTokensAndPerformingLocalization), ResourceStrings.resourceCulture);

    internal static string ResourcePivotActivityDuplicateKeysError => ResourceStrings.ResourceManager.GetString(nameof (ResourcePivotActivityDuplicateKeysError), ResourceStrings.resourceCulture);

    internal static string ResourcePivotActivityError => ResourceStrings.ResourceManager.GetString(nameof (ResourcePivotActivityError), ResourceStrings.resourceCulture);

    internal static string ResourceResolverDuplicateKeyExceptionMessage => ResourceStrings.ResourceManager.GetString(nameof (ResourceResolverDuplicateKeyExceptionMessage), ResourceStrings.resourceCulture);

    internal static string SafeLockFailedMessage => ResourceStrings.ResourceManager.GetString(nameof (SafeLockFailedMessage), ResourceStrings.resourceCulture);

    internal static string SemiTransparencyFound => ResourceStrings.ResourceManager.GetString(nameof (SemiTransparencyFound), ResourceStrings.resourceCulture);

    internal static string ThereWereErrorsWhileApplyingCssresources => ResourceStrings.ResourceManager.GetString(nameof (ThereWereErrorsWhileApplyingCssresources), ResourceStrings.resourceCulture);

    internal static string ThereWereErrorsWhileBundlingFiles => ResourceStrings.ResourceManager.GetString(nameof (ThereWereErrorsWhileBundlingFiles), ResourceStrings.resourceCulture);

    internal static string ThereWereErrorsWhileMinifyingTheCssFiles => ResourceStrings.ResourceManager.GetString(nameof (ThereWereErrorsWhileMinifyingTheCssFiles), ResourceStrings.resourceCulture);

    internal static string Usage => ResourceStrings.ResourceManager.GetString(nameof (Usage), ResourceStrings.resourceCulture);
  }
}
