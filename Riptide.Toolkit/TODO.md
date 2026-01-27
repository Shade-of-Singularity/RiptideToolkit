# Planned
- [ ] Support auto-splitting of large messages. ([GitHub issue #175](https://github.com/RiptideNetworking/Riptide/issues/175))
Account for practical limit before splitting being '1,472' bytes on datagram (?).

# Considerations
- [ ] Consider moving to struct-base approach for MessageID and GroupID properties.
- [ ] Replace separate group indexers with one large indexer, which will use (byte/ushort/uint/ulong/ulong x2/ulong x3/ulong x4) as a base to store which groups define which MessageIDs. Use it only in RAM-hungry mode.