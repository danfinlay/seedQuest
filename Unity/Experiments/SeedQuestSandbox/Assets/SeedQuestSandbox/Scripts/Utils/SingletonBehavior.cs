﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeedQuest.Utils
{

    public class SingletonBehavior {

        private static SingletonBehavior instance = null;
        public static SingletonBehavior Instance {
            get {
                if (instance == null)
                    instance = new SingletonBehavior();

                return instance;
            }
        }
    }
}