// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightServiceDocumentSerializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightServiceDocumentSerializer : ODataJsonLightSerializer
  {
    internal ODataJsonLightServiceDocumentSerializer(
      ODataJsonLightOutputContext jsonLightOutputContext)
      : base(jsonLightOutputContext, true)
    {
    }

    internal void WriteServiceDocument(ODataServiceDocument serviceDocument) => this.WriteTopLevelPayload((Action) (() =>
    {
      this.JsonWriter.StartObjectScope();
      this.WriteContextUriProperty(ODataPayloadKind.ServiceDocument);
      this.JsonWriter.WriteValuePropertyName();
      this.JsonWriter.StartArrayScope();
      if (serviceDocument.EntitySets != null)
      {
        foreach (ODataServiceDocumentElement entitySet in serviceDocument.EntitySets)
          this.WriteServiceDocumentElement(entitySet, "EntitySet");
      }
      if (serviceDocument.Singletons != null)
      {
        foreach (ODataServiceDocumentElement singleton in serviceDocument.Singletons)
          this.WriteServiceDocumentElement(singleton, "Singleton");
      }
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      if (serviceDocument.FunctionImports != null)
      {
        foreach (ODataFunctionImportInfo functionImport in serviceDocument.FunctionImports)
        {
          if (functionImport == null)
            throw new ODataException(Strings.ValidationUtils_WorkspaceResourceMustNotContainNullItem);
          if (!stringSet.Contains(functionImport.Name))
          {
            stringSet.Add(functionImport.Name);
            this.WriteServiceDocumentElement((ODataServiceDocumentElement) functionImport, "FunctionImport");
          }
        }
      }
      this.JsonWriter.EndArrayScope();
      this.JsonWriter.EndObjectScope();
    }));

    private void WriteServiceDocumentElement(
      ODataServiceDocumentElement serviceDocumentElement,
      string kind)
    {
      ValidationUtils.ValidateServiceDocumentElement(serviceDocumentElement, ODataFormat.Json);
      this.JsonWriter.StartObjectScope();
      this.JsonWriter.WriteName("name");
      this.JsonWriter.WriteValue(serviceDocumentElement.Name);
      if (!string.IsNullOrEmpty(serviceDocumentElement.Title) && !serviceDocumentElement.Title.Equals(serviceDocumentElement.Name, StringComparison.Ordinal))
      {
        this.JsonWriter.WriteName("title");
        this.JsonWriter.WriteValue(serviceDocumentElement.Title);
      }
      if (kind != null)
      {
        this.JsonWriter.WriteName(nameof (kind));
        this.JsonWriter.WriteValue(kind);
      }
      this.JsonWriter.WriteName("url");
      this.JsonWriter.WriteValue(this.UriToString(serviceDocumentElement.Url));
      this.JsonWriter.EndObjectScope();
    }
  }
}
