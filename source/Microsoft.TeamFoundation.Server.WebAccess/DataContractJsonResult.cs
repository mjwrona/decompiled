// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.DataContractJsonResult
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class DataContractJsonResult : SecureJsonResult
  {
    public DataContractJsonResult()
    {
    }

    public DataContractJsonResult(object data) => this.Data = data;

    public override object GetSecureData() => this.Data is IEnumerable data ? this.GetWrappedArray(data) : this.Data;

    public override void ExecuteResult(ControllerContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && StringComparer.OrdinalIgnoreCase.Equals(context.HttpContext.Request.HttpMethod, "GET"))
        throw new InvalidOperationException("Json get is not allowed");
      HttpResponseBase response = context.HttpContext.Response;
      response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;
      if (this.ContentEncoding != null)
      {
        response.ContentEncoding = this.ContentEncoding;
      }
      else
      {
        response.Charset = "utf-8";
        response.ContentEncoding = Encoding.UTF8;
      }
      if (this.Data == null)
        return;
      object secureData = this.GetSecureData();
      new DataContractJsonSerializer(secureData.GetType(), (IEnumerable<Type>) new Type[1]
      {
        this.Data.GetType()
      }).WriteObject(response.OutputStream, secureData);
    }
  }
}
