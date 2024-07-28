// Decompiled with JetBrains decompiler
// Type: Nest.ResponseBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Nest
{
  public abstract class ResponseBase : IResponse, IElasticsearchResponse
  {
    private Error _error;
    private IApiCallDetails _originalApiCall;
    private ServerError _serverError;
    private int? _statusCode;

    public virtual IApiCallDetails ApiCall => this._originalApiCall;

    public string DebugInformation
    {
      get
      {
        StringBuilder sb = new StringBuilder();
        sb.Append((!this.IsValid ? "Inv" : "V") + "alid NEST response built from a ");
        StringBuilder stringBuilder = sb;
        IApiCallDetails apiCall = this.ApiCall;
        string str = (apiCall != null ? apiCall.ToString().ToCamelCase() : (string) null) ?? "null ApiCall which is highly exceptional, please open a bug if you see this";
        stringBuilder.AppendLine(str);
        if (!this.IsValid)
          this.DebugIsValid(sb);
        if (this.ApiCall != null)
          ResponseStatics.DebugInformationBuilder(this.ApiCall, sb);
        return sb.ToString();
      }
    }

    public virtual bool IsValid
    {
      get
      {
        int? httpStatusCode = (int?) this.ApiCall?.HttpStatusCode;
        int num = 404;
        if (httpStatusCode.GetValueOrDefault() == num & httpStatusCode.HasValue)
          return false;
        IApiCallDetails apiCall = this.ApiCall;
        return (apiCall != null ? (apiCall.Success ? 1 : 0) : 0) != 0 && this.ServerError == null;
      }
    }

    public Exception OriginalException => this.ApiCall?.OriginalException;

    public ServerError ServerError
    {
      get
      {
        if (this._serverError != null)
          return this._serverError;
        if (this._error == null)
          return (ServerError) null;
        this._serverError = new ServerError(this._error, this._statusCode);
        return this._serverError;
      }
    }

    [DataMember(Name = "error")]
    internal Error Error
    {
      get => this._error;
      set
      {
        this._error = value;
        this._serverError = (ServerError) null;
      }
    }

    [DataMember(Name = "status")]
    internal int? StatusCode
    {
      get => this._statusCode;
      set
      {
        this._statusCode = value;
        this._serverError = (ServerError) null;
      }
    }

    [IgnoreDataMember]
    IApiCallDetails IElasticsearchResponse.ApiCall
    {
      get => this._originalApiCall;
      set => this._originalApiCall = value;
    }

    bool IElasticsearchResponse.TryGetServerErrorReason(out string reason)
    {
      reason = this.ServerError?.Error?.ToString();
      return !reason.IsNullOrEmpty();
    }

    protected virtual void DebugIsValid(StringBuilder sb)
    {
    }

    public override string ToString()
    {
      string str = !this.IsValid ? "Inv" : "V";
      IApiCallDetails apiCall = this.ApiCall;
      string camelCase = apiCall != null ? apiCall.ToString().ToCamelCase() : (string) null;
      return str + "alid NEST response built from a " + camelCase;
    }
  }
}
