using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public interface IDataFile<T>
{
    T Load(string filename);
    void Save(string filename, T data);

}