// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Document
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace Microsoft.Azure.Documents
{
  public class Document : Resource, IDynamicMetaObjectProvider
  {
    public string AttachmentsLink => this.SelfLink.TrimEnd('/') + "/" + this.GetValue<string>("_attachments");

    [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
    public int? TimeToLive
    {
      get => this.GetValue<int?>("ttl");
      set => this.SetValue("ttl", (object) value);
    }

    internal static Document FromObject(object document, JsonSerializerSettings settings = null)
    {
      if (document == null)
        return (Document) null;
      if (CustomTypeExtensions.IsAssignableFrom(typeof (Document), document.GetType()))
        return (Document) document;
      JObject jobject = settings == null ? JObject.FromObject(document) : JObject.FromObject(document, JsonSerializer.Create(settings));
      Document document1 = new Document();
      document1.SerializerSettings = settings;
      document1.propertyBag = jobject;
      return document1;
    }

    private object GetProperty(string propertyName, Type returnType)
    {
      JToken jtoken = this.propertyBag != null ? this.propertyBag[propertyName] : throw new DocumentClientException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.PropertyNotFound, (object) propertyName), (Exception) null, new HttpStatusCode?());
      if (jtoken != null)
        return this.SerializerSettings != null ? jtoken.ToObject(returnType, JsonSerializer.Create(this.SerializerSettings)) : jtoken.ToObject(returnType);
    }

    private object SetProperty(string propertyName, object value)
    {
      if (value != null)
      {
        if (this.propertyBag == null)
          this.propertyBag = new JObject();
        this.propertyBag[propertyName] = JToken.FromObject(value);
      }
      else if (this.propertyBag != null)
        this.propertyBag.Remove(propertyName);
      return value;
    }

    private T AsType<T>()
    {
      if ((object) typeof (T) == (object) typeof (Document) || (object) typeof (T) == (object) typeof (object))
        return (T) this;
      if (this.propertyBag == null)
        return default (T);
      return this.SerializerSettings != null ? this.propertyBag.ToObject<T>(JsonSerializer.Create(this.SerializerSettings)) : this.propertyBag.ToObject<T>();
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter) => (DynamicMetaObject) new Document.DocumentDynamicMetaObject(this, parameter);

    private class DocumentDynamicMetaObject : DynamicMetaObject
    {
      private readonly Document document;

      public DocumentDynamicMetaObject(Document document, Expression expression)
        : base(expression, BindingRestrictions.Empty, (object) document)
      {
        this.document = document;
      }

      public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
      {
        if (Document.DocumentDynamicMetaObject.IsResourceProperty(binder.Name))
          return base.BindGetMember(binder);
        string name = "GetProperty";
        Expression[] expressionArray = new Expression[2]
        {
          (Expression) Expression.Constant((object) binder.Name),
          (Expression) Expression.Constant((object) binder.ReturnType)
        };
        return new DynamicMetaObject((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, this.LimitType), CustomTypeExtensions.GetMethod(typeof (Document), name, BindingFlags.Instance | BindingFlags.NonPublic), expressionArray), BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType));
      }

      public override DynamicMetaObject BindSetMember(
        SetMemberBinder binder,
        DynamicMetaObject value)
      {
        if (Document.DocumentDynamicMetaObject.IsResourceProperty(binder.Name))
          return base.BindSetMember(binder, value);
        string name = "SetProperty";
        BindingRestrictions typeRestriction = BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType);
        Expression[] expressionArray = new Expression[2]
        {
          (Expression) Expression.Constant((object) binder.Name),
          (Expression) Expression.Convert(value.Expression, typeof (object))
        };
        return new DynamicMetaObject((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, this.LimitType), CustomTypeExtensions.GetMethod(typeof (Document), name, BindingFlags.Instance | BindingFlags.NonPublic), expressionArray), typeRestriction);
      }

      public override DynamicMetaObject BindConvert(ConvertBinder binder) => new DynamicMetaObject((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, this.LimitType), CustomTypeExtensions.GetMethod(typeof (Document), "AsType", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(binder.Type)), BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType));

      public override IEnumerable<string> GetDynamicMemberNames()
      {
        List<string> source = new List<string>();
        foreach (KeyValuePair<string, JToken> keyValuePair in this.document.propertyBag)
        {
          if (!Document.DocumentDynamicMetaObject.IsResourceSerializedProperty(keyValuePair.Key))
            source.Add(keyValuePair.Key);
        }
        return (IEnumerable<string>) source.ToList<string>();
      }

      internal static bool IsResourceSerializedProperty(string propertyName) => propertyName == "id" || propertyName == "_rid" || propertyName == "_etag" || propertyName == "_ts" || propertyName == "_self" || propertyName == "_attachments" || propertyName == "ttl";

      internal static bool IsResourceProperty(string propertyName) => propertyName == "Id" || propertyName == "ResourceId" || propertyName == "ETag" || propertyName == "Timestamp" || propertyName == "SelfLink" || propertyName == "AttachmentsLink" || propertyName == "TimeToLive";
    }
  }
}
