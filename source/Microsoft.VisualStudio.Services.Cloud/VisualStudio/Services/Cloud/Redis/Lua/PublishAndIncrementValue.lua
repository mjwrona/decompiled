-- KEYS[1] - Redis key to be incremented and published
-- ARGV[1] - Current message ID from caller
-- RESULT  - True if the callers counter was used to update cache counter, otherwise false

local result = false
local callerValue = tonumber(ARGV[1])
local newId = redis.call('INCR', KEYS[1])
if newId <= callerValue then
    result = true
    newId = callerValue + 1
    redis.call('SET', KEYS[1], newId)
end

local payload = newId .. ' ' .. ARGV[2]
redis.call('PUBLISH', KEYS[1], payload)
return result
