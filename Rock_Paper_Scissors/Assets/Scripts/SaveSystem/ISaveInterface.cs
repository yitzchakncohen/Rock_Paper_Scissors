using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveInterface<T>
{
    public T Save();
    public void Load(T loadData);
}
