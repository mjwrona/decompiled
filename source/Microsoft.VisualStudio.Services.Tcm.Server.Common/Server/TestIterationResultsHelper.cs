// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestIterationResultsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TestIterationResultsHelper
  {
    private const int c_singleIdActionPathLength = 8;

    public static bool IsValidActionPath(IVssRequestContext requestContext, string actionPath)
    {
      if (string.IsNullOrEmpty(actionPath) || actionPath.Length % 8 != 0)
      {
        requestContext.TraceVerbose("BusinessLayer", "Action Path '" + actionPath + "' is not valid.");
        return false;
      }
      if (actionPath.Length > 8)
      {
        foreach (IEnumerable<char> source in actionPath.Batch<char>(8))
        {
          string actionPath1 = new string(source.ToArray<char>());
          if (!TestIterationResultsHelper.IsValidActionPath(requestContext, actionPath1))
          {
            requestContext.TraceVerbose("BusinessLayer", "Action Path '" + actionPath1 + "' is not valid in '" + actionPath + "'.");
            return false;
          }
        }
        return true;
      }
      int result = 0;
      return int.TryParse(actionPath, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out result) && result > 0;
    }

    public static string GenerateActionPath(
      IVssRequestContext requestContext,
      int id,
      string parentActionPath)
    {
      if (8 >= id.ToString("x").Length)
        requestContext.TraceVerbose("BusinessLayer", string.Format("Int32.MaxValue in hexadecimal is 7FFFFFFF which is 8 characters. Provided Id={0}", (object) id));
      return parentActionPath + string.Format(string.Format("{{0:x{0}}}", (object) 8), (object) id);
    }
  }
}
