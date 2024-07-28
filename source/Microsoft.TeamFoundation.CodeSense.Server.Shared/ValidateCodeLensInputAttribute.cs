// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.ValidateCodeLensInputAttribute
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class ValidateCodeLensInputAttribute : ValidateInputAttribute
  {
    public override bool ValidateAndSetParameters(HttpActionContext actionContext)
    {
      int result1 = -1;
      if (!(actionContext.ActionArguments["pathVersion"] is string str1))
        str1 = string.Empty;
      string str2 = str1;
      string[] strArray = str2.Split('@');
      bool flag = string.IsNullOrEmpty(str2) || strArray.Length != 2 || string.IsNullOrEmpty(strArray[0]) || string.IsNullOrEmpty(strArray[1]) || !int.TryParse(strArray[1], out result1);
      IVssRequestContext property = (IVssRequestContext) actionContext.ControllerContext.Request.Properties[TfsApiPropertyKeys.TfsRequestContext];
      if (property == null)
        return !flag;
      using (ExportLifetimeContext<ITfsVersionControlClient> lifeTimeExport = CodeLensExtension.CreateLifeTimeExport<ITfsVersionControlClient>(property))
        flag = flag || !lifeTimeExport.Value.IsItemPathValid(strArray[0]);
      if (!flag)
      {
        property.Items["codelensServerPath"] = (object) strArray[0];
        property.Items["codelensVersion"] = (object) result1;
        object s;
        actionContext.ActionArguments.TryGetValue("timeStamp", out s);
        DateTime result2;
        if (s != null && DateTime.TryParseExact(s as string, "yyyyMMddHHmmss", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result2))
          property.Items["timeStamp"] = (object) result2;
      }
      return !flag;
    }

    public override bool IsAPIEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("CodeSense.Server.WebAPI");
  }
}
