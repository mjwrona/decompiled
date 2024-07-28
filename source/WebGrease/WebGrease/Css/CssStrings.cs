// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.CssStrings
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace WebGrease.Css
{
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  public class CssStrings
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal CssStrings()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) CssStrings.resourceMan, (object) null))
          CssStrings.resourceMan = new ResourceManager("WebGrease.Css.CssStrings", typeof (CssStrings).Assembly);
        return CssStrings.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => CssStrings.resourceCulture;
      set => CssStrings.resourceCulture = value;
    }

    public static string CssLowercaseValidationError => CssStrings.ResourceManager.GetString(nameof (CssLowercaseValidationError), CssStrings.resourceCulture);

    public static string CssLowercaseValidationParentNodeError => CssStrings.ResourceManager.GetString(nameof (CssLowercaseValidationParentNodeError), CssStrings.resourceCulture);

    public static string CssSelectorHackError => CssStrings.ResourceManager.GetString(nameof (CssSelectorHackError), CssStrings.resourceCulture);

    public static string DuplicateBackgroundFormatError => CssStrings.ResourceManager.GetString(nameof (DuplicateBackgroundFormatError), CssStrings.resourceCulture);

    public static string DuplicateImageReferenceWithDifferentRulesError => CssStrings.ResourceManager.GetString(nameof (DuplicateImageReferenceWithDifferentRulesError), CssStrings.resourceCulture);

    public static string ExpectedAstNode => CssStrings.ResourceManager.GetString(nameof (ExpectedAstNode), CssStrings.resourceCulture);

    public static string ExpectedEnum => CssStrings.ResourceManager.GetString(nameof (ExpectedEnum), CssStrings.resourceCulture);

    public static string ExpectedIdentifierOrString => CssStrings.ResourceManager.GetString(nameof (ExpectedIdentifierOrString), CssStrings.resourceCulture);

    public static string ExpectedOperator => CssStrings.ResourceManager.GetString(nameof (ExpectedOperator), CssStrings.resourceCulture);

    public static string ExpectedSimpleSelector => CssStrings.ResourceManager.GetString(nameof (ExpectedSimpleSelector), CssStrings.resourceCulture);

    public static string ExpectedSingleValue => CssStrings.ResourceManager.GetString(nameof (ExpectedSingleValue), CssStrings.resourceCulture);

    public static string ExpectedValue => CssStrings.ResourceManager.GetString(nameof (ExpectedValue), CssStrings.resourceCulture);

    public static string FileNotFoundError => CssStrings.ResourceManager.GetString(nameof (FileNotFoundError), CssStrings.resourceCulture);

    public static string InnerExceptionFile => CssStrings.ResourceManager.GetString(nameof (InnerExceptionFile), CssStrings.resourceCulture);

    public static string InnerExceptionSelector => CssStrings.ResourceManager.GetString(nameof (InnerExceptionSelector), CssStrings.resourceCulture);

    public static string InvalidDimensionsError => CssStrings.ResourceManager.GetString(nameof (InvalidDimensionsError), CssStrings.resourceCulture);

    public static string OriginalFileElementEmptyError => CssStrings.ResourceManager.GetString(nameof (OriginalFileElementEmptyError), CssStrings.resourceCulture);

    public static string RepeatedPropertyNameError => CssStrings.ResourceManager.GetString(nameof (RepeatedPropertyNameError), CssStrings.resourceCulture);

    public static string TooManyLengthsError => CssStrings.ResourceManager.GetString(nameof (TooManyLengthsError), CssStrings.resourceCulture);
  }
}
