## Runtime Handler changing
This is not allowed at the moment, to somehow allow for multi-threading.
As you can imagine - modifying an array in a hot spot is very hard, so this is postponed until taler date.

## Proper multi-threading
We need to use C# Monitor for NetworkIndex, as I suspect multi-threaded code might be in race conditions at times.