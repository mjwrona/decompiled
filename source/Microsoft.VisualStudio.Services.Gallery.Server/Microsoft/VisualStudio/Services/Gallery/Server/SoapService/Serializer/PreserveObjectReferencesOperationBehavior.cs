// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Serializer.PreserveObjectReferencesOperationBehavior
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Serializer
{
  public class PreserveObjectReferencesOperationBehavior : DataContractSerializerOperationBehavior
  {
    public PreserveObjectReferencesOperationBehavior(OperationDescription operationDescription)
      : base(operationDescription)
    {
    }

    public override XmlObjectSerializer CreateSerializer(
      Type type,
      string name,
      string ns,
      IList<Type> knownTypes)
    {
      return (XmlObjectSerializer) new DataContractSerializer(type, name, ns, (IEnumerable<Type>) knownTypes, int.MaxValue, false, true, (IDataContractSurrogate) null);
    }

    public override XmlObjectSerializer CreateSerializer(
      Type type,
      XmlDictionaryString name,
      XmlDictionaryString ns,
      IList<Type> knownTypes)
    {
      return (XmlObjectSerializer) new DataContractSerializer(type, name, ns, (IEnumerable<Type>) knownTypes, int.MaxValue, false, true, (IDataContractSurrogate) null);
    }
  }
}
