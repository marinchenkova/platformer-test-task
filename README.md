# Platformer Test Task

Данный проект является примером выполнения тестового задания "Разработка микро-прототипа платформера с видом сбоку" на движке Unity 2022.3.6f1 LTS. 

Ссылки на билды:
- [`platformer.zip`](https://drive.google.com/file/d/1Rb0s3saUky23n0HtJesmEOFI-nWOMOZ6/view?usp=drive_link)

### Содержание

- [Демонстрация игрового процесса](https://github.com/marinchenkova/platformer-test-task/blob/master/README.md#демонстрация-игрового-процесса)
- [Техническое задание](https://github.com/marinchenkova/platformer-test-task/blob/master/README.md#техническое-задание)
- [Архитектура проекта](https://github.com/marinchenkova/platformer-test-task/blob/master/README.md#архитектура-проекта)
- [Пайплайн запуска](https://github.com/marinchenkova/platformer-test-task/blob/master/README.md#пайплайн-запуска)
  
### Демонстрация игрового процесса

https://github.com/marinchenkova/platformer-test-task/assets/22106355/1ed283d2-1333-405a-bfc9-d83719b6d3df

### Техническое задание

Условия:
1. Собрать из примитивов две небольшие комнаты соединенные коридором. В комнатах должны быть расположены платформы, стены и шипы (на ваш вкус). 
2. Выбрать любого персонажа на сайте mixamo и анимации для него (можно использовать любой контент, которым располагаете).
3. Собрать аниматор контроллер и управление. Персонаж должен уметь передвигаться, прыгать и умирать, попадая на шипы.
4. Персонаж должен двигаться с помощью юнити физики.
5. Добавить любые дополнительные элементы и усложнения на ваш вкус.
6. Кратко описать структуру проекта: какой скрипт за что отвечает, почему.

Подумайте над тем, как потенциальный игрок будет взаимодействовать с персонажем и ощущать его.

Требования:
- Unity 2022.3.6
- Нельзя использовать готовые решения (если только они не ваши персональные).

### Архитектура проекта

В качестве базы для построения игровой логики используется архитектура Entity + Components, 
реализованная в виде пакета [Assets/Scripts/Entities](https://github.com/marinchenkova/platformer-test-task/tree/master/Assets/Scripts/Entities) 

- Сущность `Entity` представляет собой readonly-структуру, указатель на список прикрепленных к ней компонентов в мире сущностей `World`:

```csharp
public readonly struct Entity : IEquatable<Entity> {

    public readonly World world;

    private readonly long _id;

    // ...
}
```

- Компоненты должны быть классами и реализовать интерфейс `IEntityComponent`
- Компоненты принимают сигналы при прикреплении/откреплении, уничтожении сущности для запуска логики 

```csharp
public interface IEntityComponent {

    void OnEnable(Entity entity) {}

    void OnDisable(Entity entity) {}

    void OnDestroy(Entity entity) {}
}
```

- Сущности и компоненты хранятся в созданном мире `World`
- Мир запускается с помощью monovehaviour `WorldLauncher`
- При создании мира в конструктор класса `World` в виде интерфейсов передаются ссылки на сервисы, такие как источник апдейтов, хранилище сущностей, хранилище и фабрика компонентов и т.п. 
- Компоненты через взаимодействие с миром могут подписываться на вызовы Update, создавать вью, добавлять и удалять другие компоненты и сущности

```csharp
public sealed class World {

    public World(
        IEntityIdProvider entityIdProvider,
        IEntityStorage entityStorage,
        IEntityComponentStorage entityComponentStorage,
        IEntityComponentFactory entityComponentFactory,
        IEntityViewProvider entityViewProvider,
        IWorldTickProvider worldTickProvider
    ) {
        // ...
    }

    // ...
}
```

```csharp
public static class EntityExtensions {

    public static T GetComponent<T>(this Entity entity) where T : class, IEntityComponent {
        return entity.world?.GetComponent<T>(entity);
    }

    public static void SetComponent<T>(this Entity entity, T component) where T : class, IEntityComponent {
        entity.world?.SetComponent<T>(entity, component);
    }

    public static void RemoveComponent<T>(this Entity entity) where T : class, IEntityComponent {
        entity.world?.RemoveComponent<T>(entity);
    }

    // ...
}
```

- Для создания сущностей с набором компонентов из редактора были добавлены классы:

1. `EntityPrefab` - scriptable object, который хранит набор компонентов для инициализации любой сущности

<p align="center">
<img width="666" alt="image" src="https://github.com/marinchenkova/platformer-test-task/assets/22106355/092e4004-024d-44a8-8285-f2d76c4a1c31">
</p>

2. `EntityBuilder` - monobehaviour, аналог EntityPrefab, который может хранить компоненты с ссылками на объекты на сцене

<p align="center">
<img width="666" alt="image" src="https://github.com/marinchenkova/platformer-test-task/assets/22106355/60e19421-27ee-4f32-aab0-38023be44c4a">
</p>

3. `EntitySource` - monobehaviour, который использует EntityBuilder для создания persistent-сущности, жизненный цикл которой будет ограничен жизненным циклом monobehaviour

<p align="center">
<img width="666" alt="image" src="https://github.com/marinchenkova/platformer-test-task/assets/22106355/0e0b9104-709a-4bca-84cb-ca5da4dde0a3">
</p>

4. `EntityView` - monobehaviour, используемый при создании вью, позволяет добавлять вью-компоненты для связи между сущностью и Unity-компонентами

```csharp
public interface IEntityView {

    Entity Entity { get; }

    void Bind(Entity entity);
    void Unbind();
}
```

<p align="center">
<img width="666" alt="image" src="https://github.com/marinchenkova/platformer-test-task/assets/22106355/e4729143-da01-4cc6-839a-1cd4d0674ff7">
</p>

5. `WorldReference` - scriptable object, который хранит ссылку на созданный мир 

- Компоненты хранятся с помощью атрибута `[SerializeReference]`, для их редактирования реализован простой инспектор с браузером компонентов

```csharp
[SerializeReference] private IEntityComponent[] _components;
```

- Создание вью для сущностей производится через `World`, который вызывает сервис для создания сущностей, использующий пул
- В качестве вью для сущности может использоваться любой префаб или объект на сцене, у которого есть `Transform`

### Пайплайн запуска

1. Рут-сцена Scene_Startup

Содержит monobehaviour `GlobalStartup`, который запускает игровую сцену и задает некоторые базовые настройки

<p align="center">
<img width="666" alt="image" src="https://github.com/marinchenkova/platformer-test-task/assets/22106355/7b720704-7d86-4c77-aace-91fed72af4a4">
</p>

2. Игровая сцена Scene_Level1

Здесь запускаются все игровые сущности. Запуск начинается с инициализации мира сущностей, в том числе сущности GameState.

3. Сущность GameState

Persistent-сущность, которая живет все время, пока открыта игровая сцена. 
Отвечает за респавн и уничтожение игрока, имеет свойства и события для связи с UI и другими сущностями.

<p align="center">
<img width="666" alt="image" src="https://github.com/marinchenkova/platformer-test-task/assets/22106355/19de899d-c29c-454e-9faa-976698f66361">
</p>

4. Платформы

Все интерактивные платформы на сцене строятся с помощью связок EntitySource + EntityBuilder. 
Их различные комбинации позволяют создавать, обрабатывать и уничтожать сущности при определенных условиях.

<p align="center">
<img width="666" alt="image" src="https://github.com/marinchenkova/platformer-test-task/assets/22106355/0363a6f2-cd27-4a26-90ba-43a2ae77ad44">
</p>

<p align="center">
<img width="666" alt="image" src="https://github.com/marinchenkova/platformer-test-task/assets/22106355/5bec4b6b-e6eb-4f21-b9fa-a45ce045d2bc">
</p>

<p align="center">
<img width="666" alt="image" src="https://github.com/marinchenkova/platformer-test-task/assets/22106355/da304442-6103-45f8-9be0-45d942e23622">
</p>
