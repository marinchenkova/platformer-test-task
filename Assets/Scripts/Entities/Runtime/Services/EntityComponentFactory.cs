using System;
using Entities.Core;
using UnityEngine;

namespace Entities.Services {

    public sealed class EntityComponentFactory : IEntityComponentFactory {

        public IEntityComponent Copy(IEntityComponent sample) {
            if (sample == null) return null;

            // Not the best solution, but it is one of the simplest for components implemented as classes.
            // The point is: this copy method is not most important thing in the test task.
            try {
                return (IEntityComponent) JsonUtility.FromJson(JsonUtility.ToJson(sample), sample.GetType());
            }
            catch (ArgumentException) {
                Debug.LogError($"Cannot convert component of type {sample.GetType()} from json string." +
                               $"Returning default.");
                return default;
            }
        }
    }

}
