// Decompiled with JetBrains decompiler
// Type: WebGrease.ImageAssemble.ImageAssembleStrings
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace WebGrease.ImageAssemble
{
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  internal class ImageAssembleStrings
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ImageAssembleStrings()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) ImageAssembleStrings.resourceMan, (object) null))
          ImageAssembleStrings.resourceMan = new ResourceManager("WebGrease.ImageAssemble.ImageAssembleStrings", typeof (ImageAssembleStrings).Assembly);
        return ImageAssembleStrings.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ImageAssembleStrings.resourceCulture;
      set => ImageAssembleStrings.resourceCulture = value;
    }

    internal static string AdditionalDetailsMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (AdditionalDetailsMessage), ImageAssembleStrings.resourceCulture);

    internal static string BitDepthParsingErrorMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (BitDepthParsingErrorMessage), ImageAssembleStrings.resourceCulture);

    internal static string DirectoryDoesNotExistMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (DirectoryDoesNotExistMessage), ImageAssembleStrings.resourceCulture);

    internal static string DuplicateInputFilePathsMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (DuplicateInputFilePathsMessage), ImageAssembleStrings.resourceCulture);

    internal static string EightBitPNGCannotbeSpritedMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (EightBitPNGCannotbeSpritedMessage), ImageAssembleStrings.resourceCulture);

    internal static string IgnoredFilesMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (IgnoredFilesMessage), ImageAssembleStrings.resourceCulture);

    internal static string ImageHashNameUpdateFailedMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (ImageHashNameUpdateFailedMessage), ImageAssembleStrings.resourceCulture);

    internal static string ImageLoadOutofMemoryExceptionMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (ImageLoadOutofMemoryExceptionMessage), ImageAssembleStrings.resourceCulture);

    internal static string ImagePositionValues => ImageAssembleStrings.ResourceManager.GetString(nameof (ImagePositionValues), ImageAssembleStrings.resourceCulture);

    internal static string ImageSaveExternalExceptionMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (ImageSaveExternalExceptionMessage), ImageAssembleStrings.resourceCulture);

    internal static string InputFilesDuplicateParameterMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (InputFilesDuplicateParameterMessage), ImageAssembleStrings.resourceCulture);

    internal static string InputFilesMissingPositionMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (InputFilesMissingPositionMessage), ImageAssembleStrings.resourceCulture);

    internal static string InputFilesPathAndPositionMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (InputFilesPathAndPositionMessage), ImageAssembleStrings.resourceCulture);

    internal static string InputImageListNoImageMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (InputImageListNoImageMessage), ImageAssembleStrings.resourceCulture);

    internal static string InvalidImagePositionMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (InvalidImagePositionMessage), ImageAssembleStrings.resourceCulture);

    internal static string InvalidInputParameterMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (InvalidInputParameterMessage), ImageAssembleStrings.resourceCulture);

    internal static string InvalidInputParameterValueMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (InvalidInputParameterValueMessage), ImageAssembleStrings.resourceCulture);

    internal static string InvalidPaddingValueMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (InvalidPaddingValueMessage), ImageAssembleStrings.resourceCulture);

    internal static string MissingInputParameterMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (MissingInputParameterMessage), ImageAssembleStrings.resourceCulture);

    internal static string NoInputFilesMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (NoInputFilesMessage), ImageAssembleStrings.resourceCulture);

    internal static string NoInputFileToProcessMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (NoInputFileToProcessMessage), ImageAssembleStrings.resourceCulture);

    internal static string NoInputParametersMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (NoInputParametersMessage), ImageAssembleStrings.resourceCulture);

    internal static string PNGBitDepthNotSupportedMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (PNGBitDepthNotSupportedMessage), ImageAssembleStrings.resourceCulture);

    internal static string SingleInputImageCannotBeSpritedMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (SingleInputImageCannotBeSpritedMessage), ImageAssembleStrings.resourceCulture);

    internal static string SinglePNGCannotBeSpritedMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (SinglePNGCannotBeSpritedMessage), ImageAssembleStrings.resourceCulture);

    internal static string ToolCommandLineErrorMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (ToolCommandLineErrorMessage), ImageAssembleStrings.resourceCulture);

    internal static string ToolSuccessfulCompletionMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (ToolSuccessfulCompletionMessage), ImageAssembleStrings.resourceCulture);

    internal static string ToolUsageMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (ToolUsageMessage), ImageAssembleStrings.resourceCulture);

    internal static string ValueMissingForInputParameterMessage => ImageAssembleStrings.ResourceManager.GetString(nameof (ValueMissingForInputParameterMessage), ImageAssembleStrings.resourceCulture);
  }
}
