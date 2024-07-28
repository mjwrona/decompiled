// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.PropertyTypeNotSupportedException
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [Obsolete("Please use Microsoft.VisualStudio.Services.Common.PropertyTypeNotSupportedException instead.")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [ExceptionMapping("0.0", "3.0", "PropertyTypeNotSupportedException", "Microsoft.TeamFoundation.Framework.Common.PropertyTypeNotSupportedException, Microsoft.TeamFoundation.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class PropertyTypeNotSupportedException : TeamFoundationPropertyValidationException
  {
    public PropertyTypeNotSupportedException(string propertyName, Type type)
      : base(propertyName, TFCommonResources.UnsupportedPropertyValueType((object) propertyName, (object) type.FullName))
    {
    }
  }
}
