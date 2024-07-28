// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckSuiteContextBinder2
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal sealed class CheckSuiteContextBinder2 : ObjectBinder<CheckSuiteContext>
  {
    private SqlColumnBinder m_checkSuiteId = new SqlColumnBinder("BatchRequestId");
    private SqlColumnBinder m_context = new SqlColumnBinder("Context");
    private SqlColumnBinder m_resources = new SqlColumnBinder("Resources");

    protected override CheckSuiteContext Bind()
    {
      string json = this.m_context.GetString((IDataReader) this.Reader, false);
      JObject jobject;
      try
      {
        jobject = JObject.Parse(json);
        if (!jobject.HasValues)
          jobject = (JObject) null;
      }
      catch (JsonReaderException ex)
      {
        jobject = (JObject) null;
      }
      string str = this.m_resources.GetString((IDataReader) this.Reader, true) ?? string.Empty;
      List<Resource> resourceList;
      try
      {
        resourceList = JsonConvert.DeserializeObject<List<Resource>>(str);
      }
      catch (Exception ex)
      {
        resourceList = new List<Resource>();
      }
      return new CheckSuiteContext()
      {
        SuiteId = this.m_checkSuiteId.GetGuid((IDataReader) this.Reader),
        Context = jobject,
        Resources = resourceList
      };
    }
  }
}
