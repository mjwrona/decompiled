// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ClientResources
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Documents
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class ClientResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal ClientResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ClientResources.resourceMan == null)
          ClientResources.resourceMan = new ResourceManager("Microsoft.Azure.Documents.ClientResources", typeof (ClientResources).GetAssembly());
        return ClientResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ClientResources.resourceCulture;
      set => ClientResources.resourceCulture = value;
    }

    internal static string AuthTokenNotFound => ClientResources.ResourceManager.GetString(nameof (AuthTokenNotFound), ClientResources.resourceCulture);

    internal static string BadQuery_IllegalMemberAccess => ClientResources.ResourceManager.GetString(nameof (BadQuery_IllegalMemberAccess), ClientResources.resourceCulture);

    internal static string BadQuery_InvalidArrayIndexExpression => ClientResources.ResourceManager.GetString(nameof (BadQuery_InvalidArrayIndexExpression), ClientResources.resourceCulture);

    internal static string BadQuery_InvalidArrayIndexType => ClientResources.ResourceManager.GetString(nameof (BadQuery_InvalidArrayIndexType), ClientResources.resourceCulture);

    internal static string BadQuery_InvalidComparison => ClientResources.ResourceManager.GetString(nameof (BadQuery_InvalidComparison), ClientResources.resourceCulture);

    internal static string BadQuery_InvalidComparisonType => ClientResources.ResourceManager.GetString(nameof (BadQuery_InvalidComparisonType), ClientResources.resourceCulture);

    internal static string BadQuery_InvalidExpression => ClientResources.ResourceManager.GetString(nameof (BadQuery_InvalidExpression), ClientResources.resourceCulture);

    internal static string BadQuery_InvalidLeftExpression => ClientResources.ResourceManager.GetString(nameof (BadQuery_InvalidLeftExpression), ClientResources.resourceCulture);

    internal static string BadQuery_InvalidMemberAccessExpression => ClientResources.ResourceManager.GetString(nameof (BadQuery_InvalidMemberAccessExpression), ClientResources.resourceCulture);

    internal static string BadQuery_InvalidMethodCall => ClientResources.ResourceManager.GetString(nameof (BadQuery_InvalidMethodCall), ClientResources.resourceCulture);

    internal static string BadQuery_InvalidQueryType => ClientResources.ResourceManager.GetString(nameof (BadQuery_InvalidQueryType), ClientResources.resourceCulture);

    internal static string BadQuery_InvalidReturnType => ClientResources.ResourceManager.GetString(nameof (BadQuery_InvalidReturnType), ClientResources.resourceCulture);

    internal static string BadQuery_TooManySelectManyArguments => ClientResources.ResourceManager.GetString(nameof (BadQuery_TooManySelectManyArguments), ClientResources.resourceCulture);

    internal static string BadQuery_TransformQueryException => ClientResources.ResourceManager.GetString(nameof (BadQuery_TransformQueryException), ClientResources.resourceCulture);

    internal static string BadSession => ClientResources.ResourceManager.GetString(nameof (BadSession), ClientResources.resourceCulture);

    internal static string BinaryOperatorNotSupported => ClientResources.ResourceManager.GetString(nameof (BinaryOperatorNotSupported), ClientResources.resourceCulture);

    internal static string ConstantTypeIsNotSupported => ClientResources.ResourceManager.GetString(nameof (ConstantTypeIsNotSupported), ClientResources.resourceCulture);

    internal static string ConstructorInvocationNotSupported => ClientResources.ResourceManager.GetString(nameof (ConstructorInvocationNotSupported), ClientResources.resourceCulture);

    internal static string ExpectedMethodCallsMethods => ClientResources.ResourceManager.GetString(nameof (ExpectedMethodCallsMethods), ClientResources.resourceCulture);

    internal static string ExpressionTypeIsNotSupported => ClientResources.ResourceManager.GetString(nameof (ExpressionTypeIsNotSupported), ClientResources.resourceCulture);

    internal static string FailedToEvaluateSpatialExpression => ClientResources.ResourceManager.GetString(nameof (FailedToEvaluateSpatialExpression), ClientResources.resourceCulture);

    internal static string InputIsNotIDocumentQuery => ClientResources.ResourceManager.GetString(nameof (InputIsNotIDocumentQuery), ClientResources.resourceCulture);

    internal static string InvalidArgumentsCount => ClientResources.ResourceManager.GetString(nameof (InvalidArgumentsCount), ClientResources.resourceCulture);

    internal static string InvalidCallToUserDefinedFunctionProvider => ClientResources.ResourceManager.GetString(nameof (InvalidCallToUserDefinedFunctionProvider), ClientResources.resourceCulture);

    internal static string InvalidRangeError => ClientResources.ResourceManager.GetString(nameof (InvalidRangeError), ClientResources.resourceCulture);

    internal static string InvalidSkipValue => ClientResources.ResourceManager.GetString(nameof (InvalidSkipValue), ClientResources.resourceCulture);

    internal static string InvalidTakeValue => ClientResources.ResourceManager.GetString(nameof (InvalidTakeValue), ClientResources.resourceCulture);

    internal static string InvalidTypesForMethod => ClientResources.ResourceManager.GetString(nameof (InvalidTypesForMethod), ClientResources.resourceCulture);

    internal static string MediaLinkInvalid => ClientResources.ResourceManager.GetString(nameof (MediaLinkInvalid), ClientResources.resourceCulture);

    internal static string MemberBindingNotSupported => ClientResources.ResourceManager.GetString(nameof (MemberBindingNotSupported), ClientResources.resourceCulture);

    internal static string MethodNotSupported => ClientResources.ResourceManager.GetString(nameof (MethodNotSupported), ClientResources.resourceCulture);

    internal static string NotSupported => ClientResources.ResourceManager.GetString(nameof (NotSupported), ClientResources.resourceCulture);

    internal static string OnlyLINQMethodsAreSupported => ClientResources.ResourceManager.GetString(nameof (OnlyLINQMethodsAreSupported), ClientResources.resourceCulture);

    internal static string PartitionKeyExtractError => ClientResources.ResourceManager.GetString(nameof (PartitionKeyExtractError), ClientResources.resourceCulture);

    internal static string PartitionPropertyNotFound => ClientResources.ResourceManager.GetString(nameof (PartitionPropertyNotFound), ClientResources.resourceCulture);

    internal static string PartitionResolver_DatabaseAlreadyExist => ClientResources.ResourceManager.GetString(nameof (PartitionResolver_DatabaseAlreadyExist), ClientResources.resourceCulture);

    internal static string PartitionResolver_DatabaseDoesntExist => ClientResources.ResourceManager.GetString(nameof (PartitionResolver_DatabaseDoesntExist), ClientResources.resourceCulture);

    internal static string PathExpressionsOnly => ClientResources.ResourceManager.GetString(nameof (PathExpressionsOnly), ClientResources.resourceCulture);

    internal static string RangeNotFoundError => ClientResources.ResourceManager.GetString(nameof (RangeNotFoundError), ClientResources.resourceCulture);

    internal static string StringCompareToInvalidConstant => ClientResources.ResourceManager.GetString(nameof (StringCompareToInvalidConstant), ClientResources.resourceCulture);

    internal static string StringCompareToInvalidOperator => ClientResources.ResourceManager.GetString(nameof (StringCompareToInvalidOperator), ClientResources.resourceCulture);

    internal static string UdfNameIsNullOrEmpty => ClientResources.ResourceManager.GetString(nameof (UdfNameIsNullOrEmpty), ClientResources.resourceCulture);

    internal static string UnaryOperatorNotSupported => ClientResources.ResourceManager.GetString(nameof (UnaryOperatorNotSupported), ClientResources.resourceCulture);

    internal static string UnexpectedAuthTokenType => ClientResources.ResourceManager.GetString(nameof (UnexpectedAuthTokenType), ClientResources.resourceCulture);

    internal static string UnexpectedTokenType => ClientResources.ResourceManager.GetString(nameof (UnexpectedTokenType), ClientResources.resourceCulture);

    internal static string UnsupportedPartitionKey => ClientResources.ResourceManager.GetString(nameof (UnsupportedPartitionKey), ClientResources.resourceCulture);

    internal static string ValueAndAnonymousTypesAndGeometryOnly => ClientResources.ResourceManager.GetString(nameof (ValueAndAnonymousTypesAndGeometryOnly), ClientResources.resourceCulture);
  }
}
