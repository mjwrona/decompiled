// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.ProcessCustomization.Process.BasicFieldPropertiesFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.ProcessCustomization.Process
{
  public class BasicFieldPropertiesFactory
  {
    public IBasicFieldProperties Create(ProcessFieldResult field) => (IBasicFieldProperties) BasicFieldPropertiesFactory.CreateFactoryProps(field);

    public IBasicFieldProperties Create(FieldEntry field) => (IBasicFieldProperties) BasicFieldPropertiesFactory.CreateFactoryProps(field);

    public IFieldLockingProperties CreateLockingProps(ProcessFieldResult field) => (IFieldLockingProperties) BasicFieldPropertiesFactory.CreateFactoryProps(field);

    public IFieldLockingProperties CreateLockingProps(FieldEntry field) => (IFieldLockingProperties) BasicFieldPropertiesFactory.CreateFactoryProps(field);

    private static BasicFieldPropertiesFactory.BasicFieldProperties CreateFactoryProps(
      ProcessFieldResult field)
    {
      return new BasicFieldPropertiesFactory.BasicFieldProperties()
      {
        Name = field.Name,
        ReferenceName = field.ReferenceName,
        Description = field.Description,
        Type = (FieldType) Enum.Parse(typeof (FieldType), field.Type.ToString()),
        IsLocked = field.IsLocked
      };
    }

    private static BasicFieldPropertiesFactory.BasicFieldProperties CreateFactoryProps(
      FieldEntry field)
    {
      return new BasicFieldPropertiesFactory.BasicFieldProperties()
      {
        Name = field.Name,
        ReferenceName = field.ReferenceName,
        Description = field.Description,
        Type = (FieldType) Enum.Parse(typeof (FieldType), field.FieldType.ToString()),
        IsLocked = field.IsLocked
      };
    }

    private class BasicFieldProperties : IBasicFieldProperties, IFieldLockingProperties
    {
      public string Description { get; set; }

      public string Name { get; set; }

      public string ReferenceName { get; set; }

      public FieldType Type { get; set; }

      public bool IsLocked { get; set; }
    }
  }
}
