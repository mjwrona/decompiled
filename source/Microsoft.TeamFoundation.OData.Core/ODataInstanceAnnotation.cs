// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataInstanceAnnotation
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.JsonLight;
using System;
using System.Xml;

namespace Microsoft.OData
{
  public sealed class ODataInstanceAnnotation : ODataAnnotatable
  {
    public ODataInstanceAnnotation(string name, ODataValue value)
      : this(name, value, false)
    {
    }

    internal ODataInstanceAnnotation(
      string annotationName,
      ODataValue annotationValue,
      bool isCustomAnnotation)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(annotationName, nameof (annotationName));
      if (!isCustomAnnotation && ODataAnnotationNames.IsODataAnnotationName(annotationName))
        throw new ArgumentException(Strings.ODataInstanceAnnotation_ReservedNamesNotAllowed((object) annotationName, (object) "odata."));
      ODataInstanceAnnotation.ValidateName(annotationName);
      ODataInstanceAnnotation.ValidateValue(annotationValue);
      this.Name = annotationName;
      this.Value = annotationValue;
    }

    public string Name { get; private set; }

    public ODataValue Value { get; private set; }

    internal static void ValidateName(string name)
    {
      if (name.IndexOf('.') >= 0 && name[0] != '.')
      {
        if (name[name.Length - 1] != '.')
        {
          try
          {
            XmlConvert.VerifyNCName(name);
            return;
          }
          catch (XmlException ex)
          {
            throw new ArgumentException(Strings.ODataInstanceAnnotation_BadTermName((object) name), (Exception) ex);
          }
        }
      }
      throw new ArgumentException(Strings.ODataInstanceAnnotation_NeedPeriodInName((object) name));
    }

    internal static void ValidateValue(ODataValue value)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataValue>(value, nameof (value));
      if (value is ODataStreamReferenceValue)
        throw new ArgumentException(Strings.ODataInstanceAnnotation_ValueCannotBeODataStreamReferenceValue, nameof (value));
    }
  }
}
