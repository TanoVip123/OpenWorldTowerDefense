public enum BuildingType
{
    [StringValue("None")]
    None,
    [StringValue("Tower")]
    Tower,
    [StringValue("Wall")]
    Wall,
    [StringValue("Unit Production")]
    UnitProduction,
    [StringValue("Resource Production")]
    ResourceProduction,
    [StringValue("Research")]
    Research,
}

public enum PhysicShapeType
{
    [StringValue("None")]
    None,
    [StringValue("Circle")]
    Circle,
    [StringValue("Box")]
    Box,
    [StringValue("Polygon")]
    Polygon,
    [StringValue("Grid")]
    Grid,
}
