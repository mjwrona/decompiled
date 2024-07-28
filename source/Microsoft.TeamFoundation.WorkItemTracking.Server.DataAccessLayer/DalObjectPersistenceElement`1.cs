// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalObjectPersistenceElement`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalObjectPersistenceElement<T> : DalSqlElement
  {
    private List<T> m_objects = new List<T>();
    private List<Type> m_prerequisiteElementTypes;
    private bool m_addedPrerequisiteElementTypes;

    public DalObjectPersistenceElement() => this.Objects = (IList<T>) this.m_objects.AsReadOnly();

    protected void AddPrerequisiteSingletonElement<TElement>() where TElement : DalSqlElement
    {
      if (this.m_prerequisiteElementTypes == null)
        this.m_prerequisiteElementTypes = new List<Type>();
      this.m_prerequisiteElementTypes.Add(typeof (TElement));
    }

    public IList<T> Objects { get; private set; }

    public virtual IList<T> ValidObjects => this.Objects;

    public virtual void AddObjects(IEnumerable<T> objects)
    {
      this.m_objects.AddRange(objects);
      if (!objects.Any<T>())
        return;
      this.IsNeeded = true;
      if (this.m_addedPrerequisiteElementTypes || this.m_prerequisiteElementTypes == null || this.m_update == null)
        return;
      foreach (Type prerequisiteElementType in this.m_prerequisiteElementTypes)
      {
        DalSqlElement singletonElement = this.m_update.GetSingletonElement(prerequisiteElementType);
        if (singletonElement != null)
          singletonElement.IsNeeded = true;
      }
      this.m_addedPrerequisiteElementTypes = true;
    }

    public virtual void AddObject(T o) => this.AddObjects((IEnumerable<T>) new T[1]
    {
      o
    });
  }
}
