using System;

namespace SukotSystemCore.Exceptions
{
    // Thrown by a Service when a request conflicts with existing data in a way that is not the
    // optimistic-concurrency case (Part C's DbUpdateConcurrencyException) - e.g. a duplicate
    // phone number on the Rabbi roster. Maps to 409, same as a concurrency conflict, but logged
    // as a distinct, expected business outcome rather than a bug.
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
