// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ODataSwaggerConverter
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Microsoft.AspNet.OData
{
  public class ODataSwaggerConverter
  {
    private static readonly Uri DefaultMetadataUri = new Uri("http://localhost");
    private const string DefaultHost = "default";
    private const string DefaultbasePath = "/odata";

    public Uri MetadataUri { get; set; }

    public string Host { get; set; }

    public string BasePath { get; set; }

    public IEdmModel EdmModel { get; private set; }

    public virtual Version SwaggerVersion => new Version(2, 0);

    protected virtual JObject SwaggerDocument { get; set; }

    protected virtual JObject SwaggerPaths { get; set; }

    protected virtual JObject SwaggerTypeDefinitions { get; set; }

    public ODataSwaggerConverter(IEdmModel model)
    {
      this.EdmModel = model != null ? model : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      this.MetadataUri = ODataSwaggerConverter.DefaultMetadataUri;
      this.Host = "default";
      this.BasePath = "/odata";
    }

    public virtual JObject GetSwaggerModel()
    {
      if (this.SwaggerDocument != null)
        return this.SwaggerDocument;
      this.InitializeStart();
      this.InitializeDocument();
      this.InitializeContainer();
      this.InitializeTypeDefinitions();
      this.InitializeOperations();
      this.InitializeEnd();
      return this.SwaggerDocument;
    }

    protected virtual void InitializeStart()
    {
      this.SwaggerDocument = (JObject) null;
      this.SwaggerPaths = (JObject) null;
      this.SwaggerTypeDefinitions = (JObject) null;
    }

    protected virtual void InitializeDocument() => this.SwaggerDocument = new JObject()
    {
      {
        "swagger",
        (JToken) this.SwaggerVersion.ToString()
      },
      {
        "info",
        (JToken) new JObject()
        {
          {
            "title",
            (JToken) "OData Service"
          },
          {
            "description",
            (JToken) ("The OData Service at " + this.MetadataUri?.ToString())
          },
          {
            "version",
            (JToken) "0.1.0"
          },
          {
            "x-odata-version",
            (JToken) "4.0"
          }
        }
      },
      {
        "host",
        (JToken) this.Host
      },
      {
        "schemes",
        (JToken) new JArray((object) "http")
      },
      {
        "basePath",
        (JToken) this.BasePath
      },
      {
        "consumes",
        (JToken) new JArray((object) "application/json")
      },
      {
        "produces",
        (JToken) new JArray((object) "application/json")
      }
    };

    protected virtual void InitializeContainer()
    {
      this.SwaggerPaths = new JObject();
      this.SwaggerDocument.Add("paths", (JToken) this.SwaggerPaths);
      if (this.EdmModel.EntityContainer == null)
        return;
      foreach (IEdmEntitySet entitySet in this.EdmModel.EntityContainer.EntitySets())
      {
        this.SwaggerPaths.Add("/" + entitySet.Name, (JToken) ODataSwaggerUtilities.CreateSwaggerPathForEntitySet((IEdmNavigationSource) entitySet));
        this.SwaggerPaths.Add(ODataSwaggerUtilities.GetPathForEntity((IEdmNavigationSource) entitySet), (JToken) ODataSwaggerUtilities.CreateSwaggerPathForEntity((IEdmNavigationSource) entitySet));
      }
      foreach (IEdmOperationImport operationImport in this.EdmModel.EntityContainer.OperationImports())
        this.SwaggerPaths.Add(ODataSwaggerUtilities.GetPathForOperationImport(operationImport), (JToken) ODataSwaggerUtilities.CreateSwaggerPathForOperationImport(operationImport));
    }

    protected virtual void InitializeTypeDefinitions()
    {
      this.SwaggerTypeDefinitions = new JObject();
      this.SwaggerDocument.Add("definitions", (JToken) this.SwaggerTypeDefinitions);
      foreach (IEdmStructuredType edmStructuredType in this.EdmModel.SchemaElements.OfType<IEdmStructuredType>())
        this.SwaggerTypeDefinitions.Add(edmStructuredType.FullTypeName(), (JToken) ODataSwaggerUtilities.CreateSwaggerTypeDefinitionForStructuredType(edmStructuredType));
    }

    protected virtual void InitializeOperations()
    {
      if (this.EdmModel.EntityContainer == null)
        return;
      foreach (IEdmOperation operation in this.EdmModel.SchemaElements.OfType<IEdmOperation>())
      {
        if (operation.IsBound)
        {
          IEdmType definition = operation.Parameters.First<IEdmOperationParameter>().Type.Definition;
          if (definition.TypeKind == EdmTypeKind.Entity)
          {
            IEdmEntityType entityType = (IEdmEntityType) definition;
            foreach (IEdmEntitySet edmEntitySet in this.EdmModel.EntityContainer.EntitySets().Where<IEdmEntitySet>((Func<IEdmEntitySet, bool>) (es => es.EntityType().Equals((object) entityType))))
              this.SwaggerPaths.Add(ODataSwaggerUtilities.GetPathForOperationOfEntity(operation, (IEdmNavigationSource) edmEntitySet), (JToken) ODataSwaggerUtilities.CreateSwaggerPathForOperationOfEntity(operation, (IEdmNavigationSource) edmEntitySet));
          }
          else if (definition.TypeKind == EdmTypeKind.Collection && definition is IEdmCollectionType edmCollectionType && edmCollectionType.ElementType.Definition.TypeKind == EdmTypeKind.Entity)
          {
            IEdmEntityType entityType = (IEdmEntityType) edmCollectionType.ElementType.Definition;
            foreach (IEdmEntitySet edmEntitySet in this.EdmModel.EntityContainer.EntitySets().Where<IEdmEntitySet>((Func<IEdmEntitySet, bool>) (es => es.EntityType().Equals((object) entityType))))
              this.SwaggerPaths.Add(ODataSwaggerUtilities.GetPathForOperationOfEntitySet(operation, (IEdmNavigationSource) edmEntitySet), (JToken) ODataSwaggerUtilities.CreateSwaggerPathForOperationOfEntitySet(operation, (IEdmNavigationSource) edmEntitySet));
          }
        }
      }
    }

    protected virtual void InitializeEnd()
    {
      this.SwaggerTypeDefinitions.Add("_Error", (JToken) new JObject()
      {
        {
          "properties",
          (JToken) new JObject()
          {
            {
              "error",
              (JToken) new JObject()
              {
                {
                  "$ref",
                  (JToken) "#/definitions/_InError"
                }
              }
            }
          }
        }
      });
      this.SwaggerTypeDefinitions.Add("_InError", (JToken) new JObject()
      {
        {
          "properties",
          (JToken) new JObject()
          {
            {
              "code",
              (JToken) new JObject()
              {
                {
                  "type",
                  (JToken) "string"
                }
              }
            },
            {
              "message",
              (JToken) new JObject()
              {
                {
                  "type",
                  (JToken) "string"
                }
              }
            }
          }
        }
      });
    }
  }
}
