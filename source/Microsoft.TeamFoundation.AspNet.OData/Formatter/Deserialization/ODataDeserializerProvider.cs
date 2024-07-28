// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializerProvider
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Net.Http;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  public abstract class ODataDeserializerProvider
  {
    public abstract ODataDeserializer GetODataDeserializer(Type type, HttpRequestMessage request);

    public abstract ODataEdmTypeDeserializer GetEdmTypeDeserializer(IEdmTypeReference edmType);
  }
}
