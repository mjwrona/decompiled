// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.SecureJsonResult
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Collections;
using System.Configuration;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class SecureJsonResult : JsonResult
  {
    private const int c_defaultMaxJsonLength = 20971520;
    private static int? sm_maxJsonLength;
    public const string MaxJSONLengthSettingsKey = "maxJsonLength";

    public SecureJsonResult() => this.MaxJsonLength = new int?(SecureJsonResult.DefaultMaxJsonLength);

    public static int DefaultMaxJsonLength
    {
      get
      {
        if (!SecureJsonResult.sm_maxJsonLength.HasValue)
        {
          int result;
          if (!int.TryParse(ConfigurationManager.AppSettings["maxJsonLength"], out result))
            result = 0;
          if (result <= 0)
            result = 20971520;
          SecureJsonResult.sm_maxJsonLength = new int?(result);
        }
        return SecureJsonResult.sm_maxJsonLength.Value;
      }
    }

    public override void ExecuteResult(ControllerContext context)
    {
      this.Data = this.GetSecureData();
      base.ExecuteResult(context);
    }

    public virtual object GetSecureData()
    {
      object secureData = this.Data;
      if (this.Data != null && this.Data is IEnumerable data && !(data is IDictionary))
        secureData = this.GetWrappedArray(data);
      return secureData;
    }

    protected object GetWrappedArray(IEnumerable data) => (object) new JsonArrayWrapper()
    {
      __wrappedArray = data
    };
  }
}
