namespace SukotSystemCore.Enums
{
    // Lifecycle of an inspection Order (הזמנה).
    //
    // New        -> visible to every Rabbi browsing unclaimed requests in a city.
    // InProgress -> a Rabbi took responsibility for it / it's "in treatment" (בטיפול) - the
    //               claim is the resource-competition action, Part C: the Service must check
    //               Status == New and rely on Order's concurrency token when saving the
    //               transition to InProgress, so that if two Rabbis claim the same Order at
    //               the same time, only one save succeeds and the other throws
    //               DbUpdateConcurrencyException -> 409).
    //               From this point the Order must stop appearing in other Rabbis' "new" lists.
    // Completed  -> the assigned Rabbi finished the inspection and recorded the outcome.
    // Cancelled  -> the Customer cancelled the request before a Rabbi claimed it.
    //
    // Values are pinned explicitly so the underlying integer stored by EF Core never shifts
    // if the enum is reordered later.
    public enum OrderStatus
    {
        New = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3
    }
}
