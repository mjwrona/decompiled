// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.CosmosLinq
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Linq
{
  public static class CosmosLinq
  {
    public static object InvokeUserDefinedFunction(string udfName, params object[] arguments) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.InvalidCallToUserDefinedFunctionProvider));
  }
}
