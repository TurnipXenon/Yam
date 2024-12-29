#nullable enable
using System.Collections.Generic;
using Godot;

namespace Yam.Core.Common;

public abstract class GenericPooler<TPooledObject, TPooledObjectArgs>
{
    protected readonly List<TPooledObject> InUse = new();
    protected readonly Stack<TPooledObject> Available = new();

    protected abstract TPooledObject? InstantiatePooledObject(TPooledObjectArgs args);
    protected abstract TPooledObject? RevivePooledObject(TPooledObjectArgs args);
    protected abstract void DestroyPooledObject(TPooledObject pooledObject);
    protected abstract void Free(TPooledObject pooledObject);

    public TPooledObject? Request(TPooledObjectArgs args)
    {
        TPooledObject? newObject;

        if (Available.Count > 0)
        {
            newObject = RevivePooledObject(args);
        }
        else
        {
            newObject = InstantiatePooledObject(args);
        }

        if (newObject == null)
        {
            GD.PrintErr("Instantiating Pooled Object failed");
        }
        else
        {
            InUse.Add(newObject);
        }

        return newObject;
    }


    public void Release(TPooledObject pooledObject)
    {
        InUse.Remove(pooledObject);
        Available.Push(pooledObject);
    }

    public void DeleteAll()
    {
        InUse.AddRange(Available);
        Available.Clear();
        InUse.ForEach(b => DestroyPooledObject(b));
        InUse.Clear();
    }
}