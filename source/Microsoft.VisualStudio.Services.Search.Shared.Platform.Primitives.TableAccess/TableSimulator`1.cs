// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.TableSimulator`1
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class TableSimulator<T> : ITable<T>, IDisposable
  {
    private List<T> m_simulatorEntityStore;

    public TableSimulator() => this.m_simulatorEntityStore = new List<T>();

    public void Dispose()
    {
    }

    public T Insert(T platformEntity)
    {
      if ((object) platformEntity == null)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (platformEntity)));
      if (this.IsEntityInStore(platformEntity))
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: "Entity already present");
      this.m_simulatorEntityStore.Add(platformEntity);
      return platformEntity;
    }

    public void Delete(T platformEntity)
    {
      if ((object) platformEntity == null)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (platformEntity)));
      if (!this.IsEntityInStore(platformEntity))
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: "Entity not present");
      this.m_simulatorEntityStore.Remove(platformEntity);
    }

    public T Update(T platformEntity)
    {
      if ((object) platformEntity == null)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (platformEntity)));
      int index = this.IsEntityInStore(platformEntity) ? this.GetEntityIndexInStore(platformEntity) : throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: "Entity not present");
      if (index >= 0)
      {
        this.m_simulatorEntityStore.RemoveAt(index);
        this.m_simulatorEntityStore.Add(platformEntity);
      }
      return platformEntity;
    }

    public int CheckAndUpdate(T oldEntity, T newEntity)
    {
      int index = (object) oldEntity != null && (object) newEntity != null ? this.GetEntityIndexInStore(oldEntity) : throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException("oldEntity or newEntity"));
      if (index < 0)
        return 0;
      this.m_simulatorEntityStore.RemoveAt(index);
      this.m_simulatorEntityStore.Add(newEntity);
      return 1;
    }

    public T RetriveTableEntity(TableEntityFilterList filterList)
    {
      List<T> objList = this.RetriveTableEntityList(1, filterList);
      return objList.Count <= 0 ? default (T) : objList[0];
    }

    public List<T> AddTableEntityBatch(List<T> listAzurePlatformEntity, bool merge)
    {
      if (listAzurePlatformEntity == null || listAzurePlatformEntity.Count == 0)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("platformEntityList is null or empty"));
      foreach (T inputEntity in listAzurePlatformEntity)
      {
        if (this.IsEntityInStore(inputEntity))
        {
          if (!merge)
            throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: "Entity already present, Duplicate Values cannot be added.");
          this.m_simulatorEntityStore.RemoveAt(this.GetEntityIndexInStore(inputEntity));
        }
        this.m_simulatorEntityStore.Add(inputEntity);
      }
      return listAzurePlatformEntity;
    }

    public List<T> RetriveTableEntityList(int count, TableEntityFilterList filterList)
    {
      if (filterList == null)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("filterList is null"));
      List<T> objList = new List<T>();
      foreach (T obj in this.m_simulatorEntityStore)
      {
        bool flag = true;
        foreach (TableEntityFilter filter in (List<TableEntityFilter>) filterList)
        {
          if (!("eq" == filter.CompareOperator) && !("ne" == filter.CompareOperator))
            throw new NotImplementedException();
          if (!QueryOperators.Operate(obj.GetType().GetProperty(filter.Property).GetValue((object) obj, (object[]) null).ToString(), filter.CompareOperator, filter.Value))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          objList.Add(obj);
      }
      return objList;
    }

    private bool CompareTableEntities(object obj1, object obj2)
    {
      if (obj1.GetType() != obj2.GetType())
        return false;
      foreach (PropertyInfo property in obj1.GetType().GetProperties())
      {
        Type type = property.GetType();
        object obj3 = property.GetValue(obj1);
        object obj4 = property.GetValue(obj2);
        if (obj3 == null)
        {
          if (obj4 != null)
            return false;
        }
        else if (!obj3.Equals(obj4) && (type.IsPrimitive || type.GetProperties().Length == 0 && !this.CompareTableEntities(obj3, obj4)))
          return false;
      }
      return true;
    }

    private bool IsEntityInStore(T inputEntity) => this.GetEntityIndexInStore(inputEntity) >= 0;

    private int GetEntityIndexInStore(T inputEntity)
    {
      if (inputEntity.GetType().IsAssignableFrom(typeof (CustomRepositoryEntity)))
      {
        CustomRepositoryEntity inputRepoEntity = (object) inputEntity as CustomRepositoryEntity;
        return (this.m_simulatorEntityStore as List<CustomRepositoryEntity>).FindIndex((Predicate<CustomRepositoryEntity>) (item => item.CollectionId == inputRepoEntity.CollectionId && item.ProjectName == inputRepoEntity.ProjectName && item.RepositoryName == inputRepoEntity.RepositoryName));
      }
      if (inputEntity.GetType().IsAssignableFrom(typeof (DisabledFile)))
      {
        DisabledFile disabledFile = (object) inputEntity as DisabledFile;
        return (this.m_simulatorEntityStore as List<DisabledFile>).FindIndex((Predicate<DisabledFile>) (item => item.FilePath == disabledFile.FilePath));
      }
      if (inputEntity.GetType().IsAssignableFrom(typeof (IndexingUnit)))
      {
        IndexingUnit indexingUnit = (object) inputEntity as IndexingUnit;
        return (this.m_simulatorEntityStore as List<IndexingUnit>).FindIndex((Predicate<IndexingUnit>) (item => item.TFSEntityId == indexingUnit.TFSEntityId && item.IndexingUnitType == indexingUnit.IndexingUnitType && item.EntityType.Name == indexingUnit.EntityType.Name));
      }
      if (inputEntity.GetType().IsAssignableFrom(typeof (IndexingUnitChangeEvent)))
      {
        IndexingUnitChangeEvent indexingUnitChangeEvent = (object) inputEntity as IndexingUnitChangeEvent;
        return (this.m_simulatorEntityStore as List<IndexingUnitChangeEvent>).FindIndex((Predicate<IndexingUnitChangeEvent>) (item => item.Id == indexingUnitChangeEvent.Id));
      }
      if (inputEntity.GetType().IsAssignableFrom(typeof (ReindexingStatusEntry)))
      {
        ReindexingStatusEntry reindexingStatus = (object) inputEntity as ReindexingStatusEntry;
        return (this.m_simulatorEntityStore as List<ReindexingStatusEntry>).FindIndex((Predicate<ReindexingStatusEntry>) (item => item.CollectionId == reindexingStatus.CollectionId));
      }
      if (inputEntity.GetType().IsAssignableFrom(typeof (ClassificationNode)))
      {
        ClassificationNode classificationNode = (object) inputEntity as ClassificationNode;
        return (this.m_simulatorEntityStore as List<ClassificationNode>).FindIndex((Predicate<ClassificationNode>) (item => item.Id == classificationNode.Id));
      }
      PackageContainer container = inputEntity.GetType().IsAssignableFrom(typeof (PackageContainer)) ? (object) inputEntity as PackageContainer : throw new NotImplementedException();
      return (this.m_simulatorEntityStore as List<PackageContainer>).FindIndex((Predicate<PackageContainer>) (item => item.ContainerId == container.ContainerId));
    }
  }
}
