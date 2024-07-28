// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServerJsonSerializationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ServerJsonSerializationHelper
  {
    private static readonly Type[] s_excludedTypes = new Type[5]
    {
      typeof (VssJsonCollectionWrapperBase),
      typeof (WrappedException),
      typeof (ISecuredObjectContainer),
      typeof (Exception),
      typeof (ReferenceLinks)
    };

    public static bool IsExcludedType(Type objectType) => ((IEnumerable<Type>) ServerJsonSerializationHelper.s_excludedTypes).Any<Type>((Func<Type, bool>) (_ => _.IsAssignableFrom(objectType)));

    internal static void AddCallback(JsonContract contract, Type objectType)
    {
      if (ServerJsonSerializationHelper.IsExcludedType(objectType))
        return;
      contract.OnSerializingCallbacks.Add(new SerializationCallback(ServerJsonSerializationHelper.ValidateSecurity));
    }

    internal static void ValidateSecurity(object o, StreamingContext context) => ServerJsonSerializationHelper.ValidateSecurity(o);

    public static void ValidateSecurity(object o)
    {
      HttpContextBase current = HttpContextFactory.Current;
      if (current == null)
        return;
      if (!(current.Items[(object) HttpContextConstants.SecurityTracking] is TrackedSecurityCollection securityCollection))
        return;
      try
      {
        securityCollection.Validate(o);
      }
      catch (Exception ex)
      {
        current.Items[(object) HttpContextConstants.SecurityTrackingException] = (object) true;
        throw;
      }
    }

    internal static void VssSecureJsonConverterObjectValidation(
      object value,
      JsonSerializer serializer)
    {
      if (value == null || serializer == null || !(serializer.ContractResolver is ServerVssCamelCasePropertyNamesContractResolver) && !(serializer.ContractResolver is ServerVssCamelCasePropertyNamesPreserveEnumsContractResolver) || ServerJsonSerializationHelper.IsExcludedType(value.GetType()))
        return;
      ServerJsonSerializationHelper.ValidateSecurity(value);
    }

    internal static bool IsCurrentContextAnonymousOrPublic()
    {
      HttpContextBase current = HttpContextFactory.Current;
      if (current == null || !(current.Items[(object) HttpContextConstants.IVssRequestContext] is IVssRequestContext requestContext))
        return false;
      return requestContext.IsPublicUser() || requestContext.IsAnonymousPrincipal();
    }

    public static void EnsureValidRootType(Type type)
    {
      HttpContextBase current = HttpContextFactory.Current;
      TrackedSecurityCollection securityCollection;
      if (!(current?.Items[(object) HttpContextConstants.IVssRequestContext] is IVssRequestContext requestContext) || !requestContext.RootContext.Items.TryGetValue<TrackedSecurityCollection>(RequestContextItemsKeys.SecurityTracking, out securityCollection))
        return;
      if (!current.Items.Contains((object) HttpContextConstants.SecurityTracking))
        current.Items[(object) HttpContextConstants.SecurityTracking] = (object) securityCollection;
      if (!ServerJsonSerializationHelper.IsValidSecuredObjectType(type))
      {
        current.Items[(object) HttpContextConstants.SecurityTrackingException] = (object) true;
        string message = string.Format("Cannot return type {0}, from a public API as it doesn't implement ISecuredObject", (object) type);
        requestContext.Trace(2124135760, TraceLevel.Error, "ServerVssJsonMediaTypeFormatter", "AnonymousAccessKalypsoAlert", message);
        throw new InvalidOperationException(message);
      }
    }

    private static bool IsValidSecuredObjectType(Type type)
    {
      if (typeof (ISecuredObject).IsAssignableFrom(type) || typeof (ISecuredObjectContainer).IsAssignableFrom(type) || typeof (Exception).IsAssignableFrom(type) || type.IsEnum)
        return true;
      if (typeof (IEnumerable).IsAssignableFrom(type))
      {
        if (type.IsGenericType)
        {
          foreach (Type genericTypeArgument in type.GenericTypeArguments)
          {
            if (!ServerJsonSerializationHelper.IsValidSecuredObjectType(genericTypeArgument))
              return false;
          }
          return true;
        }
        if (type.IsArray)
          return ServerJsonSerializationHelper.IsValidSecuredObjectType(type.GetElementType());
      }
      return false;
    }
  }
}
