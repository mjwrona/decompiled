// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ODataSwaggerUtilities
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Microsoft.AspNet.OData
{
  internal static class ODataSwaggerUtilities
  {
    public static JObject CreateSwaggerPathForEntitySet(IEdmNavigationSource navigationSource)
    {
      if (!(navigationSource is IEdmEntitySet navigationSource1))
        return new JObject();
      return new JObject()
      {
        {
          "get",
          (JToken) new JObject().Summary("Get EntitySet " + navigationSource1.Name).OperationId(navigationSource1.Name + "_Get").Description("Returns the EntitySet " + navigationSource1.Name).Tags(navigationSource1.Name).Parameters(new JArray().Parameter("$expand", "query", "Expand navigation property", "string").Parameter("$select", "query", "select structural property", "string").Parameter("$orderby", "query", "order by some property", "string").Parameter("$top", "query", "top elements", "integer").Parameter("$skip", "query", "skip elements", "integer").Parameter("$count", "query", "include count in response", "boolean")).Responses(new JObject().Response("200", "EntitySet " + navigationSource1.Name, (IEdmType) navigationSource1.EntityType()).DefaultErrorResponse())
        },
        {
          "post",
          (JToken) new JObject().Summary("Post a new entity to EntitySet " + navigationSource1.Name).OperationId(navigationSource1.Name + "_Post").Description("Post a new entity to EntitySet " + navigationSource1.Name).Tags(navigationSource1.Name).Parameters(new JArray().Parameter(navigationSource1.EntityType().Name, "body", "The entity to post", (IEdmType) navigationSource1.EntityType())).Responses(new JObject().Response("200", "EntitySet " + navigationSource1.Name, (IEdmType) navigationSource1.EntityType()).DefaultErrorResponse())
        }
      };
    }

    public static JObject CreateSwaggerPathForEntity(IEdmNavigationSource navigationSource)
    {
      if (!(navigationSource is IEdmEntitySet navigationSource1))
        return new JObject();
      JArray parameters = new JArray();
      foreach (IEdmStructuralProperty structuralProperty in navigationSource1.EntityType().Key())
      {
        string format;
        string primitiveTypeAndFormat = ODataSwaggerUtilities.GetPrimitiveTypeAndFormat(structuralProperty.Type.Definition as IEdmPrimitiveType, out format);
        parameters.Parameter(structuralProperty.Name, "path", "key: " + structuralProperty.Name, primitiveTypeAndFormat, format);
      }
      return new JObject()
      {
        {
          "get",
          (JToken) new JObject().Summary("Get entity from " + navigationSource1.Name + " by key.").OperationId(navigationSource1.Name + "_GetById").Description("Returns the entity with the key from " + navigationSource1.Name).Tags(navigationSource1.Name).Parameters((parameters.DeepClone() as JArray).Parameter("$select", "query", "description", "string")).Responses(new JObject().Response("200", "EntitySet " + navigationSource1.Name, (IEdmType) navigationSource1.EntityType()).DefaultErrorResponse())
        },
        {
          "patch",
          (JToken) new JObject().Summary("Update entity in EntitySet " + navigationSource1.Name).OperationId(navigationSource1.Name + "_PatchById").Description("Update entity in EntitySet " + navigationSource1.Name).Tags(navigationSource1.Name).Parameters((parameters.DeepClone() as JArray).Parameter(navigationSource1.EntityType().Name, "body", "The entity to patch", (IEdmType) navigationSource1.EntityType())).Responses(new JObject().Response("204", "Empty response").DefaultErrorResponse())
        },
        {
          "delete",
          (JToken) new JObject().Summary("Delete entity in EntitySet " + navigationSource1.Name).OperationId(navigationSource1.Name + "_DeleteById").Description("Delete entity in EntitySet " + navigationSource1.Name).Tags(navigationSource1.Name).Parameters((parameters.DeepClone() as JArray).Parameter("If-Match", "header", "If-Match header", "string")).Responses(new JObject().Response("204", "Empty response").DefaultErrorResponse())
        }
      };
    }

    public static JObject CreateSwaggerPathForOperationImport(IEdmOperationImport operationImport)
    {
      if (operationImport == null)
        return new JObject();
      bool flag = operationImport is IEdmFunctionImport;
      JArray parameters = new JArray();
      foreach (IEdmOperationParameter parameter in operationImport.Operation.Parameters)
        parameters.Parameter(parameter.Name, flag ? "path" : "body", "parameter: " + parameter.Name, parameter.Type.Definition);
      JObject responses = new JObject();
      if (operationImport.Operation.ReturnType == null)
        responses.Response("204", "Empty response");
      else
        responses.Response("200", "Response from " + operationImport.Name, operationImport.Operation.ReturnType.Definition);
      JObject jobject = new JObject().Summary("Call operation import  " + operationImport.Name).OperationId(operationImport.Name + (flag ? "_FunctionImportGet" : "_ActionImportPost")).Description("Call operation import  " + operationImport.Name).Tags(flag ? "Function Import" : "Action Import");
      if (parameters.Count > 0)
        jobject.Parameters(parameters);
      jobject.Responses(responses.DefaultErrorResponse());
      return new JObject()
      {
        {
          flag ? "get" : "post",
          (JToken) jobject
        }
      };
    }

    public static JObject CreateSwaggerPathForOperationOfEntitySet(
      IEdmOperation operation,
      IEdmNavigationSource navigationSource)
    {
      IEdmEntitySet edmEntitySet = navigationSource as IEdmEntitySet;
      if (operation == null || edmEntitySet == null)
        return new JObject();
      bool flag = operation is IEdmFunction;
      JArray parameters = new JArray();
      foreach (IEdmOperationParameter operationParameter in operation.Parameters.Skip<IEdmOperationParameter>(1))
        parameters.Parameter(operationParameter.Name, flag ? "path" : "body", "parameter: " + operationParameter.Name, operationParameter.Type.Definition);
      JObject responses = new JObject();
      if (operation.ReturnType == null)
        responses.Response("204", "Empty response");
      else
        responses.Response("200", "Response from " + operation.Name, operation.ReturnType.Definition);
      JObject jobject = new JObject().Summary("Call operation  " + operation.Name).OperationId(operation.Name + (flag ? "_FunctionGet" : "_ActionPost")).Description("Call operation  " + operation.Name).Tags(edmEntitySet.Name, flag ? "Function" : "Action");
      if (parameters.Count > 0)
        jobject.Parameters(parameters);
      jobject.Responses(responses.DefaultErrorResponse());
      return new JObject()
      {
        {
          flag ? "get" : "post",
          (JToken) jobject
        }
      };
    }

    public static JObject CreateSwaggerPathForOperationOfEntity(
      IEdmOperation operation,
      IEdmNavigationSource navigationSource)
    {
      IEdmEntitySet navigationSource1 = navigationSource as IEdmEntitySet;
      if (operation == null || navigationSource1 == null)
        return new JObject();
      bool flag = operation is IEdmFunction;
      JArray parameters = new JArray();
      foreach (IEdmStructuralProperty structuralProperty in navigationSource1.EntityType().Key())
      {
        string format;
        string primitiveTypeAndFormat = ODataSwaggerUtilities.GetPrimitiveTypeAndFormat(structuralProperty.Type.Definition as IEdmPrimitiveType, out format);
        parameters.Parameter(structuralProperty.Name, "path", "key: " + structuralProperty.Name, primitiveTypeAndFormat, format);
      }
      foreach (IEdmOperationParameter operationParameter in operation.Parameters.Skip<IEdmOperationParameter>(1))
        parameters.Parameter(operationParameter.Name, flag ? "path" : "body", "parameter: " + operationParameter.Name, operationParameter.Type.Definition);
      JObject responses = new JObject();
      if (operation.ReturnType == null)
        responses.Response("204", "Empty response");
      else
        responses.Response("200", "Response from " + operation.Name, operation.ReturnType.Definition);
      JObject jobject = new JObject().Summary("Call operation  " + operation.Name).OperationId(operation.Name + (flag ? "_FunctionGetById" : "_ActionPostById")).Description("Call operation  " + operation.Name).Tags(navigationSource1.Name, flag ? "Function" : "Action");
      if (parameters.Count > 0)
        jobject.Parameters(parameters);
      jobject.Responses(responses.DefaultErrorResponse());
      return new JObject()
      {
        {
          flag ? "get" : "post",
          (JToken) jobject
        }
      };
    }

    public static string GetPathForEntity(IEdmNavigationSource navigationSource)
    {
      if (!(navigationSource is IEdmEntitySet navigationSource1))
        return string.Empty;
      string str = "/" + navigationSource1.Name + "(";
      foreach (IEdmStructuralProperty structuralProperty in navigationSource1.EntityType().Key())
        str = structuralProperty.Type.Definition.TypeKind != EdmTypeKind.Primitive || ((IEdmPrimitiveType) structuralProperty.Type.Definition).PrimitiveKind != EdmPrimitiveTypeKind.String ? str + "{" + structuralProperty.Name + "}, " : str + "'{" + structuralProperty.Name + "}', ";
      return str.Substring(0, str.Length - 2) + ")";
    }

    public static string GetPathForOperationImport(IEdmOperationImport operationImport)
    {
      if (operationImport == null)
        return string.Empty;
      string str = "/" + operationImport.Name + "(";
      if (operationImport.IsFunctionImport())
      {
        foreach (IEdmOperationParameter parameter in operationImport.Operation.Parameters)
          str = str + parameter.Name + "={" + parameter.Name + "},";
      }
      if (str.EndsWith(",", StringComparison.Ordinal))
        str = str.Substring(0, str.Length - 1);
      return str + ")";
    }

    public static string GetPathForOperationOfEntitySet(
      IEdmOperation operation,
      IEdmNavigationSource navigationSource)
    {
      IEdmEntitySet edmEntitySet = navigationSource as IEdmEntitySet;
      if (operation == null || edmEntitySet == null)
        return string.Empty;
      string str = "/" + edmEntitySet.Name + "/" + operation.FullName() + "(";
      if (operation.IsFunction())
      {
        foreach (IEdmOperationParameter operationParameter in operation.Parameters.Skip<IEdmOperationParameter>(1))
        {
          if (operationParameter.Type.Definition.TypeKind == EdmTypeKind.Primitive && ((IEdmPrimitiveType) operationParameter.Type.Definition).PrimitiveKind == EdmPrimitiveTypeKind.String)
            str = str + operationParameter.Name + "='{" + operationParameter.Name + "}',";
          else
            str = str + operationParameter.Name + "={" + operationParameter.Name + "},";
        }
      }
      if (str.EndsWith(",", StringComparison.Ordinal))
        str = str.Substring(0, str.Length - 1);
      return str + ")";
    }

    public static string GetPathForOperationOfEntity(
      IEdmOperation operation,
      IEdmNavigationSource navigationSource)
    {
      IEdmEntitySet edmEntitySet = navigationSource as IEdmEntitySet;
      if (operation == null || edmEntitySet == null)
        return string.Empty;
      string str = ODataSwaggerUtilities.GetPathForEntity((IEdmNavigationSource) edmEntitySet) + "/" + operation.FullName() + "(";
      if (operation.IsFunction())
      {
        foreach (IEdmOperationParameter operationParameter in operation.Parameters.Skip<IEdmOperationParameter>(1))
        {
          if (operationParameter.Type.Definition.TypeKind == EdmTypeKind.Primitive && ((IEdmPrimitiveType) operationParameter.Type.Definition).PrimitiveKind == EdmPrimitiveTypeKind.String)
            str = str + operationParameter.Name + "='{" + operationParameter.Name + "}',";
          else
            str = str + operationParameter.Name + "={" + operationParameter.Name + "},";
        }
      }
      if (str.EndsWith(",", StringComparison.Ordinal))
        str = str.Substring(0, str.Length - 1);
      return str + ")";
    }

    public static JObject CreateSwaggerTypeDefinitionForStructuredType(IEdmStructuredType edmType)
    {
      if (edmType == null)
        return new JObject();
      JObject jobject1 = new JObject();
      foreach (IEdmStructuralProperty structuralProperty in edmType.StructuralProperties())
      {
        JObject jobject2 = new JObject().Description(structuralProperty.Name);
        ODataSwaggerUtilities.SetSwaggerType(jobject2, structuralProperty.Type.Definition);
        jobject1.Add(structuralProperty.Name, (JToken) jobject2);
      }
      return new JObject()
      {
        {
          "properties",
          (JToken) jobject1
        }
      };
    }

    private static void SetSwaggerType(JObject obj, IEdmType edmType)
    {
      if (edmType.TypeKind == EdmTypeKind.Complex || edmType.TypeKind == EdmTypeKind.Entity)
        obj.Add("$ref", (JToken) ("#/definitions/" + edmType.FullTypeName()));
      else if (edmType.TypeKind == EdmTypeKind.Primitive)
      {
        string format;
        string primitiveTypeAndFormat = ODataSwaggerUtilities.GetPrimitiveTypeAndFormat((IEdmPrimitiveType) edmType, out format);
        obj.Add("type", (JToken) primitiveTypeAndFormat);
        if (format == null)
          return;
        obj.Add("format", (JToken) format);
      }
      else if (edmType.TypeKind == EdmTypeKind.Enum)
      {
        obj.Add("type", (JToken) "string");
      }
      else
      {
        if (edmType.TypeKind != EdmTypeKind.Collection)
          return;
        IEdmType definition = ((IEdmCollectionType) edmType).ElementType.Definition;
        JObject jobject = new JObject();
        ODataSwaggerUtilities.SetSwaggerType(jobject, definition);
        obj.Add("type", (JToken) "array");
        obj.Add("items", (JToken) jobject);
      }
    }

    private static string GetPrimitiveTypeAndFormat(
      IEdmPrimitiveType primitiveType,
      out string format)
    {
      format = (string) null;
      switch (primitiveType.PrimitiveKind)
      {
        case EdmPrimitiveTypeKind.Boolean:
          return "boolean";
        case EdmPrimitiveTypeKind.Byte:
          format = "byte";
          return "string";
        case EdmPrimitiveTypeKind.DateTimeOffset:
          format = "date-time";
          return "string";
        case EdmPrimitiveTypeKind.Double:
          format = "double";
          return "number";
        case EdmPrimitiveTypeKind.Int16:
        case EdmPrimitiveTypeKind.Int32:
          format = "int32";
          return "integer";
        case EdmPrimitiveTypeKind.Int64:
          format = "int64";
          return "integer";
        case EdmPrimitiveTypeKind.Single:
          format = "float";
          return "number";
        case EdmPrimitiveTypeKind.String:
          return "string";
        case EdmPrimitiveTypeKind.Date:
          format = "date";
          return "string";
        default:
          return "string";
      }
    }

    private static JObject Responses(this JObject obj, JObject responses)
    {
      obj.Add(nameof (responses), (JToken) responses);
      return obj;
    }

    private static JObject ResponseRef(
      this JObject responses,
      string name,
      string description,
      string refType)
    {
      responses.Add(name, (JToken) new JObject()
      {
        {
          nameof (description),
          (JToken) description
        },
        {
          "schema",
          (JToken) new JObject()
          {
            {
              "$ref",
              (JToken) refType
            }
          }
        }
      });
      return responses;
    }

    private static JObject Response(
      this JObject responses,
      string name,
      string description,
      IEdmType type)
    {
      JObject jobject = new JObject();
      ODataSwaggerUtilities.SetSwaggerType(jobject, type);
      responses.Add(name, (JToken) new JObject()
      {
        {
          nameof (description),
          (JToken) description
        },
        {
          "schema",
          (JToken) jobject
        }
      });
      return responses;
    }

    private static JObject DefaultErrorResponse(this JObject responses) => responses.ResponseRef("default", "Unexpected error", "#/definitions/_Error");

    private static JObject Response(this JObject responses, string name, string description)
    {
      responses.Add(name, (JToken) new JObject()
      {
        {
          nameof (description),
          (JToken) description
        }
      });
      return responses;
    }

    private static JObject Parameters(this JObject obj, JArray parameters)
    {
      obj.Add(nameof (parameters), (JToken) parameters);
      return obj;
    }

    private static JArray Parameter(
      this JArray parameters,
      string name,
      string kind,
      string description,
      string type,
      string format = null)
    {
      JObject jobject = new JObject()
      {
        {
          nameof (name),
          (JToken) name
        },
        {
          "in",
          (JToken) kind
        },
        {
          nameof (description),
          (JToken) description
        },
        {
          nameof (type),
          (JToken) type
        }
      };
      if (!string.IsNullOrEmpty(format))
        jobject.Add(nameof (format), (JToken) format);
      parameters.Add((JToken) jobject);
      return parameters;
    }

    private static JArray Parameter(
      this JArray parameters,
      string name,
      string kind,
      string description,
      IEdmType type)
    {
      JObject jobject1 = new JObject()
      {
        {
          nameof (name),
          (JToken) name
        },
        {
          "in",
          (JToken) kind
        },
        {
          nameof (description),
          (JToken) description
        }
      };
      if (kind != "body")
      {
        ODataSwaggerUtilities.SetSwaggerType(jobject1, type);
      }
      else
      {
        JObject jobject2 = new JObject();
        ODataSwaggerUtilities.SetSwaggerType(jobject2, type);
        jobject1.Add("schema", (JToken) jobject2);
      }
      parameters.Add((JToken) jobject1);
      return parameters;
    }

    private static JObject Tags(this JObject obj, params string[] tags)
    {
      obj.Add(nameof (tags), (JToken) new JArray((object[]) tags));
      return obj;
    }

    private static JObject Summary(this JObject obj, string summary)
    {
      obj.Add(nameof (summary), (JToken) summary);
      return obj;
    }

    private static JObject Description(this JObject obj, string description)
    {
      obj.Add(nameof (description), (JToken) description);
      return obj;
    }

    private static JObject OperationId(this JObject obj, string operationId)
    {
      obj.Add(nameof (operationId), (JToken) operationId);
      return obj;
    }
  }
}
