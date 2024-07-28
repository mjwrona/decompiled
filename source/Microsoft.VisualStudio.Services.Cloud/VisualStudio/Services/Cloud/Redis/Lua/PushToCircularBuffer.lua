-- KEYS[1]  - Redis circular buffer name
-- ARGV[1]  - Length to cap the circular buffer to
-- ARGV[2]  - TTL for the key in seconds, set only once for a key
-- ARGV[3+] - messages to push, items in the order will be present in the buffer from left to right
local buffer = KEYS[1]
local length = ARGV[1]
local ttl = ARGV[2]

-- push messages by skipping the first arg, we use LUA 5.1, so unpack, RPUSH returns currentLength
-- Note: consider LTRIM first to reduce the size and then push if needed (note that LTRIM won't trim a list of length 1, so we would have to delete the key), would deleting key once in a while better than growing and shrinking list?
--       leaving it as is for now, this should be fine for small list
local currentLength = redis.call("RPUSH", buffer, unpack(ARGV, 3, #ARGV))

-- If number of items that we pushed (ignoring the additional special args) is equal to the length of the new list, the key is just created, so set TTL on it now
if currentLength == #ARGV-2 then
	redis.call('EXPIRE', buffer, tonumber(ttl))
end

-- now trim the list from the left for the length we need, there by creating circular buffer
local difference = currentLength-tonumber(length)
if difference > 0 then
	redis.call("LTRIM", buffer, difference, -1)
end

return