﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : Animal
{
    void Start()
    {
        OnStart("Prey");
    }

    void Update()
    {
        OnUpdate();
    }
}
