// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.OperationTypeParser
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi
{
  public static class OperationTypeParser
  {
    public static IBatchOperationType Parse<T>(
      IEnumerable<IBatchOperationType> protocolSpecificOperationTypes,
      T operationEnum)
      where T : struct, IFormattable, IComparable
    {
      if (!typeof (T).GetTypeInfo().IsEnum)
        throw new ArgumentException("T must be an enum.");
      return CommonBatchOperationTypes.Types.Concat<IBatchOperationType>(protocolSpecificOperationTypes).FirstOrDefault<IBatchOperationType>((Func<IBatchOperationType, bool>) (o => o.Name.ToLower().Equals(operationEnum.ToString().ToLower()))) ?? throw new ArgumentException(string.Format("{0} is not a valid Operation Type.", (object) operationEnum));
    }
  }
}
