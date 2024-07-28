// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.JsonLdUris
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi
{
  public static class JsonLdUris
  {
    public static readonly Uri NuGetServicesVocab = new Uri("http://schema.nuget.org/services#");
    public static readonly Uri RdfSchemaVocab = new Uri("http://www.w3.org/2000/01/rdf-schema#");
    public static readonly Uri RdfSchemaCommentType = new Uri(JsonLdUris.RdfSchemaVocab, "#comment");
    public static readonly Uri RdfSchemaLabelType = new Uri(JsonLdUris.RdfSchemaVocab, "#label");
  }
}
