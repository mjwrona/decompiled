// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ExceptionUtil
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal static class ExceptionUtil
  {
    internal static ODataException CreateResourceNotFoundError(string identifier) => ExceptionUtil.ResourceNotFoundError(Microsoft.OData.Strings.RequestUriProcessor_ResourceNotFound((object) identifier));

    internal static ODataException ResourceNotFoundError(string errorMessage) => (ODataException) new ODataUnrecognizedPathException(errorMessage);

    internal static ODataException CreateSyntaxError() => ExceptionUtil.CreateBadRequestError(Microsoft.OData.Strings.RequestUriProcessor_SyntaxError);

    internal static ODataException CreateBadRequestError(string message) => new ODataException(message);

    internal static void ThrowIfTypesUnrelated(
      IEdmType type,
      IEdmType secondType,
      string segmentName)
    {
      if (!UriEdmHelpers.IsRelatedTo(type.AsElementType(), secondType.AsElementType()))
        throw new ODataException(Microsoft.OData.Strings.PathParser_TypeMustBeRelatedToSet((object) type, (object) secondType, (object) segmentName));
    }

    internal static ODataErrorException CreatePropertyNotFoundException(
      string propertyName,
      string typeName,
      bool isOpenProperty = false)
    {
      ODataError odataError = ExceptionUtil.GenerateODataError(propertyName, typeName, isOpenProperty ? "10002" : "10001");
      return new ODataErrorException(Microsoft.OData.Strings.MetadataBinder_PropertyNotDeclared((object) typeName, (object) propertyName), odataError);
    }

    private static ODataError GenerateODataError(
      string propertyName,
      string typeName,
      string errorCode)
    {
      return new ODataError()
      {
        ErrorCode = errorCode,
        Target = propertyName,
        Details = (ICollection<ODataErrorDetail>) new List<ODataErrorDetail>()
        {
          new ODataErrorDetail()
          {
            ErrorCode = errorCode,
            Target = typeName
          }
        }
      };
    }
  }
}
