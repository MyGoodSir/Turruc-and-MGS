using Unity.Entities;

namespace TMGS.Rotate
{
    //structs are value types. Usually no separate memory is allocated for them on the heap so there is usually no need for garbage collection to worry about them.
    //classes, however, DO allocate separate memory locations and are marked by the garbage collector to be cleaned up when they go out of scope.
    public struct Rotate : IComponentData
    {
        public float radiansPerSecond;
    }
}