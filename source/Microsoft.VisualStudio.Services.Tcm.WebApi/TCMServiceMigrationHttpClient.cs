// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.WebApi.TCMServiceMigrationHttpClient
// Assembly: Microsoft.VisualStudio.Services.Tcm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DCD48481-6B90-4012-9254-BC9E7077DAC8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.WebApi.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Tcm.WebApi
{
  [ResourceArea("0905EF5A-EF15-46A1-8ADD-19E722C614F5")]
  public class TCMServiceMigrationHttpClient : TCMServiceMigrationHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();

    public TCMServiceMigrationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TCMServiceMigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TCMServiceMigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TCMServiceMigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TCMServiceMigrationHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    static TCMServiceMigrationHttpClient()
    {
      TCMServiceMigrationHttpClient.s_translatedExceptions.Add("AccessDeniedException", typeof (AccessDeniedException));
      TCMServiceMigrationHttpClient.s_translatedExceptions.Add("TestObjectNotFoundException", typeof (TestObjectNotFoundException));
      TCMServiceMigrationHttpClient.s_translatedExceptions.Add("TeamProjectNotFoundException", typeof (TeamProjectNotFoundException));
      TCMServiceMigrationHttpClient.s_translatedExceptions.Add("InvalidPropertyException", typeof (InvalidPropertyException));
      TCMServiceMigrationHttpClient.s_translatedExceptions.Add("TestObjectInUseException", typeof (TestObjectInUseException));
      TCMServiceMigrationHttpClient.s_translatedExceptions.Add("TestObjectUpdatedException", typeof (TestObjectUpdatedException));
    }

    public HttpClient HttpClient => this.Client;

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) TCMServiceMigrationHttpClient.s_translatedExceptions;
  }
}
