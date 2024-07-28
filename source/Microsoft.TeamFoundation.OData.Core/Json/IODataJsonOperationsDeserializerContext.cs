// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.IODataJsonOperationsDeserializerContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData.Json
{
  internal interface IODataJsonOperationsDeserializerContext
  {
    IJsonReader JsonReader { get; }

    Uri ProcessUriFromPayload(string uriFromPayload);

    void AddActionToResource(ODataAction action);

    void AddFunctionToResource(ODataFunction function);
  }
}
