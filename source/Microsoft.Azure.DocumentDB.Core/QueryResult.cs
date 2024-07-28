// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.QueryResult
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace Microsoft.Azure.Documents
{
  internal sealed class QueryResult : IDynamicMetaObjectProvider
  {
    private readonly JContainer jObject;
    private readonly string ownerFullName;
    private JsonSerializer jsonSerializer;

    public QueryResult(JContainer jObject, string ownerFullName, JsonSerializer jsonSerializer)
    {
      this.jObject = jObject;
      this.ownerFullName = ownerFullName;
      this.jsonSerializer = jsonSerializer;
    }

    public QueryResult(
      JContainer jObject,
      string ownerFullName,
      JsonSerializerSettings serializerSettings = null)
      : this(jObject, ownerFullName, serializerSettings != null ? JsonSerializer.Create(serializerSettings) : JsonSerializer.Create())
    {
    }

    public JContainer Payload => this.jObject;

    public string OwnerFullName => this.ownerFullName;

    public JsonSerializer JsonSerializer => this.jsonSerializer;

    public override string ToString()
    {
      using (StringWriter stringWriter = new StringWriter())
      {
        this.jsonSerializer.Serialize((TextWriter) stringWriter, (object) this.jObject);
        return stringWriter.ToString();
      }
    }

    private IEnumerable<string> GetDynamicMemberNames()
    {
      List<string> source = new List<string>();
      if (this.jObject is JObject jObject)
      {
        foreach (KeyValuePair<string, JToken> keyValuePair in jObject)
          source.Add(keyValuePair.Key);
      }
      return (IEnumerable<string>) source.ToList<string>();
    }

    private object Convert(Type type)
    {
      if ((object) type == (object) typeof (object))
        return (object) this;
      object obj;
      if ((object) type == (object) typeof (Database))
      {
        Database database = new Database();
        database.propertyBag = this.jObject as JObject;
        obj = (object) database;
      }
      else if ((object) type == (object) typeof (DocumentCollection))
      {
        DocumentCollection documentCollection = new DocumentCollection();
        documentCollection.propertyBag = this.jObject as JObject;
        obj = (object) documentCollection;
      }
      else if ((object) type == (object) typeof (User))
      {
        User user = new User();
        user.propertyBag = this.jObject as JObject;
        obj = (object) user;
      }
      else if ((object) type == (object) typeof (UserDefinedType))
      {
        UserDefinedType userDefinedType = new UserDefinedType();
        userDefinedType.propertyBag = this.jObject as JObject;
        obj = (object) userDefinedType;
      }
      else if ((object) type == (object) typeof (Permission))
      {
        Permission permission = new Permission();
        permission.propertyBag = this.jObject as JObject;
        obj = (object) permission;
      }
      else if ((object) type == (object) typeof (Attachment))
      {
        Attachment attachment = new Attachment();
        attachment.propertyBag = this.jObject as JObject;
        obj = (object) attachment;
      }
      else if ((object) type == (object) typeof (Document))
      {
        Document document = new Document();
        document.propertyBag = this.jObject as JObject;
        obj = (object) document;
      }
      else if ((object) type == (object) typeof (Conflict))
      {
        Conflict conflict = new Conflict();
        conflict.propertyBag = this.jObject as JObject;
        obj = (object) conflict;
      }
      else if ((object) type == (object) typeof (Trigger))
      {
        Trigger trigger = new Trigger();
        trigger.propertyBag = this.jObject as JObject;
        obj = (object) trigger;
      }
      else if ((object) type == (object) typeof (Offer))
        obj = (object) OfferTypeResolver.ResponseOfferTypeResolver.Resolve(this.jObject as JObject);
      else if (CustomTypeExtensions.IsAssignableFrom(typeof (Document), type))
      {
        obj = this.jsonSerializer == null ? (object) (Resource) this.jObject.ToObject(type) : (object) (Resource) this.jObject.ToObject(type, this.jsonSerializer);
        ((JsonSerializable) obj).propertyBag = this.jObject as JObject;
      }
      else if (CustomTypeExtensions.IsAssignableFrom(typeof (Attachment), type))
      {
        obj = (object) (Resource) this.jObject.ToObject(type);
        ((JsonSerializable) obj).propertyBag = this.jObject as JObject;
      }
      else if ((object) type == (object) typeof (Schema))
      {
        Schema schema = new Schema();
        schema.propertyBag = this.jObject as JObject;
        obj = (object) schema;
      }
      else
        obj = this.jsonSerializer == null ? this.jObject.ToObject(type) : this.jObject.ToObject(type, this.jsonSerializer);
      if (obj is Resource resource)
        resource.AltLink = PathsHelper.GeneratePathForNameBased(type, this.ownerFullName, resource.Id);
      return obj;
    }

    private object GetProperty(string propertyName, Type returnType) => (this.jObject[(object) propertyName] ?? throw new DocumentClientException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.PropertyNotFound, (object) propertyName), (Exception) null, new HttpStatusCode?())).ToObject(returnType);

    private object SetProperty(string propertyName, object value)
    {
      if (value == null)
        return value;
      this.jObject[(object) propertyName] = JToken.FromObject(value);
      return (object) true;
    }

    private T AsType<T>() => (T) this.Convert(typeof (T));

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter) => (DynamicMetaObject) new QueryResult.DocumentDynamicMetaObject(this, parameter);

    private class DocumentDynamicMetaObject : DynamicMetaObject
    {
      private readonly QueryResult queryResult;

      public DocumentDynamicMetaObject(QueryResult queryResult, Expression expression)
        : base(expression, BindingRestrictions.Empty, (object) queryResult)
      {
        this.queryResult = queryResult;
      }

      public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
      {
        string name = "GetProperty";
        Expression[] expressionArray = new Expression[2]
        {
          (Expression) Expression.Constant((object) binder.Name),
          (Expression) Expression.Constant((object) binder.ReturnType)
        };
        return new DynamicMetaObject((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, this.LimitType), CustomTypeExtensions.GetMethod(typeof (QueryResult), name, BindingFlags.Instance | BindingFlags.NonPublic), expressionArray), BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType));
      }

      public override DynamicMetaObject BindSetMember(
        SetMemberBinder binder,
        DynamicMetaObject value)
      {
        string name = "SetProperty";
        BindingRestrictions typeRestriction = BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType);
        Expression[] expressionArray = new Expression[2]
        {
          (Expression) Expression.Constant((object) binder.Name),
          (Expression) Expression.Convert(value.Expression, typeof (object))
        };
        return new DynamicMetaObject((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, this.LimitType), CustomTypeExtensions.GetMethod(typeof (QueryResult), name, BindingFlags.Instance | BindingFlags.NonPublic), expressionArray), typeRestriction);
      }

      public override DynamicMetaObject BindConvert(ConvertBinder binder) => new DynamicMetaObject((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, this.LimitType), CustomTypeExtensions.GetMethod(typeof (QueryResult), "AsType", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(binder.Type)), BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType));

      public override IEnumerable<string> GetDynamicMemberNames() => this.queryResult.GetDynamicMemberNames();
    }
  }
}
