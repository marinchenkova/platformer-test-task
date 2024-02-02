using System;
using UnityEngine;

namespace Entities.Core {

    public static class EntityExtensions {

        public static bool IsAlive(this Entity entity) {
            return entity.world?.ContainsEntity(entity) ?? false;
        }

        public static void Destroy(this Entity entity) {
            entity.world?.DestroyEntity(entity);
        }

        public static T GetComponent<T>(this Entity entity) where T : class, IEntityComponent {
            return entity.world?.GetComponent<T>(entity);
        }

        public static T GetOrAddComponent<T>(this Entity entity) where T : class, IEntityComponent, new() {
            return entity.world?.GetOrAddComponent<T>(entity);
        }

        public static ComponentCollection GetComponents(this Entity entity) {
            return entity.world != null ? entity.world.GetComponents(entity) : default;
        }

        public static void SetComponent<T>(this Entity entity, T component) where T : class, IEntityComponent {
            entity.world?.SetComponent<T>(entity, component);
        }

        public static void SetComponentCopy<T>(this Entity entity, T component) where T : class, IEntityComponent {
            entity.world?.SetComponentCopy<T>(entity, component);
        }

        public static void RemoveComponent<T>(this Entity entity) where T : class, IEntityComponent {
            entity.world?.RemoveComponent<T>(entity);
        }

        public static void RemoveComponent(this Entity entity, Type componentType) {
            entity.world?.RemoveComponent(entity, componentType);
        }

        public static void RemoveComponents(this Entity entity) {
            entity.world?.RemoveComponents(entity);
        }

        public static Entity SearchEntityOnEntityView(this Transform transform) {
            return transform == null ? default : transform.GetComponent<IEntityView>()?.Entity ?? default;
        }
    }

}
