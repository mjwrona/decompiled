// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TeamFoundationServerInvalidResponseException
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.TeamFoundation
{
  [ExceptionMapping("0.0", "3.0", "TeamFoundationServerInvalidResponseException", "Microsoft.TeamFoundation.TeamFoundationServerInvalidResponseException, Microsoft.TeamFoundation.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class TeamFoundationServerInvalidResponseException : TeamFoundationServerException
  {
    private const string c_ResponseStatusCode = "ResponseStatusCode";
    private const string c_ResponseData = "ResponseData";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public HttpStatusCode ResponseStatusCode => (HttpStatusCode) this.Data[(object) nameof (ResponseStatusCode)];

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string ResponseData => this.Data[(object) nameof (ResponseData)] as string;

    public TeamFoundationServerInvalidResponseException(
      string message,
      Exception innerException,
      HttpStatusCode statusCode,
      string responseData)
      : base(message, innerException)
    {
      this.Data[(object) nameof (ResponseStatusCode)] = (object) statusCode;
      this.Data[(object) nameof (ResponseData)] = (object) responseData;
    }

    protected TeamFoundationServerInvalidResponseException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
      this.Data[(object) nameof (ResponseStatusCode)] = (object) (HttpStatusCode) info.GetInt32(nameof (ResponseStatusCode));
      this.Data[(object) nameof (ResponseData)] = (object) info.GetString(nameof (ResponseData));
    }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("ResponseStatusCode", (int) this.Data[(object) "ResponseStatusCode"]);
      info.AddValue("ResponseData", this.Data[(object) "ResponseData"]);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string FormatInvalidServerResponseMessage(HttpWebResponse response) => TFCommonResources.InvalidServerResponse((object) TeamFoundationServerInvalidResponseException.FormatHttpStatus(response));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string FormatHttpStatus(HttpWebResponse response) => TFCommonResources.HttpStatusInfo((object) ((int) response.StatusCode).ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) response.StatusDescription);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetResponseString(Stream responseStream)
    {
      string empty = string.Empty;
      try
      {
        byte[] numArray = new byte[65536];
        int count = responseStream.Read(numArray, 0, numArray.Length);
        empty = Encoding.UTF8.GetString(numArray, 0, count);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.Error("Error getting response data", ex);
      }
      return empty;
    }
  }
}
