// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.HandleNullPropagationOptionHelper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System.Linq;

namespace Microsoft.AspNet.OData.Query
{
  internal static class HandleNullPropagationOptionHelper
  {
    internal const string EntityFrameworkQueryProviderNamespace = "System.Data.Entity.Internal.Linq";
    internal const string ObjectContextQueryProviderNamespaceEF5 = "System.Data.Objects.ELinq";
    internal const string ObjectContextQueryProviderNamespaceEF6 = "System.Data.Entity.Core.Objects.ELinq";
    internal const string ObjectContextQueryProviderNamespaceEFCore2 = "Microsoft.EntityFrameworkCore.Query.Internal";
    internal const string Linq2SqlQueryProviderNamespace = "System.Data.Linq";
    internal const string Linq2ObjectsQueryProviderNamespace = "System.Linq";

    public static bool IsDefined(HandleNullPropagationOption value) => value == HandleNullPropagationOption.Default || value == HandleNullPropagationOption.True || value == HandleNullPropagationOption.False;

    public static void Validate(HandleNullPropagationOption value, string parameterValue)
    {
      if (!HandleNullPropagationOptionHelper.IsDefined(value))
        throw Error.InvalidEnumArgument(parameterValue, (int) value, typeof (HandleNullPropagationOption));
    }

    public static HandleNullPropagationOption GetDefaultHandleNullPropagationOption(IQueryable query)
    {
      HandleNullPropagationOption propagationOption;
      switch (query.Provider.GetType().Namespace)
      {
        case "System.Data.Entity.Internal.Linq":
        case "System.Data.Linq":
        case "System.Data.Objects.ELinq":
        case "System.Data.Entity.Core.Objects.ELinq":
          propagationOption = HandleNullPropagationOption.False;
          break;
        default:
          propagationOption = HandleNullPropagationOption.True;
          break;
      }
      return propagationOption;
    }
  }
}
