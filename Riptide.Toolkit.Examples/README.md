# Purpose
`Riptide.Toolkit.Examples` contains... Well... Usage examples of a Toolkit.

Important point is that you **cannot** use regular `Riptide` classes with `Riptide.Toolkit` at the moment:
- `Riptide.Client` replaced with `Riptide.Toolkit.AdvancedClient` (it uses new message handling system).
- `Riptide.Server` replaced with `Riptide.Toolkit.AdvancedServer` (it uses new message handling system).
- `Riptide.MessageHandlerAttribute` replaced with `Riptide.Toolkit.AdvancedMessageAttribute` (it defines ModID).
- `Riptide.Message.Create(...)` replaced with `Riptide.Toolkit.NetMessage.Create(...)` (it defines special headers).
Regular `Riptide.Client` or `Riptide.Server` will only work with regular `MessageHandlerAttribute`s.

And if you try to send message created using `Riptide.Message.Create(...)` method - it will not work, or will produce errors.

## Native Riptide support.
It was impossible to avoid creating `Riptide.Toolkit.NetMessage.Create(...)` methods due to technical limitations,
so if Riptide will ever be updated to allow custom header definition - we might remove `NetMessage` class entirely.
