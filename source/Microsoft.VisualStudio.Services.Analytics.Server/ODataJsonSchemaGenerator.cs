// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ODataJsonSchemaGenerator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class ODataJsonSchemaGenerator : IODataJsonSchemaGenerator
  {
    private IEdmModel Model;
    private Dictionary<string, bool> referenceNameDictionary;
    protected ODataUriParser Parser;

    public ODataJsonSchemaGenerator(IEdmModel model, ODataJsonSchemaGeneratorData data)
    {
      this.Model = model;
      this.Parser = this.GetParser(model, data.EntitySet, data.ODataQuery);
      this.referenceNameDictionary = new Dictionary<string, bool>();
    }

    public ODataJsonSchemaGenerator(
      IEdmModel model,
      ODataJsonSchemaGeneratorData data,
      Dictionary<string, bool> referenceNameDictionary)
    {
      this.Model = model;
      this.Parser = this.GetParser(model, data.EntitySet, data.ODataQuery);
      this.referenceNameDictionary = referenceNameDictionary;
    }

    protected virtual ODataMashupFunction[] GetListTransformationsAnnotation() => (ODataMashupFunction[]) null;

    protected virtual ODataMashupFunction[] GetRecordTransformationsAnnotation() => (ODataMashupFunction[]) null;

    protected virtual SelectItemTranslator<KeyValuePair<string, ODataJsonSchema>?> GetSelectItemTranslator(
      IEdmModel model)
    {
      return (SelectItemTranslator<KeyValuePair<string, ODataJsonSchema>?>) new ODataJsonSchemaSelectItemTranslator(model);
    }

    private ODataUriParser GetParser(IEdmModel model, string entitySet, string odataQuery)
    {
      UriBuilder uriBuilder = new UriBuilder("http", "schemagenerator");
      string absoluteUri1 = uriBuilder.Uri.AbsoluteUri;
      uriBuilder.Path = entitySet;
      uriBuilder.Query = odataQuery;
      string absoluteUri2 = uriBuilder.Uri.AbsoluteUri;
      return new ODataUriParser(model, new Uri(absoluteUri1), new Uri(absoluteUri2));
    }

    public ODataJsonSchema Generate()
    {
      ODataJsonSchema odataJsonSchema = new ODataJsonSchema()
      {
        Type = ODataJsonSchemaType.Array,
        Items = new ODataJsonSchema()
        {
          Type = ODataJsonSchemaType.Object,
          Properties = (IDictionary<string, ODataJsonSchema>) new Dictionary<string, ODataJsonSchema>(),
          TransformationsAnnotation = this.GetRecordTransformationsAnnotation()
        },
        TransformationsAnnotation = this.GetListTransformationsAnnotation()
      };
      SelectExpandClause selectAndExpand = this.Parser.ParseSelectAndExpand();
      if (selectAndExpand != null)
      {
        SelectItemTranslator<KeyValuePair<string, ODataJsonSchema>?> selectItemTranslator = this.GetSelectItemTranslator(this.Model);
        List<KeyValuePair<string, ODataJsonSchema>> keyValuePairList = new List<KeyValuePair<string, ODataJsonSchema>>();
        foreach (SelectItem selectedItem in selectAndExpand.SelectedItems)
        {
          KeyValuePair<string, ODataJsonSchema>? nullable = selectedItem.TranslateWith<KeyValuePair<string, ODataJsonSchema>?>(selectItemTranslator);
          if (nullable.HasValue)
          {
            KeyValuePair<string, ODataJsonSchema> keyValuePair = nullable.Value;
            string displayName;
            if (string.IsNullOrEmpty(keyValuePair.Value.DisplayNameAnnotation))
            {
              keyValuePair = nullable.Value;
              IDictionary<string, ODataJsonSchema> properties = keyValuePair.Value.Properties;
              if (properties == null)
              {
                displayName = (string) null;
              }
              else
              {
                keyValuePair = properties.FirstOrDefault<KeyValuePair<string, ODataJsonSchema>>();
                displayName = keyValuePair.Value?.DisplayNameAnnotation;
              }
            }
            else
            {
              keyValuePair = nullable.Value;
              displayName = keyValuePair.Value.DisplayNameAnnotation;
            }
            this.UpdateReferenceNameDictionary(displayName);
            keyValuePairList.Add(nullable.Value);
          }
        }
        foreach (KeyValuePair<string, ODataJsonSchema> keyValuePair in keyValuePairList)
        {
          string displayNameAnnotation = keyValuePair.Value.DisplayNameAnnotation;
          string referenceNameAnnotation = keyValuePair.Value.ReferenceNameAnnotation;
          if (this.ShouldAppendReferenceName(displayNameAnnotation, referenceNameAnnotation))
            keyValuePair.Value.DisplayNameAnnotation = displayNameAnnotation + " (" + referenceNameAnnotation + ")";
          odataJsonSchema.Items.Properties.Add(keyValuePair);
        }
      }
      return odataJsonSchema;
    }

    public void UpdateReferenceNameDictionary(string displayName)
    {
      if (displayName == null)
        return;
      if (this.referenceNameDictionary.ContainsKey(displayName))
        this.referenceNameDictionary[displayName] = true;
      else
        this.referenceNameDictionary.Add(displayName, false);
    }

    public bool ShouldAppendReferenceName(string displayName, string referenceName)
    {
      bool flag = !string.IsNullOrEmpty(referenceName) && !referenceName.StartsWith("System.") && !referenceName.StartsWith("Microsoft.VSTS.");
      return !string.IsNullOrEmpty(displayName) & flag && this.referenceNameDictionary[displayName];
    }
  }
}
