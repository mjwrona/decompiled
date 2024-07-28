// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.DataAccess.ServiceEndpointExecutionRecordBinder
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Data;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.DataAccess
{
  internal sealed class ServiceEndpointExecutionRecordBinder : 
    ObjectBinder<Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord>
  {
    private SqlColumnBinder m_endpointId = new SqlColumnBinder("EndpointId");
    private SqlColumnBinder m_endpointType = new SqlColumnBinder("EndpointType");
    private SqlColumnBinder m_endpointData = new SqlColumnBinder("EndpointData");
    private SqlColumnBinder m_endpointUrl = new SqlColumnBinder("EndpointUrl");
    private SqlColumnBinder m_endpointAuthorizationParameters = new SqlColumnBinder("EndpointAuthorizationParameters");
    private SqlColumnBinder m_Id = new SqlColumnBinder("Id");
    private SqlColumnBinder m_planType = new SqlColumnBinder("PlanType");
    private SqlColumnBinder m_definitionReference = new SqlColumnBinder("DefinitionReference");
    private SqlColumnBinder m_ownerReference = new SqlColumnBinder("OwnerReference");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");
    private SqlColumnBinder m_ownerDetails = new SqlColumnBinder("OwnerDetails");

    protected override Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord Bind()
    {
      Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord endpointExecutionRecord = new Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord();
      endpointExecutionRecord.EndpointId = this.m_endpointId.GetGuid((IDataReader) this.Reader);
      Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionData endpointExecutionData1 = new Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionData();
      endpointExecutionData1.Id = this.m_Id.GetInt64((IDataReader) this.Reader);
      endpointExecutionData1.EndpointType = this.m_endpointType.GetString((IDataReader) this.Reader, true, (string) null);
      endpointExecutionData1.PlanType = this.m_planType.GetString((IDataReader) this.Reader, false);
      endpointExecutionData1.StartTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      endpointExecutionData1.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      byte? nullableByte = this.m_result.GetNullableByte((IDataReader) this.Reader);
      endpointExecutionData1.Result = nullableByte.HasValue ? new ServiceEndpointExecutionResult?((ServiceEndpointExecutionResult) nullableByte.GetValueOrDefault()) : new ServiceEndpointExecutionResult?();
      endpointExecutionData1.OwnerDetails = this.m_ownerDetails.GetString((IDataReader) this.Reader, true, (string) null);
      Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionData endpointExecutionData2 = endpointExecutionData1;
      string json1 = this.m_endpointData.GetString((IDataReader) this.Reader, false, (string) null);
      if (!string.IsNullOrWhiteSpace(json1))
        endpointExecutionData2.EndpointData = JObject.Parse(json1);
      string json2 = this.m_endpointAuthorizationParameters.GetString((IDataReader) this.Reader, true, (string) null);
      if (!string.IsNullOrWhiteSpace(json2))
        endpointExecutionData2.EndpointAuthorizationParameters = JObject.Parse(json2);
      byte[] bytes1 = this.m_definitionReference.GetBytes((IDataReader) this.Reader, false);
      if (bytes1 != null && bytes1.Length != 0)
        endpointExecutionData2.Definition = JsonUtility.Deserialize<ServiceEndpointExecutionOwner>(bytes1);
      byte[] bytes2 = this.m_ownerReference.GetBytes((IDataReader) this.Reader, false);
      if (bytes2 != null && bytes2.Length != 0)
        endpointExecutionData2.Owner = JsonUtility.Deserialize<ServiceEndpointExecutionOwner>(bytes2);
      string uriString = this.m_endpointUrl.GetString((IDataReader) this.Reader, true, string.Empty);
      if (!string.IsNullOrEmpty(uriString))
        endpointExecutionData2.EndpointUrl = new Uri(uriString);
      endpointExecutionRecord.Data = endpointExecutionData2;
      return endpointExecutionRecord;
    }
  }
}
