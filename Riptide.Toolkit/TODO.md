# Planned
- [ ] Support auto-splitting of large messages. ([GitHub issue #175](https://github.com/RiptideNetworking/Riptide/issues/175))
Account for practical limit before splitting being '1,472' bytes on datagram (?).
- [ ] Support relaying by using first (4 bits/byte) to encode special SystemMessageID, indicating how server should treat received data.
- [ ] Support packet splitting with high reliability by either introducing TCP, or encoding special data in SystemMessageID.
- [x] Reworked storage to be attribute-based instead.
- [ ] Test Relaying.
- [ ] Add optional logging for messages lost due to pool limit, for quick debugging of memory usage.
- [ ] Replace group declaration with GroupAttribute(params byte[]) and DefaultGroupAttribute and such (damn it, 4th rewrite T^T)(Attributes are too useful T^T)

# Considerations
- [ ] Consider moving to struct-base approach for MessageID and GroupID properties.
- [ ] Replace separate group indexers with one large indexer, which will use (byte/ushort/uint/ulong/ulong x2/ulong x3/ulong x4) as a base to store which groups define which MessageIDs. Use it only in RAM-hungry mode.