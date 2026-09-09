using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class World
{
    private interface IComponentPool
    {
        public void Remove(EntityID entity);
        public bool Has(EntityID entity);
    }

    private class ComponentPool<T> : IComponentPool
    {
        // Dense component storage
        private T[] _components;

        // Dense entity storage (parallel to components)
        private EntityID[] _entities;

        // Sparse map: EntityID -> index in dense arrays
        private readonly Dictionary<EntityID, int> _index;

        public int Count { get; private set; }

        private const int DEFAULT_CAPACITY = 256;

        public ComponentPool()
        {
            _components = new T[DEFAULT_CAPACITY];
            _entities = new EntityID[DEFAULT_CAPACITY];
            _index = new Dictionary<EntityID, int>();
            Count = 0;
        }

        public void Add(EntityID entity, T component)
        {
            // If entity already exists → replace in place
            if (_index.TryGetValue(entity, out int existingIndex))
            {
                _components[existingIndex] = component;
                return;
            }

            EnsureCapacity(Count + 1);
            _components[Count] = component;
            _entities[Count] = entity;
            _index[entity] = Count;
            Count++;
        }

        public void Remove(EntityID entity)
        {
            if (!_index.TryGetValue(entity, out int removeIndex))
            {
                return;
            }

            int lastIndex = Count - 1;

            // Move last element into removed slot
            _components[removeIndex] = _components[lastIndex];
            _entities[removeIndex] = _entities[lastIndex];

            // Update moved entity index to removeSlot
            _index[_entities[lastIndex]] = removeIndex;

            // Remove old entry of the removed entity
            _index.Remove(entity);

            Count--;
        }

        public bool Has(EntityID entity)
            => _index.ContainsKey(entity);

        public ref T GetRef(EntityID entity) => ref _components[_index[entity]];

        public bool TryGet(EntityID entity, out T component)
        {
            if (_index.TryGetValue(entity, out int index))
            {
                component = _components[index];
                return true;
            }

            component = default;
            return false;
        }

        public IEnumerable<EntityID> GetEntityIDs()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return _entities[i];
            }
        }

        public IEnumerable<KeyValuePair<EntityID, T>> GetAll()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return new KeyValuePair<EntityID, T>(
                    _entities[i],
                    _components[i]
                );
            }
        }

        private void EnsureCapacity(int size)
        {
            if (size <= _components.Length)
            {
                return;
            }

            int newCapacity = _components.Length * 2;

            Array.Resize(ref _components, newCapacity);
            Array.Resize(ref _entities, newCapacity);
        }
    }
    // Maps component types to a dictionary of EntityIDs and their component instances.
    // This is a more efficient structure for querying entities by component type, which is a common operation in ECS.
    private Dictionary<Type, IComponentPool> _entityComponentPool;

    // A dictionary mapping EntityID to a set of component pools that the entity is part of. This allows for efficient removal of all components associated with an entity when it is unregistered.
    private Dictionary<EntityID, HashSet<IComponentPool>> _entityPools;

    // Still need reference to gameObject for now since we need to manipulate the GameObject in presentation system.
    private Dictionary<EntityID, GameObject> _entityObjects;

    // Entity Registry
    private HashSet<EntityID> _registeredEntities;
    // Event and Command State

    public EventBus EventBus { get; private set; }
    public CommandBuffer Commands { get; private set; }
    public EventBuffer Events { get; private set; }

    // Entity State
    public List<EntityID> selectedEntities;
    public List<EntityID> highlightEntities;

    // System State
    public Dictionary<EWorldPhase, WorldPhase> Phases { get; private set; }
    private Dictionary<Type, IGameSystem> _systems;

    // Versioning and Synchronization
    private long _worldEventBufferVersionUpdate;
    private long _worldEventBufferVersionFixedUpdate;

    // Grid state
    public GridMap GridMap { get; private set; }

    public enum EWorldPhase
    {
        Command,
        Simulation,
        EventProcessing,
        DataProcessing,
        Presentation
    }

    public void Initialize()
    {
        // Initialize Event and Command Managers
        EventBus = new EventBus();
        Commands = new CommandBuffer();
        Events = new EventBuffer();
        GridMap = new GridMap();

        // Initialize GridMap
        GridMap.initialize();

        // Initialize Entity State
        selectedEntities = new List<EntityID>();
        highlightEntities = new List<EntityID>();

        // Initialize Entity Data
        _entityComponentPool = new Dictionary<Type, IComponentPool>();
        _entityObjects = new Dictionary<EntityID, GameObject>();
        _entityPools = new Dictionary<EntityID, HashSet<IComponentPool>>();
        _registeredEntities = new HashSet<EntityID>();

        // Initialize component pools for each component type. This allows us to efficiently manage components of different types.
        foreach (Type componentType in ComponentRegistry.Types)
        {
            //Take the generic class ComponentPool<T> and replace T with a runtime type. AKA this is create ComponentPool<componentType>
            Type poolType = typeof(ComponentPool<>).MakeGenericType(componentType);

            //Instantiate an object from a Type that you only know at runtime.
            _entityComponentPool[componentType] = (IComponentPool)Activator.CreateInstance(poolType);
        }

        // Initialize Systems
        _systems = new Dictionary<Type, IGameSystem>();
        Phases = new Dictionary<EWorldPhase, WorldPhase>
        {
            [EWorldPhase.Command] = new WorldPhase(),
            [EWorldPhase.Simulation] = new WorldPhase(),
            [EWorldPhase.EventProcessing] = new WorldPhase(),
            [EWorldPhase.Presentation] = new WorldPhase(),
            [EWorldPhase.DataProcessing] = new WorldPhase()
        };

        // Initialize versioning
        // Start at -1 so that it always process the first batch of events and commands.
        _worldEventBufferVersionUpdate = -1;
        _worldEventBufferVersionFixedUpdate = -1;
    }

    // Register an entity and return its EntityID. This can be used when you want to create an entity without a GameObject, such as for pure data entities.
    public EntityID RegisterEntity()
    {
        EntityID entityId = EntityIDGenerator.GenerateID(); // Generate a unique EntityID
        _entityPools[entityId] = new HashSet<IComponentPool>(); // Initialize an empty set of component pools for this entity
        _registeredEntities.Add(entityId);
        Debug.Log($"EntityView with EntityID {entityId} registered to the world.");
        return entityId;
    }

    // Register an entity and return its EntityID. This can be used when you want to create an entity with a GameObject, such as for entities that have a visual representation in the scene.
    public EntityID RegisterEntity(GameObject entityObject)
    {
        EntityID entityId = EntityIDGenerator.GenerateID(); // Generate a unique EntityID
        _entityPools[entityId] = new HashSet<IComponentPool>();
        _entityObjects[entityId] = entityObject; // Store the GameObject for this entity
        _registeredEntities.Add(entityId);

        Debug.Log($"EntityView with EntityID {entityId} registered to the world.");
        return entityId;
    }

    public bool UnregisterEntity(EntityID entityId)
    {
        foreach (IComponentPool pool in _entityPools[entityId])
        {
            pool.Remove(entityId);
        }
        _entityPools.Remove(entityId);
        _entityObjects.Remove(entityId);
        _registeredEntities.Remove(entityId);
        Debug.Log($"EntityView with EntityID {entityId} unregistered from the world.");
        return true;
    }

    // Update is called once per frame
    public void Update(float deltaTime)
    {
        // This solution has a problem though, we are essentially binding Update to run at the same rate as FixedUpdate
        // We might need to go a step further and consider a PerSystem versioning if we want to allow different systems to run at different rates, but that might be an overkill for our current need.
        // Only process events if there are new events in the buffer that haven't been processed in the update phase yet.
        // Debug.Log($"World Update checked");
        if (_worldEventBufferVersionUpdate == Events.Version)
        {
            if (_worldEventBufferVersionUpdate == _worldEventBufferVersionFixedUpdate)
            {
                Events.SwapBuffers();
            }
            return;
        }
        // Debug.Log("World Update started");

        // Currently only Presentation Systems need to be called in update.
        // The input System is handled by Unity's so it is also considered to be an "Update type" System.
        Phases[EWorldPhase.Presentation].Update(deltaTime);
        Phases[EWorldPhase.Command].Update(deltaTime);
        Phases[EWorldPhase.DataProcessing].Update(deltaTime);
        _worldEventBufferVersionUpdate = Events.Version;

        // For clean lifecycle ownership, Update clock is incharge of updating EventBuffer.
        // However, it should only do so if FixedUpdate has already processed the events,
        //  otherwise we might end up in a situation where FixedUpdated is supposed to process events
        // but Update has already swapped the buffer and cleared the events before FixedUpdate can process them.

    }

    // Only Physics Sync should be in FixedUpdate
    public void FixedUpdate(float fixedDeltaTime)
    {
        // Debug.Log($"World FixedUpdate checked");
        if (_worldEventBufferVersionFixedUpdate >= Events.Version)
        {
            // Process events and commands for the fixed update phase
            return;
        }

        // Debug.Log("World FixedUpdate started");

        // TODO: Only Simulation Systems need to be called in fixed update since it syncs the physics simulation. Need fixing.
        Phases[EWorldPhase.EventProcessing].FixedUpdate(fixedDeltaTime);
        Phases[EWorldPhase.Simulation].FixedUpdate(fixedDeltaTime);

        _worldEventBufferVersionFixedUpdate = Events.Version;

        // For clean lifecycle ownership, FixedUpdate is incharge of updating CommandsBuffer.
        // We don't need to consider about the version of CommandBuffer
        // since only FixedUpdate Consume the event here, so we can safely swap the buffer and clear the commands without worrying about synchronization with Update.
        Commands.SwapBuffers();
    }

    // Retrieve the GameObject associated with an EntityID, return null if not found.
    public bool GetEntityObject(EntityID entityId, out GameObject gameObject)
    {
        if (_entityObjects.TryGetValue(entityId, out GameObject entityObject))
        {
            gameObject = entityObject;
            return true;
        }
        gameObject = null;
        return false;
    }

    // IGameSystem must be a class since they need to implement behaviour, this allow null by default.
    public void AddSystem<T>(T system) where T : class, IGameSystem => _systems[typeof(T)] = system;
    public T GetSystem<T>() where T : class, IGameSystem => _systems.TryGetValue(typeof(T), out IGameSystem system) ? system as T : null;

    // why not just use object as the component (AKA object component instead of T component) and get type later? You will need to cast later since now component is stored as object type.
    // Having T here make it easy to define the casting type at adding time
    public void AddComponentToEntity<T>(EntityID entityId, T component) where T : IComponent
    {
        // We make sure to create an entry for each entity in RegisterEntity, so we can assume the entityId is always valid and has an entry in _entityComponents.
        ((ComponentPool<T>)_entityComponentPool[typeof(T)]).Add(entityId, component);
        _entityPools[entityId].Add(_entityComponentPool[typeof(T)]); // Add the component pool to the entity's set of component pools
        Debug.Log($"Add component to World: {typeof(T)} for EntityID: {entityId}");
    }

    public void RemoveComponentFromEntity<T>(EntityID entityId) where T : IComponent
    {
        ((ComponentPool<T>)_entityComponentPool[typeof(T)]).Remove(entityId);
        _entityPools[entityId].Remove(_entityComponentPool[typeof(T)]); // Remove the component pool from the entity's set of component pools
        Debug.Log($"Remove component from World: {typeof(T)} for EntityID: {entityId}");
    }

    // Get component, return a ref. This is so that we don't unintentionally copy struct when we don't need
    // We return a ref so we are not creating a new copy for struct and this is modifiable
    public ref T GetComponentFromEntity<T>(EntityID entityId) where T : IComponent
    {
        if (!_entityComponentPool.TryGetValue(typeof(T), out IComponentPool pool))
        {
            throw new InvalidOperationException($"Component pool {typeof(T)} does not exist.");
        }

        return ref ((ComponentPool<T>)pool).GetRef(entityId);
    }

    // Try get does return a copy of the struct so it SHOULD ONLY BE USED FOR READING, not modifying
    public bool TryGetComponentFromEntity<T>(EntityID entityId, out T component) where T : IComponent
    {
        if (_entityComponentPool.TryGetValue(typeof(T), out IComponentPool pool) && ((ComponentPool<T>)pool).TryGet(entityId, out T _component))
        {
            component = _component;
            return true;
        }

        component = default;
        return false;
    }

    // the TRyGet (out) pattern of Unity return a copy for struct so beware of that
    public IEnumerable<EntityID> GetEntitiesWithComponent<T>() where T : IComponent
    {
        if (_entityComponentPool.TryGetValue(typeof(T), out IComponentPool pool))
        {
            ComponentPool<T> typed = (ComponentPool<T>)pool;
            return typed.GetEntityIDs();
        }

        return Enumerable.Empty<EntityID>();
    }
}
