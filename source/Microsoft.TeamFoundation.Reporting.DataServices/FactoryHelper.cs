// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.FactoryHelper
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public static class FactoryHelper
  {
    public static ConstructorInfo MatchConstructor(Type implementationType, Type interfaceType) => ((IEnumerable<Type>) implementationType.GetInterfaces()).Contains<Type>(interfaceType) ? implementationType.GetConstructor(Type.EmptyTypes) : throw new ArgumentException(nameof (implementationType));

    public static T InstantiateService<T>(
      IVssRequestContext requestContext,
      ConstructorInfo constructorInfo)
      where T : class, IDataServicesService
    {
      T obj = constructorInfo.Invoke((object[]) null) as T;
      obj.RequestContext = requestContext;
      return obj;
    }

    public static TInterface Instantiate<TInterface>(Type implementationType) where TInterface : class => implementationType.GetConstructor(Type.EmptyTypes).Invoke((object[]) null) as TInterface;

    public static TInterface InstantiateService<TInterface, TImplementation>(
      IVssRequestContext requestContext)
      where TInterface : class, IDataServicesService
      where TImplementation : TInterface
    {
      TInterface @interface = FactoryHelper.Instantiate<TInterface>(typeof (TImplementation));
      @interface.RequestContext = requestContext;
      return @interface;
    }
  }
}
