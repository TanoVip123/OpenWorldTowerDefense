// Simple struct to represent an entity's unique identifier as an integer.
public readonly struct EntityID
{
    public readonly int Id { get; }

    public EntityID(int id) => Id = id;

    public override string ToString() => Id.ToString();
}

// Entity ID generator
public static class EntityIDGenerator
{
    private static int _currentId = 0;

    public static EntityID GenerateID() => new(_currentId++);
}

