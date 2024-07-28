// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Attachment
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace Microsoft.Azure.Documents
{
  public class Attachment : Resource, IDynamicMetaObjectProvider
  {
    [JsonProperty(PropertyName = "contentType")]
    public string ContentType
    {
      get => this.GetValue<string>("contentType");
      set => this.SetValue("contentType", (object) value);
    }

    [JsonProperty(PropertyName = "media")]
    public string MediaLink
    {
      get => this.GetValue<string>("media");
      set => this.SetValue("media", (object) value);
    }

    internal static Attachment FromObject(object attachment, JsonSerializerSettings settings = null)
    {
      if (attachment == null)
        return (Attachment) null;
      if (CustomTypeExtensions.IsAssignableFrom(typeof (Attachment), attachment.GetType()))
        return (Attachment) attachment;
      JObject jobject = JObject.FromObject(attachment);
      Attachment attachment1 = new Attachment();
      attachment1.propertyBag = jobject;
      attachment1.SerializerSettings = settings;
      return attachment1;
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
      if ((object) typeof (T) == (object) typeof (Attachment) || (object) typeof (T) == (object) typeof (object))
        return (T) this;
      if (this.propertyBag == null)
        return default (T);
      return this.SerializerSettings != null ? this.propertyBag.ToObject<T>(JsonSerializer.Create(this.SerializerSettings)) : this.propertyBag.ToObject<T>();
    }

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter) => (DynamicMetaObject) new Attachment.AttachmentDynamicMetaObject(this, parameter);

    private class AttachmentDynamicMetaObject : DynamicMetaObject
    {
      private readonly Attachment attachment;

      public AttachmentDynamicMetaObject(Attachment attachment, Expression expression)
        : base(expression, BindingRestrictions.Empty, (object) attachment)
      {
        this.attachment = attachment;
      }

      public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
      {
        if (Attachment.AttachmentDynamicMetaObject.IsResourceProperty(binder.Name))
          return base.BindGetMember(binder);
        string name = "GetProperty";
        Expression[] expressionArray = new Expression[2]
        {
          (Expression) Expression.Constant((object) binder.Name),
          (Expression) Expression.Constant((object) binder.ReturnType)
        };
        return new DynamicMetaObject((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, this.LimitType), CustomTypeExtensions.GetMethod(typeof (Attachment), name, BindingFlags.Instance | BindingFlags.NonPublic), expressionArray), BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType));
      }

      public override DynamicMetaObject BindSetMember(
        SetMemberBinder binder,
        DynamicMetaObject value)
      {
        if (Attachment.AttachmentDynamicMetaObject.IsResourceProperty(binder.Name))
          return base.BindSetMember(binder, value);
        string name = "SetProperty";
        BindingRestrictions typeRestriction = BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType);
        Expression[] expressionArray = new Expression[2]
        {
          (Expression) Expression.Constant((object) binder.Name),
          (Expression) Expression.Convert(value.Expression, typeof (object))
        };
        return new DynamicMetaObject((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, this.LimitType), CustomTypeExtensions.GetMethod(typeof (Attachment), name, BindingFlags.Instance | BindingFlags.NonPublic), expressionArray), typeRestriction);
      }

      public override DynamicMetaObject BindConvert(ConvertBinder binder) => new DynamicMetaObject((Expression) Expression.Call((Expression) Expression.Convert(this.Expression, this.LimitType), CustomTypeExtensions.GetMethod(typeof (Attachment), "AsType", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(binder.Type)), BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType));

      public override IEnumerable<string> GetDynamicMemberNames()
      {
        List<string> dynamicMemberNames = new List<string>();
        foreach (KeyValuePair<string, JToken> keyValuePair in this.attachment.propertyBag)
        {
          if (!Attachment.AttachmentDynamicMetaObject.IsResourceSerializedProperty(keyValuePair.Key))
            dynamicMemberNames.Add(keyValuePair.Key);
        }
        return (IEnumerable<string>) dynamicMemberNames;
      }

      internal static bool IsResourceSerializedProperty(string propertyName) => propertyName == "id" || propertyName == "_rid" || propertyName == "_etag" || propertyName == "_ts" || propertyName == "_self" || propertyName == "contentType" || propertyName == "media";

      internal static bool IsResourceProperty(string propertyName) => propertyName == "Id" || propertyName == "ResourceId" || propertyName == "ETag" || propertyName == "Timestamp" || propertyName == "SelfLink" || propertyName == "MediaLink" || propertyName == "ContentType";
    }
  }
}
