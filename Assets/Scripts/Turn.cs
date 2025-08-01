using UnityEngine;

/**
 * The turn covers everything that occurs during the span of 1 Turn (i.e. clock cycle)
 * This means that every action must have a new position.
 * 
 * Throwing apps occur on the same turn as a movement, hence it is part of 1 action 
 */
public struct Turn
{
    public Vector2 Position;

    public Tile TileObj;
    
    // TODO: Throwing will require thrown object + end location
}
